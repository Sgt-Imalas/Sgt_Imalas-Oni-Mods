using Blueprints;
using BlueprintsV2.BlueprintsV2.BlueprintData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{

    public sealed class TileVisual : ICleanableVisual
    {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }
        public int DirtyCell { get; private set; } = -1;

        private readonly BuildingConfig buildingConfig;
        private readonly bool hasReplacementLayer;

        public TileVisual(BuildingConfig buildingConfig, int cell)
        {
            Offset = buildingConfig.Offset;
            this.buildingConfig = buildingConfig;
            hasReplacementLayer = buildingConfig.BuildingDef.ReplacementLayer != ObjectLayer.NumLayers;

            Vector3 positionCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCbc, Grid.SceneLayer.Ore, "BlueprintModTileVisualizer", LayerMask.NameToLayer("Place"));
            Visualizer.transform.SetPosition(positionCbc);
            Visualizer.SetActive(true);
            ModAPI.API_Methods.ApplyAdditionalBuildingData(Visualizer, buildingConfig);

            if (Visualizer.GetComponent<Rotatable>() != null)
            {
                Visualizer.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }

            KBatchedAnimController batchedAnimController = Visualizer.GetComponent<KBatchedAnimController>();
            if (batchedAnimController != null)
            {
                batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
                batchedAnimController.isMovable = true;
                batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset();
                batchedAnimController.Play("place");
            }

            VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
            DirtyCell = cell;
            UpdateGrid(cell);
        }

        public bool IsPlaceable(int cellParam)
        {
            return ValidCell(cellParam) && HasTech();
        }

        public void MoveVisualizer(int cellParam)
        {
            Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer));
            VisualsUtilities.SetVisualizerColor(cellParam, GetVisualizerColor(cellParam), Visualizer, buildingConfig);
            UpdateGrid(cellParam);
        }

        public bool TryUse(int cellParam)
        {
            Clean();

            if (BlueprintsState.InstantBuild)
            {
                if (ValidCell(cellParam))
                {
                    Vector3 positionCbc = Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer);
                    GameObject building = buildingConfig.BuildingDef.Create(positionCbc, null, buildingConfig.SelectedElements, buildingConfig.BuildingDef.CraftRecipe, 293.15F, buildingConfig.BuildingDef.BuildingComplete);
                    if (building == null)
                    {
                        return false;
                    }

                    buildingConfig.BuildingDef.MarkArea(cellParam, buildingConfig.Orientation, buildingConfig.BuildingDef.TileLayer, building);
                    buildingConfig.BuildingDef.RunOnArea(cellParam, buildingConfig.Orientation, cell0 => TileVisualizer.RefreshCell(cell0, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));

                    if (building.GetComponent<Deconstructable>() != null)
                    {
                        building.GetComponent<Deconstructable>().constructionElements = buildingConfig.SelectedElements.ToArray();
                    }

                    if (building.GetComponent<Rotatable>() != null)
                    {
                        building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                    }

                    VisualsUtilities.SetVisualizerColor(cellParam, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);

                    ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);
                    return true;
                }
            }

            else if (IsPlaceable(cellParam))
            {
                Vector3 positionCbc = Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer);
                GameObject building = buildingConfig.BuildingDef.Instantiate(positionCbc, buildingConfig.Orientation, buildingConfig.SelectedElements);
                if (building == null)
                {
                    return false;
                }

                if (building.GetComponent<Rotatable>() != null)
                {
                    building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                }

                if (Visualizer.TryGetComponent<KBatchedAnimController>(out var vis))
                {
                    vis.TintColour = BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                }
                if (building.TryGetComponent<KBatchedAnimController>(out var kbac))
                {
                    kbac.Play("place");
                }

                VisualsUtilities.SetVisualizerColor(cellParam, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);

                ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);
                return true;
            }

            return false;
        }

        public void Clean()
        {
            if (DirtyCell != -1 && Grid.IsValidBuildingCell(DirtyCell))
            {
                //PUtil.LogDebug("1 " + DirtyCell);
                if (Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer] == Visualizer)
                {
                    Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer] = null;
                }
                //PUtil.LogDebug("2 " + DirtyCell);
                if (hasReplacementLayer && Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] == Visualizer)
                {
                    Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] = null;
                }
                //PUtil.LogDebug("3 " + DirtyCell);
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
                //PUtil.LogDebug("4 " + DirtyCell);
                TileVisualizer.RefreshCell(DirtyCell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
            }

            DirtyCell = -1;
        }

        public bool ValidCell(int cell)
        {
            return Grid.IsValidCell(cell) && Grid.IsVisible(cell) && !HasTile(cell) && buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cell, buildingConfig.Orientation, out string _);
        }

        public bool HasTech()
        {
            return BlueprintsState.InstantBuild || !BlueprintsAssets.Options.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID);
        }

        public bool HasTile(int cell)
        {
            return Grid.Objects[cell, (int)buildingConfig.BuildingDef.TileLayer] != null;
        }

        public Color GetVisualizerColor(int cell)
        {
            if (!ValidCell(cell))
            {
                return BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
            }

            else if (!HasTech())
            {
                return BlueprintsAssets.BLUEPRINTS_COLOR_NOTECH;
            }

            else
            {
                return BlueprintsAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
            }
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
                            if (!ValidCell(cell))
                            {
                                VisualsUtilities.SetVisualizerColor(cell, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
                            }

                            bool replacing = hasReplacementLayer && CanReplace(cell);
                            World.Instance.blockTileRenderer.AddBlock(LayerMask.NameToLayer("Overlay"), buildingConfig.BuildingDef, replacing, SimHashes.Void, cell);
                            if (replacing && !visualizerSeated && Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] == null)
                            {
                                Grid.Objects[cell, (int)buildingConfig.BuildingDef.ReplacementLayer] = Visualizer;
                            }
                        }
                    }

                    TileVisualizer.RefreshCell(cell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
                }

                DirtyCell = cell;
            }
        }
    }

}
