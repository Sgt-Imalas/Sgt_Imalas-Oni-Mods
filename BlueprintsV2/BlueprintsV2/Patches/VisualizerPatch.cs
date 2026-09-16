using BlueprintsV2.BlueprintsV2.Visualizers.CustomTileRenderer;
using HarmonyLib;
using Rendering;
using UnityEngine;

namespace BlueprintsV2.Patches
{
	internal class VisualizerPatch
	{
		[HarmonyPatch(typeof(BlockTileRenderer), nameof(BlockTileRenderer.GetCellColour))]
		public static class BlockTileRendererGetCellColour
		{
			public static void Postfix(BlockTileRenderer __instance,int cell, SimHashes element, ref Color __result)
			{
				if(__instance is CustomTileRenderer customRenderer)
				{
					customRenderer.GetCachedCellColor(ref __result, cell, element);
				}
			}
		}
		//[HarmonyPatch(typeof(PlanScreen), nameof(PlanScreen.GetBuildingPriority))]
		//public static class PlanScreen_GetBuildingPriority_Patch
		//{
		//    public static bool Prefix(PlanScreen __instance, ref PrioritySetting __result)
		//    {
		//        if(__instance.ProductInfoScreen == null ||__instance.ProductInfoScreen.materialSelectionPanel == null || __instance.ProductInfoScreen.materialSelectionPanel.PriorityScreen == null)
		//        {
		//            __result = new(PriorityScreen.PriorityClass.basic, 5);
		//            return false;
		//        }

		//        return true;

		//    }
		//}
	}
}
