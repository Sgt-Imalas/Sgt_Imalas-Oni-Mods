using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    class BuffLargeOxidizerTank
    {
        [HarmonyPatch(typeof(OxidizerTank), "OnSpawn")]
        public static class IncreaseCapacityto1350Oxidizers
        {
            public static void Prefix(OxidizerTank __instance)
            {
                if (Config.Instance.BuffLargeOxidizer && __instance.maxFillMass == OxidizerTankClusterConfig.FuelCapacity)
                {
                    __instance.maxFillMass = 1350f;
                }
                else if (!Config.Instance.BuffLargeOxidizer && __instance.maxFillMass == 1350f)
                {
                    if (__instance.targetFillMass > OxidizerTankClusterConfig.FuelCapacity)
                        __instance.targetFillMass = OxidizerTankClusterConfig.FuelCapacity;
                    __instance.maxFillMass = 900f;
                }
            }
        }
        //[HarmonyPatch(typeof(FuelTank), "OnSpawn")]
        //public static class IncreaseCapacityto1350FuelTanks
        //{
        //    public static void Postfix(FuelTank __instance)
        //    {
        //        if (Config.Instance.BuffLargeOxidizer && __instance.physicalFuelCapacity == 900f)
        //        {
        //            __instance.physicalFuelCapacity = 1350f;
        //        }
        //        else if (!Config.Instance.BuffLargeOxidizer && __instance.physicalFuelCapacity == 1350f)
        //        {
        //            if (__instance.targetFillMass > 900f)
        //                __instance.targetFillMass = 900f;
        //            __instance.physicalFuelCapacity = 900f;
        //        }
        //    }

        //}

    }
}
//