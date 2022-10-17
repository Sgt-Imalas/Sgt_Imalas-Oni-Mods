using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    class PetrolRocketUseAllCombustiblesPatch
    {
        [HarmonyPatch(typeof(KeroseneEngineClusterConfig), "DoPostConfigureComplete")]
        public static class ReplaceFuelTagInPetrolRocket
        {
            public static void Postfix(GameObject go)
            {
                RocketEngineCluster rocketEngineCluster = go.GetComponent<RocketEngineCluster>();
                rocketEngineCluster.fuelTag = GameTags.CombustibleLiquid;
            }

        }
        [HarmonyPatch(typeof(KeroseneEngineClusterSmallConfig), "DoPostConfigureComplete")]
        public static class ReplaceFuelTagInSmallPetrolRocket
        {
            public static void Postfix(GameObject go)
            {
                RocketEngineCluster rocketEngineCluster = go.GetComponent<RocketEngineCluster>();
                rocketEngineCluster.fuelTag = GameTags.CombustibleLiquid;
            }

        }
    }
}
//