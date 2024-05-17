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

    public sealed class UtilityVisual : IVisual
    {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }

        private readonly BuildingConfig buildingConfig;
        private int cell;

        public UtilityVisual(BuildingConfig buildingConfig, int cell)
        {
            Offset = buildingConfig.Offset;
            this.buildingConfig = buildingConfig;
            this.cell = cell;

            Vector3 positionCbc = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
            Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCbc, Grid.SceneLayer.Ore, "BlueprintModUtilityVisualizer", LayerMask.NameToLayer("Place"));
            Visualizer.transform.SetPosition(positionCbc);
            Visualizer.SetActive(true);

            if (Visualizer.GetComponent<Rotatable>() != null)
            {
                Visualizer.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }
            ModAPI.API_Methods.ApplyAdditionalBuildingData(Visualizer, buildingConfig);

            if (Visualizer.TryGetComponent<KBatchedAnimController>(out var batchedAnimController))
            {
                IUtilityNetworkMgr utilityNetworkManager = buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager();

                if (utilityNetworkManager != null && buildingConfig.GetPipeFlags(out int flags))
                {
                    string animation = utilityNetworkManager.GetVisualizerString((UtilityConnections)flags) + "_place";

                    if (batchedAnimController.HasAnimation(animation))
                    {
                        batchedAnimController.Play(animation);
                    }
                }

                batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
                batchedAnimController.isMovable = true;
                batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset();
                batchedAnimController.TintColour = GetVisualizerColor(cell);

                batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
            }

            else
            {
                Visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
            }

            VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
        }

        public bool IsPlaceable(int cellParam)
        {
            return ValidCell(cellParam) && HasTech();
        }

        public void MoveVisualizer(int cellParam)
        {
            if (cellParam != cell)
            {
                Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, Grid.SceneLayer.Building));
                VisualsUtilities.SetVisualizerColor(cellParam, GetVisualizerColor(cellParam), Visualizer, buildingConfig);

                cell = cellParam;
            }
        }

        public bool TryUse(int cellParam)
        {
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

                    if (building.GetComponent<Rotatable>() != null)
                    {
                        building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                    }

                    if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && building.GetComponent<KAnimGraphTileVisualizer>() != null && buildingConfig.GetPipeFlags(out var flags))
                    {
                        building.GetComponent<KAnimGraphTileVisualizer>().UpdateConnections((UtilityConnections)flags);
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

                if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && building.GetComponent<KAnimGraphTileVisualizer>() != null&& buildingConfig.GetPipeFlags(out var flags))
                {
                    building.GetComponent<KAnimGraphTileVisualizer>().UpdateConnections((UtilityConnections)flags);
                }

                if (ToolMenu.Instance != null)
                {
                    building.FindOrAddComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
                }

                VisualsUtilities.SetVisualizerColor(cellParam, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
                ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);
                return true;
            }

            return false;
        }

        public bool ValidCell(int cellParam)
        {
            return Grid.IsValidCell(cellParam) && Grid.IsVisible(cellParam) && !HasUtility(cellParam) && buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cellParam, buildingConfig.Orientation, out string _);
        }

        public bool HasTech()
        {
            return (BlueprintsState.InstantBuild || !BlueprintsAssets.Options.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID));
        }

        public bool HasUtility(int cellParam)
        {
            return Grid.Objects[cellParam, (int)buildingConfig.BuildingDef.TileLayer] != null;
        }

        public Color GetVisualizerColor(int cellParam)
        {
            if (!ValidCell(cellParam))
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
    }
}
