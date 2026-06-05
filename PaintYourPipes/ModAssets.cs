using PeterHan.PLib.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static InventoryOrganization;
using static PaintYourPipes.STRINGS;
using static UtilLibs.SupplyClosetUtils;

namespace PaintYourPipes
{
	internal class ModAssets
	{
		public static HashSet<string> AnimsToGreyScale = [
			"utilities_conveyorbridge_kanim",
			"utilityliquidbridge_kanim",
			"utilitygasbridge_kanim",

			"utilities_liquid_kanim",
			"utilities_liquid_radiant_kanim",
			"utilities_liquid_insulated_kanim",

			"utilities_gas_kanim",
			"utilities_gas_radiant_kanim",
			"utilities_gas_insulated_kanim",

			"utilities_conveyor_kanim",

			"utilities_electric_kanim",
			"utilities_electric_insulated_kanim",
			"utilities_electric_conduct_kanim",
			"utilities_electric_conduct_hiwatt_kanim",
			"utilities_electric_rubber_kanim",

			"utilityelectricbridge_kanim",
			"heavywatttile_kanim",
			"utilityelectricbridgeconductive_kanim",
			"heavywatttile_conductive_kanim",
			"utilityelectricbridgerubber_kanim",

			"logic_wires_kanim",
			"logic_ribbon_kanim",
			"logic_bridge_kanim",
			"logic_ribbon_bridge_kanim",

			"logic_buffer_kanim",
			"logic_filter_kanim",
			"logic_memory_kanim",
			"logic_ribbon_reader_kanim",
			"logic_ribbon_writer_kanim",

			///GigawattWire:
			"gigawatt_wire_kanim",
			"gigawatt_wire_bridge_kanim",
			"jacketed_wire_kanim",
			"jacketed_wire_bridge_kanim",
			"megawatt_wire_kanim",
			"megawatt_wire_bridge_kanim",
			
			///HPA:
			///gas
			"pressure_gas_pipe_kanim",
			"pressure_gas_bridge_kanim",
			///liquid
			"pressure_liquid_pipe_kanim",
			"pressure_liquid_bridge_kanim",
			///solid
			"hpa_rail_insulated_kanim",
			"hpa_rail_kanim",
			"hpa_rail_bridge_kanim",
			///DupesLogistics:
			"logistic_rail_kanim",
			"logistic_bridge_kanim",

			///PlasticUtilities:
			"plastic_utilities_gas_kanim",
			"plastic_utilities_liquid_kanim"
				];

		public static bool TryGetGreyScaleAnim(string animName, out KAnimFile anim) => greyscaleAnims.TryGetValue(animName, out anim);
		//{
		//	SgtLogger.l($"Trying to get greyscale anim for {animName}");
		//	if (!greyscaleAnims.TryGetValue(animName, out anim))
		//	{
		//		var originalAnim = Assets.GetAnim(animName);
		//		if(originalAnim == null)
		//		{
		//			SgtLogger.error($"Couldn't find original anim for {animName}");
		//			return false;
		//		}
		//		anim = MakeGreyscaleSkin(originalAnim);
		//	}
		//	return anim != null;
		//}

		static Dictionary<string, KAnimFile> greyscaleAnims = new();

		public const string SubCategoryID = "PYP_GREYSCALE_SKINS";
		static SkinCollection category = null;

