using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class StickerBombConfig_Patches
	{

        [HarmonyPatch(typeof(StickerBombConfig), nameof(StickerBombConfig.CreatePrefab))]
        public class StickerBombConfig_CreatePrefab_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.Drywall_Hides_Pipes;
			public static void Postfix(GameObject __result)
            {
				if (__result.TryGetComponent<KBatchedAnimController>(out var kbac))
					kbac.sceneLayer = ModAssets.AboveDrywallLayer;

			}
        }
	}
}
