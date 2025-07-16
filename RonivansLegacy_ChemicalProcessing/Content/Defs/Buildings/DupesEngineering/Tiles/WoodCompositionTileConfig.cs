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
    class WoodCompositionTileConfig : IBuildingConfig
	{
		public static string ID = "WoodenCompositionTile";

		public override BuildingDef CreateBuildingDef()
		{
			string kanim = "floor_wooden_kanim";
			float[] mass = [375,75];
			string[] cost = [GameTags.BuildableRaw.ToString(),GameTags.BuildingWood.ToString()];

			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 1, kanim, 100, 5f, mass, cost, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NONE);
			BuildingTemplates.CreateFoundationTileDef(def);
			def.ThermalConductivity = 0.01f; // THERMAL CONDUCTIVITY
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
			def.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_metal_tops_decor_info");
			def.DragBuild = true;

			AssetUtils.AddCustomTileAtlas(def, "tiles_wooden", false, "tiles_solid");
			AssetUtils.AddCustomTileTops(def, "tiles_wooden_tops", false, "tiles_bunker_tops_decor_info");

			def.AddSearchTerms((string)global::STRINGS.SEARCH_TERMS.TILE);

			return def;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), tag);

			var simCellOccupier = go.AddOrGet<SimCellOccupier>();
			simCellOccupier.notifyOnMelt = true;
			simCellOccupier.doReplaceElement = true;
			simCellOccupier.movementSpeedMultiplier = 1.2f; //== DUPLICANTSTATS.MOVEMENT_MODIFIERS.BONUS_5;
			go.AddOrGet<Insulator>();

			go.AddOrGet<TileTemperature>();
			go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = Hash.SDBMLower("tiles_wooden_comp_tops");
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
