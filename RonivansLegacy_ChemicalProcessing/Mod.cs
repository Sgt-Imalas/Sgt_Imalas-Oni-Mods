using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Patches;
using System;
using System.Collections.Generic;
using ElementUtilNamespace;
using UtilLibs;
using UtilLibs.BuildingPortUtils;
using UtilLibs.SharedTweaks;

namespace RonivansLegacy_ChemicalProcessing
{
	public class Mod : UserMod2
	{
		public static bool WriteWikiData => Mod.GenerateWiki && Mod.Instance.mod.IsDev && DlcManager.IsExpansion1Active();
		public static bool GenerateWiki = false;
		public static bool IsDev => Instance.mod.IsDev;

		public static Mod Instance;
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			Instance = this;

			ModAssets.LoadAssets();
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			HarmonyInstance = harmony;

			SgtLogger.LogVersion(this, harmony);

			SgtLogger.log("Current Config Settings:");
			UtilMethods.ListAllPropertyValues(Config.Instance, (s) => s.Contains("System.Action"));
			base.OnLoad(harmony);

			TryCatchElements(harmony);
			ConduitDisplayPortPatching.PatchAll(harmony);
			BuildingDatabase.RegisterAdditionalBuildingElements();
			AdditionalRecipes.RegisterTags();

			ResearchNotificationMessageFix.Register();
			ResearchScreenCollapseEntries.Register();

			ElementConverterDescriptionImprovement.Register();

			ResearchScreenBetterConnectionLines.Register();
			DynamicMaterialSelectorHeaderHeight.Register();
			SelectedRecipeQueueScreenSizeFix.Register();
			SkillsWidgetBetterConnectionLines.Register();
			AttachmentPointTagNameFix.Register();

			TranslationFix.Register();
		}

		void TryCatchElements(Harmony harmony)
		{
			try
			{
				SgtElementUtil.ExecuteElementEnumPatches(harmony);
				SgtLogger.l("Testing enum.tostring...");
				SgtLogger.l(SimHashes.Aerogel.ToString());
			}
			catch (Exception ex) 
			{
				SgtLogger.l("This system is incompatible with elemental patches in its current state.\nIf this is a Proton app under Linux, turn off Proton.\nIf you are on windows, enable BottomUp-ASLR in your exploit protection settings to get running again.\n\nException below:");
				Debug.LogException(ex);
			}
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			DisableOldRonivanMods(harmony, mods);
			CustomizeBuildings.IntegrationPatches(harmony);
			HighPressureConduitRegistration.InitCache(true);
			Config.Instance.SetElementMaxMassIfApplicable();
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
			ListOilWellPatches(harmony);
		}
		static void ListOilWellPatches(Harmony harmony)
		{
			try
			{
				var m_OilWellCapConfig_ConfigureBuildingTemplate = AccessTools.Method(typeof(OilWellCapConfig), nameof(OilWellCapConfig.ConfigureBuildingTemplate));
				var patches = Harmony.GetPatchInfo(m_OilWellCapConfig_ConfigureBuildingTemplate);
				if(patches != null)
				{
					foreach (var patch in patches.Prefixes)
						SgtLogger.l($"OilWell prefix patched by {patch.owner}:{patch.PatchMethod.Name}");
					foreach (var patch in patches.Postfixes)
						SgtLogger.l($"OilWell postfix patched by {patch.owner}:{patch.PatchMethod.Name}");
					foreach (var patch in patches.Transpilers)
						SgtLogger.l($"OilWell transpiler patched by {patch.owner}:{patch.PatchMethod.Name}");
					foreach (var patch in patches.Finalizers)
						SgtLogger.l($"OilWell finalizer patched by {patch.owner}:{patch.PatchMethod.Name}");
				}
			}
			catch (Exception ex)
			{
				SgtLogger.l("Error while listing oil well patches:\n" + ex.Message);
			}
		}
	}
}
