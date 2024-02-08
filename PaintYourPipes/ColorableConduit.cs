using KSerialization;
using Satsuma;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AmbienceManager;
using static ConduitFlow;
using static STRINGS.UI.STARMAP;

namespace PaintYourPipes
{
    [SerializationConfig(MemberSerialization.OptIn)]
    internal class ColorableConduit : KMonoBehaviour, ISidescreenButtonControl
    {

        public IBridgedNetworkItem NetworkItem;
        [MyCmpGet] public SolidConduit solidConduit;//extrawurst for solid conduits

        public static bool ShowOverlayTint;

        public static Dictionary<int, Dictionary<int, ColorableConduit>> ConduitsByLayer = new()
        {
            { (int)ObjectLayer.GasConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LiquidConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.SolidConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.Wire,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.GasConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.SolidConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.WireConnectors,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.Building,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LogicWire,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LogicGate,new Dictionary<int, ColorableConduit>() },

        };
        public static void FlushDictionary()
        {
            AllConduits.Clear();
            ConduitsByLayer = new()
            {
            { (int)ObjectLayer.GasConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LiquidConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.SolidConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.Wire,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.GasConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.SolidConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.WireConnectors,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.Building,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LogicWire,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LogicGate,new Dictionary<int, ColorableConduit>() },
            };
        }

        public static HashSet<ColorableConduit> AllConduits = new HashSet<ColorableConduit>();

        public static void RefreshAll()
        {
            foreach (var cond in AllConduits)
            {
                cond.RefreshColor();
            }
        }

        public static List<ObjectLayer> GetRelevantObjectLayers(ObjectLayer target)
        {
            switch (target)
            {
                case ObjectLayer.Wire:
                case ObjectLayer.WireConnectors:
                case ObjectLayer.ReplacementWire:
                case ObjectLayer.Building: //Edge case: high wattage tile bridges
                    return new List<ObjectLayer>() { ObjectLayer.Wire, ObjectLayer.WireConnectors, ObjectLayer.ReplacementWire, ObjectLayer.Building };
                case ObjectLayer.GasConduitConnection:
                case ObjectLayer.GasConduit:
                case ObjectLayer.GasConduitTile:
                    return new List<ObjectLayer>() { ObjectLayer.GasConduit, ObjectLayer.GasConduitConnection, ObjectLayer.GasConduitTile };
                case ObjectLayer.LiquidConduitConnection:
                case ObjectLayer.LiquidConduit:
                case ObjectLayer.LiquidConduitTile:
                    return new List<ObjectLayer>() { ObjectLayer.LiquidConduit, ObjectLayer.LiquidConduitConnection, ObjectLayer.LiquidConduitTile };
                case ObjectLayer.SolidConduitConnection:
                case ObjectLayer.SolidConduit:
                case ObjectLayer.SolidConduitTile:
                    return new List<ObjectLayer>() { ObjectLayer.SolidConduit, ObjectLayer.SolidConduitConnection, ObjectLayer.SolidConduitTile };
                case ObjectLayer.LogicWire:
                case ObjectLayer.LogicWireTile:
                case ObjectLayer.LogicGate:
                    return new List<ObjectLayer>() { ObjectLayer.LogicWire, ObjectLayer.LogicGate, ObjectLayer.LogicWireTile };
                default: return new List<ObjectLayer>() { };
            }
        }

        public static void RefreshOfConduitType(ObjectLayer targetLayer)
        {
            var layers = GetRelevantObjectLayers(targetLayer);

            if(layers.Count == 0)
                RefreshAll();

            foreach (var layerTarget in layers)
            {
                RefreshList(layerTarget);

            }
        }
        private static void RefreshList(ObjectLayer targetLayer)
        {
            if (!ConduitsByLayer.ContainsKey((int)targetLayer))
                return;

            foreach (var target in ConduitsByLayer[(int)targetLayer].Values)
                target.RefreshColor();
        }

        [MyCmpAdd]
        CopyBuildingSettings buildingSettings;

        [MyCmpGet]
        KPrefabID prefab;
        [MyCmpGet]
        public BuildingComplete buildingComplete;

        [Serialize]
        private string colorHex = string.Empty;