		internal static void AssignGreyScaleSkin(BuildingDef result)
		{
			return;
			var anim = result.AnimFiles[0];
			if (anim == null)
			{
				return;
			}
			if (greyscaleAnims.TryGetValue(anim.name, out var greyAnim))
			{
				if (category == null)
				{
					SkinCollection.CategoryInit(InventoryPermitCategories.BUILDINGS, SubCategoryID, Assets.GetSprite("brush"), 1);
					SkinCollection.RegisterSkinInjectionPatch();
				}
				string buildingId = result.PrefabID.ToString();
				string buildingName = result.Name;
				string skinId = "PYP_" + buildingId + "_greyscaled";

				if (!InventoryOrganization.subcategoryIdToPermitIdsMap.ContainsKey(SubCategoryID))
					InventoryOrganization.subcategoryIdToPermitIdsMap[SubCategoryID] = new List<string>();

				SupplyClosetUtils.AddItemsToSubcategory(SubCategoryID, [skinId]);
				//InventoryOrganization.subcategoryIdToPermitIdsMap[SubCategoryID].Add(skinId);
				SgtLogger.l($"Added greyscale skin to {result.Name}");
				//Db.GetBuildingFacades().Add(skinId, string.Format(STRISKININFO.NAME, buildingName), string.Format(SKININFO.DESC, buildingName), Database.PermitRarity.Universal, buildingId, greyAnim,null,null,null,null);
			}
		}
		internal static void MakeGreyscaleVariantsForValidAnims(IEnumerable<KAnimFile> animList)
		{
			foreach (var existingAnim in animList)
			{
				if (existingAnim == null || !AnimsToGreyScale.Contains(existingAnim.name))
					continue;
				MakeGreyscaleSkin(existingAnim);
			}
		}
		internal static KAnimFile MakeGreyscaleSkin(KAnimFile existingAnim)
		{
			if (greyscaleAnims.ContainsKey(existingAnim.name))
				return greyscaleAnims[existingAnim.name];

			List<Texture2D> greyScaledTextures = [];
			foreach (var texture in existingAnim.textureList)
			{
				greyScaledTextures.Add(GetGreyScaleTexture(texture, texture.name + "_greyscale"));
			}
			KAnimFile.Mod desatModFile = new()
			{
				anim = existingAnim.animBytes,
				build = existingAnim.buildBytes,
				textures = greyScaledTextures,
			};
			var modifiedAnimName = existingAnim.name.Replace("_kanim", "_greyscale_kanim");
			var modified = ModUtil.AddKAnimMod(modifiedAnimName, desatModFile);
			SgtLogger.l("Adding greyscale anim with anim name " + modifiedAnimName);
			if (modified != null)
			{
				SgtLogger.l("Anim was created successfully");
				greyscaleAnims.Add(existingAnim.name, modified);
			}
			else
			{
				SgtLogger.error("Failed to add greyscale anim");
			}
			if(Assets.Anims != null && !Assets.Anims.Contains(modified))
				Assets.Anims.Add(modified);
			return modified;
		}

		static Texture2D GetGreyScaleTexture(Texture sourceTexture, string name)
		{
			Texture2D newTexture = GetReadableCopy(sourceTexture as Texture2D);
			var pixels = newTexture.GetPixels32();
			for (int i = 0; i < pixels.Length; ++i)
			{

				Color32 p = pixels[i];
				float gray = Mathf.Clamp01(((Color)p).grayscale * 1.5f);
				byte g = (byte)(gray * 255);
				pixels[i] = new Color32(g, g, g, p.a);
			}
			newTexture.SetPixels32(pixels);
			newTexture.Apply();
			newTexture.name = name;
			return newTexture;
		}

		/// <summary>
		/// Returns a readable copy of the texture, so it can be modified.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static Texture2D GetReadableCopy(Texture2D source)
		{
			if (source == null || source.width == 0 || source.height == 0) return null;

			RenderTexture renderTex = RenderTexture.GetTemporary(
						source.width,
						source.height,
						0,
						RenderTextureFormat.Default,
						RenderTextureReadWrite.Linear);

			Graphics.Blit(source, renderTex);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			Texture2D readableText = new Texture2D(source.width, source.height);


			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			return readableText;
		}


		public class HotKeys
		{
			public static PAction ToggleOverlayColors { get; private set; }

			public const string TOGGLE_OVERLAY = "PYP_TOGGLE_COLORING_OVERLAY";

			public static void Register()
			{
				ToggleOverlayColors = new PActionManager().CreateAction(
					TOGGLE_OVERLAY,
					STRINGS.HOTKEYACTIONS.TOGGLE_OVERLAY_COLOR,
					new PKeyBinding());

			}
		}
	}
}
