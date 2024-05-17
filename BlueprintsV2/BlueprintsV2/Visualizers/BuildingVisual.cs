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
    public sealed class BuildingVisual : IVisual
    {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }

        private int cell;

        private readonly BuildingConfig buildingConfig;

        public BuildingVisual(BuildingConfig buildingConfig, int cell)
        {
            Offset = buildingConfig.Offset;
            this.buildingConfig = buildingConfig;
            this.cell = cell;

            Vector3 positionCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCbc, Grid.SceneLayer.Ore, "BlueprintModBuildingVisualizer", LayerMask.NameToLayer("Place"));
            Visualizer.transform.SetPosition(positionCbc);
            Visualizer.SetActive(true);

            if (Visualizer.GetComponent<Rotatable>() != null)
            {
                Visualizer.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }
            ModAPI.API_Methods.ApplyAdditionalBuildingData(Visualizer, buildingConfig);

            if (Visualizer.TryGetComponent<KBatchedAnimController>(out var batchedAnimController))
            {
                batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
                batchedAnimController.isMovable = true;
                batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset();
                batchedAnimController.TintColour = GetVisualizerColor(cell);

                batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
                batchedAnimController.Play("place");
            }
            else
            {
                Visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
            }
        }

        public bool IsPlaceable(int cellParam)
        {
            return ValidCell(cellParam) && HasTech();
        }

        public void MoveVisualizer(int cellParam)
        {
            if (cell != cellParam)
            {
                Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer));

                if (Visualizer.GetComponent<KBatchedAnimController>() != null)
                {
                    Visualizer.GetComponent<KBatchedAnimController>().TintColour = GetVisualizerColor(cellParam);
                }

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
                    //GameObject building = buildingConfig.BuildingDef.Build(cellParam, buildingConfig.Orientation,null, buildingConfig.SelectedElements,293.15f);
                    GameObject building = buildingConfig.BuildingDef.Create(positionCbc, null, selected_elements: buildingConfig.SelectedElements, buildingConfig.BuildingDef.CraftRecipe, 293.15F, buildingConfig.BuildingDef.BuildingComplete);
                    if (building == null)
                    {
                        return false;
                    }

                    buildingConfig.BuildingDef.MarkArea(cellParam, buildingConfig.Orientation, buildingConfig.BuildingDef.ObjectLayer, building);

                    if (building.GetComponent<Deconstructable>() != null)
                    {
                        building.GetComponent<Deconstructable>().constructionElements = buildingConfig.SelectedElements.ToArray();
                    }

                    if (building.GetComponent<Rotatable>() != null)
                    {
                        building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                    }

                    ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);
                    
                    if (Visualizer.TryGetComponent<KBatchedAnimController>(out var vis))
                    {
                        vis.TintColour = BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                    }

                    building.SetActive(true);
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
                ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);

                if (Visualizer.TryGetComponent<KBatchedAnimController>(out var vis))
                {
                    vis.TintColour = BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                }
                if (building.TryGetComponent<KBatchedAnimController>(out var kbac))
                {
                    kbac.Play("place");
                }

                if (ToolMenu.Instance != null)
                {
                    building.FindOrAddComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
                }

                building.SetActive(true);
                return true;
            }

            return false;
        }

        public bool ValidCell(int cellParam)
        {
            return Grid.IsValidCell(cellParam) && Grid.IsVisible(cellParam) && buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cellParam, buildingConfig.Orientation, out string _);
        }

        public bool HasTech()
        {
            return (BlueprintsState.InstantBuild || !BlueprintsAssets.Options.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID));
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
