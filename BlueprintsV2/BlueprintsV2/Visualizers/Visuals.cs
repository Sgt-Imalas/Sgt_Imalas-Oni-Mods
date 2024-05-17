using Blueprints;
using BlueprintsV2.BlueprintsV2.BlueprintData;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
    public static class VisualsUtilities
    {
        public static void SetVisualizerColor(int cell, Color color, GameObject visualizer, BuildingConfig buildingConfig)
        {
            if (buildingConfig.BuildingDef.isKAnimTile && buildingConfig.BuildingDef.BlockTileAtlas != null && !BlueprintsState.ColoredCells.ContainsKey(cell))
            {
                BlueprintsState.ColoredCells.Add(cell, new CellColorPayload(color, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));
                TileVisualizer.RefreshCell(cell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
            }

            if (visualizer.GetComponent<KBatchedAnimController>() != null)
            {
                visualizer.GetComponent<KBatchedAnimController>().TintColour = color;
            }
        }
    }

}
