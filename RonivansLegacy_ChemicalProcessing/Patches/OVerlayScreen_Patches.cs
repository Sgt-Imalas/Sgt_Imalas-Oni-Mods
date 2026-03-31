using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class OVerlayScreen_Patches
	{

        [HarmonyPatch(typeof(OverlayScreen), nameof(OverlayScreen.RegisterModes))]
        public class OverlayScreen_RegisterModes_Patch
		{
			[HarmonyPrepare] public static bool Prepare() => Config.Toxicity;
			public static void Postfix(OverlayScreen __instance)
            {
				OverlayScreen.Instance.RegisterMode(new RonivanAIO_ToxicityOverlayMode());
            }
        }
	}
}
