using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace LightBridge.Buildings
{
    internal class LightBridgeConfig : IBuildingConfig
    {
        public const string ID = "LB_LightBridgeEmitter";
        public override BuildingDef CreateBuildingDef()
        {

            string[] Materials = new string[]
            {
                MATERIALS.REFINED_METAL,
                MATERIALS.TRANSPARENT
            };
            float[] MaterialCosts = new float[] { 400, 200 };

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                    ID,
                    2,
                    1,
                    "hard_light_bridge_kanim",
                    200,
                    60f,
                    MaterialCosts,
                    Materials,
                    1600f,
                    BuildLocationRule.Tile,
                    noise: NOISE_POLLUTION.NONE,
                    decor: BUILDINGS.DECOR.PENALTY.TIER0);

            BuildingTemplates.CreateFoundationTileDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            //buildingDef.ForegroundLayer = Grid.SceneLayer.FXFront;
            //buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.Floodable = false;
            buildingDef.DefaultAnimState = "on";
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.CanMove = false;
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

            //SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            //simCellOccupier.doReplaceElement = true;
            //simCellOccupier.notifyOnMelt = true;

            MakeBaseSolid.Def solidBase = go.AddOrGetDef<MakeBaseSolid.Def>();
            solidBase.occupyFoundationLayer = false;
            solidBase.solidOffsets = new CellOffset[]
            {
                new CellOffset(0, 0),
                new CellOffset(1, 0)
            };

            go.AddOrGet<TileTemperature>();
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
            LessUpdatedFakeFloor fakeFloorAdder = go.AddOrGet<LessUpdatedFakeFloor>();
            fakeFloorAdder.initiallyActive = false;
            go.AddOrGet<LightBridge>();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
