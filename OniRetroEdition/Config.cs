using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>, IOptions
	{
		public enum EarlierVersion
		{
			Alpha,
			[Option("Thermal Update")]
			Beta
		}

		[Option("Iron Ore Tile version", "")]
		[JsonProperty]
		public EarlierVersion IronOreTexture { get; set; } = EarlierVersion.Beta;

		[Option("Connect certain tile tops", "")]
		[JsonProperty]
		public bool TileTopsMerge { get; set; } = true;

		//[Option("mop becomes water succ", "")]
		//[JsonProperty]
		//public bool succmop { get; set; }

		//[Option("manual space can opener", "")]
		//[JsonProperty]
		//public bool manualRailgunPayloadOpener { get; set; }

		[Option("manual slime machine", "")]
		[JsonProperty]
		public bool manualSlimemachine { get; set; } = true;

		[Option("Gas Element Sensor takes power", "")]
		[JsonProperty]
		public bool gassensorpower { get; set; } = true;

		[Option("Gamma Ray Oven uses radbolts", "")]
		[JsonProperty]
		public bool GammaRayOvenRadbolts { get; set; } = true;
		[Option("liquid element sensor power requirement", "")]
		[JsonProperty]
		public bool liquidsensorpower { get; set; } = false;
		[Option("Duplicants rot forever", "when activated, unburied duplicants will rot forever. otherwise they will decompose into bones.")]
		[JsonProperty]
		public bool endlessRotting { get; set; } = false;


		[Option("Old Pipe Icons", "pipe input and output icons are replaced with older versions that change based on the connection state")]
		[JsonProperty]
		public bool oldPipeIcons { get; set; } = true;

		[Option("Old Dupe Outfits", "all outfits are red")]
		[JsonProperty]
		public bool oldDupeSuits { get; set; } = true;

		[Option("Old Lights", "old lighting color")]
		[JsonProperty]
		public bool oldLights { get; set; } = true;

		[Option("Old Shading", "old shading colors")]
		[JsonProperty]
		public bool oldShading { get; set; } = true;

		[Option("Liquid Opacity")]
		[JsonProperty]
		[Limit(0.1f, 1f)]
		public float waterOpacity { get; set; } = 0.4f;


		//[Option("LightColor", "", "Lighting Settings")]
		//[JsonProperty]
		//[RestartRequired]
		//public UnityEngine.Color32 RetroYellow { get; set; } = new Color(1, 0.5f, 0.11f) * 3.5f;


		//[Option("LightColor Multiplier", "higher = more intense", "Lighting Settings")]
		//[JsonProperty]
		//[RestartRequired]
		//public float RetroYellow_Mult { get; set; } = 3.5f;

		//[Option("LightColor Overlay", "", "Lighting Settings")]
		//[JsonProperty]
		//[RestartRequired]
		//public UnityEngine.Color32 RetroYellowOverlay { get; set; } = new Color(1, 0.5f, 0.11f) * 2f;

		//[Option("LightColor Overlay Multiplier", "higher = more intense", "Lighting Settings")]
		//[JsonProperty]
		//[RestartRequired]
		//public float RetroYellowOverlay_Mult { get; set; } = 2;

		//[Option("DarkenTints1", "", "Lighting Settings")]
		//[JsonProperty]
		//public UnityEngine.Color32 DarkenTints1 { get; set; } = new UnityEngine.Color(0.846f, 0.846f, 0.846f, 1.000f);
		//[Option("DarkenTints2", "", "Lighting Settings")]
		//public UnityEngine.Color32 DarkenTints2 { get; set; } = UnityEngine.Color.white;
		//[Option("DarkenTints3", "", "Lighting Settings")]
		//public UnityEngine.Color32 DarkenTints3 { get; set; } = new UnityEngine.Color(1, 0.809f, 0.809f, 1.000f);
		//[Option("CharacterLit", "", "Lighting Settings")]
		//public UnityEngine.Color32 CharacterLit { get; set; } = new UnityEngine.Color(1.000f, 0.98f, 0.816f, 1.000f);
		//[Option("CharacterUnLit", "", "Lighting Settings")]
		//public UnityEngine.Color32 CharacterUnLit { get; set; } = new UnityEngine.Color(0.651f, 0.647f, 0.757f, 1.000f);
		//[Option("Global_LightColor", "", "Lighting Settings")]
		//public UnityEngine.Color32 GlobalLightColor { get; set; } = Color.white;


		//[Option("EdgeGlowCutoffStart", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float EdgeGlowCutoffStart
		//{
		//	get => Lighting.Instance?.Settings?.EdgeGlowCutoffStart ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.EdgeGlowCutoffStart = value;
		//	}
		//}
		//[Option("EdgeGlowCutoffEnd", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float EdgeGlowCutoffEnd
		//{
		//	get => Lighting.Instance?.Settings?.EdgeGlowCutoffEnd ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.EdgeGlowCutoffEnd = value;
		//	}
		//}
		//[Option("EdgeGlowIntensity", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float EdgeGlowIntensity
		//{
		//	get => Lighting.Instance?.Settings?.EdgeGlowIntensity ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.EdgeGlowIntensity = value;
		//	}
		//}
		//[Option("BackgroundDarkening", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float BackgroundDarkening
		//{
		//	get => Lighting.Instance?.Settings?.BackgroundDarkening ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.BackgroundDarkening = value;
		//	}
		//}
		//[Option("BackgroundClip", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float BackgroundClip
		//{
		//	get => Lighting.Instance?.Settings?.BackgroundClip ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.BackgroundClip = value;
		//	}
		//}
		//[Option("BackgroundUVScale", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float BackgroundUVScale
		//{
		//	get => Lighting.Instance?.Settings?.BackgroundUVScale ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.BackgroundUVScale = value;
		//	}
		//}
		//[Option("AnimIntensity", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float AnimIntensity
		//{
		//	get => Lighting.Instance?.Settings?.AnimIntensity ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.AnimIntensity = value;
		//	}
		//}
		//[Option("GasMinOpacity", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float GasMinOpacity
		//{
		//	get => Lighting.Instance?.Settings?.GasMinOpacity ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.GasMinOpacity = value;
		//	}
		//}
		//[Option("GasMaxOpacity", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float GasMaxOpacity
		//{
		//	get => Lighting.Instance?.Settings?.GasMaxOpacity ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.GasMaxOpacity = value;
		//	}
		//}
		//[Option("disableLighting", "", "Lighting Settings")]
		//[JsonIgnore]
		//public bool disableLighting
		//{
		//	get => Lighting.Instance?.disableLighting ?? false;
		//	set
		//	{
		//		Lighting.Instance?.disableLighting = value;
		//	}
		//}
		//[Option("directSunlightScale", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float directSunlightScale
		//{
		//	get => Lighting.Instance?.Settings?.substanceEdgeParameters.directSunlightScale ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.substanceEdgeParameters.directSunlightScale = value;
		//	}
		//}
		//[Option("ShowDust", "", "Lighting Settings")]
		//[JsonIgnore]
		//public float ShowDust
		//{
		//	get => Lighting.Instance?.Settings?.ShowDust ?? 0;
		//	set
		//	{
		//		Lighting.Instance?.Settings?.ShowDust = value;
		//	}
		//}
		//[Option("substanceEdgeParameters.intensity", "lower = darker", "Lighting Settings")]
		//[JsonIgnore]
		//public float substanceEdgeParameters_intensity
		//{
		//	get => Lighting.Instance?.Settings?.substanceEdgeParameters.intensity ?? 0;
		//	set
		//	{
		//		if (Lighting.Instance?.Settings == null)
		//			return;
		//		var current = Lighting.Instance.Settings.substanceEdgeParameters;
		//		current.intensity = value;
		//		Lighting.Instance.Settings.substanceEdgeParameters = current;
		//	}
		//}

		public IEnumerable<IOptionsEntry> CreateOptions()
		{
			return [];
		}

		void ApplyLightingChanges()
		{
			var __instance = Lighting.Instance;
			if (__instance == null)
				return;

			//__instance.Settings.DarkenTints[0] = Config.Instance.DarkenTints1;
			//__instance.Settings.DarkenTints[1] = Config.Instance.DarkenTints2;
			//__instance.Settings.DarkenTints[2] = Config.Instance.DarkenTints3;

			//__instance.Settings.characterLighting.litColour = Config.Instance.CharacterLit;
			//__instance.Settings.characterLighting.unlitColour = Config.Instance.CharacterUnLit;

			//__instance.Settings.LightColour = Config.Instance.GlobalLightColor;


			//SgtLogger.l("LIGHTING UPDATE:");
			//SgtLogger.l($"DarkenTints1: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.DarkenTints[0]) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.DarkenTints1)}");
			//SgtLogger.l($"DarkenTints2: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.DarkenTints[0]) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.DarkenTints2)}");
			//SgtLogger.l($"DarkenTints3: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.DarkenTints[0]) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.DarkenTints3)}");
			//SgtLogger.l($"CharacterLit: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.characterLighting.litColour) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.CharacterLit)}");
			//SgtLogger.l($"CharacterUnLit: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.characterLighting.unlitColour) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.CharacterUnLit)}");
			//SgtLogger.l($"LightColour: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.LightColour) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.GlobalLightColor)}");

			//__instance.UpdateLighting();
		}

		public void OnOptionsChanged()
		{

			Instance = this;
			POptions.WriteSettings(this);
			Instance = POptions.ReadSettings<Config>();
			this.ApplyLightingChanges();
		}
	}
}
