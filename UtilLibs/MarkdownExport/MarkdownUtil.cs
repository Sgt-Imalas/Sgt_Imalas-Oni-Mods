using ClipperLib;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using static GameUtil;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public static class MarkdownUtil
	{
		public static string FormatLineBreaks(string input) => input.Replace("\n", "<br/>");
		public static string GetTagName(Tag tag)
		{
			var prefab = Assets.TryGetPrefab(tag);
			if (prefab == null)
				return Strip(L("STRINGS.MISC.TAGS." + tag.ToString().ToUpperInvariant()));

			if (ElementLoader.GetElement(tag) != null)
				return Strip(L("STRINGS.ELEMENTS." + tag.ToString().ToUpperInvariant() + ".NAME"));
			if (Assets.GetBuildingDef(tag.ToString()) != null)
				return Strip(L("STRINGS.BUILDINGS.PREFABS." + tag.ToString().ToUpperInvariant() + ".NAME"));

			return Strip(prefab.GetProperName());
		}
		public static string GetElementState(Element.State state)
		{
			state = state & Element.State.Solid;

			switch (state)
			{
				case Element.State.Solid:
					return MarkdownUtil.GetTagName(GameTags.Solid);
				case Element.State.Liquid:
					return MarkdownUtil.GetTagName(GameTags.Liquid);
				case Element.State.Gas:
					return MarkdownUtil.GetTagName(GameTags.Gas);
			}
			throw new NotImplementedException();
		}
		public static string GetElementState(ConduitType state)
		{
			switch (state)
			{
				case ConduitType.Solid:
					return MarkdownUtil.GetTagName(GameTags.Solid);
				case ConduitType.Liquid:
					return MarkdownUtil.GetTagName(GameTags.Liquid);
				case ConduitType.Gas:
					return MarkdownUtil.GetTagName(GameTags.Gas);
			}
			throw new NotImplementedException();
		}

		public static string Strip(string input) => STRINGS.UI.StripLinkFormatting(input);
		public static string StrippedBuildingName(string ID) => Strip(L($"STRINGS.BUILDINGS.PREFABS.{ID.ToUpperInvariant()}.NAME"));

		public static string GetPortDescription(ConduitType conduitType, bool input, string material = null)
		{
			if (material == null)
				material = GetElementState(conduitType);

			switch (conduitType)
			{
				case ConduitType.Gas:
				case ConduitType.Liquid:
					return string.Format(L(input ? "PIPE_INPUT" : "PIPE_OUTPUT"), material);
				case ConduitType.Solid:
					return string.Format(L(input ? "RAIL_INPUT" : "RAIL_OUTPUT"), material);
			}
			throw new NotImplementedException();
		}

		static Dictionary<Texture2D, Texture2D> Copies = new Dictionary<Texture2D, Texture2D>();
		public static Texture2D GetReadableCopy(Texture2D source)
		{
			if (Copies.ContainsKey(source))
				return Copies[source];

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
			Copies[source] = readableText;
			return readableText;
		}

		static Dictionary<Sprite, Texture2D> Copies2 = new Dictionary<Sprite, Texture2D>();
		static Texture2D GetSingleSpriteFromTexture(Sprite sprite, Color tint = default)
		{
			if (sprite == null || sprite.rect == null || sprite.rect.width <= 0 || sprite.rect.height <= 0)
				return null;

			bool useTint = tint != default;

			if (useTint || !Copies2.ContainsKey(sprite))
			{
				var output = new Texture2D(Mathf.RoundToInt(sprite.textureRect.width), Mathf.RoundToInt(sprite.textureRect.height));
				var r = sprite.textureRect;
				if (r.width == 0 || r.height == 0)
					return null;

				var readableTexture = GetReadableCopy(sprite.texture);

				if (readableTexture == null)
					return null;

				var pixels = readableTexture.GetPixels(Mathf.RoundToInt(r.x), Mathf.RoundToInt(r.y), Mathf.RoundToInt(r.width), Mathf.RoundToInt(r.height));
				if (useTint)
				{
					var tintedPixels = new Color[pixels.Length];
					for (int i = 0; i < pixels.Length; i++)
					{
						tintedPixels[i] = pixels[i] * tint;
					}
					//SgtLogger.l(Mathf.RoundToInt(output.width)* Mathf.RoundToInt(output.height)+" > "+tintedPixels.Length+" ?");
					output.SetPixels(tintedPixels);
				}
				else
				{
					output.SetPixels(pixels);
				}
				output.Apply();
				output.name = sprite.texture.name + " " + sprite.name;

				if (useTint)
					return output;

				Copies2.Add(sprite, output);
			}
			return Copies2[sprite];
		}
		public static void WriteUISpriteToFile(Sprite sprite, string folder, string id, Color tint = default)
		{
			id = SanitationUtils.SanitizeName(id);

			Directory.CreateDirectory(folder);
			string fileName = Path.Combine(folder, id + ".png");
			var tex = GetSingleSpriteFromTexture(sprite, tint);

			if (tex == null)
				return;

			var imageBytes = tex.EncodeToPNG();
			File.WriteAllBytes(fileName, imageBytes);
		}

		internal static string GetElementTransitionProperties(Element element)
		{
			string property = "";
			string transitionsInto_low = "";
			if (element.lowTempTransition != null)
			{
				transitionsInto_low = "<br/>" + Math.Round(GameUtil.GetTemperatureConvertedFromKelvin(element.lowTemp, TemperatureUnit.Celsius), 2).ToString()+"°C";
				transitionsInto_low += " -> " + GetTagName(element.lowTempTransition.tag);
				if (element.lowTempTransitionOreID != SimHashes.Vacuum && element.lowTempTransitionOreMassConversion > 0)
					transitionsInto_low += ", " + GetTagName(element.lowTempTransitionOreID.CreateTag());
				transitionsInto_low += "<br/>";
			}

			string transitionsInto_high = "";
			if (element.highTempTransition != null)
			{
				transitionsInto_high = "<br/>"+Math.Round(GameUtil.GetTemperatureConvertedFromKelvin(element.highTemp, TemperatureUnit.Celsius), 2).ToString() + "°C";
				transitionsInto_high += " -> " + GetTagName(element.highTempTransition.tag);
				if (element.highTempTransitionOreID != SimHashes.Vacuum && element.highTempTransitionOreMassConversion > 0)
					transitionsInto_high += ", " + GetTagName(element.highTempTransitionOreID.CreateTag());
				transitionsInto_high += "<br/>";
			}
			var categoryTag = GetTagName(element.materialCategory) + "<br/>";

			if (element.highTempTransition != null && element.lowTempTransition == null)
			{
				property += FormatLineBreaks(string.Format(L("STRINGS.ELEMENTS.ELEMENTDESCSOLID"), categoryTag, transitionsInto_high, element.hardness));
			}
			else if (element.highTempTransition != null && element.lowTempTransition != null)
			{
				property += FormatLineBreaks(string.Format(L("STRINGS.ELEMENTS.ELEMENTDESCLIQUID"), categoryTag , transitionsInto_low, transitionsInto_high));
			}
			else if (element.highTempTransition == null && element.lowTempTransition != null)
			{
				property += FormatLineBreaks(string.Format(L("STRINGS.ELEMENTS.ELEMENTDESCGAS"), categoryTag , transitionsInto_low));
			}

			property += "<br/><br/>";


			var oreTags = element.oreTags.Distinct().Where(tag => tag != element.materialCategory);

			if (oreTags.Any())
			{
				var tags = string.Join(", ",
						oreTags
						.Select(t => MarkdownUtil.GetTagName(t))
						.StableSort()
						.Where(val => !val.Contains("MISSING")));
				property += string.Format(L("STRINGS.ELEMENTS.ELEMENTPROPERTIES"), tags);
			}


			return property;
		}
		internal static string GetElementPhysicalProperties(Element element)
		{
			string property = "";

			property += FormatLineBreaks(
				L("STRINGS.ELEMENTS.THERMALPROPERTIES").TrimStart('\n')
				.Replace("{SPECIFIC_HEAT_CAPACITY}", "<br/>"+GameUtil.GetFormattedSHC(element.specificHeatCapacity))
				.Replace("{THERMAL_CONDUCTIVITY}", "<br/>" + GameUtil.GetFormattedThermalConductivity(element.thermalConductivity)));

			property += "<br/>";

			property += FormatLineBreaks(
				string.Format(L("STRINGS.ELEMENTS.RADIATIONPROPERTIES"),
				element.radiationAbsorptionFactor,
				GameUtil.GetFormattedRads((element.radiationPer1000Mass * 1.10000002384186f / 600.0f), GameUtil.TimeSlice.PerCycle)));

			return property;
		}
	}
}
