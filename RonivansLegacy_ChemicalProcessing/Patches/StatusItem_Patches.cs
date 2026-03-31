using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class StatusItem_Patches
	{

        [HarmonyPatch(typeof(StatusItem), nameof(StatusItem.GetStatusItemOverlayBySimViewMode))]
        public class StatusItem_GetStatusItemOverlayBySimViewMode_Patch
		{
			[HarmonyPrepare] public static bool Prepare() => Config.Toxicity;
			public static void Prefix(StatusItem __instance)
			{
				if (!StatusItem.overlayBitfieldMap.ContainsKey(RonivanAIO_ToxicityOverlayMode.ID))
					StatusItem.overlayBitfieldMap.Add(RonivanAIO_ToxicityOverlayMode.ID, StatusItem.StatusItemOverlays.None);
			}
        }
	}
}
