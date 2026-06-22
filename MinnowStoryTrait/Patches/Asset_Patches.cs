using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace MinnowStoryTrait.Patches
{
	internal class Asset_Patches
	{
		class Assets_Patches
		{
			[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
			public class Assets_OnPrefabInit_Patch
			{
				[HarmonyPriority(Priority.LowerThanNormal)]
				public static void Prefix(Assets __instance)
				{
					InjectionMethods.AddSpriteToAssets(__instance, "CGM_MinnowTrait_icon");
					InjectionMethods.AddSpriteToAssets(__instance, "CGM_MinnowTrait_image");
				}
			}
		}
	}
}
