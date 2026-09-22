using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class StickerBomb_Patches
	{

        [HarmonyPatch(typeof(StickerBomb), nameof(StickerBomb.OnSpawn))]
        public class StickerBomb_CreatePrefab_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.Drywall_Hides_Pipes;
			public static void Prefix(StickerBomb __instance)
            {
				if (__instance.TryGetComponent<KBatchedAnimController>(out var kbac))
					kbac.SetSceneLayer(ModAssets.AboveDrywallLayer);
				else
					SgtLogger.l("No kbac on stickerbomb!");

			}
        }
	}
}
