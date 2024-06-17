
using BlueprintsV2.BlueprintsV2.BlueprintData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
    public class BuildingVisual : IVisual
    {
        public GameObject Visualizer { get; protected set; }
        public Vector2I Offset { get; protected set; }

        protected int cell;

        protected readonly BuildingConfig buildingConfig;

        public BuildingVisual(BuildingConfig buildingConfig, int cell)
        {
            Offset = buildingConfig.Offset;
            this.buildingConfig = buildingConfig;
            this.cell = cell;

            Vector3 positionCbc = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
            Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCbc, Grid.SceneLayer.Front, "BlueprintModBuildingVisualizer", LayerMask.NameToLayer("Place"));
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

        public virtual bool IsPlaceable(int cellParam)
        {
            return ValidCell(cellParam) && HasTech();
        }

        public virtual void MoveVisualizer(int cellParam)
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

        public virtual bool PlaceFinishedBuilding(int cellParam)
        {
            Vector3 positionCbc = Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer);
            GameObject building = buildingConfig.BuildingDef.Create(positionCbc, null, selected_elements: GetConstructionElements(), buildingConfig.BuildingDef.CraftRecipe, 293.15f, buildingConfig.BuildingDef.BuildingComplete);
            if (building == null)
            {
                return false;
            }

            buildingConfig.BuildingDef.MarkArea(cellParam, buildingConfig.Orientation, buildingConfig.BuildingDef.ObjectLayer, building);
            if (buildingConfig.BuildingDef.IsFoundation)
                buildingConfig.BuildingDef.RunOnArea(cellParam, buildingConfig.Orientation, cell0 => TileVisualizer.RefreshCell(cell0, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));

            if (building.GetComponent<Deconstructable>() != null)
            {
                building.GetComponent<Deconstructable>().constructionElements = this.GetConstructionElements();
            }

            if (building.GetComponent<Rotatable>() != null)
            {
                building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }

            ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);

            if (Visualizer.TryGetComponent<KBatchedAnimController>(out var vis))
            {
                vis.TintColour = ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
            }
            if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && building.GetComponent<KAnimGraphTileVisualizer>() != null && buildingConfig.GetConduitFlags(out var flags))
            {
                building.GetComponent<KAnimGraphTileVisualizer>().UpdateConnections((UtilityConnections)flags);
            }

            building.SetActive(true);
            return true;
        }

        private Tag[] GetConstructionElements()
        {
            var ingredients = buildingConfig.BuildingDef.CraftRecipe.Ingredients;
            var elements = new List<Tag>(buildingConfig.SelectedElements.Count);
            for (int i = 0; i < ingredients.Count; ++i)
            {
                var ingredient = ingredients[i];
                Tag selectedElement;
                if (i < buildingConfig.SelectedElements.Count)
                {
                    selectedElement = buildingConfig.SelectedElements[i];
                }
                else
                {
                    //should never happen, just in case to prevent crash.
                    selectedElement = ModAssets.GetFirstAvailableMaterial(ingredient.tag, ingredient.amount);
                }
                var key = new BlueprintSelectedMaterial(selectedElement, ingredient.tag);

                if(ModAssets.TryGetReplacementTag(key, out var replacement))
                {
                    selectedElement = replacement;
                }
                elements.Add(selectedElement);
            }

            return elements.ToArray();
        }

        private bool ViableReplacementCandidate(GameObject toReplace)
        {
            if (toReplace.TryGetComponent<BuildingComplete>(out var component))
            {
                return (component.Def.Replaceable && buildingConfig.BuildingDef.CanReplace(toReplace) && (component.Def != buildingConfig.BuildingDef || GetConstructionElements()[0] != component.GetComponent<PrimaryElement>().Element.tag));
            }
            return false;
        }

        public virtual bool PlacePlannedBuilding(int cellParam)
        {
            Vector3 positionCbc = Grid.CellToPosCBC(cellParam, buildingConfig.BuildingDef.SceneLayer);
            GameObject building = buildingConfig.BuildingDef.Instantiate(positionCbc, buildingConfig.Orientation, this.GetConstructionElements());
            if (building == null)
            {
                return false;
            }

            if (building.GetComponent<Rotatable>() != null)
            {
                building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }
            ModAPI.API_Methods.ApplyAdditionalBuildingData(building, buildingConfig);

            if (Visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
            {
                kbac.TintColour = ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                kbac.Play("place");
            }
            if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && building.TryGetComponent<KAnimGraphTileVisualizer>(out var vis) && buildingConfig.GetConduitFlags(out var flags))
            {
                vis.UpdateConnections((UtilityConnections)flags);
            }

            if (ToolMenu.Instance != null)
            {
                building.FindOrAddComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
            }

            building.SetActive(true);
            return true;
        }
        public virtual bool TryUse(int cellParam)
        {
            //return TryBuild(cellParam);
            if (BlueprintState.InstantBuild)
            {
                if (ValidCell(cellParam))
                {
                    for (int index = 0; index < buildingConfig.BuildingDef.PlacementOffsets.Length; ++index)
                    {
                        CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(buildingConfig.BuildingDef.PlacementOffsets[index], buildingConfig.Orientation);
                        int offsetCell = Grid.OffsetCell(cellParam, rotatedCellOffset);

                        WorldDamage.Instance.DestroyCell(offsetCell);
                    }
                    return PlaceFinishedBuilding(cellParam);
                }
            }
            else if (IsPlaceable(cellParam))
            {
                return PlacePlannedBuilding(cellParam);
            }
            return false;
        }
        public virtual void ClearTilePreview(int cell)
        {
            var def = buildingConfig.BuildingDef;

            if (!Grid.IsValidBuildingCell(cell) || !def.IsTilePiece)
                return;
            GameObject tileLayerObject = Grid.Objects[cell, (int)def.TileLayer];
            if (Visualizer == tileLayerObject)
                Grid.Objects[cell, (int)def.TileLayer] = null;
            if (!def.isKAnimTile)
                return;
            GameObject replacementLayerObject = null;
            if (def.ReplacementLayer != ObjectLayer.NumLayers)
                replacementLayerObject = Grid.Objects[cell, (int)def.ReplacementLayer];
            if (tileLayerObject != null && !tileLayerObject.TryGetComponent<Constructable>(out _) || !(replacementLayerObject == null) && !replacementLayerObject != Visualizer)
                return;
            Grid.Objects[cell, (int)def.ReplacementLayer] = null;
            World.Instance.blockTileRenderer.RemoveBlock(def, false, SimHashes.Void, cell);
            World.Instance.blockTileRenderer.RemoveBlock(def, true, SimHashes.Void, cell);
            TileVisualizer.RefreshCell(cell, def.TileLayer, def.ReplacementLayer);
        }

        /// <summary>
        /// experiment to allow replace building over stuff based on regular build tool, not in use.
        /// </summary>
        /// <param name="cellParam"></param>
        /// <returns></returns>
        public virtual bool TryBuild(int cellParam)
        {
            ClearTilePreview(cellParam);
            Vector3 posCbc = Grid.CellToPosCBC(cellParam, Grid.SceneLayer.Building);
            GameObject builtItem = null;
            var def = buildingConfig.BuildingDef;
            var buildingOrientation = buildingConfig.Orientation;
            var selectedElements = GetConstructionElements();
            var visualizer = Visualizer;

            SgtLogger.l("Visualizer test");
            SgtLogger.Assert("Visualizer was null", visualizer);

            bool instantBuild = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

            if (Grid.Objects[cellParam, (int)def.TileLayer] == Visualizer)
                Grid.Objects[cellParam, (int)def.TileLayer] = null;

            if (Grid.Objects[cellParam, (int)def.ObjectLayer] == Visualizer)
                Grid.Objects[cellParam, (int)def.ObjectLayer] = null;

            if (Grid.Objects[cellParam, (int)def.ReplacementLayer] == Visualizer)
                Grid.Objects[cellParam, (int)def.ReplacementLayer] = null;

            if (!instantBuild)
            {
                builtItem = def.TryPlace(visualizer, posCbc, buildingOrientation, selectedElements, null);
            }
            else if (def.IsValidBuildLocation(visualizer, posCbc, buildingOrientation) && def.IsValidPlaceLocation(visualizer, posCbc, buildingOrientation, out string _))
            {
                builtItem = def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, null, false, GameClock.Instance.GetTime());
            }
            if (builtItem == null && def.ReplacementLayer != ObjectLayer.NumLayers)
            {
                GameObject replacementCandidate = def.GetReplacementCandidate(cell);
                if (replacementCandidate != null && !def.IsReplacementLayerOccupied(cell))
                {
                    BuildingComplete component = replacementCandidate.GetComponent<BuildingComplete>();
                    if (component != null && component.Def.Replaceable && def.CanReplace(replacementCandidate) && (component.Def != def
                                || selectedElements[0] != replacementCandidate.GetComponent<PrimaryElement>().Element.tag))
                    {
                        if (!instantBuild)
                        {
                            builtItem = def.TryReplaceTile(visualizer, posCbc, buildingOrientation, selectedElements, null);
                            Grid.Objects[cell, (int)def.ReplacementLayer] = builtItem;
                        }
                        else if (def.IsValidBuildLocation(visualizer, posCbc, buildingOrientation, true) && def.IsValidPlaceLocation(visualizer, posCbc, buildingOrientation, true, out string _))
                            builtItem = InstantBuildReplace(cell, posCbc, replacementCandidate);
                    }
                }
            }

            SgtLogger.Assert("builtItem", builtItem);
            PostProcessBuild(instantBuild, posCbc, builtItem);
            return builtItem != null;
        }
        private GameObject InstantBuildReplace(int cell, Vector3 pos, GameObject tile)
        {
            var def = buildingConfig.BuildingDef;
            var buildingOrientation = buildingConfig.Orientation;
            var selectedElements = GetConstructionElements();

            if (!tile.TryGetComponent<SimCellOccupier>(out var SCO))
            {
                UnityEngine.Object.Destroy(tile);
                return def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, null, false, GameClock.Instance.GetTime());
            }
            SCO.DestroySelf(() =>
            {
                UnityEngine.Object.Destroy(tile);
                PostProcessBuild(true, pos, def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, null, false, GameClock.Instance.GetTime()));
            });
            return null;
        }

        private void PostProcessBuild(bool instantBuild, Vector3 pos, GameObject builtItem)
        {
            if (builtItem == null)
                return;
            if (!instantBuild)
            {
                Prioritizable component = builtItem.GetComponent<Prioritizable>();
                if (component != null)
                {
                    if (ToolMenu.Instance != null)
                        component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
                }
            }
            ModAPI.API_Methods.ApplyAdditionalBuildingData(builtItem, buildingConfig);

            if (Visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
            {
                kbac.TintColour = ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                kbac.Play("place");
            }
            if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && builtItem.TryGetComponent<KAnimGraphTileVisualizer>(out var vis) && buildingConfig.GetConduitFlags(out var flags))
            {
                vis.UpdateConnections((UtilityConnections)flags);
            }
        }

        public virtual bool HasTech()
        {
            return (BlueprintState.InstantBuild || !Config.Instance.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID));
        }
        public virtual bool ValidCell(int cellParam)
        {
            var pos = Grid.CellToPos(cellParam);
            if (Grid.IsValidCell(cellParam)
                && Grid.IsVisible(cellParam))
            {
                bool IsValidPlaceLocation = buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cellParam, buildingConfig.Orientation, out string faiReason);
                bool IgnorableFailReason =
                    faiReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_WALL
                    || faiReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER
                    || faiReason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER_FLOOR;



                return IsValidPlaceLocation || IgnorableFailReason;
                //replacement = buildingConfig.BuildingDef.IsValidReplaceLocation(pos, buildingConfig.Orientation, buildingConfig.BuildingDef.ReplacementLayer, buildingConfig.BuildingDef.ObjectLayer);
                //return (validCell || replacement);
            }

            return false;
        }
        public virtual Color GetVisualizerColor(int cellParam)
        {
            if (!ValidCell(cellParam))
            {
                return ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
            }

            else if (!HasTech())
            {
                return ModAssets.BLUEPRINTS_COLOR_NOTECH;
            }

            else
            {
                return ModAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
            }
        }
    }
}
