using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DemoliorStoryTrait.Patches
{
    class LargeImpactorVanillaConfig_Patches
    {

        [HarmonyPatch(typeof(LargeImpactorVanillaConfig), nameof(LargeImpactorVanillaConfig.ConfigCommon))]
        public class LargeImpactorVanillaConfig_ConfigCommon_Patch
        {
            public static void Postfix(GameObject __result)
            {
                if (Config.Instance.PipReplaceDemoliorSprite && __result.TryGetComponent<LargeImpactorCrashStamp>(out var stamp))
                    stamp?.largeStampTemplate = "poi/asteroid_impacts/potato_pip_impactor";

			}
        }
    }
}
