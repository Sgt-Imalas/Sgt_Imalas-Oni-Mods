using ClusterTraitGenerationManager.ClusterData;
using Database;
using HarmonyLib;
using Klei.CustomSettings;
using ProcGen;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
	internal class Dlc2Patches
	{
		[HarmonyPatch(typeof(RetiredColonyInfoScreen), nameof(RetiredColonyInfoScreen.IsAchievementValidForDLCContext))]
		public static class Fix_existing_games_with_ceres
		{
			public static void Postfix(RetiredColonyInfoScreen __instance, ref bool __result, IHasDlcRestrictions restrictions, string clusterTag)
			{
				if (__result == false
					&& clusterTag != null
					&& DlcManager.IsCorrectDlcSubscribed(restrictions.GetRequiredDlcIds(),restrictions.GetForbiddenDlcIds())
					&& Game.IsCorrectDlcActiveForCurrentSave(restrictions)
					&& SaveGameData.Instance != null
					&& SaveGameData.IsCustomCluster()
					)
				{
					__result = SaveGameData.Instance.IsClusterTagAsteroidInCluster(clusterTag);
				}
			}
		}
		[HarmonyPatch(typeof(ColonyAchievement), nameof(ColonyAchievement.IsValidForSave))]
		public static class SteamAheadAchievment_ExistingSaves
		{
			public static void Postfix(ColonyAchievement __instance, ref bool __result)
			{
				if (__result == false
					&& __instance.clusterTag != null
					&& SaveGameData.Instance != null
					&& SaveGameData.IsCustomCluster())
				{
					__result = SaveGameData.Instance.IsClusterTagAsteroidInCluster(__instance.clusterTag);
				}
			}
		}
		[HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.GetCurrentClusterLayout))]
		public static class ClusterLayouts_GetCurrentClusterLayout
		{
			public static void Postfix(ref ClusterLayout __result)
			{
				if (Patches.StillLoading || Patches.ApplyCustomGen.IsGenerating)
					return;

				SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout);
				if (currentQualitySetting.id == CGSMClusterManager.CustomClusterID)
				{
					
					bool ceres = false, prehistoric = false;
					if (SaveGameData.Instance != null)
					{
						ceres = SaveGameData.Instance.IsClusterTagAsteroidInCluster("CeresCluster");
						prehistoric = SaveGameData.Instance.IsClusterTagAsteroidInCluster("PrehistoricCluster");
					}
					else
					{
						SgtLogger.warning("savegamedata was null!");
					}

					__result = CGSMClusterManager.GenerateDummyCluster(DlcManager.IsExpansion1Active(), ceres, prehistoric);
				}
			}
		}

		[HarmonyPatch(typeof(ClusterLayouts), nameof(ClusterLayouts.GetClusterData))]
		public static class ClusterLayouts_GetClusterData
		{
			public static void Postfix(string name, ref ClusterLayout __result)
			{
				if (Patches.StillLoading || Patches.ApplyCustomGen.IsGenerating)
					return;
				//SgtLogger.l(name, "currentQualitySetting");
				if (name == CGSMClusterManager.CustomClusterID)
				{
					bool ceres = false, prehistoric = false;
					if (SaveGameData.Instance != null)
					{
						ceres = SaveGameData.Instance.IsClusterTagAsteroidInCluster("CeresCluster");
						prehistoric = SaveGameData.Instance.IsClusterTagAsteroidInCluster("PrehistoricCluster");
					}
					else 
					{
						SgtLogger.warning("savegamedata was null!");
					}

					__result = CGSMClusterManager.GenerateDummyCluster(DlcManager.IsExpansion1Active(), ceres, prehistoric);
				}
			}
		}
		[HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.OnDeserialized))]
		public static class CustomGameSettings_OnDeserialized
		{
			public static void Prefix(CustomGameSettings __instance)
			{
				SgtLogger.l("CustomGameSettings.OnDeserialized");
				if (__instance.CurrentQualityLevelsBySetting.TryGetValue(CustomGameSettingConfigs.ClusterLayout.id, out var clusterDefaultName))
				{
					SgtLogger.l(clusterDefaultName);
					if (clusterDefaultName != CGSMClusterManager.CustomClusterID)
						return;

					bool ceres = false, prehistoric = false;
					if (SaveGameData.Instance != null)
					{
						ceres = SaveGameData.Instance.IsClusterTagAsteroidInCluster("CeresCluster");
						prehistoric = SaveGameData.Instance.IsClusterTagAsteroidInCluster("PrehistoricCluster");
					}
					else
					{
						SgtLogger.warning("savegamedata was null!");
					}
					SgtLogger.l("creating dummy cluster for savegame, ceres active: " + ceres);
					SettingsCache.clusterLayouts.clusterCache[CGSMClusterManager.CustomClusterID] = CGSMClusterManager.GenerateDummyCluster(DlcManager.IsExpansion1Active(), ceres, prehistoric);
				}
			}
		}

		[HarmonyPatch]
		public static class DataBanks
		{
			[HarmonyPostfix]
			public static void Postfix(GameObject inst)
			{
				if (Game.IsDlcActiveForCurrentSave("DLC2_ID") && SaveGameData.Instance != null && SaveGameData.Instance.IsClusterTagAsteroidInCluster("CeresCluster"))
					inst.AddOrGet<KBatchedAnimController>().SwapAnims(new KAnimFile[1]
					{
					Assets.GetAnim((HashedString) "floppy_disc_ceres_kanim")
					});
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IEntityConfig.OnSpawn);
				yield return typeof(OrbitalResearchDatabankConfig).GetMethod(name);
				yield return typeof(ResearchDatabankConfig).GetMethod(name);
			}
		}
	}
}
