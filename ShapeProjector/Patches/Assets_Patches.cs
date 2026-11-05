using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace ShapeProjector.Patches
{
	internal class Assets_Patches
	{

        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
			{
				var shapesymbols = AssetUtils.AddAllSpritesInAssetsSubDir(__instance, "shapesymbols");
				foreach (var sprite in shapesymbols)
				{
					ModAssets.ShapeSymbolMap[sprite.name] = sprite;
				}
				var originsymbols = AssetUtils.AddAllSpritesInAssetsSubDir(__instance, "originsymbols");
				foreach (var sprite in originsymbols)
				{
					ModAssets.OriginSymbolMap[sprite.name] = sprite;
				}
			}
        }
	}
}
