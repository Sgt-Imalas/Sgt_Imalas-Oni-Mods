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
        /// old way of getting internal temp for critters
        /// </summary>
        [HarmonyPatch(typeof(CritterTemperatureMonitor.Instance))]
        [HarmonyPatch(nameof(CritterTemperatureMonitor.Instance.GetTemperatureExternal))]
        public static class CritterTemperatureMonitor_TryDamage_Patch
        {
            public static bool Prefix(CritterTemperatureMonitor.Instance __instance, ref float __result)
            {
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
    }
}
