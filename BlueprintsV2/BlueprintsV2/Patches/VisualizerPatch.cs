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
                if (__result != Color.red && element == SimHashes.Void && BlueprintsState.ColoredCells.ContainsKey(cell))
                {
                    __result = BlueprintsState.ColoredCells[cell].Color;
                }
            }
        }
    }
}
