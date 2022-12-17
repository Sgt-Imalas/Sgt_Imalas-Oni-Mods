using MoveDupeHere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static LogicGateBase;

namespace ConveyorTiles
{
    class ConveyorTile: IBuildingConfig
    {
        public const string ID = "CT_ConveyorTile";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;
        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[]
            {
                60f,
                10
            };
            string[] materialType = new string[]
            {
                MATERIALS.REFINED_METAL,
                MATERIALS.PLASTIC
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: "conveyorbelt_kanim",
                hitpoints: 15,
                construction_time: 20f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 1600f,
                BuildLocationRule.Tile,
                decor: decorValue,
                noise: noiseLevel);


            buildingDef.IsFoundation = true;
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.TileLayer = ObjectLayer.FoundationTile;
            buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;

            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.DefaultAnimState = "off";
            buildingDef.BaseTimeUntilRepair = -1f;

            buildingDef.RequiresPowerInput = true;

            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 4f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.05f;
            buildingDef.AddLogicPowerPort = false;
            //buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
            //{
            //    LogicPorts.Port.InputPort(ConveyorTileSM.PORT_ID, new CellOffset(0, 0), (string) global::STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT, (string) global::STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_ACTIVE, (string) global::STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_INACTIVE, true)
            //};

            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.doReplaceElement = true;
            simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.NEUTRAL;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<AnimTileable>();

        }


        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<ConveyorTileSM>();
            //mdh.targetCellOffset = new CellOffset(0, 1);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
           
        }
    }
}
