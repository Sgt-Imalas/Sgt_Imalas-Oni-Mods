using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing
{
	public class Mod : UserMod2
	{

		public static bool WriteWikiData => Mod.GenerateWiki && Mod.Instance.mod.IsDev && DlcManager.IsExpansion1Active();
		public static bool GenerateWiki = false;

		public static Mod Instance;
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			Instance = this;

			ModAssets.LoadAssets();
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));

			BuildingDatabase.RegisterBuildings();
			HarmonyInstance = harmony;

			base.OnLoad(harmony);

			SgtLogger.LogVersion(this, harmony);
			ConduitDisplayPortPatching.PatchAll(harmony);
			BuildingDatabase.RegisterAdditionalBuildingElements();
			AdditionalRecipes.RegisterTags();

			SharedTweaks.ResearchNotificationMessageFix.ExecutePatch();
			SharedTweaks.ResearchScreenCollapseEntries.ExecutePatch();
			SharedTweaks.ElementConverterDescriptionImprovement.ExecutePatch();
			SharedTweaks.ResearchScreenBetterConnectionLines.ExecutePatch();
			SharedTweaks.DynamicMaterialSelectorHeaderHeight.ExecutePatch();
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			DisableOldRonivanMods(harmony, mods);
		}

		internal static void RegisterLocalizedDescription()
		{
			Instance.mod.title = Strings.Get("STRINGS.RONIVANSLEGACY_AIO_NAME");
			Instance.mod.description = Strings.Get("STRINGS.RONIVANSLEGACY_AIO_DESC");
		}

		static HashSet<string> RonivanModIds = [
			"Ronivan.ChemProcessingBiochemistry",
			"Ronivan.ChemProcessing",
			"Ronivan.CustomGenerators",
			"Ronivan.CustomReservoirs",
			"Ronivan.CustomBuildings", //Dupes Engineering
			//part of dupes engineering:
			"Ronivan.WoodenSetStructures","Ronivan.CustomTiles",

			"Ronivan.DupesLogistics",
			"Ronivan.DupesMachinery",
			"Ronivan.DupesRefrigeration",
			"Ronivan.HighPressureApplications",
			"Ronivan.MineralProcessingMetallurgy",
			"Ronivan.MineralProcessingMining",
			"Ronivan.NuclearProcessing",
		];

		static void DisableOldRonivanMods(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			foreach (var mod in mods)
			{
				if (RonivanModIds.Contains(mod.staticID))
				{
					mod.SetEnabledForActiveDlc(false);
					harmony.UnpatchAll(mod.staticID);
				}

			}
		}
	}
}
