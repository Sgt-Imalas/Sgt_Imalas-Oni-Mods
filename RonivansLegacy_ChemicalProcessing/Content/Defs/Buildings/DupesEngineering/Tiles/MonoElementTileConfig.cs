using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using TUNING;
using UnityEngine;
using UtilLibs;
using static RoomProber;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles
{
	class MonoElementTileConfig : IBuildingConfig
	{
		public static string DEFAULT_ID = "MonoElementTile";
		public static string ID = DEFAULT_ID;

		public static HashSet<Tag> TileTypes =
			[SimHashes.SandStone.CreateTag(),
			SimHashes.Granite.CreateTag(),
			SimHashes.IgneousRock.CreateTag(),
			SimHashes.Obsidian.CreateTag(),
			SimHashes.Brick.CreateTag(),
			];

		public SimHashes Element = SimHashes.Void;
		public static string[] defaultCost = [string.Join("&", TileTypes.Select(tag => tag.ToString()).ToArray())];

		public static string GetCustomTileID(SimHashes element)
		{
			if (element == SimHashes.Void)
				return DEFAULT_ID;
			return $"Custom{element}Tile";
		}

		public override BuildingDef CreateBuildingDef()
		{
			if (Element != SimHashes.Void)
			{
				ID = $"Custom{Element}Tile";
				MultivariantBuildings.RegisterMaterialVariant(DEFAULT_ID, ID, Element.CreateTag());
			}

			bool isDefaultID = ID == DEFAULT_ID;


			string idLower = Element.ToString().ToLowerInvariant();
			//string tileID = isDefaultID ? "MonoElementTile" : ID;
			string kanim = isDefaultID ? "floor_sandstone_kanim" : $"floor_{idLower}_kanim";
			string[] cost = isDefaultID ? defaultCost : [Element.ToString()];
			//SgtLogger.l("Kanimname: " + kanim);

			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 1, kanim, 100, 5f, [200], cost, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NONE);
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

			string atlasId = "tiles_" + idLower;
			string topsId = "tiles_" + idLower + "_tops";

			SgtLogger.l("atlas id: "+atlasId+", tops id: "+topsId);
			def.DragBuild = true;
			if (isDefaultID)
				return def;

			AssetUtils.AddCustomTileAtlas(def, atlasId, false, "tiles_solid");
			AssetUtils.AddCustomTileTops(def, topsId, false, "tiles_bunker_tops_decor_info");
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
			simCellOccupier.strengthMultiplier = 1.5f;
			simCellOccupier.movementSpeedMultiplier = 1.2f; //== DUPLICANTSTATS.MOVEMENT_MODIFIERS.BONUS_1;

			go.AddOrGet<TileTemperature>();
			go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = Hash.SDBMLower(ID.ToLowerInvariant() + "_connector_id");
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
			go.AddOrGet<TileTemperature>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);
			go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles, false);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<KAnimGridTileVisualizer>();
		}
	}
	class MonoElementTileSandstoneConfig : MonoElementTileConfig
	{
		public MonoElementTileSandstoneConfig()
		{
			Element = SimHashes.SandStone;
		}
	}
	class MonoElementTileGraniteConfig : MonoElementTileConfig
	{
		public MonoElementTileGraniteConfig()
		{
			Element = SimHashes.Granite;
		}
	}
	class MonoElementTileIgneousRockConfig : MonoElementTileConfig
	{
		public MonoElementTileIgneousRockConfig()
		{
			Element = SimHashes.IgneousRock;
		}

		internal static void RegisterLegacyMigration()
		{
			var id = GetCustomTileID(SimHashes.IgneousRock);
			var prefab = Assets.TryGetPrefab(id);
			var savemng = SaveLoader.Instance.saveManager;

			if(savemng != null && prefab != null)
				savemng.prefabMap.Add("CustomIgneousTile", prefab);
		}
	}
	class MonoElementTileObsidianConfig : MonoElementTileConfig
	{
		public MonoElementTileObsidianConfig()
		{
			Element = SimHashes.Obsidian;
		}
	}
	class MonoElementTileBrickConfig : MonoElementTileConfig
	{
		public MonoElementTileBrickConfig()
		{
			Element = SimHashes.Brick;
		}

	}
}
