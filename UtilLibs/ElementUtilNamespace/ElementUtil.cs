using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;
using static ElementLoader;

namespace ElementUtilNamespace
{
	public class SgtElementUtil
	{
		public static readonly Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>();
		public static readonly Dictionary<string, object> ReverseSimHashNameLookup = new Dictionary<string, object>();
		public static readonly List<ElementInfo> Elements = new List<ElementInfo>();

		public static void ExecuteElementEnumPatches(Harmony harmony)
		{
			ElementEnumPatches(harmony);
			//ElementLoaderPatches(harmony);
		}
		#region noEnumToStringPatch

		static void ElementLoaderPatches(Harmony harmony)
		{
			SgtLogger.l("Attempting to patch Enum.Parse...");
			try
			{
				var original = AccessTools.Method(typeof(Enum), nameof(Enum.Parse), [typeof(Type), typeof(string), typeof(bool)]);
				var m_prefix = new HarmonyMethod(typeof(SgtElementUtil), nameof(SimhashParse_EnumPatch));
				harmony.Patch(original, prefix: m_prefix);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error:\n" + e);
				return;
			}

			SgtLogger.l("Attempting to patch GameTagExtensions.CreateTag...");
			try
			{
				var original = AccessTools.Method(typeof(GameTagExtensions), nameof(GameTagExtensions.CreateTag));
				var m_prefix = new HarmonyMethod(typeof(SgtElementUtil), nameof(GameTagExtensions_CreateTag_Patch));
				harmony.Patch(original, prefix: m_prefix);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error:\n" + e);
				return;
			}
			SgtLogger.l("Attempting to patch GameTagExtensions.Create...");
			try
			{
				var original = AccessTools.Method(typeof(GameTagExtensions), nameof(GameTagExtensions.Create));
				var m_prefix = new HarmonyMethod(typeof(SgtElementUtil), nameof(GameTagExtensions_CreateTag_Patch));
				harmony.Patch(original, prefix: m_prefix);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error:\n" + e);
				return;
			}
			SgtLogger.l("Attempting to patch CodexEntryGenerator_Elements.GenerateEntries...");
			try
			{
				var original = AccessTools.Method(typeof(CodexEntryGenerator_Elements), nameof(CodexEntryGenerator_Elements.GenerateEntries));
				var m_transpiler = new HarmonyMethod(typeof(SgtElementUtil), nameof(CodexEntryGenerator_Elements_Transpiler));
				harmony.Patch(original, transpiler: m_transpiler);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error:\n" + e);
				return;
			}
			///Todo here: add a patch to ClosestOxygenCanisterSensor.GetForbbidenTags if any of the elements are breathable!
			SgtLogger.l("Element patches successful!");
		}

