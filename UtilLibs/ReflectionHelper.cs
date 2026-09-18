using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UtilLibs
{
	public static class ReflectionHelper
	{
		public static bool TryGetType(string typeName, out Type type)
		{
			type = AccessTools.TypeByName(typeName);
			if (type == null)
				Debug.LogWarning($"[ReflectionHelper] Type '{typeName}' not found.");
			return type != null;
		}
		public static bool TryGetMethodInfo(string typeName, string methodName, out System.Reflection.MethodInfo methodInfo) => TryGetMethodInfo(typeName, methodName, null, out methodInfo);
		public static bool TryGetMethodInfo(string typeName, string methodName, Type[] parameters, out System.Reflection.MethodInfo methodInfo)
		{
			methodInfo = null;
			if (!TryGetType(typeName, out Type type))
				return false;
			methodInfo = AccessTools.Method(type, methodName, parameters);

			if (methodInfo == null)
				Debug.LogWarning($"[ReflectionHelper] method '{methodName}' not found on type {type}");

			return methodInfo != null;
		}
		public static bool TryGetFieldInfo(string typeName, string fieldName, out System.Reflection.FieldInfo fieldInfo)
		{
			fieldInfo = null;
			if (!TryGetType(typeName, out Type type))
				return false;
			fieldInfo = AccessTools.Field(type, fieldName);

			if (fieldInfo == null)
				Debug.LogWarning($"[ReflectionHelper] field '{fieldName}' not found on type {type}");

			return fieldInfo != null;
		}
		public static bool TryGetPropertyGetter(string typeName, string propertyName, out System.Reflection.MethodInfo getter)
		{
			getter = null;
			if (!TryGetType(typeName, out Type type))
				return false;
			getter = AccessTools.PropertyGetter(type, propertyName);

			if (getter == null)
				Debug.LogWarning($"[ReflectionHelper] getter for '{propertyName}' not found on type {type}");

			return getter != null;
		}
		public static bool TryGetPropertySetter(string typeName, string propertyName, out System.Reflection.MethodInfo setter)
		{
			setter = null;
			if (!TryGetType(typeName, out Type type))
				return false;
			setter = AccessTools.PropertySetter(type, propertyName);

			if (setter == null)
				Debug.LogWarning($"[ReflectionHelper] setter for '{propertyName}' not found on type {type}");

			return setter != null;
		}
		public static bool TryCreateDelegate<T>(string typeName, string methodName, out T del, object instance = null, Type[] generics = null, bool matchParametersLazy = false, bool matchReturnType = true) where T : Delegate
		{
			del = null;
			if (!TryGetType(typeName, out Type type))
				return false;

			//infer parameters from the delegate definition; delegate needs to match signature of target!
			var delegateInvoke = typeof(T).GetMethod("Invoke");
			Type[] parameters = [.. delegateInvoke.GetParameters().Select(p => p.ParameterType)];

			//optional for exact matching, not sure if I want this.
			if (matchParametersLazy && parameters.Length == 0)
				parameters = null;

			MethodInfo methodInfo = AccessTools.Method(type, methodName, parameters, generics);
			if (methodInfo == null)
			{
				string parameterNames = parameters == null 
					? "*"
					: string.Join(", ", parameters.Select(t => t.Name));
				Debug.LogWarning($"[ReflectionHelper] Method '{methodName}' not found on type {type} with the parameters '{parameterNames}'.");
				return false;
			}

			if (matchReturnType && methodInfo.ReturnType != delegateInvoke.ReturnType)
			{
				Debug.LogWarning($"[ReflectionHelper] Method '{methodName}' had a return type mismatch. Expected {delegateInvoke.ReturnType}, got {methodInfo.ReturnType}.");
				return false;
			}

			try
			{
				del = (instance == null)
				? (T)Delegate.CreateDelegate(typeof(T), methodInfo)
				: (T)Delegate.CreateDelegate(typeof(T), instance, methodInfo);
				return del != null;
			}
			catch (Exception ex) 
			{
				Debug.LogWarning($"[ReflectionHelper] Could not create delegate '{typeof(T)}' for method '{typeName}.{methodName}'.");
				return false;
			}
		}
		public static bool TryCreatePropertyGetterDelegate<T>(string typeName, string propertyName, out T del, object instance = null) where T : Delegate
		{
			del = null;
			if (!TryGetPropertyGetter(typeName, propertyName, out var propertyGetter))
				return false;
			del = instance == null ? (T)Delegate.CreateDelegate(typeof(T), propertyGetter) : (T)Delegate.CreateDelegate(typeof(T), instance, propertyGetter);
			return del != null;
		}
		public static bool TryCreatePropertySetterDelegate<T>(string typeName, string propertyName, out T del, object instance = null) where T : Delegate
		{
			del = null;
			if (!TryGetPropertySetter(typeName, propertyName, out var propertySetter))
				return false;
			del = instance == null ? (T)Delegate.CreateDelegate(typeof(T), propertySetter) : (T)Delegate.CreateDelegate(typeof(T), instance, propertySetter);
			return del != null;
		}

		private static readonly Dictionary<string, Type?> _cachedTypes = [];
		public static bool TryGetComponentMod(this GameObject go, string componentName, out Component component)
		{
			component = null;
			if (!_cachedTypes.TryGetValue(componentName, out var cached))
			{
				cached = CacheTypeWithName(componentName);
			}
			if (cached == null)
				return false;
			return go.TryGetComponent(cached, out component);
		}
		private static Type? CacheTypeWithName(string componentName)
		{
			Type cmp = typeof(Component);
			foreach (var possibleType in AccessTools.AllTypes())
			{
				if (possibleType.Name == componentName && cmp.IsAssignableFrom(possibleType))
				{
					_cachedTypes[componentName] = possibleType;
					return possibleType;
				}
			}
			_cachedTypes[componentName] = null;
			return null;
		}
	}
}
