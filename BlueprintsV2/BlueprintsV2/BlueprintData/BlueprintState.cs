using BlueprintsV2.ModAPI;
using BlueprintsV2.Tools;
using BlueprintsV2.Visualizers;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDING.STATUSITEMS;

namespace BlueprintsV2.BlueprintData
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

    public static class BlueprintState
    {
        public static string SelectedBlueprintFolder = string.Empty;

        public static bool InstantBuild => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

        private static readonly List<IVisual> FoundationVisuals = new();
        private static readonly List<IVisual> DependentVisuals = new();
        private static readonly List<ICleanableVisual> CleanableVisuals = new();

        public static readonly Dictionary<int, CellColorPayload> ColoredCells = new();

        public static Blueprint CreateBlueprint(Vector2I topLeft, Vector2I bottomRight, MultiToolParameterMenu filter = null)
        {
            Blueprint blueprint = new Blueprint("unnamed", "");

            int blueprintHeight = (topLeft.y - bottomRight.y);
            bool collectingGasTiles = filter != null && filter.AllowedLayer(SolidTileFiltering.ObjectLayerFilterKey);

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
                                else if (building == null)
                                {
                                    gameObject.TryGetComponent(out building);
                                }
                                //SgtLogger.l($"{gameObject != null} && {building != null} && {API_Methods.IsBuildable(building.Def)} && {(filter == null || filter.BuildingDefAllowedWithCurrentFilters(building.Def))}");
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
                                        buildingConfig.SetConduitFlags((int)networkMngCmp.GetNetworkManager()?.GetConnections(cell, false));
                                    }
                                    API_Methods.StoreAdditionalBuildingData(gameObject, buildingConfig);

                                    if (!blueprint.BuildingConfigurations.Contains(buildingConfig))
                                    {
                                        blueprint.BuildingConfigurations.Add(buildingConfig);
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
            //empty blueprint that caught some gas/liquid pockets, clear to not spam quasi empty blueprints
            if (blueprint.BuildingConfigurations.Count == 0 && blueprint.DigLocations.Count > 0)
            {
                blueprint.DigLocations.Clear();
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

            foreach (BuildingConfig buildingConfig in blueprint.BuildingConfigurations)
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

            if (UseBlueprintTool.Instance.HoverCard != null)
            {
                UseBlueprintTool.Instance.HoverCard.prefabErrorCount = errors;
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

        static int _state = 0;

        static List<Tuple<float, float>> ShiftStates = new List<Tuple<float, float>>()
        {
            new (0,0),
            new (0,1),
            new (1,1),
            new (1,0),
            new(0.5f,0.5f)

        };

        public static void NextAnchorState()
        {
            _state = (_state + 1) % ShiftStates.Count;
            var mousePos = PlayerController.GetCursorPos(KInputManager.GetMousePos());

            UpdateVisual(new((int)mousePos.x, (int)mousePos.y),true);
        }

        static Vector2I GetShiftedPositions(Vector2I startPos, Blueprint bp = null)
        {
            if (bp == null)
                bp = ModAssets.SelectedBlueprint;
            if (bp == null)
                return startPos;


            var current = ShiftStates[_state];

            float diffX = current.first;
            float diffY = current.second;

            var dimensions = bp.Dimensions;
            SgtLogger.l(dimensions.ToString());
            int shiftX = (int)(dimensions.X * diffX);
            int shiftY = (int)(dimensions.Y * diffY);


            var newVector = new Vector2I(startPos.X - shiftX, startPos.Y - shiftY);

            return newVector;
        }


        public static void UpdateVisual(Vector2I topLeft, bool forcingRedraw = false)
        {
            CleanDirtyVisuals();
            topLeft = GetShiftedPositions(topLeft);

            FoundationVisuals.ForEach(foundationVisual =>
            {
                var newPos = (topLeft + foundationVisual.Offset);
                foundationVisual.MoveVisualizer(Grid.XYToCell(newPos.x, newPos.y), forcingRedraw);
            });
            DependentVisuals.ForEach(dependentVisual =>
            {
                var newPos = (topLeft + dependentVisual.Offset);
                dependentVisual.MoveVisualizer(Grid.XYToCell(newPos.x, newPos.y), forcingRedraw);
            });
        }
        public static void UseBlueprint(Vector2I topLeft)
        {
            CleanDirtyVisuals();
            topLeft = GetShiftedPositions(topLeft);
            FoundationVisuals.ForEach(foundationVisual =>
            {
                var newPos = (topLeft + foundationVisual.Offset);
                foundationVisual.TryUse(Grid.XYToCell(newPos.x, newPos.y));
            });
            DependentVisuals.ForEach(dependentVisual =>
            {
                var newPos = (topLeft + dependentVisual.Offset);
                dependentVisual.TryUse(Grid.XYToCell(newPos.x, newPos.y));
            });
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
