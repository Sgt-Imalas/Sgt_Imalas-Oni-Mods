using Klei.AI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtilLibs;

namespace ElementUtilNamespace
{
	public class SgtElementUtil
	{
		public static readonly Dictionary<SimHashes, string> SimHashNameLookup = new Dictionary<SimHashes, string>();
		public static readonly Dictionary<string, object> ReverseSimHashNameLookup = new Dictionary<string, object>();
		public static readonly List<ElementInfo> elements = new List<ElementInfo>();


		public static bool SimHashToString_EnumPatch(Enum __instance, ref string __result)
		{
			if (__instance is SimHashes hashes)
			{
				return !SimHashNameLookup.TryGetValue(hashes, out __result);
			}

			return true;
		}
		public static bool SimhashParse_EnumPatch(Type enumType, string value, ref object __result)
		{
			if (enumType.Equals(typeof(SimHashes)))
			{
				return !SgtElementUtil.ReverseSimHashNameLookup.TryGetValue(value, out __result);
			}
			return true;
		}



		public static SimHashes RegisterSimHash(string name)
		{
			var simHash = (SimHashes)Hash.SDBMLower(name);
			SgtLogger.l("Element: " + name + ", simhash " + simHash);
			SimHashNameLookup.Add(simHash, name);
			ReverseSimHashNameLookup.Add(name, simHash);

			return simHash;
		}

		public static void SetTexture_Main(Material material, string texture) => SetTexture(material, texture, "_MainTex");
		public static void SetTexture_ShineMask(Material material, string texture, Color? specularColor = null)
		{
			SetTexture(material, texture, "_ShineMask");
			if (specularColor.HasValue)
				material.SetColor("_ShineColour", specularColor.Value);

		}

		public static void SetTexture_NormalNoise(Material material, string normal) => SetTexture(material, normal, "_NormalNoise");

		public static Substance CreateSubstance(SimHashes id, bool specular, string anim, Element.State state, Color color, Material material, Color uiColor, Color conduitColor, Color? specularColor, string normal)
		{
			var animFile = Assets.Anims.Find(a => a.name == anim);

			if (animFile == null)
			{
				animFile = Assets.Anims.Find(a => a.name == "glass_kanim");
			}

			var newMaterial = new Material(material);

			if (state == Element.State.Solid)
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

			Substance substance = ModUtil.CreateSubstance(id.ToString(), state, animFile, newMaterial, color, uiColor, conduitColor);
			substance.anims = [animFile]; 
			return substance;
		}

		// TODO: load from an assetbundle later
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
				element.attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, decor, element.name, true));
			}

			if (overHeat != 0)
			{
				element.attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, overHeat, element.name, false));
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

		public static ElementsAudio.ElementAudioConfig CopyElementAudioConfig(ElementsAudio.ElementAudioConfig reference, SimHashes id)
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
