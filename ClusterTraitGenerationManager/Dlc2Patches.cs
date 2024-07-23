using ClusterTraitGenerationManager.ClusterData;
using HarmonyLib;
using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
    internal class Dlc2Patches
    {
        [HarmonyPatch(typeof(RetiredColonyInfoScreen), nameof(RetiredColonyInfoScreen.IsAchievementValidForDLCContext))]
        public static class Fix_existing_games_with_ceres
        {
            public static void Postfix(RetiredColonyInfoScreen __instance, ref bool __result, string[] dlcid, string clusterTag)
            {
                if (__result == false
                    && clusterTag != null
                    && Game.clusterId == CGSMClusterManager.CustomClusterID
                    && DlcManager.IsContentSubscribed(DlcManager.DLC2_ID)
                    && SaveLoader.Instance.IsDlcListActiveForCurrentSave(dlcid)
                    && SaveGameData.Instance != null
                    )
                {
                    __result = SaveGameData.Instance.IsCeresAsteroidInCluster(clusterTag);
                }
            }
        }
        [HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.GetCurrentClusterLayout))]
        public static class ClusterLayouts_GetCurrentClusterLayout
        {
            public static void Postfix(ref ClusterLayout __result)
            {
                SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout);
                if (currentQualitySetting.id == CGSMClusterManager.CustomClusterID)
                {
                    bool ceres = false;
                    if (SaveGameData.Instance != null)
                    {
                        ceres = SaveGameData.Instance.IsCeresAsteroidInCluster("CeresCluster");
                    }
                    else
                    {
                        SgtLogger.warning("savegamedata was null!");
                    }

                    __result = CGSMClusterManager.GenerateDummyCluster(DlcManager.IsExpansion1Active(), ceres);
                }
            }
        }

        [HarmonyPatch(typeof(ClusterLayouts), nameof(ClusterLayouts.GetClusterData))]
        public static class ClusterLayouts_GetClusterData
        {
            public static void Postfix(string name, ref ClusterLayout __result)
            {
                //SgtLogger.l(name, "currentQualitySetting");
                if (name == CGSMClusterManager.CustomClusterID)
                {
                    bool ceres = false;
                    if (SaveGameData.Instance != null)
                    {
                        ceres = SaveGameData.Instance.IsCeresAsteroidInCluster("CeresCluster");
                    }
                    else
                    {
                        SgtLogger.warning("savegamedata was null!");
                    }

                    __result = CGSMClusterManager.GenerateDummyCluster(DlcManager.IsExpansion1Active(), ceres);
                }
            }
        }
        [HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.OnDeserialized))]
        public static class CustomGameSettings_OnDeserialized
        {
            public static void Prefix(CustomGameSettings __instance)
            {
                SgtLogger.l("CustomGameSettings.OnDeserialized");
                if(__instance.CurrentQualityLevelsBySetting.TryGetValue(CustomGameSettingConfigs.ClusterLayout.id, out var clusterDefaultName))
                {
                    SgtLogger.l(clusterDefaultName);
                    if (clusterDefaultName != CGSMClusterManager.CustomClusterID)
                        return;

                    bool ceres = false;
                    if (SaveGameData.Instance != null)
                    {
                        ceres = SaveGameData.Instance.IsCeresAsteroidInCluster("CeresCluster");
                    }
                    else
                    {
                        SgtLogger.warning("savegamedata was null!");
                    }
                    SgtLogger.l("creating dummy cluster for savegame, ceres active: " + ceres);
                    SettingsCache.clusterLayouts.clusterCache[CGSMClusterManager.CustomClusterID] = CGSMClusterManager.GenerateDummyCluster(DlcManager.IsExpansion1Active(), ceres);
                }
            }
        }

        [HarmonyPatch]
        public static class DataBanks
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject inst)
            {
                if (SaveLoader.Instance.IsDLCActiveForCurrentSave("DLC2_ID") && SaveGameData.Instance != null && SaveGameData.Instance.IsCeresAsteroidInCluster("CeresCluster"))
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
