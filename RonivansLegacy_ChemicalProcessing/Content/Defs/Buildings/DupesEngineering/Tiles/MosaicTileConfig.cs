using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles
{
    class MosaicTileConfig : IBuildingConfig
	{
		public static string ID = "AIO_MosaicTile";

		public override BuildingDef CreateBuildingDef()
		{
			float[] mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
			string[] materials = MATERIALS.PRECIOUS_ROCKS;
			EffectorValues tieR1 = BUILDINGS.DECOR.BONUS.TIER2;
			EffectorValues noise = NOISE_POLLUTION.NONE;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "glassy_tile_kanim", 100, 30f, mass, materials, 1600f, BuildLocationRule.Tile, tieR1, noise);
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.Overheatable = false;
			buildingDef.IsFoundation = true;
			buildingDef.UseStructureTemperature = false;
			buildingDef.TileLayer = ObjectLayer.FoundationTile;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
			buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
			buildingDef.isKAnimTile = true;
			buildingDef.BlockTileAtlas = Assets.GetTextureAtlas("tiles_POI");
			buildingDef.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_POI_place");
			buildingDef.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
			buildingDef.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_POI_tops_decor_info");
			buildingDef.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_POI_tops_decor_info");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<SimCellOccupier>().doReplaceElement = true;
			go.AddOrGet<TileTemperature>();
			go.AddOrGet<SimCellOccupier>().movementSpeedMultiplier = 1.5f; //== DUPLICANTSTATS.MOVEMENT_MODIFIERS.BONUS_3;
			go.AddOrGet<KAnimGridTileVisualizer>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
			go.AddComponent<SimTemperatureTransfer>();
			go.GetComponent<Deconstructable>().allowDeconstruction = true;

			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(GameTags.FloorTiles);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<KAnimGridTileVisualizer>();
		}
	}
}
