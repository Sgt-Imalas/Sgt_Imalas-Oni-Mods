
using BlueprintsV2.BlueprintData;
using BlueprintsV2.Tools;
using UnityEngine;

namespace BlueprintsV2.Visualizers
{

	public class TileVisual : BuildingVisual, ICleanableVisual
	{
		public int DirtyCell { get; private set; } = -1;

		private readonly bool hasReplacementLayer;

		public TileVisual(BuildingConfig buildingConfig, int cell) : base(buildingConfig, cell)
		{
			hasReplacementLayer = buildingConfig.BuildingDef.ReplacementLayer != ObjectLayer.NumLayers;
			VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
			this.cell = -1;
			DirtyCell = cell;
			UpdateGrid(cell);
		}

		public override void ForceRedraw()
		{
			VisualsUtilities.SetTileColor(cell, GetVisualizerColor(cell), buildingConfig);
		}

		public override void MoveVisualizer(int cellParam, bool forceRedraw)
		{
			if (cellParam != cell || forceRedraw)
			{
				Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer));
				VisualsUtilities.SetVisualizerColor(cellParam, GetVisualizerColor(cellParam), Visualizer, buildingConfig);
				UpdateGrid(cellParam);
			}
		}

		public void Clean()
		{
			if (DirtyCell != -1 && Grid.IsValidBuildingCell(DirtyCell))
			{
				if (buildingConfig.BuildingDef.isKAnimTile)
				{
					GameObject tileLayerObject = Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer];
					if (tileLayerObject == null || tileLayerObject.GetComponent<Constructable>() == null)
					{
						World.Instance.blockTileRenderer.RemoveBlock(buildingConfig.BuildingDef, false, SimHashes.Void, DirtyCell);
					}

					GameObject replacementLayerObject = hasReplacementLayer ? null : Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer];
					if (replacementLayerObject == null || replacementLayerObject == Visualizer)
					{
						World.Instance.blockTileRenderer.RemoveBlock(buildingConfig.BuildingDef, true, SimHashes.Void, DirtyCell);
					}
				}
				if (Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer] == Visualizer)
				{
					Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer] = null;
				}
				//PUtil.LogDebug("2 " + DirtyCell);
				if (hasReplacementLayer && Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] == Visualizer)
				{
					Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] = null;
				}
				//PUtil.LogDebug("4 " + DirtyCell);
				TileVisualizer.RefreshCell(DirtyCell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
			}
			DirtyCell = -1;

		}
		private bool CanReplace(int cell)
		{
			CellOffset[] placementOffsets = buildingConfig.BuildingDef.PlacementOffsets;

			for (int index = 0; index < placementOffsets.Length; ++index)
			{
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(placementOffsets[index], buildingConfig.Orientation);
				int offsetCell = Grid.OffsetCell(cell, rotatedCellOffset);

				if (!Grid.IsValidBuildingCell(cell) || Grid.Objects[offsetCell, (int)buildingConfig.BuildingDef.ObjectLayer] == null || Grid.Objects[offsetCell, (int)buildingConfig.BuildingDef.ReplacementLayer] != null)
				{
					return false;
				}
			}

			return true;
		}
		private void UpdateGrid(int cell)
		{
			Clean();
			if (Grid.IsValidBuildingCell(cell))
			{
				bool visualizerSeated = false;

				if (Grid.Objects[cell, (int)buildingConfig.BuildingDef.TileLayer] == null)
				{
					Grid.Objects[cell, (int)buildingConfig.BuildingDef.TileLayer] = Visualizer;
					visualizerSeated = true;
				}

				if (buildingConfig.BuildingDef.isKAnimTile)
				{
					GameObject tileLayerObject = Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer];
					GameObject replacementLayerObject = hasReplacementLayer ? Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] : null;

					if (tileLayerObject == null || tileLayerObject.GetComponent<Constructable>() == null && replacementLayerObject == null)
					{
						if (buildingConfig.BuildingDef.BlockTileAtlas != null)
						{
							if (!BlueprintState.InstantBuild && BlueprintState.ForceMaterialChange && CanRebuildWithMaterial(cell, out _))
							{
								VisualsUtilities.SetVisualizerColor(cell, ModAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT, Visualizer, buildingConfig);
							}

							if (!ValidCell(cell))
							{
								VisualsUtilities.SetVisualizerColor(cell, ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
							}

							bool replacing = hasReplacementLayer && CanReplace(cell);
							if (Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] == null)
							{
								World.Instance.blockTileRenderer.AddBlock(LayerMask.NameToLayer("Overlay"), buildingConfig.BuildingDef, replacing, SimHashes.Void, cell);
								if (replacing && !visualizerSeated && Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] == null)
								{
									Grid.Objects[cell, (int)buildingConfig.BuildingDef.ReplacementLayer] = Visualizer;
								}
							}

							TileVisualizer.RefreshCell(cell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);

						}
					}
				}

				DirtyCell = cell;
			}
		}
	}

}
