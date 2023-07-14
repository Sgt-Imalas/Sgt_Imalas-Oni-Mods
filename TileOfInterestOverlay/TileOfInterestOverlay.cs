using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SingleEntityReceptacle;
using UnityEngine;
using TUNING;

namespace TileOfInterestOverlay
{
    internal class TileOfInterestOverlay : OverlayModes.Mode
    {
        static Color none = Color.black, workable = Color.blue, building = Color.red;

        public enum HighlightType
        {
            None,Building,Workable
        }

        /// <summary>
        /// The ID of this overlay mode.
        /// </summary>
        public static readonly HashedString ID = new HashedString("TILEOFINTEREST");

        /// <summary>
        /// Retrieves the overlay color for a particular cell when in the crop view.
        /// </summary>
        /// <param name="cell">The cell to check.</param>
        /// <returns>The overlay color for this cell.</returns>
        internal static Color GetColor(SimDebugView _, int cell)
        {
            var shade = Color.black;
            var reason = Instance.cells[cell];
            switch (reason)
            {
                case HighlightType.None:
                    break;
                case HighlightType.Workable:
                    shade = workable;
                    break;
                case HighlightType.Building:
                    shade = building;
                    break;
            }
            return shade;
        }

        /// <summary>
        /// The instance of this class created by OverlayScreen.
        /// </summary>
        internal static TileOfInterestOverlay Instance { get; private set; }

        /// <summary>
        /// The types of objects that are visible in the overlay.
        /// </summary>
        private readonly int cameraLayerMask;

        /// <summary>
        /// The cells used for the overlay.
        /// </summary>
        private readonly HighlightType[] cells;

        /// <summary>
        /// The conditions used to highlight plants that are in range of the current cell.
        /// </summary>
        private readonly OverlayModes.ColorHighlightCondition[] conditions;

        /// <summary>
        /// The target plants that are visible on screen.
        /// </summary>
        private readonly ICollection<KMonoBehaviour> layerTargets;

        /// <summary>
        /// The partitioner used to selectively iterate plants.
        /// </summary>
        private UniformGrid<KMonoBehaviour> partition;

        /// <summary>
        /// Cached legend colors used for pip planting.
        /// </summary>
        private readonly List<LegendEntry> TileOfInterestLegend;

        /// <summary>
        /// A collection of all prefab IDs that are considered valid plants.
        /// </summary>
        private readonly ICollection<Tag> targetTags;

        /// <summary>
        /// The types of objects that can be selected in the overlay.
        /// </summary>
        private readonly int selectionMask;

        /// <summary>
        /// The layer to be used for the overlay.
        /// </summary>
        private readonly int targetLayer;

        public TileOfInterestOverlay()
        {

            //string plantCountText = string.Format((pc == 1) ? PPO.TOOLTIPS.PLANTCOUNT_1 :
            //    PPO.TOOLTIPS.PLANTCOUNT, pc, TileOfInterestOverlayTests.PlantRadius);
            cameraLayerMask = LayerMask.GetMask(new string[] {
                "MaskedOverlay",
                "MaskedOverlayBG"
            });
            cells = new HighlightType[Grid.CellCount];
            for (int i = 0; i < Grid.CellCount; i++)
                // Avoid a big green screen flash on first load
                cells[i] = HighlightType.None;
            conditions = new OverlayModes.ColorHighlightCondition[] {
                new OverlayModes.ColorHighlightCondition(GetHighlightColor, ShouldHighlight)
            };
            layerTargets = new HashSet<KMonoBehaviour>();
            legendFilters = CreateDefaultFilters();
            Instance = this;
            TileOfInterestLegend = new List<LegendEntry>
            {
                new LegendEntry("tile2", "desc2",
                    workable),
                new LegendEntry("tile3","desc3",
                building),
            };
            partition = null;


            targetTags = new HashSet<Tag>(Assets.GetPrefabTagsWithComponent<Workable>().Concat(Assets.GetPrefabTagsWithComponent<BuildingComplete>().Distinct()));
            selectionMask = LayerMask.GetMask(new string[] {
                "MaskedOverlay"
            });
            targetLayer = LayerMask.NameToLayer("MaskedOverlay");
        }

        public override void Disable()
        {
            UnregisterSaveLoadListeners();
            DisableHighlightTypeOverlay(layerTargets);
            CameraController.Instance.ToggleColouredOverlayView(false);
            Camera.main.cullingMask &= ~cameraLayerMask;
            partition?.Clear();

            layerTargets.Clear();
            SelectTool.Instance.ClearLayerMask();
            base.Disable();
        }

