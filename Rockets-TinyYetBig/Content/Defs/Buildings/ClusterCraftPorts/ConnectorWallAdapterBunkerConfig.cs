using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
	public class ConnectorWallAdapterBunkerConfig : IBuildingConfig
	{
		public const string ID = "RTB_WallConnectionAdapterBunker";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{

			string[] Materials = [

				SimHashes.Steel.ToString(),
				MATERIALS.REFINED_METAL
			];
			float[] MaterialCosts = [800, 100 ];

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
					ID,
					1,
					2,
					"rocket_loader_extension_bunker_kanim",
					//	"loader_wall_adapter_tile_kanim",
					200,
					60f,
					MaterialCosts,
					Materials,
					1600f,
					BuildLocationRule.Tile,
					noise: NOISE_POLLUTION.NONE,
					decor: BUILDINGS.DECOR.PENALTY.TIER0);

			//BuildingTemplates.CreateFoundationTileDef(buildingDef);

			buildingDef.TileLayer = ObjectLayer.FoundationTile;
			buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
			buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.IsFoundation = true;
			buildingDef.ThermalConductivity = 0.01f;
			//buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.DefaultAnimState = "off";
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.CanMove = false;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.UseStructureTemperature = false;
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

			SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
			simCellOccupier.doReplaceElement = true;
			simCellOccupier.strengthMultiplier = 10f;
			simCellOccupier.notifyOnMelt = true;

			go.AddOrGet<TileTemperature>();
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;


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
			SymbolOverrideControllerUtil.AddToPrefab(go);
			go.TryGetComponent<KPrefabID>(out var prefabId);

			prefabId.AddTag(GameTags.FloorTiles);
			prefabId.AddTag(GameTags.Bunker);
		}
	}
}
