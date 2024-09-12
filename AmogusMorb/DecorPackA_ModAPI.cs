// version 1.0

using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

// You can copy paste this file straight into your mod, just change the namespace and you are good to go
// Decor Pack I promises to not make breaking changes to this API
namespace AmogusMorb
{
	public class DecorPackA_ModAPI
	{
		public static int MoodlampChangedEvent = Hash.SDBMLower("DecorPackA_OnMoodlampChanged");

		public struct VariantChangedEvent
		{
			public string lampId;
			public bool tintable;
			public bool usesSecondaryController;
			public Color color;
			public Vector2 offset;
			public HashSet<HashedString> tags;
			public Dictionary<HashedString, object> data;
		}

		/// <summary>
		/// Add a new moodlamp
		/// </summary>
		/// <param name="ID"></param>
		/// <param name="name"></param>
		/// <param name="category"></param>
		/// <param name="kAnimFile"></param>
		/// <param name="color"></param>
		/// <param name="playModeWhenOn"></param>
		/// <param name="tags">Tags used to modify behavior. Can be any arbitrary hash for your own use (namespacing recommended)</param>
		/// <returns></returns>
		public static AddMoodlampDelegate AddMoodLamp;

		// returns the LampVariant db entry as an object
		public delegate object AddMoodlampDelegate(string ID, string name, string category, string kAnimFile,
			Color color, KAnim.PlayMode playModeWhenOn = KAnim.PlayMode.Paused, HashSet<HashedString> tags = null);

		public static Action<Type> AddComponentToLampPrefab;

		public static bool TryGetData<T>(object data, HashedString key, out T result)
		{
			result = default;

			if (data == null)
				return false;

			if (data is Dictionary<HashedString, object> dict && dict.TryGetValue(key, out var value))
			{
				if (value == null) return false;

				result = (T)value;

				return true;
			}

			return false;
		}

		public static bool HasTag(object data, HashedString tag)
		{
			return TryGetData<HashSet<HashedString>>(data, "Tags", out var tags) && tags.Contains(tag);
		}


		public static T GetCustomDataOrDefault<T>(object data, HashedString key, T defaultValue)
		{
			if (TryGetData<Dictionary<HashedString, object>>(data, "Data", out var dict))
			{
				if (dict.TryGetValue(key, out var value))
				{
					return value == null ? defaultValue : (T)value;
				}
			}

			return defaultValue;
		}

		public static T GetDataOrDefault<T>(object data, HashedString key, T defaultValue)
		{
			if (data is Dictionary<HashedString, object> dict && dict.TryGetValue(key, out var value))
			{
				return value == null ? defaultValue : (T)value;
			}

			return defaultValue;
		}

		public static class Keys
		{
			public static HashedString
				Id = "LampId",
				Color = "Color",
				Tags = "Tags",
				Data = "Data";
		}

		/// <summary>
		/// Call this at Db.Initialize. Please do not use late priority.
		/// </summary>
		/// <param name="logWarnings">Log Warnings. Recommended to set false if Decor Pack I is not a hard dependency for your mod.</param>
		/// <returns>Success of finding the DecorPackI API</returns>
		public static bool TryInitialize(bool logWarnings = false)
		{
			var type = Type.GetType("DecorPackA.ModAPI, DecorPackA");

			if (type == null)
			{
				if (logWarnings) Debug.LogWarning("DP ModAPI type is null.");
				return false;
			}

			var m_AddMoodLamp = AccessTools.Method(type, "AddMoodLamp",
				new[]
				{
					typeof(string),
					typeof(string),
					typeof(string),
					typeof(string),
					typeof(Color),
					typeof(KAnim.PlayMode),
					typeof(HashSet<HashedString>)
				});

			if (m_AddMoodLamp == null)
			{
				if (logWarnings) Debug.LogWarning("AddMoodLamp is not a method.");
				return false;
			}

			AddMoodLamp = (AddMoodlampDelegate)Delegate.CreateDelegate(typeof(AddMoodlampDelegate), m_AddMoodLamp);

			var m_AddComponentToMoodlampPrefab = AccessTools.Method(type, "AddComponentToMoodlampPrefab", new[] { typeof(Type) });

			if (m_AddComponentToMoodlampPrefab == null)
			{
				if (logWarnings) Debug.LogWarning("AddComponentToMoodlampPrefab is not a method.");
				return false;
			}

			AddComponentToLampPrefab = (Action<Type>)Delegate.CreateDelegate(typeof(Action<Type>), m_AddComponentToMoodlampPrefab);

			return AddMoodLamp != null;
		}
	}
}
