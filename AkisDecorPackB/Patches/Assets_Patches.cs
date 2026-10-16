using HarmonyLib;
using UtilLibs;

namespace AkisDecorPackB.Patches
{
	internal class Assets_Patches
	{

        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, ModAssets.Sprites.FOSSIL_BG);
			}
        }
	}
}
