using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace UtilityGlass
{
    internal class ReinforcedGlassConfig : IBuildingConfig
    {
        public const string ID = "ug_reinforcedglass";
        public static readonly int BlockTileConnectorID = Hash.SDBMLower("tiles_ug_reinforcedglass_tops");

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR2 = new float[2]{50f,50f};
            string[] transparents = new string[2] { "Transparent" ,"Steel"};
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR0 = BUILDINGS.DECOR.BONUS.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "ug_reinforcedglass_kanim", 100, 30f, tieR2, transparents, 800f, BuildLocationRule.Tile, tieR0, noise);
            BuildingTemplates.CreateFoundationTileDef(buildingDef);
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.AudioCategory = "Glass";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.GlassTile;
            buildingDef.isKAnimTile = true;
            buildingDef.BlockTileIsTransparent = true;
            buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
            buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_info");
            buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_place_info");
            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;

            AssetUtils.AddCustomTileAtlas(buildingDef, ID, true, "tiles_glass");
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.setTransparent = true;
            simCellOccupier.strengthMultiplier = 10f;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = BlockTileConnectorID;
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
            //go.GetComponent<KPrefabID>().AddTag(GameTags.Window);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            KPrefabID prefabId = go.GetComponent<KPrefabID>();
            prefabId.AddTag(GameTags.Bunker);
            prefabId.AddTag(GameTags.FloorTiles);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<KAnimGridTileVisualizer>();
        }
    }

}
