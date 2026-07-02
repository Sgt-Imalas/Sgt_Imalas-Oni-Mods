using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using UtilLibs;
using System.Linq;

namespace ElementUtilNamespace
{
	public static class SgtElementUtil
	{
		public static readonly Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>();
		//public static readonly Dictionary<string, string> StringifiedSimHashToTagLookup = new Dictionary<string, string>();
		public static readonly Dictionary<string, object> ReverseSimHashNameLookup = new Dictionary<string, object>();
		public static readonly List<ElementInfo> Elements = new List<ElementInfo>();

		//Unused, alternative approach to enum.parse patch
		static class EnumParseRedirect
		{
			static List<MethodInfo> SimhashesEnumParsers = [
				AccessTools.Method(typeof(ElementLoader),nameof(ElementLoader.GetID)),
				AccessTools.Method(typeof(DevToolGeyserModifiers),nameof(DevToolGeyserModifiers.RenderTo)),
				AccessTools.Method(typeof(ProcGenGame.WorldGen),nameof(ProcGenGame.WorldGen.EnsureEnoughElementsInStartingBiome)),
				AccessTools.Method(typeof(DebugPaintElementScreen),nameof(DebugPaintElementScreen.OnSelectElement), [typeof(string),typeof(int)]),
				];
			public static void PatchEnumParseLocations(Harmony harmony)
			{
				foreach (var target in SimhashesEnumParsers)
				{
					if (target == null)
					{
						SgtLogger.l("Failed to find method for simhash enum parse patching");
						continue;
					}
					SgtLogger.l("Patching simhash enum parse location: " + target.DeclaringType.FullName + "." + target.Name);
					harmony.Patch(target, transpiler: new HarmonyMethod(typeof(EnumParseRedirect), nameof(SimhashEnumParseTranspiler)));
				}
			}

			static IEnumerable<CodeInstruction> SimhashEnumParseTranspiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var m_EnumParse = AccessTools.Method(typeof(Enum), nameof(Enum.Parse), [typeof(Type), typeof(string)]);
				var m_EnumParseIgnoreCase = AccessTools.Method(typeof(Enum), nameof(Enum.Parse), [typeof(Type), typeof(string), typeof(bool)]);

				List<CodeInstruction> codes = orig.ToList();
				List<CodeInstruction> output = [];


				for (int i = 0; i < codes.Count; i++)
				{
					var ci = codes[i];

					bool wrapNormal = ci.Calls(m_EnumParse);
					bool wrapIgnoreCase = ci.Calls(m_EnumParseIgnoreCase);

					///Grabs the string from the stack and caches it, then pushes a valid vanilla simhash name (steel) to avoid parse exceptions
					var wrapPrefix = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EnumParseRedirect), nameof(CacheAndRedirectElementId)));

					if (wrapNormal)
						output.Add(wrapPrefix);
					else if (wrapIgnoreCase)///needs to be inserted 1 earlier bc otherwise the bool argument for ignorecase will be on top of the stack instead of the string id
						output.Insert(output.Count - 1, wrapPrefix);

					output.Add(ci);

