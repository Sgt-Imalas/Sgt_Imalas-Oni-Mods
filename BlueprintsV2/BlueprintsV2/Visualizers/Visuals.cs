
using BlueprintsV2.BlueprintData;
using UnityEngine;

namespace BlueprintsV2.Visualizers
{
	public static class VisualsUtilities
	{
		public static void SetTileColor(int cell, Color color, BuildingConfig buildingConfig)
		{
			BlueprintState.ColoredCells[cell] = new CellColorPayload(color, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
			TileVisualizer.RefreshCell(cell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
		}

		public static void SetVisualizerColor(int cell, Color color, GameObject visualizer, BuildingConfig buildingConfig)
		{
			if (buildingConfig.BuildingDef.isKAnimTile && buildingConfig.BuildingDef.BlockTileAtlas != null && !BlueprintState.ColoredCells.ContainsKey(cell))
			{
				BlueprintState.ColoredCells.Add(cell, new CellColorPayload(color, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));
				TileVisualizer.RefreshCell(cell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
			}

			if (visualizer.GetComponent<KBatchedAnimController>() != null)
			{
				visualizer.GetComponent<KBatchedAnimController>().TintColour = color;
			}
		}
	}

}