        [MyCmpGet]
        KBatchedAnimController _animController;
        public KBatchedAnimController AnimController => _animController;
        private static readonly EventSystem.IntraObjectHandler<ColorableConduit> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ColorableConduit>((component, data) => component.OnCopySettings(data));

        public Color TintColor => GetColor();
        public string ColorHex => colorHex;

        public Color GetColor()
        {
            if (colorHex == null || colorHex == string.Empty)
                colorHex = "FFFFFF";

            var col = Util.ColorFromHex(colorHex);
            col.a = ShowOverlayTint&&SameConduitType(Patches.ActiveOverlay, this.buildingComplete.Def.ObjectLayer) ? 0 : 1;
            return col;

        }
        public void SetColor(Color color)
        {
            colorHex = color.ToHexString();
            RefreshColor();
        }

        private void OnCopySettings(object obj)
        {
            if (obj != null && obj is GameObject go && go.TryGetComponent<ColorableConduit>(out var SourceBuilding) && SameConduitType(SourceBuilding, this))
            {
                SetColor(SourceBuilding.TintColor);
                RefreshColor();
            }
        }

        public void RefreshColor(Color Override = default)
        {
            if(Override != default)
            {
                _animController.TintColour = Override;
            }
            else
                _animController.TintColour = TintColor;

            if (_animController.enabled)
            {
                _animController.enabled = false;
                _animController.enabled = true;
            }
        }
        private void OnNewConstruction(object data)
        {
            if (data is Constructable constructable
                && constructable.TryGetComponent(out ColorableConduit_UnderConstruction underConstruction))
            {
                colorHex =
                    underConstruction.HasData ?
                    underConstruction.ColorHex :
                    // HasColorOverride ? BuildFromColor : 
                    "FFFFFF";
            }
        }



        public override void OnSpawn()
        {
            if (colorHex == string.Empty)
                colorHex = "FFFFFF";

            if (!ConduitsByLayer.ContainsKey((int)buildingComplete.Def.ObjectLayer))
                ConduitsByLayer.Add((int)buildingComplete.Def.ObjectLayer, new());



            ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Min()] = this;
            ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Max()] = this;

