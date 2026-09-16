using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilLibs
{
	public static class ReflectionHelper
	{
		public static bool TryGetType(string typeName, out Type type)
		{
			type = Type.GetType(typeName);
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
		public static bool TryCreateDelegate<T>(string typeName, string methodName, Type[] parameters, out T del) where T : Delegate
		{
			del = null;
			if (!TryGetMethodInfo(typeName, methodName, parameters, out var methodInfo))
				return false;
			del = (T)Delegate.CreateDelegate(typeof(T), methodInfo);
			return del != null;
		}

		private static readonly Dictionary<string, Type?> _cachedTypes = [];
		public static bool TryGetComponentMod(this GameObject go, string componentName, out Component component)
		{
			component = null;
			if(!_cachedTypes.TryGetValue(componentName,out var cached))
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
