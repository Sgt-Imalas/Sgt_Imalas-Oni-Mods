using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
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
                SgtLogger.l("Adding aquatic dream icons");
                int count = 0;
                foreach(var kvp in Aq_Personalities.OriginalDreamIconMap)
				{
                    ++count;
					Assets.Sprites.Add("dreamIcon_"+kvp.Key, kvp.Value);
				}
                SgtLogger.l($"Added {count} aquatic dream icons");
               // string aquaName = "dreamIcon_" + Aq_Personalities.AQUATIC_MINNOW;
				//Assets.Sprites.Add(aquaName, minnowOriginal);
				AssetUtils.AddAllSpritesInAssetsSubDir(__instance, "SkillBadges");
			}
        }
	}
}
