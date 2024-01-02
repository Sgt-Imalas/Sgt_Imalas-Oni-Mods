using Database;
using HarmonyLib;
using Klei;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static _WorldGenStateCapture.ModAssets;
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
