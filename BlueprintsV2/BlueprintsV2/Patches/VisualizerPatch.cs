using BlueprintsV2.BlueprintsV2.BlueprintData;
using HarmonyLib;
using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Patches
{
    internal class VisualizerPatch
    {
        [HarmonyPatch(typeof(BlockTileRenderer), "GetCellColour")]
        public static class BlockTileRendererGetCellColour
        {
            public static void Postfix(int cell, SimHashes element, ref Color __result)
            {
                if (__result != Color.red && element == SimHashes.Void && BlueprintState.ColoredCells.ContainsKey(cell))
                {
                    __result = BlueprintState.ColoredCells[cell].Color;
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
