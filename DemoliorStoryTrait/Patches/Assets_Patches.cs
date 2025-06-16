using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
    class Assets_Patches
    {
		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "CGM_Impactor_icon");
				InjectionMethods.AddSpriteToAssets(__instance, "CGM_Impactor_image");
				InjectionMethods.AddSpriteToAssets(__instance, "ImpactorPip");
			}
		}

		[HarmonyPatch(typeof(ParallaxBackgroundObject), nameof(ParallaxBackgroundObject.Initialize))]
		public class ParallaxBackgroundObject_Initialize_Patch
		{
			public static void Prefix(ParallaxBackgroundObject __instance, ref string texture)
			{
				texture = "ImpactorPip";
			}
		}
	}
}
