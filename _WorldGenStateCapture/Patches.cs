using Database;
using HarmonyLib;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using Microsoft.Win32;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static _WorldGenStateCapture.ModAssets;
using static _WorldGenStateCapture.STRINGS.WORLDPARSERMODCONFIG;
using static WorldGenSpawner;

namespace _WorldGenStateCapture
{
    internal class Patches
    {
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }



        #region SkipOrTheGameCrashesDueToNoSave

        [HarmonyPatch(typeof(RetireColonyUtility), nameof(RetireColonyUtility.SaveColonySummaryData))]
        public static class PreventSavingWorldRetiredColonyData
        {
            public static bool Prefix(ref bool __result)
            {
                __result = true;
                return false;
            }
        }
        [HarmonyPatch(typeof(RetireColonyUtility), nameof(RetireColonyUtility.StripInvalidCharacters))]
        public static class PreventCrashOnReveal
        {
            public static bool Prefix(ref string __result, string source)
            {
                __result = source;
                return false;
            }
        }
        [HarmonyPatch(typeof(Timelapser), nameof(Timelapser.RenderAndPrint))]
        public static class PreventCrashOnReveal2
        {
            public static bool Prefix()
            {
                return false;
            }
        }
        [HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.InitialSave))]
        public static class PreventSavingWorld
        {
            public static bool Prefix(SaveLoader __instance)
            {
                return false;
            }
        }
        #endregion

        [HarmonyPatch(typeof(WattsonMessage), nameof(WattsonMessage.OnDeactivate))]
        public static class QuitGamePt2
        {
            public static void Postfix()
            {

                SgtLogger.l("gathering world data...");
                foreach (var world in ClusterManager.Instance.m_worldContainers)
                {
                    world.isDiscovered = true;
                }


                for (int i = 0; i < SaveGame.Instance.worldGenSpawner.spawnables.Count; i++)
                {
                    SaveGame.Instance.worldGenSpawner.spawnables[i].TrySpawn();
                }


                GameScheduler.Instance.ScheduleNextFrame("collect data", (_) =>
                {
                    ModAssets.AccumulateSeedData();

                    GameScheduler.Instance.ScheduleNextFrame("restart game", (_) =>
                    App.instance.Restart());
                });
            }

        }
        [HarmonyPatch(typeof(WattsonMessage), nameof(WattsonMessage.OnActivate))]
        public static class QuitGamePt1
        {
            public static void Postfix(WattsonMessage __instance)
            {
                __instance.button.SignalClick(KKeyCode.Mouse2);
            }
        }



        [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnSpawn))]

        public static class AutoLaunchParser
        {
            [HarmonyPriority(Priority.Low)]
            public static void Postfix(MainMenu __instance)
            {
                bool shouldAutoStart = ShoulDoAutoStartParsing(out _);
                var config = Config.Instance;
                if (!shouldAutoStart)
                {
                    var startParsingBTN = Util.KInstantiateUI(__instance.Button_NewGame.gameObject, __instance.Button_NewGame.transform.parent.gameObject, true);
                    startParsingBTN.name = "start parsing";
                    UIUtils.AddActionToButton(startParsingBTN.transform, "", () =>
                    {
                        ToggleAutoParse(true);
                        ReduceRemainingRuns();
                        InitAutoStart(__instance);
                    });
                    LocText componentInChildren = startParsingBTN.GetComponentInChildren<LocText>();
                    componentInChildren.text = STRINGS.STARTPARSING;

                }


                if (config.ContinuousParsing)
                {
                    InitAutoStart(__instance);
                    return;
                }

                if (shouldAutoStart)
                {
                    ReduceRemainingRuns();
                    InitAutoStart(__instance);
                }
                else
                    ToggleAutoParse(false);
            }
            public static string RegistryKey = "SeedParsing_RemainingRuns";
            public static bool ShoulDoAutoStartParsing(out int remainingRuns)
            {
                remainingRuns = KPlayerPrefs.GetInt(RegistryKey);
                return remainingRuns > 0;
            }
            public static void ReduceRemainingRuns()
            {
                int remaining = KPlayerPrefs.GetInt(RegistryKey);
                remaining--;
                KPlayerPrefs.SetInt(RegistryKey, remaining);

            }
            public static void ToggleAutoParse(bool enable)
            {
                
                KPlayerPrefs.SetInt(RegistryKey, enable ? Config.Instance.TargetNumber : -1);

            }



            public static void InitAutoStart(MainMenu __instance)
            {
                autoLoadActive = false;
                var clusterPrefix = DlcManager.IsExpansion1Active() ? Config.Instance.TargetCoordinateDLC : Config.Instance.TargetCoordinateBase;

                targetLayout = null;
                SgtLogger.l("autostarting...");
                foreach (string clusterName in SettingsCache.GetClusterNames())
                {
                    ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(clusterName);
                    if (clusterData.coordinatePrefix == clusterPrefix)
                    {
                        targetLayout = clusterData;
                        break;
                    }
                }

                if (targetLayout == null)
                    return;

                SgtLogger.l("autostart successful");
                autoLoadActive = true;
                clusterCategory = targetLayout.clusterCategory;
                __instance.NewGame();

            }
        }
        static ClusterLayout targetLayout;
        static int clusterCategory = -1;
        static bool autoLoadActive;


        [HarmonyPatch(typeof(ClusterCategorySelectionScreen), nameof(ClusterCategorySelectionScreen.OnSpawn))]
        public static class SelectClusterType
        {
            public static void Postfix(ClusterCategorySelectionScreen __instance)
            {
                if (autoLoadActive && clusterCategory != -1)
                {
                    __instance.OnClickOption((ClusterLayout.ClusterCategory)clusterCategory);
                }
            }
        }
        [HarmonyPatch(typeof(ModeSelectScreen), nameof(ModeSelectScreen.OnSpawn))]
        public static class SelectSurvivalSettings
        {
            public static void Postfix(ModeSelectScreen __instance)
            {
                if (autoLoadActive)
                {
                    __instance.OnClickSurvival();

                }
            }
        }
        [HarmonyPatch(typeof(ColonyDestinationSelectScreen), nameof(ColonyDestinationSelectScreen.OnSpawn))]
        public static class SelectCluster
        {
            public static void Postfix(ColonyDestinationSelectScreen __instance)
            {
                if (autoLoadActive)
                {
                    __instance.newGameSettings.SetSetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout, targetLayout.filePath);
                    __instance.ShuffleClicked();
                    __instance.LaunchClicked();
                }
            }
        }


        /// <summary>
        /// These patches have to run manually or they break translations on certain screens
        /// </summary>
        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public static class OnASsetPrefabPatch
        {
            public static void Postfix()
            {
                SkipMinionScreen_StartProcess.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
            }
        }

        //[HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.EnableProceedButton))]
        public static class SkipMinionScreen_StartProcess
        {

            public static void AssetOnPrefabInitPostfix(Harmony harmony)
            {
                var m_TargetMethod = AccessTools.Method("CharacterSelectionController, Assembly-CSharp:EnableProceedButton");
                var m_Postfix = AccessTools.Method(typeof(SkipMinionScreen_StartProcess), "Postfix");

                harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix));
            }

            public static void Postfix(CharacterSelectionController __instance)
            {
                if (__instance is MinionSelectScreen)
                {
                    __instance.OnProceed();

                }
            }
        }
    }
}
