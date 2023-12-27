using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class VerticalAdapterBaseConfig : IBuildingConfig
    {
        public const string ID = "RTB_VerticalAdapterBase";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {

            string[] Materials = new string[]
            {
                MATERIALS.REFINED_METAL
            };
            float[] MaterialCosts = new float[] { 400f };

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                    ID,
                    3,
                    2,
                    "loader_ladder_adapter_tile_kanim",
                    200,
                    60f,
                    MaterialCosts,
                    Materials,
                    1600f,
                    BuildLocationRule.NotInTiles,
                    noise: NOISE_POLLUTION.NONE,
                    decor: BUILDINGS.DECOR.PENALTY.TIER0);

            //BuildingTemplates.CreateLadderDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            //buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
            //buildingDef.ForegroundLayer = Grid.SceneLayer.FXFront;
            //buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = false;
            buildingDef.DefaultAnimState = "on";
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;

            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.CanMove = false;
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

            //SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            //simCellOccupier.doReplaceElement = true;
            //simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<VerticalPortAttachment>().TopOffset=new CellOffset(0,2);
            

            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(BaseModularLaunchpadPortConfig.LinkTag);
            component.AddTag(GameTags.ModularConduitPort);

            ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
            def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
            def.objectLayer = ObjectLayer.Building;
            go.AddOrGet<AnimTileable>();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