					//after the enum parse call, swap the result with our own modded simhash if applicable
					if (wrapNormal || wrapIgnoreCase)
						output.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EnumParseRedirect), nameof(SwapEnumParseResult))));
				}
				//SgtLogger.l("ENUM TRANSPILER APPLIED");
				//TranspilerHelper.PrintInstructions(output);
				return output;
			}
			static string _cachedElementName;

			static string CacheAndRedirectElementId(string elementId)
			{
				if (ReverseSimHashNameLookup.ContainsKey(elementId))
				{
					//SgtLogger.l("EnumParse: replacing modded " + elementId);
					_cachedElementName = elementId;
					return "Steel"; // Return a valid SimHashes name to avoid exceptions
				}
				else
					_cachedElementName = null;
				return elementId;
			}
			static object SwapEnumParseResult(object result)
			{
				if (_cachedElementName != null && ReverseSimHashNameLookup.TryGetValue(_cachedElementName, out object simHash))
				{
					return simHash;
				}
				return result;
			}
		}

		public static void ExecuteElementEnumPatches(Harmony harmony)
		{
			SgtLogger.l("Attempting to patch Enum.ToString's internal InternalFormat method...");
			try
			{
				var original = AccessTools.Method(typeof(Enum), "InternalFormat");
                var m_postfix = new HarmonyMethod(typeof(SgtElementUtil), nameof(SimHashInternalFormat_EnumPatch));
                harmony.Patch(original, postfix: m_postfix);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error while patching Enum.InternalFormat:\n" + e);
			}

			SgtLogger.l("Attempting to patch Enum.Parse...");
			try
			{
				var original = AccessTools.Method(typeof(Enum), nameof(Enum.Parse), [typeof(Type), typeof(string), typeof(bool)]);
				var m_postfix = new HarmonyMethod(typeof(SgtElementUtil), nameof(SimhashParse_EnumPatch));
				harmony.Patch(original, postfix: m_postfix);
			}
			catch (Exception e)
			{
				SgtLogger.error("Error while patching Enum.Parse:\n" + e);
				return;
			}
			SgtLogger.l("Element enum patches successful!");
		}

		//public static void TagManager_CreatePatch(ref string tag_string)
		//{
		//	if(StringifiedSimHashToTagLookup.TryGetValue(tag_string, out string corrected))
		//		tag_string = corrected;
		//}

		public static void SimHashInternalFormat_EnumPatch(Type eT, object value, ref string __result)
		{
            if (eT == typeof(SimHashes) && SimHashNameLookup.TryGetValue((SimHashes)value, out string id))
			{
				__result = id;
			}
		}

		public static bool SimHashToString_EnumPatch(Enum __instance, ref string __result)
		{
			if (__instance is SimHashes hashes)
			{
				return !SimHashNameLookup.TryGetValue(hashes, out __result);
			}

			return true;
		}

		public static void SimhashParse_EnumPatch(Type enumType, string value, ref object __result)
		{
			if (enumType == typeof(SimHashes) && SgtElementUtil.ReverseSimHashNameLookup.TryGetValue(value, out object id))
			{
				__result = id;
			}
		}

		public static SimHashes RegisterSimHash(string name)
		{
			var simHash = (SimHashes)Hash.SDBMLower(name);
			SgtLogger.l("Element: " + name + ", simhash " + simHash);
			SimHashNameLookup.Add(simHash, name);
			//StringifiedSimHashToTagLookup.Add(simHash.ToString(), name);
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

		public static Substance MakeMoltenMetal(this Substance substance)
		{
			substance.glows = true;
			substance.isOpaqueLiquid = true;
			substance.metalic = true;
			substance.texture = Substance.SubstanceTexture.MoltenMetal;
			return substance;
		}
		public static Substance MakeLiquidifedAtmosphericGas(this Substance substance)
		{
			///currently; liquidifed gases only seem to have these swirls + some gradient but im not adding that gradient here for now
			substance.CausticSwirls();
			return substance;
		}
		public static Substance Glows(this Substance substance, bool val = true)
		{
			substance.glows = val;
			return substance;
		}
		public static Substance Gradient(this Substance substance, Gradient val)
		{
			substance.gradient = val;
			return substance;
		}
		public static Substance CausticSwirls(this Substance substance, bool val = true)
		{
			substance.usesCaustics = val;
			return substance;
		}
		public static Substance Opaque(this Substance substance, bool val = true)
		{
			substance.isOpaqueLiquid = val;
			return substance;
		}
		public static Substance Metallic(this Substance substance, bool val = true)
		{
			substance.metalic = val;
			return substance;
		}
		public static Substance Texture(this Substance substance, Substance.SubstanceTexture texture)
		{
			substance.texture = texture;
			return substance;
		}
		public static Substance MaterialFloatProperty(this Substance substance, string propertyName, float value)
		{
			substance.material.SetFloat(propertyName, value);
			return substance;
		}


		public static Substance CreateSubstance(SimHashes id, bool specular, string anim, Element.State state,
			Color color, Material material, Color uiColor, Color conduitColor, Color? specularColor, string normal,
			bool isCloned = false)
		{
			var animFile = Assets.Anims.Find(a => a.name == anim);

			if (animFile == null)
			{
				animFile = Assets.Anims.Find(a => a.name == "glass_kanim");
			}

			var newMaterial = new Material(material);

			if (state == Element.State.Solid && !isCloned)
			{
				SetTexture_Main(newMaterial, id.ToString().ToLowerInvariant());

				if (specular)
				{
					SetTexture_ShineMask(newMaterial, id.ToString().ToLowerInvariant() + "_spec", specularColor);
				}

				if (!normal.IsNullOrWhiteSpace())
				{
					SetTexture_NormalNoise(newMaterial, normal);
				}
			}

			Substance substance = ModUtil.CreateSubstance(id.ToString(), state, animFile, newMaterial, color, uiColor,
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