            AllConduits.Add(this);
            base.OnSpawn();
            //GameScheduler.Instance.ScheduleNextFrame("deayed initial refresh", (_) => RefreshColor());
            NetworkItem = this.GetComponent<IBridgedNetworkItem>();
            RefreshColor();
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            Subscribe((int)GameHashes.NewConstruction, OnNewConstruction);
        }

        public override void OnCleanUp()
        {
            ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer].Remove((buildingComplete.PlacementCells.Min()));
            ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer].Remove((buildingComplete.PlacementCells.Max()));
            AllConduits.Remove(this);
            Unsubscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            base.OnCleanUp();
        }

        public string SidescreenButtonText => STRINGS.PAINTABLEBUILDING.LABEL;

        public string SidescreenButtonTooltip => STRINGS.PAINTABLEBUILDING.TOOLTIP;

        public static void SetOverlayTint(bool value)
        {
            ShowOverlayTint = value;
            RefreshOfConduitType(Patches.ActiveOverlay);
            //RefreshAll();
        }
        public static void ToggleOverlayTint() => SetOverlayTint(!ShowOverlayTint);


        public static void ToggleNormalTint(int layer)
        {
            if (!ConduitsByLayer.ContainsKey(layer)||ShowOverlayTint)
                return;
            foreach(var entry in ConduitsByLayer[layer])
            {
                entry.Value.RefreshColor(Color.white);
            }
        }

        public static bool SameConduitType(ObjectLayer first, ObjectLayer second)
        {
            switch (first)
            {
                case ObjectLayer.Wire:
                case ObjectLayer.WireConnectors:
                case ObjectLayer.ReplacementWire:
                case ObjectLayer.Building: //Edge case: high wattage tile bridges
                    return second == ObjectLayer.Wire || second == ObjectLayer.WireTile || second == ObjectLayer.WireConnectors || second == ObjectLayer.ReplacementWire || second == ObjectLayer.Building;
                case ObjectLayer.GasConduitConnection:
                case ObjectLayer.GasConduit:
                case ObjectLayer.GasConduitTile:
                    return second == ObjectLayer.GasConduit || second == ObjectLayer.GasConduitConnection || second == ObjectLayer.GasConduitTile;
                case ObjectLayer.LiquidConduitConnection:
                case ObjectLayer.LiquidConduit:
                case ObjectLayer.LiquidConduitTile:
                    return second == ObjectLayer.LiquidConduit || second == ObjectLayer.LiquidConduitConnection || second == ObjectLayer.GasConduitTile;
                case ObjectLayer.SolidConduitConnection:
                case ObjectLayer.SolidConduit:
                case ObjectLayer.SolidConduitTile:
                    return second == ObjectLayer.SolidConduit || second == ObjectLayer.SolidConduitConnection || second == ObjectLayer.GasConduitTile;
                case ObjectLayer.LogicWire:
                case ObjectLayer.LogicWireTile:
                case ObjectLayer.LogicGate:
                    return second == ObjectLayer.LogicWire || second == ObjectLayer.LogicWireTile || second == ObjectLayer.LogicGate;
            }
            return false;
        }
        public static bool SameConduitType(ColorableConduit first, ColorableConduit second) => SameConduitType(first.buildingComplete.Def.ObjectLayer, second.buildingComplete.Def.ObjectLayer);

        private static bool LayerFromColorBuilding(ColorableConduit building, bool bridges, out int targetLayer)
        {
            targetLayer = -1;
            switch (building.buildingComplete.Def.ObjectLayer)
            {
                case ObjectLayer.GasConduitConnection:
                case ObjectLayer.GasConduit:
                case ObjectLayer.GasConduitTile:
                    targetLayer = bridges ? (int)ObjectLayer.GasConduitConnection : (int)ObjectLayer.GasConduit;
                    break;
                case ObjectLayer.LiquidConduitConnection:
                case ObjectLayer.LiquidConduit:
                case ObjectLayer.LiquidConduitTile:
                    targetLayer = bridges ? (int)ObjectLayer.LiquidConduitConnection : (int)ObjectLayer.LiquidConduit;
                    break;
                case ObjectLayer.SolidConduitConnection:
                case ObjectLayer.SolidConduit:
                case ObjectLayer.SolidConduitTile:
                    targetLayer = bridges ? (int)ObjectLayer.SolidConduitConnection : (int)ObjectLayer.SolidConduit;
                    break;
                case ObjectLayer.Wire:
                case ObjectLayer.WireConnectors:
                case ObjectLayer.ReplacementWire:
                case ObjectLayer.Building: //Edge case: high wattage tile bridges
                    targetLayer = bridges ? (int)ObjectLayer.WireConnectors : (int)ObjectLayer.Wire;
                    break;
                case ObjectLayer.LogicWire:
                case ObjectLayer.LogicGate:
                case ObjectLayer.LogicWireTile:
                    targetLayer = bridges ? (int)ObjectLayer.LogicGate : (int)ObjectLayer.LogicWire;
                    break;
            }

            return (targetLayer != -1);
        }

        internal static bool TryGetColorable(int cell, ColorableConduit building, out ColorableConduit target, bool bridges = false)
        {
            target = null;
            if (!LayerFromColorBuilding(building, bridges, out int layer))
                return false;


            //Edge case: high wattage tile bridges
            bool trySecondaryLayer = (layer == (int)ObjectLayer.WireConnectors && bridges);
            if (!ConduitsByLayer[layer].ContainsKey(cell))
            {
                if (!trySecondaryLayer)
                    return false;

                layer = (int)ObjectLayer.Building;
                if (!ConduitsByLayer[layer].ContainsKey(cell))
                    return false;

            }


            target = ConduitsByLayer[layer][cell];

            return target != null;
        }
        internal static bool TryGetColorableBridge(int targetCell, ColorableConduit building, out ColorableConduit target) => TryGetColorable(targetCell, building, out target, true);

        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
        {
        }

        public bool SidescreenEnabled() => true;

        public bool SidescreenButtonInteractable() => NetworkItem != null || solidConduit!=null;

        public void OnSidescreenButtonPressed()
        {
            PaintCurrentNetwork();
        }

        public int HorizontalGroupID() => -1;

        public int ButtonSideScreenSortOrder() => 22;

        #region PaintNetworks
        HashSet<int> VisitedCells = new HashSet<int>();
        HashSet<UtilityNetwork> allNetworks = new HashSet<UtilityNetwork>();
        public void PaintCurrentNetwork()
        {
            if (NetworkItem == null&& solidConduit==null) return;
            IUtilityNetworkMgr mgr;
            switch (buildingComplete.Def.ObjectLayer)
            {
                case ObjectLayer.Wire:
                case ObjectLayer.WireConnectors:
                case ObjectLayer.ReplacementWire:
                case ObjectLayer.Building: //Edge case: high wattage tile bridges
                    mgr = Game.Instance.electricalConduitSystem;
                    break;
                case ObjectLayer.GasConduitConnection:
                case ObjectLayer.GasConduit:
                case ObjectLayer.GasConduitTile:
                    mgr = Game.Instance.gasConduitSystem;
                    break;
                case ObjectLayer.LiquidConduitConnection:
                case ObjectLayer.LiquidConduit:
                case ObjectLayer.LiquidConduitTile:
                    mgr = Game.Instance.liquidConduitSystem;
                    break;
                case ObjectLayer.SolidConduitConnection:
                case ObjectLayer.SolidConduit:
                case ObjectLayer.SolidConduitTile:
                    mgr = Game.Instance.solidConduitSystem;
                    break;
                case ObjectLayer.LogicWire:
                case ObjectLayer.LogicWireTile:
                case ObjectLayer.LogicGate:
                    mgr = Game.Instance.logicCircuitSystem;
                    break;
                default:
                    return;
            }
            allNetworks.Clear();
            VisitedCells.Clear(); 
            allNetworks.Clear();

            if(NetworkItem!=null)
                GetAllConnectedNetworks(NetworkItem.GetNetworkCell(),mgr);
            else
                GetAllConnectedNetworks(Grid.PosToCell(solidConduit), mgr);
            

            foreach (var targetLayer in GetRelevantObjectLayers(buildingComplete.Def.ObjectLayer))
            {
                if (!ConduitsByLayer.ContainsKey((int)targetLayer))
                    continue;
                foreach (var target in ConduitsByLayer[(int)targetLayer].Values)
                {
                    if(NetworkItem !=null && target.NetworkItem != null && target.NetworkItem.IsConnectedToNetworks(allNetworks))
                    {
                        target.SetColor(this.GetColor());
                    }
                    else if(solidConduit!=null && target.solidConduit!=null && allNetworks.Contains(target.solidConduit.GetNetwork()))
                    {
                        target.SetColor(this.GetColor());
                    }
                }
            }

        }
        private void GetAllConnectedNetworks(
            int cell,
            IUtilityNetworkMgr mgr)
        {
            if (VisitedCells.Contains(cell))
                return;
            VisitedCells.Add(cell);
            UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
            if (networkForCell == null)
                return;
            if(!allNetworks.Contains(networkForCell))
                allNetworks.Add(networkForCell);

            int connections = (int)mgr.GetConnections(cell, false);
            if ((connections & 2) != 0)
                this.GetAllConnectedNetworks(Grid.CellRight(cell), mgr);
            if ((connections & 1) != 0)
                this.GetAllConnectedNetworks(Grid.CellLeft(cell), mgr);
            if ((connections & 4) != 0)
                this.GetAllConnectedNetworks(Grid.CellAbove(cell), mgr);
            if ((connections & 8) != 0)
                this.GetAllConnectedNetworks(Grid.CellBelow(cell), mgr);
            
            //these networks dont go over endpoints
            if(mgr == Game.Instance.logicCircuitManager || mgr == Game.Instance.electricalConduitSystem)
            {
                return;
            }


            object endpoint = mgr.GetEndpoint(cell);
            if (endpoint == null || (endpoint is not FlowUtilityNetwork.NetworkItem networkItem))
                return;
            GameObject gameObject = networkItem.GameObject;
            if (gameObject == null)
                return;
            gameObject.GetComponent<IBridgedNetworkItem>()?.AddNetworks(allNetworks);
        }

        #endregion
    }
}
