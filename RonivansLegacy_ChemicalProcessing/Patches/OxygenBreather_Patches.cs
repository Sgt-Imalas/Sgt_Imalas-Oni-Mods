using Dupes_Industrial_Overhaul.Chemical_Processing.Disease;
using HarmonyLib;
using System.Collections.Generic;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class OxygenBreather_Patches
    {
		/// <summary>
		/// burn duplicants that step into acidic chemicals with chemical burns
		/// </summary>

        [HarmonyPatch(typeof(OxygenBreather), nameof(OxygenBreather.OnSpawn))]
        public class OxygenBreather_OnSpawn_Patch
        {
            public static void Postfix(OxygenBreather __instance)
            {
                __instance.gameObject.AddOrGet<AcidBurnMonitor>();
            }
        }
    }
}
