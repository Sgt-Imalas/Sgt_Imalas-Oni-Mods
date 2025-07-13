using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public static bool Prefix(Exhaust __instance)
            {   
                if (__instance is PoweredExhaust)
                    return false;
                return true;
            }
        }
    }
}
