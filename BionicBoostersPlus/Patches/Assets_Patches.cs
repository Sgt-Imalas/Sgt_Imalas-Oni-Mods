using HarmonyLib;
using UtilLibs;

namespace BionicBoostersPlus.Patches
{
	internal class Assets_Patches
	{

        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Postfix(Assets __instance)
            {              
				AssetUtils.AddAllSpritesInAssetsSubDir(__instance, "SkillBadges");
			}
        }
	}
}
