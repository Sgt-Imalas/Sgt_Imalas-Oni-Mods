using ClipperLib;
using FMOD;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GameUtil;
using static STRINGS.CREATURES.STATUSITEMS;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public static class MarkdownUtil
	{
		static void CleanTag(ref string tagKey)
		{
			if (tagKey.Contains("SPICEVINE"))
				tagKey = tagKey.Replace("SPICEVINE", "SPICE_VINE");

			if (tagKey.Contains("FORESTTREE"))
				tagKey = tagKey.Replace("FORESTTREE", "WOOD_TREE");

			if (tagKey.Contains("GASGRASSHARVESTED"))
				tagKey = tagKey.Replace("GASGRASSHARVESTED", "GASGRASS");

			if (tagKey.Contains("BLUEGRASS"))
				tagKey = tagKey.Replace("BLUEGRASS", "BLUE_GRASS");
		}

		public static string GetTagStringWithIcon(Tag tag, bool prefix = true)
		{
			string element = tag.ToString();
			string tagName = GetTagString(tag);
			string iconCategory = "elements";
			if (ElementLoader.GetElement(tag) == null)
			{
				Exporter.AddEntity(tag);
				iconCategory = "entities";
			}

			///.inline-icon is a css property in the wiki main css:

			//.inline - icon {
			//	vertical - align: top;
			//	width: 18px;
			//}

			if (prefix)
				return $" ![{element}](/assets/images/{iconCategory}/{element}.png){{.inline-icon}} {GetTagString(element)}";
			else
				return $"{GetTagString(element)} ![{element}](/assets/images/{iconCategory}/{element}.png){{.inline-icon}}";

		}
		public static string GetTagString(Tag tag, bool desc = false)
		{
			string endKey = ".NAME";
			if (desc)
				endKey = ".DESC";

			var tagKey = tag.ToString().ToUpperInvariant();

			//those dont follow the pattern...
			CleanTag(ref tagKey);



			var prefab = Assets.TryGetPrefab(tag);
			if (prefab == null)
			{
				if (desc)
					tagKey += "_DESC";
				return Strip(L("STRINGS.MISC.TAGS." + tagKey));
			}

			if (ElementLoader.GetElement(tag) != null)
				return Strip(L("STRINGS.ELEMENTS." + tag.ToString().ToUpperInvariant() + endKey));
			if (Assets.GetBuildingDef(tag.ToString()) != null)
				return Strip(L("STRINGS.BUILDINGS.PREFABS." + tag.ToString().ToUpperInvariant() + endKey));

			if (tagKey.Contains("SEED") && prefab.TryGetComponent<PlantableSeed>(out var seed))
			{
				var plantID = seed.PlantID.ToString().ToUpperInvariant();
				CleanTag(ref plantID);
				var seedKey = "STRINGS.CREATURES.SPECIES.SEEDS." + plantID + endKey;
				//SgtLogger.l("SeedKey: " + seedKey);
				if (HasKey(seedKey))
					return Strip(L(seedKey));
			}
			if (tagKey.Contains("GEYSERGENERIC_"))
			{
				var geyserID = tagKey.Replace("GEYSERGENERIC_", string.Empty);
				var geyserKey = "STRINGS.CREATURES.SPECIES.GEYSER." + geyserID + endKey;
				if (HasKey(geyserKey))
					return Strip(L(geyserKey));
			}


			var prod = "STRINGS.ITEMS.INDUSTRIAL_PRODUCTS." + tagKey + endKey;
			if (HasKey(prod))
				return Strip(L(prod));
			var ingredient = "STRINGS.ITEMS.INGREDIENTS." + tagKey + endKey;
			if (HasKey(ingredient))
				return Strip(L(ingredient));
			var food = "STRINGS.ITEMS.FOOD." + tagKey + endKey;
			if (HasKey(food))
				return Strip(L(food));
			var creature = "STRINGS.CREATURES.SPECIES." + tagKey + endKey;
			if (HasKey(creature))
				return Strip(L(creature));
			var comet = "STRINGS.UI.SPACEDESTINATIONS.COMETS." + tagKey + endKey;
			if (HasKey(comet))
				return Strip(L(comet));

			if (MD_Localization.TryGetManuallyRegistered(tagKey, out var loc))
				return Strip(loc);

			if (desc)
			{
				if (prefab.TryGetComponent<InfoDescription>(out var info))
					return Strip(info.description);
				return GetTagString(tag, false);
			}

			return Strip(prefab.GetProperName());
		}
		public static string GetElementState(Element.State state)
		{
			state = state & Element.State.Solid;

			switch (state)
			{
				case Element.State.Solid:
					return MarkdownUtil.GetTagString(GameTags.Solid);
				case Element.State.Liquid:
					return MarkdownUtil.GetTagString(GameTags.Liquid);
				case Element.State.Gas:
					return MarkdownUtil.GetTagString(GameTags.Gas);
			}
			throw new NotImplementedException();
		}
		public static string GetElementState(ConduitType state)
		{
			switch (state)
			{
				case ConduitType.Solid:
					return MarkdownUtil.GetTagString(GameTags.Solid);
				case ConduitType.Liquid:
					return MarkdownUtil.GetTagString(GameTags.Liquid);
				case ConduitType.Gas:
					return MarkdownUtil.GetTagString(GameTags.Gas);
			}
			throw new NotImplementedException();
		}

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
		#region spriteGetters
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
		#endregion
		internal static string GetElementTransitionProperties(Element element)
		{
			string property = "<br/>";
			string transitionsInto_low = "";
			if (element.lowTempTransition != null)
			{
				transitionsInto_low = Math.Round(GameUtil.GetTemperatureConvertedFromKelvin(element.lowTemp, TemperatureUnit.Celsius), 2).ToString() + "°C";
				string transitionPercentage = element.lowTempTransitionOreMassConversion > 0 ? (1 - element.lowTempTransitionOreMassConversion).ToString("P0") + " " : "->";
				transitionsInto_low += "<br/>" + transitionPercentage + GetTagStringWithIcon(element.lowTempTransition.tag);
				if (element.lowTempTransitionOreID != SimHashes.Vacuum && element.lowTempTransitionOreMassConversion > 0)
					transitionsInto_low += ",<br/>" + element.lowTempTransitionOreMassConversion.ToString("P0") + GetTagStringWithIcon(element.lowTempTransitionOreID.CreateTag());
				transitionsInto_low += "<br/>";
			}

			string transitionsInto_high = "";
			if (element.highTempTransition != null)
			{
				transitionsInto_high = Math.Round(GameUtil.GetTemperatureConvertedFromKelvin(element.highTemp, TemperatureUnit.Celsius), 2).ToString() + "°C";
				string transitionPercentage = element.highTempTransitionOreMassConversion > 0 ? (1 - element.highTempTransitionOreMassConversion).ToString("P0") + " " : "->";

				transitionsInto_high += "<br/>" + transitionPercentage +  GetTagStringWithIcon(element.highTempTransition.tag);
				if (element.highTempTransitionOreID != SimHashes.Vacuum && element.highTempTransitionOreMassConversion > 0)
					transitionsInto_high += ",<br/>"+ element.highTempTransitionOreMassConversion.ToString("P0") + GetTagStringWithIcon(element.highTempTransitionOreID.CreateTag());
				transitionsInto_high += "<br/>";
			}
			var categoryTag = GetTagString(element.materialCategory) + "<br/>";

			if (element.highTempTransition != null && element.lowTempTransition == null)
			{
				property += FormatLineBreaks(string.Format(L("STRINGS.ELEMENTS.ELEMENTDESCSOLID"), categoryTag, transitionsInto_high, element.hardness));
			}
			else if (element.highTempTransition != null && element.lowTempTransition != null)
			{
				property += FormatLineBreaks(string.Format(L("STRINGS.ELEMENTS.ELEMENTDESCLIQUID"), categoryTag, transitionsInto_low, transitionsInto_high));
			}
			else if (element.highTempTransition == null && element.lowTempTransition != null)
			{
				property += FormatLineBreaks(string.Format(L("STRINGS.ELEMENTS.ELEMENTDESCGAS"), categoryTag, transitionsInto_low));
			}

			property += "<br/><br/>";


			var oreTags = element.oreTags.Distinct().Where(tag => tag != element.materialCategory);

			if (oreTags.Any())
			{
				var tags = string.Join(", ",
						oreTags
						.Select(t => MarkdownUtil.GetTagString(t))
						.StableSort()
						.Where(val => !val.Contains("MISSING")));
				property += string.Format(L("STRINGS.ELEMENTS.ELEMENTPROPERTIES"), "<br/>" + tags);
			}


			return property;
		}
		internal static string GetElementPhysicalProperties(Element element)
		{
			string property = "";

			property += FormatLineBreaks(
				L("STRINGS.ELEMENTS.THERMALPROPERTIES").TrimStart('\n')
				.Replace("{SPECIFIC_HEAT_CAPACITY}", "<br/>" + GameUtil.GetFormattedSHC(element.specificHeatCapacity) + "<br/>")
				.Replace("{THERMAL_CONDUCTIVITY}", "<br/>" + GameUtil.GetFormattedThermalConductivity(element.thermalConductivity) + "<br/>"));

			property += "<br/>";

			property += FormatLineBreaks(
				string.Format(L("STRINGS.ELEMENTS.RADIATIONPROPERTIES"),
				element.radiationAbsorptionFactor + "<br/>",
				"<br/>" + GetFormattedRads((element.radiationPer1000Mass * 1.10000002384186f / 600.0f), GameUtil.TimeSlice.PerCycle)));

			return property;
		}

		public static string FormatRadbolts(int amount)
		{
			return amount + "x " + L("STRINGS.UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES");
		}

		internal static string GetFormattedMass(Tag material, float amount, GameUtil.TimeSlice slice = GameUtil.TimeSlice.None, string extraSuffix = "")
		{
			var matName = MarkdownUtil.GetTagStringWithIcon(material);

			GameUtil.ApplyTimeSlice(amount, slice);
			string massFormatted = GameUtil.GetFormattedMass(amount);
			if (GameTags.DisplayAsUnits.Contains(material))
			{
				massFormatted = "x" + amount;
			}
			if (extraSuffix.Length > 0)
				extraSuffix = " " + extraSuffix;

			massFormatted += GetTimeSlice(slice);

			return matName + " (" + massFormatted + extraSuffix + ")";
		}

		static string GetTimeSlice(GameUtil.TimeSlice slice = GameUtil.TimeSlice.None)
		{
			if (slice == TimeSlice.PerSecond)
				return L("STRINGS.UI.UNITSUFFIXES.PERSECOND");
			else if (slice == TimeSlice.PerCycle)
				return L("STRINGS.UI.UNITSUFFIXES.PERCYCLE");
			return string.Empty;
		}

		internal static string GetFormattedRads(float amount, GameUtil.TimeSlice slice = GameUtil.TimeSlice.None)
		{
			GameUtil.ApplyTimeSlice(amount, slice);
			string result = string.Empty;
			result += amount;
			result += L("STRINGS.UI.UNITSUFFIXES.RADIATION.RADS");
			result += GetTimeSlice(slice);

			return result;
		}
	}
}
