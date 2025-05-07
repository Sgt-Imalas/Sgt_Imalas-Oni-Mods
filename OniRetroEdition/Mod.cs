using HarmonyLib;
using KMod;
using OniRetroEdition.BuildingDefModification;
using OniRetroEdition.SlurpTool;
using PeterHan.PLib.Actions;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilLibs;
using static StatusItem;
using static STRINGS.BUILDINGS.PREFABS;

namespace OniRetroEdition
{
	public class Mod : UserMod2
	{
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			STEAMTURBINE.NAME = global::STRINGS.UI.FormatAsLink("Steam Turbine", nameof(STEAMTURBINE));			
			STEAMTURBINE.EFFECT = STEAMTURBINE.EFFECT.Replace("THIS BUILDING HAS BEEN DEPRECATED AND CANNOT BE BUILT.\n\n", string.Empty);
			HarmonyInstance = harmony;
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			BuildingModifications.InitializeFolderPath();
			SkinsAdder.InitializeFolderPath();
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);

			SlurpToolPatches.SlurpAction = new PActionManager().CreateAction(SlurpToolPatches.ACTION_KEY,
				STRINGS.MISC.PLACERS.SLURPPLACER.ACTION_NAME, new PKeyBinding(KKeyCode.M, Modifier.Shift));

			var overlayBitsField = typeof(StatusItem).GetFieldSafe("overlayBitfieldMap", true);
			if (overlayBitsField != null && overlayBitsField.GetValue(null) is
					IDictionary<HashedString, StatusItemOverlays> overlayBits)
				overlayBits.Add(OverlayModes.Sound.ID, StatusItemOverlays.None);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			return;
			SgtLogger.l("registering custom LUTS for OniRetroEdition");
			if (mods.Any(mod => mod.staticID == "LUTNotIncluded"))
			{
				var LUT_Registry = Global.Instance.gameObject.GetComponent("RomenHRegistry") as IDictionary<string, object>;
				if (LUT_Registry != null)
				{
					try
					{
						LUT_Registry[LUTNotIncluded_DayLUT] = AssetUtils.LoadTexture(Path.Combine(UtilMethods.ModPath, "assets", "textures", "lut_day_retro.png"), true, 2, 2);
						SgtLogger.l("registered retro LUT for day");
					}
					catch (Exception e)
					{
						SgtLogger.warning("failed to set retro lut day texture\n" + e);
					}

					try
					{
						LUT_Registry[LUTNotIncluded_NightLUT] = AssetUtils.LoadTexture(Path.Combine(UtilMethods.ModPath, "assets", "textures", "lut_night_retro.png"), true, 2, 2);
						SgtLogger.l("registered retro LUT for night");
					}
					catch (Exception e)
					{
						SgtLogger.warning("failed to set retro lut night texture\n" + e);
					}
				}
				else
					SgtLogger.warning("RomenH_Registry not found!");
			}
			else
				SgtLogger.warning("LUTNotIncluded not found!");
		}
		public const string LUTNotIncluded_DayLUT = "LUTNotIncluded.DayLUT";
		public const string LUTNotIncluded_NightLUT = "LUTNotIncluded.NightLUT";
	}
}
