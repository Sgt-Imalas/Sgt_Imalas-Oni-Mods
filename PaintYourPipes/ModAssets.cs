using PeterHan.PLib.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

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
				];
		static Dictionary<string, KAnimFile> greyscaleAnims = new();
		internal static void AssignGreyScaleSkin(BuildingDef result)
		{
			var anim = result.AnimFiles[0];
			if(anim == null)
			{
				return;
			}
			var animName = anim.name.Replace("_kanim", "_greyscale_kanim");
			if(greyscaleAnims.TryGetValue(anim.name, out var greyAnim))
			{
				result.AnimFiles[0] = greyAnim;
				SgtLogger.l($"Assigned greyscale skin to {result.Name}");
			}
			else
			{
				SgtLogger.error($"Greyscale anim not found for {result.Name}, anim name: {animName}");
			}
		}
		internal static void MakeGreyscaleVariantsForValidAnims(IEnumerable<KAnimFile> animList)
		{
			foreach(var existingAnim in animList)
			{
				if (existingAnim == null || !AnimsToGreyScale.Contains(existingAnim.name))
					continue;
				MakeGreyscaleSkin(existingAnim);
			}
		}
		internal static void MakeGreyscaleSkin(KAnimFile existingAnim)
		{
			List<Texture2D> greyScaledTextures = [];
			foreach (var texture in existingAnim.textures)
			{
				greyScaledTextures.Add(GetGreyScaleTexture(texture, texture.name + "_greyscale"));
			}
			KAnimFileData data = existingAnim.GetData();

			KAnimFile.Mod desatModFile = new()
			{
				anim = existingAnim.animBytes,
				build = existingAnim.buildBytes,
				textures = greyScaledTextures,
			};
			var animName = existingAnim.name.Replace("_kanim", "_greyscale_kanim");
			var modified = ModUtil.AddKAnimMod(animName, desatModFile);
			SgtLogger.l("Adding greyscale anim with anim name " + animName );
			if (modified != null)
			{
				SgtLogger.l("Anim was created successfully");
				greyscaleAnims.Add(existingAnim.name, modified);
			}
			else
			{
				SgtLogger.error("Failed to add greyscale anim");
			}
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
