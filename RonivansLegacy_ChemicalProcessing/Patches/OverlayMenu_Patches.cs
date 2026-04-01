using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class OverlayMenu_Patches
	{

        [HarmonyPatch(typeof(OverlayMenu), nameof(OverlayMenu.InitializeToggles))]
        public class OverlayMenu_InitializeToggles_Patch
		{
			[HarmonyPrepare] public static bool Prepare() => Config.Toxicity;
			public static void Postfix(OverlayMenu __instance)
            {
				__instance.overlayToggleInfos.Add(
					new OverlayMenu.OverlayToggleInfo("Toxicity Overlay"
					, "icon_categories_placeholder"
					, RonivanAIO_ToxicityOverlayMode.ID
					, ""
					, Action.NumActions
					, "Displays the toxicity levels of cells in the world"
					));
            }
        }
	}
}
