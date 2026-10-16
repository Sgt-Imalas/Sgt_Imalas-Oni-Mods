using HarmonyLib;
using UtilLibs;

namespace GeothermalStoryTrait.Patches
{
    class Assets_Patches
	{
		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "CGM_GeothermalHeatPump_icon");
				InjectionMethods.AddSpriteToAssets(__instance, "CGM_GeothermalHeatPump_image");
			}
		}
	}
}
