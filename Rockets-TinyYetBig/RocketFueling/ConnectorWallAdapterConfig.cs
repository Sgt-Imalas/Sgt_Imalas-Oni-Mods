using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
	public class ConnectorWallAdapterConfig : IBuildingConfig
	{
		public const string ID = "RTB_WallConnectionAdapter";
		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
		public override BuildingDef CreateBuildingDef()
		{

			string[] Materials = new string[]
			{
				MATERIALS.BUILDABLERAW,
				MATERIALS.REFINED_METAL
			};
			float[] MaterialCosts = new float[] { 800, 100 };

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
					ID,
					1,
					2,
					"loader_wall_adapter_tile_kanim",
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
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.IsFoundation = true;

			//buildingDef.ForegroundLayer = Grid.SceneLayer.FXFront;
			buildingDef.ThermalConductivity = 0.01f;
			//buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.DefaultAnimState = "on";
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
			simCellOccupier.notifyOnMelt = true;

			//MakeBaseSolid.Def solidBase = go.AddOrGetDef<MakeBaseSolid.Def>();
			//solidBase.solidOffsets = new CellOffset[]
			//{
			//    new CellOffset(0, 0),
			//    new CellOffset(0, 1)
			//};

			go.AddOrGet<Insulator>();
			go.AddComponent<Insulator>().offset = new CellOffset(0, 1);
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

			go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
		}
	}
}
