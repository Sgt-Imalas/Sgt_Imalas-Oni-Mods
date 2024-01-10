using KSerialization;
using Satsuma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static AmbienceManager;

namespace PaintYourPipes
{
    [SerializationConfig(MemberSerialization.OptIn)]
    internal class ColorableConduit : KMonoBehaviour, ICheckboxControl
    {
    //    public static string BuildFromColor = "FFFFFF";
    //    public static bool HasColorOverride = false;

        public static bool ShowOverlayTint;

        public static Dictionary<int,Dictionary<int, ColorableConduit>>ConduitsByLayer = new() 
        {
            { (int)ObjectLayer.GasConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LiquidConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.SolidConduit,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.GasConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, ColorableConduit>() },
            { (int)ObjectLayer.SolidConduitConnection,new Dictionary<int, ColorableConduit>() }

        };
        

        public static HashSet<ColorableConduit> AllConduits = new HashSet<ColorableConduit>();

        public static void RefreshAll()
        {
            foreach (var cond in AllConduits)
            {
                cond.RefreshColor();
            }
        }
        public static void RefreshOfConduitType(ObjectLayer targetLayer)
        {
            switch(targetLayer)
            {
                case ObjectLayer.GasConduit:
                    RefreshList(ObjectLayer.GasConduit);
                    RefreshList(ObjectLayer.GasConduitConnection);
                    break;
                case ObjectLayer.LiquidConduit:
                    RefreshList(ObjectLayer.LiquidConduit);
                    RefreshList(ObjectLayer.LiquidConduitConnection);
                    break;
                case ObjectLayer.SolidConduit:
                    RefreshList(ObjectLayer.SolidConduit);
                    RefreshList(ObjectLayer.SolidConduitConnection);
                    break;
            }
        }
        private static void RefreshList(ObjectLayer targetLayer)
        {
            if (!ConduitsByLayer.ContainsKey((int)targetLayer))
                return;

            foreach(var target in ConduitsByLayer[(int)targetLayer].Values)
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
            var col = Util.ColorFromHex(colorHex);
            col.a = SameConduitType(Patches.ActiveOverlay, this.buildingComplete.Def.ObjectLayer) ? 0 : 1;
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

        public void RefreshColor()
        {
            _animController.TintColour = TintColor;
        }
        public override void OnSpawn()
        {
            if(colorHex == string.Empty)
            {
                colorHex =
                   // HasColorOverride ? BuildFromColor : 
                    "FFFFFF";
            }

            if (!ConduitsByLayer.ContainsKey((int)buildingComplete.Def.ObjectLayer))
                ConduitsByLayer.Add((int)buildingComplete.Def.ObjectLayer, new());

            ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer].Add(Grid.PosToCell(this), this);

            AllConduits.Add(this);
            base.OnSpawn();
            //GameScheduler.Instance.ScheduleNextFrame("deayed initial refresh", (_) => RefreshColor());
            RefreshColor();
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }

        public override void OnCleanUp()
        {
            ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer].Remove(Grid.PosToCell(this));
            AllConduits.Remove(this);
            Unsubscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
            base.OnCleanUp();
        }

        public string CheckboxTitleKey => "STRINGS.PAINTABLEBUILDING.TITLE";

        public string CheckboxLabel => STRINGS.PAINTABLEBUILDING.LABEL;

        public string CheckboxTooltip => STRINGS.PAINTABLEBUILDING.TOOLTIP;

        public bool GetCheckboxValue() => ShowOverlayTint;

        public void SetCheckboxValue(bool value)
        {
            ShowOverlayTint = value;
            RefreshOfConduitType(Patches.ActiveOverlay);
        }

        public static bool SameConduitType(ObjectLayer first, ObjectLayer second)
        {
            switch (first)
            {
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
            }
            return false;
        }
        public  static bool SameConduitType(ColorableConduit first, ColorableConduit second) => SameConduitType(first.buildingComplete.Def.ObjectLayer, second.buildingComplete.Def.ObjectLayer);


        private static bool LayerFromColorBuilding(ColorableConduit building, bool bridges, out int targetLayer)
        {
            targetLayer = -1;
            switch (building.buildingComplete.Def.ObjectLayer)
            {
                case ObjectLayer.GasConduitConnection:
                case ObjectLayer.GasConduit:
                case ObjectLayer.GasConduitTile:
                    targetLayer = bridges ? (int)ObjectLayer.GasConduitConnection : (int) ObjectLayer.GasConduit;
                    break;
                case ObjectLayer.LiquidConduitConnection:
                case ObjectLayer.LiquidConduit:
                case ObjectLayer.LiquidConduitTile:
                    targetLayer = bridges ? (int)ObjectLayer.LiquidConduitConnection: (int) ObjectLayer.LiquidConduit;
                    break;
                case ObjectLayer.SolidConduitConnection:
                case ObjectLayer.SolidConduit:
                case ObjectLayer.SolidConduitTile:
                    targetLayer = bridges ? (int)ObjectLayer.SolidConduitConnection : (int) ObjectLayer.SolidConduit;
                    break;
            }

            return (targetLayer != -1);
        }

        internal static bool TryGetColorable(int cell, ColorableConduit building, out ColorableConduit target , bool bridges = false)
        {
            target = null;
            if (!LayerFromColorBuilding(building, bridges, out int layer))
                return false;

            if (!ConduitsByLayer[layer].ContainsKey(cell))
                return false;
            target = ConduitsByLayer[layer][cell]; 

            return target !=null;
        }
        internal static bool TryGetColorableBridge(int targetCell, ColorableConduit building, out ColorableConduit target) => TryGetColorable(targetCell, building, out target, true);
    }
}
