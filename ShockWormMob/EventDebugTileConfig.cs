using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ShockWormMob
{
    internal class EventDebugTileConfig : IBuildingConfig
    {
        public const string ID = "LCTG_DebugTile";

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] farmable = MATERIALS.FARMABLE;
            EffectorValues none1 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = none1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "farmtilerotating_kanim", 100, 30f, tieR2, farmable, 1600f, BuildLocationRule.Tile, none2, noise);
            BuildingTemplates.CreateFoundationTileDef(buildingDef);
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            buildingDef.PermittedRotations = PermittedRotations.FlipV;
            buildingDef.DragBuild = true;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.doReplaceElement = true;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<AnimTileable>();
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.AddOrGet<TestEventSpawner>();
        }
    }
}