        public override void Enable()
        {
            base.Enable();
            RegisterSaveLoadListeners();

            partition = PopulatePartition<KMonoBehaviour>(targetTags);

            CameraController.Instance.ToggleColouredOverlayView(true);
            Camera.main.cullingMask |= cameraLayerMask;
            SelectTool.Instance.SetLayerMask(selectionMask);
        }

        public override List<LegendEntry> GetCustomLegendData()
        {
            return TileOfInterestLegend;
        }

        /// <summary>
        /// Calculates the color to tint the plants found by the overlay.
        /// </summary>
        /// <returns>The color to tint the plant - red if too many, green if OK.</returns>
        private Color GetHighlightColor(KMonoBehaviour _)
        {
            var color = Color.black;
            // Same method as used by the decor overlay
            int mouseCell = Grid.PosToCell(CameraController.Instance.baseCamera.
                ScreenToWorldPoint(KInputManager.GetMousePos()));
            if (Grid.IsValidCell(mouseCell))
                color = (cells[mouseCell] == HighlightType.Workable) ?
                    workable : building;
            return color;
        }

        public override string GetSoundName()
        {
            return "Rooms";
        }

        public override void OnSaveLoadRootRegistered(SaveLoadRoot root)
        {
            // Add new plants to partitioner
            var tag = root.GetComponent<KPrefabID>().GetSaveLoadTag();
            if (targetTags.Contains(tag))
            {
                if (root.TryGetComponent<Workable>(out var workable))
                {
                    partition.Add(workable);
                    UpdateForBuilding(workable, true);
                }
                else if(root.TryGetComponent<BuildingComplete>(out var building))
                {
                    partition.Add(building);
                    UpdateForBuilding(building,true);
                }

            }
        }

        public override void OnSaveLoadRootUnregistered(SaveLoadRoot root)
        {
            // Remove plants from partitioner if they die
            if (root != null && root.gameObject != null)
            {
                if (root.TryGetComponent<Workable>(out var workable))
                {
                    layerTargets.Remove(workable);
                    partition.Remove(workable);
                    UpdateForBuilding(workable, false);
                }
                else if (root.TryGetComponent<BuildingComplete>(out var building))
                {
                    layerTargets.Remove(building);
                    partition.Remove(building);
                    UpdateForBuilding(building, false);
                }
            }
        }

        /// <summary>
        /// Calculates if the specified plant should be tinted.
        /// </summary>
        /// <param name="plant">The plant that was found.</param>
        /// <returns>Whether the plant is in range of the cell under the cursor.</returns>
        private bool ShouldHighlight(KMonoBehaviour inputItem)
        {
            int mouseCell = Grid.PosToCell(CameraController.Instance.baseCamera.
                ScreenToWorldPoint(KInputManager.GetMousePos()));
            return (cells[mouseCell] != HighlightType.None && Grid.PosToCell(inputItem) == mouseCell);           
        }

        public override void Update()
        {
            int x1, x2, y1, y2;
            var intersecting = HashSetPool<KMonoBehaviour, TileOfInterestOverlay>.Allocate();
            base.Update();
            // SimDebugView is updated on a background thread, so since plant checking
            // must be done on the FG thread, it is updated here
            Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
            x1 = min.x; x2 = max.x;
            y1 = min.y; y2 = max.y;
            // Refresh plant list with plants on the screen
            RemoveOffscreenTargets(layerTargets, min, max, null);
            partition.GetAllIntersecting(new Vector2(x1, y1), new Vector2(x2, y2),
                intersecting);
            foreach (var building in intersecting)
            {
                UpdateForBuilding(building);
                AddTargetIfVisible(building, min, max, layerTargets, targetLayer);
            }

            UpdateHighlightTypeOverlay(min, max, layerTargets, targetTags, conditions,
                OverlayModes.BringToFrontLayerSetting.Constant, targetLayer);
            intersecting.Recycle();
        }
        
        void UpdateForBuilding(KMonoBehaviour building, bool? addTrueRemoveFalse = null)
        {
            HighlightType type = HighlightType.None;
            int cell = Grid.PosToCell(building);

            if (building is Workable)
            {
                type = HighlightType.Workable;
            }
            else if (building is BuildingComplete)
            {
                type = HighlightType.Building;
            }
            if (addTrueRemoveFalse != null)
            {
                type = (addTrueRemoveFalse == false) ? HighlightType.None : type;
            }
            cells[cell] = type;
        }

        public override HashedString ViewMode()
        {
            return ID;
        }
    }
}