		public static IEnumerable<CodeInstruction> CodexEntryGenerator_Elements_Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
		{
			var objToString = AccessTools.Method(typeof(System.Object), "ToString");

			foreach (var ci in orig)
			{
				yield return ci;

				if (ci.Calls(objToString))
				{
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SgtElementUtil), nameof(TryFixElementId)));
				}
			}
		}
		static string TryFixElementId(string elementId)
		{
			if (int.TryParse(elementId, out var hash) && SimHashNameLookup.TryGetValue((SimHashes)hash, out var modElementId))
				return modElementId;
			return elementId;
		}

		public static bool GameTagExtensions_CreateTag_Patch(SimHashes id, ref Tag __result)
		{
			if (SimHashNameLookup.TryGetValue(id, out string tag_string))
			{
				__result = TagManager.Create(tag_string);
				return false;
			}
			return true;
		}
		public static bool GameTagExtensions_Create(SimHashes id, ref Tag __result)
		{
			if (SimHashNameLookup.TryGetValue(id, out string tag_string))
			{
				__result = TagManager.Create(tag_string);
				return false;
			}
			return true;
		}

		#endregion
		#region EnumToStringPatch
		static void ElementEnumPatches(Harmony harmony)
		{
			//SgtLogger.l("Attempting to patch Enum.ToString's internal InternalFormat method...");
			//try
			//{
			//	var original = AccessTools.Method(typeof(Enum), "InternalFormat");
			//	var m_prefix = new HarmonyMethod(typeof(SgtElementUtil), nameof(SimHashInternalFormat_EnumPatch));
			//	harmony.Patch(original, m_prefix);
			//}
			//catch (Exception e)
			//{
			//	SgtLogger.error("Error while patching Enum.InternalFormat:\n" + e);
			//}
			SgtLogger.l("Attempting to patch Enum.ToString...");
			try
			{
				var original = AccessTools.Method(typeof(Enum), "ToString", new Type[] { });
				var m_patch = new HarmonyMethod(typeof(SgtElementUtil), nameof(SimHashToString_EnumPatch));
				harmony.Patch(original, postfix: m_patch);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error while patching Enum.ToString:\n" + e);
			}

			SgtLogger.l("Attempting to patch Enum.Parse...");
			try
			{
				var original = AccessTools.Method(typeof(Enum), nameof(Enum.Parse), [typeof(Type), typeof(string), typeof(bool)]);
				var m_prefix = new HarmonyMethod(typeof(SgtElementUtil), nameof(SimhashParse_EnumPatch));
				harmony.Patch(original, prefix: m_prefix);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error while patching Enum.Parse:\n" + e);
				return;
			}
			SgtLogger.l("Element enum patches successful!");
		}
		public static bool SimHashInternalFormat_EnumPatch(object eT, object value, ref string __result)
		{
			if (eT == null)
			{
				SgtLogger.warning("EnumPatch: enumType was null!");
				return true;
			}

			if (value == null)
			{
				SgtLogger.warning("EnumPatch: value was null!");
				return true;
			}

			if (eT == null || eT != typeof(SimHashes) || value == null)
				return true;
			return !SimHashNameLookup.TryGetValue((SimHashes)value, out __result);
		}

		//public static bool SimHashToString_EnumPatch(Enum __instance, ref string __result)
		//{
		//	if (__instance is SimHashes hashes)
		//	{
		//		return !SimHashNameLookup.TryGetValue(hashes, out __result);
		//	}

		//	return true;
		//}
		public static void SimHashToString_EnumPatch(Enum __instance, ref string __result)
		{
			if (__instance.GetType() == typeof(SimHashes) 
				&& int.TryParse(__result, out int hash)
				&& SimHashNameLookup.TryGetValue((SimHashes)hash, out string elementId))
			{
				__result = elementId;
			}
		}
		public static bool SimhashParse_EnumPatch(Type enumType, string value, ref object __result)
		{
			if (enumType == typeof(SimHashes))
			{
				return !SgtElementUtil.ReverseSimHashNameLookup.TryGetValue(value, out __result);
			}

			return true;
		}
		#endregion

		public static SimHashes RegisterSimHash(string name)
		{
			var simHash = (SimHashes)Hash.SDBMLower(name);
			SgtLogger.l("registering Element: " + name + ", with simhash " + simHash);
			SimHashNameLookup.Add(simHash, name);
			ReverseSimHashNameLookup.Add(name, simHash);

			return simHash;
		}

		public static void SetTexture_Main(Material material, string texture) =>
			SetTexture(material, texture, "_MainTex");

		public static void SetTexture_ShineMask(Material material, string texture, Color? specularColor = null)
		{
			SetTexture(material, texture, "_ShineMask");
			if (specularColor.HasValue)
				material.SetColor("_ShineColour", specularColor.Value);
		}

		public static void SetTexture_NormalNoise(Material material, string normal) =>
			SetTexture(material, normal, "_NormalNoise");

		public static Substance CreateSubstance(SimHashes simhash, bool specular, string anim, Element.State state,
			Color color, Material material, Color uiColor, Color conduitColor, Color? specularColor, string normal,
			bool isCloned = false)
		{
			string id = SimHashNameLookup[simhash];
			var animFile = Assets.Anims.Find(a => a.name == anim);

			if (animFile == null)
			{
				animFile = Assets.Anims.Find(a => a.name == "glass_kanim");
			}

			var newMaterial = new Material(material);

			if (state == Element.State.Solid && !isCloned)
			{
				SetTexture_Main(newMaterial, id.ToLowerInvariant());

				if (specular)
				{
					SetTexture_ShineMask(newMaterial, id.ToLowerInvariant() + "_spec", specularColor);
				}

				if (!normal.IsNullOrWhiteSpace())
				{
					SetTexture_NormalNoise(newMaterial, normal);
				}
			}

			Substance substance = ModUtil.CreateSubstance(id, state, animFile, newMaterial, color, uiColor,
				conduitColor);
			substance.anims = [animFile];
			return substance;
		}

		private static void SetTexture(Material material, string texture, string property)
		{
			var path = Path.Combine(UtilMethods.ModPath, "assets", "textures", texture + ".png");

			if (TryLoadTexture(path, out var tex))
			{
				material.SetTexture(property, tex);
			}
		}

		public static bool TryLoadTexture(string path, out Texture2D texture)
		{
			texture = LoadTexture(path, true);
			return texture != null;
		}

		public static Texture2D LoadTexture(string path, bool warnIfFailed = true)
		{
			Texture2D texture = null;

			if (File.Exists(path))
			{
				var data = TryReadFile(path);
				texture = new Texture2D(1, 1);
				texture.LoadImage(data);
			}
			else if (warnIfFailed)
			{
				SgtLogger.dlogwarn($"Could not load texture at path {path}.");
			}

			return texture;
		}

		public static byte[] TryReadFile(string texFile)
		{
			try
			{
				return File.ReadAllBytes(texFile);
			}
			catch (Exception e)
			{
				SgtLogger.dlogwarn("Could not read file: " + e);
				return null;
			}
		}

		public static void AddModifier(Element element, float decor, float overHeat)
		{
			if (decor != 0)
			{
				element.attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, decor,
					element.name, true));
			}

			if (overHeat != 0)
			{
				element.attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id,
					overHeat, element.name, false));
			}
		}

		public static ElementsAudio.ElementAudioConfig GetCrystalAudioConfig(SimHashes id)
		{
			var crushedIce = ElementsAudio.Instance.GetConfigForElement(SimHashes.CrushedIce);

			return new ElementsAudio.ElementAudioConfig()
			{
				elementID = id,
				ambienceType = AmbienceType.None,
				solidAmbienceType = SolidAmbienceType.CrushedIce,
				miningSound = "PhosphateNodule", // kind of gritty glassy
				miningBreakSound = crushedIce.miningBreakSound,
				oreBumpSound = crushedIce.oreBumpSound,
				floorEventAudioCategory = "tileglass", // proper glassy sound
				creatureChewSound = crushedIce.creatureChewSound
			};
		}

		public static ElementsAudio.ElementAudioConfig CopyElementAudioConfig(
			ElementsAudio.ElementAudioConfig reference, SimHashes id)
		{
			return new ElementsAudio.ElementAudioConfig()
			{
				elementID = reference.elementID,
				ambienceType = reference.ambienceType,
				solidAmbienceType = reference.solidAmbienceType,
				miningSound = reference.miningSound,
				miningBreakSound = reference.miningBreakSound,
				oreBumpSound = reference.oreBumpSound,
				floorEventAudioCategory = reference.floorEventAudioCategory,
				creatureChewSound = reference.creatureChewSound,
			};
		}

		public static ElementsAudio.ElementAudioConfig CopyElementAudioConfig(SimHashes referenceId, SimHashes id)
		{
			var reference = ElementsAudio.Instance.GetConfigForElement(referenceId);

			return new ElementsAudio.ElementAudioConfig()
			{
				elementID = reference.elementID,
				ambienceType = reference.ambienceType,
				solidAmbienceType = reference.solidAmbienceType,
				miningSound = reference.miningSound,
				miningBreakSound = reference.miningBreakSound,
				oreBumpSound = reference.oreBumpSound,
				floorEventAudioCategory = reference.floorEventAudioCategory,
				creatureChewSound = reference.creatureChewSound,
			};
		}
	}
}