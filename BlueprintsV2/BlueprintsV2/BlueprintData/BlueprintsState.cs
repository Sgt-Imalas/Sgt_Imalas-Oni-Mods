using BlueprintsV2.BlueprintsV2.ModAPI;
using BlueprintsV2.BlueprintsV2.Tools;
using BlueprintsV2.BlueprintsV2.Visualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
    public struct CellColorPayload
    {
        public Color Color { get; private set; }
        public ObjectLayer TileLayer { get; private set; }
        public ObjectLayer ReplacementLayer { get; private set; }

        public CellColorPayload(Color color, ObjectLayer tileLayer, ObjectLayer replacementLayer)
        {
            Color = color;
            TileLayer = tileLayer;
            ReplacementLayer = replacementLayer;
        }
    }

    public static class BlueprintsState
    {
        private static int _selectedBlueprintFolderIndex;
        public static int SelectedBlueprintFolderIndex
        {
            get => _selectedBlueprintFolderIndex;

            set => _selectedBlueprintFolderIndex = Mathf.Clamp(value, 0, LoadedBlueprints.Count - 1);
        }

        public static List<BlueprintFolder> LoadedBlueprints { get; } = new();
        public static BlueprintFolder SelectedFolder => LoadedBlueprints[SelectedBlueprintFolderIndex];
        public static Blueprint SelectedBlueprint => SelectedFolder.SelectedBlueprint;

        public static bool InstantBuild => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

        private static readonly List<IVisual> FoundationVisuals = new();
        private static readonly List<IVisual> DependentVisuals = new();
        private static readonly List<ICleanableVisual> CleanableVisuals = new();

        public static readonly Dictionary<int, CellColorPayload> ColoredCells = new();

        public static bool HasBlueprints()
        {
            if (LoadedBlueprints.Count == 0)
            {
                return false;
            }

            foreach (BlueprintFolder blueprintFolder in LoadedBlueprints)
            {
                if (blueprintFolder.BlueprintCount > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static Blueprint CreateBlueprint(Vector2I topLeft, Vector2I bottomRight, MultiToolParameterMenu filter = null)
        {
            Blueprint blueprint = new Blueprint("unnamed", "");

            int blueprintHeight = (topLeft.y - bottomRight.y);
            bool collectingGasTiles = filter!=null && filter.AllowedLayer(SolidTileFiltering.ObjectLayerFilterKey);

            for (int x = topLeft.x; x <= bottomRight.x; ++x)
            {
                for (int y = bottomRight.y; y <= topLeft.y; ++y)
                {
                    int cell = Grid.XYToCell(x, y);

                    if (Grid.IsVisible(cell))
                    {
                        bool emptyCell = true;

                        for (int layer = 0; layer < Grid.ObjectLayers.Length; ++layer)
                        {
                            if (layer == (int)ObjectLayer.DigPlacer)
                            {
                                continue;
                            }

                            GameObject gameObject = Grid.Objects[cell, layer];
                            if (gameObject == null)
                                continue;

                            bool hasConstructable = gameObject.TryGetComponent<Constructable>(out var constructable);
                            bool hasDeconstructable = gameObject.TryGetComponent<Deconstructable>(out var deconstructable);

                            if (hasConstructable || hasDeconstructable)
                            {
                                Building building = null;

                                if (gameObject.TryGetComponent<BuildingComplete>(out var complete))
                                {
                                    building = complete;
                                }
                                else if (building == null && gameObject.TryGetComponent<BuildingUnderConstruction>(out var underConstruction))
                                {
                                    building = underConstruction;
                                }
                                else if(building == null)
                                {
                                    gameObject.TryGetComponent(out building);
                                }

                                if (gameObject != null && building != null && API_Methods.IsBuildable(building.Def) && (filter == null || filter.BuildingDefAllowedWithCurrentFilters(building.Def)))
                                {
                                    Vector2I centre = Grid.CellToXY(GameUtil.NaturalBuildingCell(building));

                                    BuildingConfig buildingConfig = new()
                                    {
                                        Offset = new(centre.x - topLeft.x, blueprintHeight - (topLeft.y - centre.y)),
                                        BuildingDef = building.Def,
                                        Orientation = building.Orientation
                                    };

                                    if (deconstructable != null)
                                    {
                                        buildingConfig.SelectedElements.AddRange(deconstructable.constructionElements);
                                    }
                                    else
                                    {
                                        buildingConfig.SelectedElements.AddRange(constructable.selectedElementsTags);
                                    }

                                    IHaveUtilityNetworkMgr networkMngCmp = building.Def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
                                    if (networkMngCmp != null)
                                    {
                                        buildingConfig.SetUtilityFlags((int)networkMngCmp.GetNetworkManager()?.GetConnections(cell, false));
                                    }
                                    API_Methods.StoreAdditionalBuildingData(gameObject, buildingConfig);

                                    if (!blueprint.BuildingConfiguration.Contains(buildingConfig))
                                    {
                                        blueprint.BuildingConfiguration.Add(buildingConfig);
                                    }

                                    emptyCell = false;
                                }
                            }
                        }

                        if ((emptyCell && collectingGasTiles && !Grid.IsSolidCell(cell)) || (filter.AllowedLayer(ObjectLayer.DigPlacer) && Grid.Objects[cell, 7] != null && Grid.Objects[cell, 7].name == "DigPlacer"))
                        {
                            var digLocation = new Vector2I(x - topLeft.x, blueprintHeight - (topLeft.y - y));

                            if (!blueprint.DigLocations.Contains(digLocation))
                            {
                                blueprint.DigLocations.Add(digLocation);
                            }
                        }
                    }
                }
            }

            blueprint.CacheCost();
            return blueprint;
        }

        public static void VisualizeBlueprint(Vector2I topLeft, Blueprint blueprint)
        {
            if (blueprint == null)
            {
                return;
            }

            int errors = 0;
            ClearVisuals();

            foreach (BuildingConfig buildingConfig in blueprint.BuildingConfiguration)
            {
                if (buildingConfig.BuildingDef == null || buildingConfig.SelectedElements.Count == 0)
                {
                    ++errors;
                    continue;
                }

                if (buildingConfig.BuildingDef.BuildingPreview != null)
                {
                    int cell = Grid.XYToCell(topLeft.x + buildingConfig.Offset.x, topLeft.y + buildingConfig.Offset.y);

                    if (buildingConfig.BuildingDef.IsTilePiece)
                    {
                        if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null)
                        {
                            AddVisual(new UtilityVisual(buildingConfig, cell), buildingConfig.BuildingDef);
                        }

                        else
                        {
                            AddVisual(new TileVisual(buildingConfig, cell), buildingConfig.BuildingDef);
                        }
                    }

                    else
                    {
                        AddVisual(new BuildingVisual(buildingConfig, cell), buildingConfig.BuildingDef);
                    }
                }
            }

            foreach (var digLocation in blueprint.DigLocations)
            {
                FoundationVisuals.Add(new DigVisual(Grid.XYToCell(topLeft.x + digLocation.x, topLeft.y + digLocation.y), digLocation));
            }

            if (UseBlueprintTool.Instance.GetComponent<UseBlueprintToolHoverCard>() != null)
            {
                UseBlueprintTool.Instance.GetComponent<UseBlueprintToolHoverCard>().prefabErrorCount = errors;
            }
        }

        private static void AddVisual(IVisual visual, BuildingDef buildingDef)
        {
            if (buildingDef.IsFoundation)
            {
                FoundationVisuals.Add(visual);
            }

            else
            {
                DependentVisuals.Add(visual);
            }

            if (visual is ICleanableVisual)
            {
                CleanableVisuals.Add((ICleanableVisual)visual);
            }
        }

        public static void UpdateVisual(Vector2I topLeft)
        {
            CleanDirtyVisuals();

            FoundationVisuals.ForEach(foundationVisual => foundationVisual.MoveVisualizer(Grid.XYToCell(topLeft.x + foundationVisual.Offset.x, topLeft.y + foundationVisual.Offset.y)));
            DependentVisuals.ForEach(dependentVisual => dependentVisual.MoveVisualizer(Grid.XYToCell(topLeft.x + dependentVisual.Offset.x, topLeft.y + dependentVisual.Offset.y)));
        }

        public static void UseBlueprint(Vector2I topLeft)
        {
            CleanDirtyVisuals();

            FoundationVisuals.ForEach(foundationVisual => foundationVisual.TryUse(Grid.XYToCell(topLeft.x + foundationVisual.Offset.x, topLeft.y + foundationVisual.Offset.y)));
            DependentVisuals.ForEach(dependentVisual => dependentVisual.TryUse(Grid.XYToCell(topLeft.x + dependentVisual.Offset.x, topLeft.y + dependentVisual.Offset.y)));
        }

        public static void ClearVisuals()
        {
            CleanDirtyVisuals();
            CleanableVisuals.Clear();

            FoundationVisuals.ForEach(foundationVisual => UnityEngine.Object.DestroyImmediate(foundationVisual.Visualizer));
            FoundationVisuals.Clear();

            DependentVisuals.ForEach(dependantVisual => UnityEngine.Object.DestroyImmediate(dependantVisual.Visualizer));
            DependentVisuals.Clear();
        }

        public static void CleanDirtyVisuals()
        {
            foreach (int cell in ColoredCells.Keys)
            {
                CellColorPayload cellColorPayload = ColoredCells[cell];
                TileVisualizer.RefreshCell(cell, cellColorPayload.TileLayer, cellColorPayload.ReplacementLayer);
            }

            ColoredCells.Clear();
            CleanableVisuals.ForEach(cleanableVisual => cleanableVisual.Clean());
        }
    }
}
