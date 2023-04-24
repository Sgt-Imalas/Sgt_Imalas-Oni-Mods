using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;
using UnityEngine;
using static KAnim;
using static STRINGS.DUPLICANTS.ATTRIBUTES;
using TUNING;
using UtilLibs;

namespace Rockets_TinyYetBig.RocketFueling
{
    public class LoaderTravelTubeAdapterConfig : IBuildingConfig
    {
        public const string ID = "RTB_TravelTubeConnectionAdapter";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {

            string[] Materials = new string[]
            {
                MATERIALS.REFINED_METAL 
            };
            float[] MaterialCosts = new float[] { 300f };

        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                ID, 
                1, 
                2,
                "loader_ladder_adapter_tile_kanim", 
                200, 
                40f, 
                MaterialCosts,
                Materials,
                800f, 
                BuildLocationRule.Tile, 
                noise: NOISE_POLLUTION.NONE, 
                decor: BUILDINGS.DECOR.PENALTY.TIER0);

            //BuildingTemplates.CreateLadderDef(buildingDef);
            buildingDef.TileLayer = ObjectLayer.TravelTubeTile;
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
            buildingDef.AudioCategory = "Plastic";
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


            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(BaseModularLaunchpadPortConfig.LinkTag);
            component.AddTag(GameTags.ModularConduitPort);

            ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
            def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
            def.objectLayer = ObjectLayer.Building;
            go.AddOrGet<AnimTileable>();

            go.AddOrGet<VariedSizeTravelTubePiece>();
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            AddNetworkLink(go).visualizeOnly = true;
            go.AddOrGet<BuildingCellVisualizer>();
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            AddNetworkLink(go).visualizeOnly = true;
            go.AddOrGet<BuildingCellVisualizer>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            AddNetworkLink(go).visualizeOnly = false;
            go.AddOrGet<BuildingCellVisualizer>();
            go.AddOrGet<KPrefabID>().AddTag(GameTags.TravelTubeBridges);
        }

        protected virtual TravelTubeUtilityNetworkLink AddNetworkLink(GameObject go)
        {
            TravelTubeUtilityNetworkLink utilityNetworkLink = go.AddOrGet<TravelTubeUtilityNetworkLink>();
            utilityNetworkLink.link1 = new CellOffset(0, -1);
            utilityNetworkLink.link2 = new CellOffset(0, 2);

            return utilityNetworkLink;
        }
    }
}
