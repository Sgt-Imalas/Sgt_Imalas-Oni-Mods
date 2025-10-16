using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Polymerizer_Patches
    {

        [HarmonyPatch(typeof(Polymerizer), nameof(Polymerizer.OnStorageChanged))]
        public class Polymerizer_OnStorageChanged_Patch
		{
            public static void Postfix(Polymerizer __instance, object data)
            {
                if(__instance is CustomPolymerizer ep)
				{
                    ep.UpdateCustomMeter();
				}
            }
        }
	}
}
