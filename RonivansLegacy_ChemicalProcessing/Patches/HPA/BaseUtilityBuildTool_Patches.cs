using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	internal class BaseUtilityBuildTool_Patches
	{

        [HarmonyPatch(typeof(BaseUtilityBuildTool), nameof(BaseUtilityBuildTool.CheckValidPathPiece))]
        public class BaseUtilityBuildTool_CheckValidPathPiece_Patch
        {
            public static void Postfix(BaseUtilityBuildTool __instance, ref bool __result, int cell)
            {
                if (__result)
                    return;

                if(__instance.def == null || __instance.def.BuildingComplete == null)                
                    return;

                GameObject tileAtCell = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];

				if (tileAtCell != null && StructuralTileMarker.TileAtCell(cell))
                {
                    var objLayerOccupier = Grid.Objects[cell, (int)__instance.def.ObjectLayer];
                    if (objLayerOccupier != null && !objLayerOccupier.TryGetComponent<KAnimGraphTileVisualizer>(out _))
						return;

					var tileLayerOccupier = Grid.Objects[cell, (int)__instance.def.TileLayer];
					if (tileLayerOccupier != null && !tileLayerOccupier.TryGetComponent<KAnimGraphTileVisualizer>(out _))
						return;

                    __result = true;
				}
			}
        }
	}
}
