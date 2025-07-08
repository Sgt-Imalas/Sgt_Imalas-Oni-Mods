using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles
{
    class StructureTileConfig : IBuildingConfig
	{
		public static string ID = "StructureTile";

		public override BuildingDef CreateBuildingDef()
		{
			//string tileID = isDefaultID ? "MonoElementTile" : ID;
			string kanim = "floor_structure_kanim";
			float[] mass = [100];
			string[] cost = [GameTags.RefinedMetal.ToString()];

			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 1, kanim, 100, 5f, mass, cost, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NONE);
			BuildingTemplates.CreateFoundationTileDef(def);
			def.Floodable = false;
			def.Overheatable = false;
			def.Entombable = false;
			def.UseStructureTemperature = false;
			def.AudioCategory = "Metal";
			def.AudioSize = "small";
			def.BaseTimeUntilRepair = -1f;
			def.SceneLayer = Grid.SceneLayer.TileMain;
			def.isKAnimTile = true;
			def.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
			def.BlockTileAtlas = Assets.GetTextureAtlas("tiles_metal");
			def.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_metal_place");
			def.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_info");
			def.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_place_info");
			def.DragBuild = true;

			AssetUtils.AddCustomTileAtlas(def, "tiles_structure", false, "tiles_solid");
			//AssetUtils.AddCustomTileTops(def, "tiles_structure_tops", false, "tiles_metal_tops_decor_info");

			def.AddSearchTerms((string)global::STRINGS.SEARCH_TERMS.TILE);

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), tag);

			var simCellOccupier = go.AddOrGet<SimCellOccupier>();
			simCellOccupier.notifyOnMelt = true;
			simCellOccupier.doReplaceElement = false;
			simCellOccupier.strengthMultiplier = 24f;
			simCellOccupier.movementSpeedMultiplier = 1.4f; //== DUPLICANTSTATS.MOVEMENT_MODIFIERS.BONUS_5;

			go.AddOrGet<TileTemperature>();
			go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = Hash.SDBMLower("tiles_metal_tops");
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
			//go.AddOrGet<TileTemperature>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);
			var prefab = go.GetComponent<KPrefabID>();
			prefab.AddTag(GameTags.FloorTiles, false);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<KAnimGridTileVisualizer>();
		}
	}
}
