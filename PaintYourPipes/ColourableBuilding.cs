using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PaintYourPipes
{
    [SerializationConfig(MemberSerialization.OptIn)]
    internal class ColourableBuilding : KMonoBehaviour, ICheckboxControl
    {
        public static Dictionary<int, ColourableBuilding> SolidConduits = new Dictionary<int, ColourableBuilding>();
        public static Dictionary<int, ColourableBuilding> LiquidConduits = new Dictionary<int, ColourableBuilding>();
        public static Dictionary<int, ColourableBuilding> GasConduits = new Dictionary<int, ColourableBuilding>();

        public static Dictionary<int, ColourableBuilding> SolidConduitBridges = new Dictionary<int, ColourableBuilding>();
        public static Dictionary<int, ColourableBuilding> LiquidConduitBridges = new Dictionary<int, ColourableBuilding>();
        public static Dictionary<int, ColourableBuilding> GasConduitBridges = new Dictionary<int, ColourableBuilding>();

        public static HashSet<ColourableBuilding> AllConduits = new HashSet<ColourableBuilding>();

        public static void RefreshAll()
        {
            foreach(var cond in AllConduits)
            {
                cond.RefreshColor();
            }
        }


        int conduitType = -1;

        [MyCmpAdd]
        CopyBuildingSettings buildingSettings;

        [MyCmpGet]
        KPrefabID prefab;

        [Serialize]
        private bool _showInOverlay = true;
        public bool ShowInOverlay => _showInOverlay;

        [Serialize]
        private string colorHex = "FFFFFF";

        [MyCmpGet]
        KBatchedAnimController _animController;
        public KBatchedAnimController AnimController => _animController;
        private static readonly EventSystem.IntraObjectHandler<ColourableBuilding> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ColourableBuilding>((component, data) => component.OnCopySettings(data));

        public Color TintColor => GetColor();


        public Color GetColor()
        {
            var col = Util.ColorFromHex(colorHex);
            col.a = Patches.OverlayActive ? 0 : 1;
            return col;

        }
        public void SetColor(Color color)
        {
            colorHex = color.ToHexString();
            RefreshColor();
        }


        private void OnCopySettings(object obj)
        {
            if (obj != null && obj is GameObject go && go.TryGetComponent<ColourableBuilding>(out var SourceBuilding) && SourceBuilding.conduitType == this.conduitType)
            {
                _showInOverlay = SourceBuilding.ShowInOverlay;
                SetColor(SourceBuilding.TintColor);
                RefreshColor();
            }
        }

        public void RefreshColor()
        {
            _animController.TintColour = TintColor;
        }
        public override void OnSpawn()
        {
            if (OverlayScreen.GasVentIDs.Contains(prefab.PrefabID()))
            {
                conduitType = 0;

                if(TryGetComponent<ConduitBridgeBase>(out _))
                    GasConduitBridges.Add(Grid.PosToCell(this), this);
                else 
                    GasConduits.Add(Grid.PosToCell(this), this);
            }
            else if (OverlayScreen.LiquidVentIDs.Contains(prefab.PrefabID()))
            {
                conduitType = 1;

                if (TryGetComponent<ConduitBridgeBase>(out _))
                    LiquidConduitBridges.Add(Grid.PosToCell(this), this);
                else
                    LiquidConduits.Add(Grid.PosToCell(this), this);
            }
            else if (OverlayScreen.SolidConveyorIDs.Contains(prefab.PrefabID()))
            {
                conduitType = 2;

                if (TryGetComponent<ConduitBridgeBase>(out _))
                    SolidConduitBridges.Add(Grid.PosToCell(this), this);
                else
                    SolidConduits.Add(Grid.PosToCell(this), this);
            }

            AllConduits.Add(this);
            base.OnSpawn();
            GameScheduler.Instance.ScheduleNextFrame("deayed initial refresh", (_) => RefreshColor());
            //RefreshColor();
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }

        public override void OnCleanUp()
        {
            switch (conduitType)
            {
                case 0:
                    GasConduits.Remove(Grid.PosToCell(this));
                    break;
                case 1:
                    LiquidConduits.Remove(Grid.PosToCell(this));
                    break;
                case 2:
                    SolidConduits.Remove(Grid.PosToCell(this));
                    break;
            }
            AllConduits.Remove(this);
            Unsubscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            base.OnCleanUp();
        }

        public string CheckboxTitleKey => "STRINGS.PAINTABLEBUILDING.TITLE";

        public string CheckboxLabel => STRINGS.PAINTABLEBUILDING.LABEL;

        public string CheckboxTooltip => STRINGS.PAINTABLEBUILDING.TOOLTIP;

        public bool GetCheckboxValue() => ShowInOverlay;

        public void SetCheckboxValue(bool value)
        {
            _showInOverlay = value;
            RefreshColor();
        }

        internal static bool TryGetColorable(int cell, ColourableBuilding building, out ColourableBuilding target)
        {
            target = null;
            switch (building.conduitType)
            {
                case 0:
                    if(GasConduits.ContainsKey(cell))
                        target = GasConduits[cell];      
                    break;
                case 1:
                    if (LiquidConduits.ContainsKey(cell))
                        target = LiquidConduits[cell];
                    break;
                case 2:
                    if (SolidConduits.ContainsKey(cell))
                        target = SolidConduits[cell];
                    break;
            }
            return target !=null;
        }
        internal static bool TryGetColorableBridge(int cell, ColourableBuilding building, out ColourableBuilding target)
        {
            target = null;
            switch (building.conduitType)
            {
                case 0:
                    if (GasConduitBridges.ContainsKey(cell))
                        target = GasConduitBridges[cell];
                    break;
                case 1:
                    if (LiquidConduitBridges.ContainsKey(cell))
                        target = LiquidConduitBridges[cell];
                    break;
                case 2:
                    if (SolidConduitBridges.ContainsKey(cell))
                        target = SolidConduitBridges[cell];
                    break;
            }
            return target != null;
        }
    }
}
