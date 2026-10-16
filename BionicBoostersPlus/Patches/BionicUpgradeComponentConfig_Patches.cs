using BionicBoostersPlus.Content.ModDb;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BionicBoostersPlus.Patches
{
	internal class BionicUpgradeComponentConfig_Patches
	{

        [HarmonyPatch(typeof(BionicUpgradeComponentConfig), nameof(BionicUpgradeComponentConfig.CreatePrefabs))]
        public class BionicUpgradeComponentConfig_CreatePrefabs_Patch
        {
            public static void Postfix(BionicUpgradeComponentConfig __instance, List<GameObject> __result)
            {
                BB_Boosters.RegisterBoosters(__instance, __result);
            }
        }
	}
}
