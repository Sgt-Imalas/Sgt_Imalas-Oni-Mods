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
    class ReinforcedConcreteTileConfig : IBuildingConfig
	{
		public static string ID = "ReinforcedConcreteTile";

		public override BuildingDef CreateBuildingDef()
		{
			string kanim = "floor_concrete_kanim";
			float[] mass = [100, 50];
			string[] cost = [ModElements.ConcreteBlock_Solid.Tag.ToString(),GameTags.RefinedMetal.ToString()];

			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 1, kanim, 100, 5f, mass, cost, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NONE);
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
			def.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_info");
			def.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_bunker_tops_decor_place_info");
			def.DragBuild = true;

			AssetUtils.AddCustomTileAtlas(def, "tiles_concrete", false, "tiles_solid");
			AssetUtils.AddCustomTileTops(def, "tiles_concrete_tops", false, "tiles_bunker_tops_decor_info");

			def.AddSearchTerms((string)global::STRINGS.SEARCH_TERMS.TILE);
			def.AddSearchTerms("Bunker");

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), tag);

			var simCellOccupier = go.AddOrGet<SimCellOccupier>();
			simCellOccupier.notifyOnMelt = true;
			simCellOccupier.doReplaceElement = true;
			simCellOccupier.strengthMultiplier = 12f;
			simCellOccupier.movementSpeedMultiplier = 1.1f; //== DUPLICANTSTATS.MOVEMENT_MODIFIERS.BONUS_1;

			go.AddOrGet<TileTemperature>();
			go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = Hash.SDBMLower(ID.ToLowerInvariant() + "_connector_id");
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
			go.AddOrGet<TileTemperature>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);
			var prefab = go.GetComponent<KPrefabID>();
			prefab.AddTag(GameTags.FloorTiles, false);
			prefab.AddTag(GameTags.Bunker, false);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<KAnimGridTileVisualizer>();
		}
	}
}
