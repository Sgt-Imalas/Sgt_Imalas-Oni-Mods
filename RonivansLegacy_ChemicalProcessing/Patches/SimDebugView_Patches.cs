using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SimDebugView_Patches
	{

        [HarmonyPatch(typeof(SimDebugView), nameof(SimDebugView.OnPrefabInit))]
        public class SimDebugView_OnPrefabInit_Patch
        {
			[HarmonyPrepare] public static bool Prepare() => Config.Toxicity;

			public static void Postfix(SimDebugView __instance)
            {
				__instance.getColourFuncs.Add(RonivanAIO_ToxicityOverlayMode.ID, RonivanAIO_ToxicityOverlayMode.GetColorForCell);

			}
        }
	}
}
