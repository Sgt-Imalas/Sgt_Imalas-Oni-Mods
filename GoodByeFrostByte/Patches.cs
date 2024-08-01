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

namespace GoodByeFrostByte
{
    internal class Patches
    {

        /// <summary>
        /// remove happiness penalty from "uncomfortable" critter status effect
        /// </summary>
        [HarmonyPatch(typeof(CritterTemperatureMonitor))]
        [HarmonyPatch(nameof(CritterTemperatureMonitor.InitializeStates))]
        public static class CritterTemperatureMonitor_TryDamage_Patch
        {
            public static void Postfix(CritterTemperatureMonitor __instance)
            {
                if (Config.Instance.OldCritterTemperatureHappyness)
                    __instance.uncomfortableEffect.SelfModifiers.RemoveAll(modifier => modifier.AttributeId == Db.Get().CritterAttributes.Happiness.Id);
            }
        }


        /// <summary>
        /// old way of getting internal temp for critters
        /// </summary>
        [HarmonyPatch(typeof(CritterTemperatureMonitor.Instance))]
        [HarmonyPatch(nameof(CritterTemperatureMonitor.Instance.GetTemperatureExternal))]
        public static class CritterTemperatureMonitor_GetTemperatureExternal_Patch
        {
            public static bool Prefix(CritterTemperatureMonitor.Instance __instance, ref float __result)
            {
                if (!Config.Instance.OldCritterTemperatureDetection)
                    return true;

                __result = __instance.GetTemperatureInternal();
                return false;
            }
        }
        /// <summary>
        /// less movespeed penalty
        /// </summary>
        [HarmonyPatch(typeof(AttributeConverterInstance))]
        [HarmonyPatch(nameof(AttributeConverterInstance.Evaluate))]
        public static class AttributeConverterInstance_Evaluate_Patch
        {
            public static bool Prefix(AttributeConverterInstance __instance, ref float __result)
            {
                if (!Config.Instance.LogarithmicSpeedDebuff)
                    return true;

                float totalValue = __instance.attributeInstance.GetTotalValue();
                if (__instance.converter.Id == "MovementSpeed" && totalValue < 0)
                {
                    float Multiplied = __instance.converter.multiplier * totalValue; //-0.5f
                    float baseValue = __instance.converter.baseValue; //0;
                                                                      //result wanted: 0.66 MS -> -0.44
                    float division = 1f / ((-Multiplied) + 1); //50% ms at -10, 33%ms at -20, 66%ms at 

                    __result = baseValue - 1 + division;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// sleep chore disable "interupt by cold temperature"
        /// </summary>
        [HarmonyPatch(typeof(SleepChore.StatesInstance))]
        [HarmonyPatch(nameof(SleepChore.StatesInstance.CheckTemperature))]
        public static class SleepChore_CheckTemperature_Patch
        {
            public static bool Prefix()
            {
                return !Config.Instance.DisableDupeColdSleep;
            }
        }


        /// <summary>
        /// disable frostbite
        /// </summary>
        [HarmonyPatch(typeof(ScaldingMonitor.Instance))]
        [HarmonyPatch(nameof(ScaldingMonitor.Instance.IsScolding))]
        public static class ScaldingMonitor_IsScolding_Patch
        {
            public static bool Prefix(ref bool __result)
            {
                if (Config.Instance.DisableDupeColdDamage)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// adjust scolding temp
        /// </summary>
        [HarmonyPatch(typeof(ScaldingMonitor.Instance))]
        [HarmonyPatch(nameof(ScaldingMonitor.Instance.GetScoldingThreshold))]
        public static class ScaldingMonitor_GetScoldingThreshold_Patch
        {
            public static void Postfix(ref float __result)
            {
                __result = UtilMethods.GetKelvinFromC(Config.Instance.FrostBiteThreshold);
            }
        }

        /// <summary>
        /// Disable Heat debuff for dupes
        /// </summary>
        [HarmonyPatch(typeof(ExternalTemperatureMonitor.Instance))]
        [HarmonyPatch(nameof(ExternalTemperatureMonitor.Instance.IsTooHot))]
        public static class ExternalTemperatureMonitor_IsTooHot_Patch
        {
            public static bool Prefix(CritterTemperatureMonitor.Instance __instance, ref bool __result)
            {
                if (Config.Instance.DisableDupeHeatDebuff)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// Disable cold debuff for dupes
        /// </summary>
        [HarmonyPatch(typeof(ExternalTemperatureMonitor.Instance))]
        [HarmonyPatch(nameof(ExternalTemperatureMonitor.Instance.IsTooCold))]
        public static class ExternalTemperatureMonitor_IsTooCold_Patch
        {
            public static bool Prefix(CritterTemperatureMonitor.Instance __instance, ref bool __result)
            {
                if (Config.Instance.DisableDupeColdDebuff)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
