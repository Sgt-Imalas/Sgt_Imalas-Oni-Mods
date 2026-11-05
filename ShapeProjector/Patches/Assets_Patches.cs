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
				var textnoteIcons = AssetUtils.AddAllSpritesInAssetsSubDir(__instance, "shapesymbols");
				foreach (var sprite in textnoteIcons)
				{
					ModAssets.SymbolMap[sprite.name] = sprite;
				}
			}
        }
	}
}
