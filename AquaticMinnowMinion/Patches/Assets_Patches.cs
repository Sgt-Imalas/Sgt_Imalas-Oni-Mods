using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace AquaticMinnowMinion.Patches
{
	internal class Assets_Patches
	{

        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Postfix(Assets __instance)
            {
                var minnowOriginal = Assets.GetSprite("dreamIcon_Minnow");
                if (minnowOriginal == null)
                {
                    SgtLogger.warning("Could not find minnows original dream icon :rwcry:");
                    return;
                }
                string aquaName = "dreamIcon_" + Aq_Personalities.AQUATIC_MINNOW;
				Assets.Sprites.Add(aquaName, minnowOriginal);
				AssetUtils.AddAllSpritesInAssetsSubDir(__instance, "SkillBadges");
			}
        }
	}
}
