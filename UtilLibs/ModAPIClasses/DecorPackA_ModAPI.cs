// version 1.0

using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

// You can copy paste this file straight into your mod, just change the namespace and you are good to go
// Decor Pack I promises to not make breaking changes to this API
namespace UtilLibs.ModAPIClasses
{
	public static class DecorPackA_ModAPI
	{
		public static class Keys
		{
			public static HashedString
				Id = "LampId",
				Color = "Color",
				Tags = "Tags",
				Data = "Data";
		}
		public static class TAGS
		{
			public static HashedString
				RAINBOW = "Rainbow",
				TINTABLE = "Tintable",
				ROTATABLE = "Rotatable",
				SHIFTY = "Shifty",
				USESSECONDARY = "UsesSecondary", // enables the secondary overlay
				SKIP_ANIMATION_UPDATE = "SkipAnimationUpdate"; // skips default handling of animaiton change


			public static HashedString
				SHIFTY_COLOR2 = "Shifty_Color2",
				SHIFTY_DURATION = "Shifty_Duration";
		}
		public static class LAMPCATEGORIES
		{
			public const string
				CRITTERS = "critters",
				MODS = "mods",
				FIGURES = "figures",
				SPACE = "space",
				MEDIA = "media",
				CREATORS = "creators",
				CUSTOMIZABLE = "customizable",
				MISC = "misc";
		}


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
		private static AddMoodlampDelegate _addMoodLamp;
		public static object AddMoodLamp(string ID, string name, string kAnimFile, float r, float g, float b, string category = LAMPCATEGORIES.MISC, string tooltipOverride = null, KAnim.PlayMode playModeWhenOn = KAnim.PlayMode.Paused, HashSet<HashedString> tags = null) 
			=> AddMoodLamp(ID, name, kAnimFile, new Color(r, g, b), category, tooltipOverride, playModeWhenOn, tags);
		public static object AddMoodLamp(string ID, string name, string kAnimFile, Color color = default, string category = LAMPCATEGORIES.MISC, string tooltipOverride = null, KAnim.PlayMode playModeWhenOn = KAnim.PlayMode.Paused, HashSet<HashedString> tags = null)
		{
			if (_addMoodLamp == null)
			{
				Debug.LogWarning("DecorPackA_ModAPI.AddMoodLamp is null. Make sure DecorPackA is installed and enabled.");
				return null;
			}
			return _addMoodLamp(ID, name, tooltipOverride, category, kAnimFile, color, playModeWhenOn, tags);
		}

		public static object SetMoodLampData(this object moodLampInstance, HashedString key, object data)
		{

			var m_setData = AccessTools.Method(moodLampInstance.GetType(), "SetData", new[] { typeof(HashedString), typeof(object) });
			if (m_setData != null)
			{
				m_setData.Invoke(moodLampInstance, [key, data]);
			}
			else
				Debug.LogWarning("DecorPackA_ModAPI.AddMoodLampTags: Tags method not found on moodLampInstance. Make sure DecorPackA is installed and enabled.");
			return moodLampInstance;
		}
		public static object AddMoodLampTags(this object moodLampInstance, HashedString tag) => AddMoodLampTags(moodLampInstance, new HashedString[] { tag });
		public static object AddMoodLampTags(this object moodLampInstance, HashedString[] tags)
		{
			var m_tags = AccessTools.Method(moodLampInstance.GetType(), "Tags", new[] { typeof(HashedString[]) });
			if (m_tags != null)
			{
				m_tags.Invoke(moodLampInstance, [tags]);
			}
			else
				Debug.LogWarning("DecorPackA_ModAPI.AddMoodLampTags: Tags method not found on moodLampInstance. Make sure DecorPackA is installed and enabled.");
			return moodLampInstance;
		}
		public static object MakeMoodLampShifty(this object moodLampInstance, float r, float g, float b, float shiftingDuration = 7f)
		{
			moodLampInstance.AddMoodLampTags([TAGS.SHIFTY]);
			moodLampInstance.SetMoodLampData(TAGS.SHIFTY_COLOR2, new Color(r, g, b));
			moodLampInstance.SetMoodLampData(TAGS.SHIFTY_DURATION, shiftingDuration);
			return moodLampInstance;
		}



		// returns the LampVariant db entry as an object
		public delegate object AddMoodlampDelegate(string ID, string name, string tooltipOverride, string category, string kAnimFile, Color color, KAnim.PlayMode playModeWhenOn = KAnim.PlayMode.Paused, HashSet<HashedString> tags = null);

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

			_addMoodLamp = (AddMoodlampDelegate)Delegate.CreateDelegate(typeof(AddMoodlampDelegate), m_AddMoodLamp);

			var m_AddComponentToMoodlampPrefab = AccessTools.Method(type, "AddComponentToMoodlampPrefab", new[] { typeof(Type) });

			if (m_AddComponentToMoodlampPrefab == null)
			{
				if (logWarnings) Debug.LogWarning("AddComponentToMoodlampPrefab is not a method.");
				return false;
			}

			AddComponentToLampPrefab = (Action<Type>)Delegate.CreateDelegate(typeof(Action<Type>), m_AddComponentToMoodlampPrefab);

			return _addMoodLamp != null;
		}
	}
}
