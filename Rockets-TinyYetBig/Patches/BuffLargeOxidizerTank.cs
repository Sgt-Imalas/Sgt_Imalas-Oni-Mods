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
        //[HarmonyPatch(typeof(OxidizerTankClusterConfig), "DoPostConfigureComplete")]
        //public static class IncreaseCapacityto1350
        //{
        //    public static void Postfix(GameObject go)
        //    {
        //        if (Config.Instance.BuffLargeOxidizer)
        //        {
        //            OxidizerTank oxidizerTank = go.AddOrGet<OxidizerTank>();
        //            oxidizerTank.targetFillMass = 1350f;
        //            oxidizerTank.maxFillMass = 1350f;
        //        }
        //    }

        //}
        [HarmonyPatch(typeof(OxidizerTank), "OnSpawn")]
        public static class IncreaseCapacityto1350
        {
            public static void Postfix(OxidizerTank __instance)
            {
                if (Config.Instance.BuffLargeOxidizer && __instance.maxFillMass == 900f)
                {
                    __instance.maxFillMass = 1350f;
                }
                else if (!Config.Instance.BuffLargeOxidizer && __instance.maxFillMass == 1350f)
                {
                    if (__instance.targetFillMass > 900f)
                        __instance.targetFillMass = 900f;
                    __instance.maxFillMass = 900f;
                }
            }

        }

    }
}
//