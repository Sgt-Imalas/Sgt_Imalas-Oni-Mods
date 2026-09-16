using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Exhaust_Patches
    {
        /// <summary>
        /// skip exhaust setting active state, it is set by vent state machine instead
        /// </summary>
        [HarmonyPatch(typeof(Exhaust), nameof(Exhaust.OnConduitStateChanged))]
        public class Exhaust_OnConduitStateChanged_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;

			public static bool Prefix(Exhaust __instance)
            {   
                if (__instance is PoweredExhaust)
                    return false;
                return true;
            }
        }
    }
}
