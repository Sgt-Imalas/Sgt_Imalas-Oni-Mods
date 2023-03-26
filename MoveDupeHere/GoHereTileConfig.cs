using MoveDupeHere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace LogicSatellites.Buildings
{
    class GoHereTileConfig: IBuildingConfig
    {
        public const string ID = "MDH_GoHereTile";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;
        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[2]
            {
                25f,
                75f
            };
            string[] materialType = new string[2]
            {
                MATERIALS.REFINED_METAL,
                MATERIALS.BUILDABLERAW
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: "moveHereTile_kanim",
                hitpoints: 30,
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
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.TileLayer = ObjectLayer.FoundationTile;
            buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;

            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.DefaultAnimState = "off_up";
            buildingDef.BaseTimeUntilRepair = -1f;

            buildingDef.RequiresPowerInput = true;

            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 10f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.InputPort(MoveDupeHereSM.PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_INACTIVE, true)
            };

            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.doReplaceElement = true;
            simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.BONUS_2;
            simCellOccupier.notifyOnMelt = true;

        }


        public override void DoPostConfigureComplete(GameObject go)
        {
            var ownable = go.AddOrGet<Ownable>();
            ownable.tintWhenUnassigned = false;
            ownable.slotID = Db.Get().AssignableSlots.RocketCommandModule.Id;
            var mdh =go.AddOrGet<MoveDupeHereSM>();
            mdh.targetCellOffset = new CellOffset(0, 1);
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
           
        }
    }
}
