using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    internal class PlatedRocketWallConfig : IBuildingConfig
    {
        public const string ID = "RTB_PlatedRocketWallTile";
        public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_rocket_wall_int");

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR2 = {10000f };
            string[] construction_materials = new string[1]
            {
                SimHashes.Lead.ToString()
            };
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR0 = BUILDINGS.DECOR.BONUS.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "floor_rocket_kanim", 1000, 60f, tieR2, construction_materials, 800f, BuildLocationRule.Tile, tieR0, noise);
            buildingDef.DebugOnly = true;
            BuildingTemplates.CreateFoundationTileDef(buildingDef);
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.Replaceable = false;
            buildingDef.Invincible = true;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.isKAnimTile = true;
            buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("tiles_rocket_wall_int");
            buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_rocket_wall_int_place");
            buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
            buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_rocket_wall_ext_decor_info");
            buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_rocket_wall_ext_place_decor_info");
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.strengthMultiplier = 10f;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = RocketWallTileConfig.BlockTileConnectorID;
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.Bunker);
            component.AddTag(GameTags.FloorTiles);
            component.AddTag(GameTags.RocketEnvelopeTile);
            component.AddTag(GameTags.NoRocketRefund);
            go.GetComponent<Deconstructable>().allowDeconstruction = false;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<KAnimGridTileVisualizer>();
        }
    }
}
