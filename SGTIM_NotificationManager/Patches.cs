using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static SGTIM_NotificationManager.ModAssets;

namespace SGTIM_NotificationManager
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
            }
        }
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

        /// <summary>
        /// Starving
        /// </summary>
        [HarmonyPatch(typeof(CalorieMonitor.Instance))]
        [HarmonyPatch(nameof(CalorieMonitor.Instance.IsStarving))]
        public static class StarvingNotification
        {
            public static bool Prefix(CalorieMonitor.Instance __instance, ref bool __result)
            {
                float CalorieWarning = 600f;
                float percentage = CalorieWarning / __instance.calories.GetMax();
                __result = __instance.GetCalories0to1() <  percentage;
                return false;
            }
        }

        [HarmonyPatch(typeof(SuffocationMonitor.Instance))]
        [HarmonyPatch(nameof(SuffocationMonitor.Instance.IsSuffocating))]
        public static class SuffocationNotification
        {
            public static bool Prefix(SuffocationMonitor.Instance __instance, ref bool __result)
            {
                float timeToSuffocation = 50f;
                float breathValuePercentage =  timeToSuffocation/110f;
                __result = (double)__instance.breath.deltaAttribute.GetTotalValue() <= 0.0 && (double)__instance.breath.value <= breathValuePercentage;
                return false;
            }
        }
        [HarmonyPatch(typeof(SuitSuffocationMonitor.Instance))]
        [HarmonyPatch(nameof(SuitSuffocationMonitor.Instance.IsSuffocating))]
        public static class SuffocationNotificationSuit
        {
            public static bool Prefix(SuffocationMonitor.Instance __instance, ref bool __result)
            {
                float timeToSuffocation = 50f;
                float breathValuePercentage = timeToSuffocation / 110f;
                __result = (double)__instance.breath.deltaAttribute.GetTotalValue() <= 0.0 && (double)__instance.breath.value <= breathValuePercentage;
                return false;
            }
        }


        //[HarmonyPatch(typeof(SweepBotStationConfig))]
        //[HarmonyPatch(nameof(SweepBotStationConfig.CreateBuildingDef))]
        //public class AddUtilityPort
        //{
        //    public static void Postfix(BuildingDef __result)
        //    {
        //        __result.OutputConduitType = ConduitType.Liquid;
        //        __result.UtilityOutputOffset = new CellOffset(0, 0);
        //    }
        //}
        //[HarmonyPatch(typeof(SweepBotStationConfig))]
        //[HarmonyPatch(nameof(SweepBotStationConfig.DoPostConfigureComplete))]
        //public class AddConsumer
        //{
        //    public static void Postfix(GameObject go)
        //    {
        //        var dispenser = go.AddOrGet<ConduitDispenser>();
        //        dispenser.conduitType = ConduitType.Liquid;
        //        dispenser.elementFilter = (SimHashes[]) null;
        //        dispenser.storage = go.GetComponents<Storage>().First( x => x.fetchCategory == Storage.FetchCategory.StorageSweepOnly);
        //    }
        //}
    }
}
