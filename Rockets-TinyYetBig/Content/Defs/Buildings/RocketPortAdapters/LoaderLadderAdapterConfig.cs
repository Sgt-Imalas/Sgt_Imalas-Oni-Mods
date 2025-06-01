using Rockets_TinyYetBig.NonRocketBuildings;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
	public class LoaderLadderAdapterConfig : IBuildingConfig
	{
		public const string ID = "RTB_LadderConnectionAdapter";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{

			string[] Materials = new string[]
			{
				MATERIALS.REFINED_METAL
			};
			float[] MaterialCosts = new float[] { 300f };

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
					ID,
					1,
					2,
					//"loader_ladder_adapter_tile_kanim",
					"rocket_loader_extension_ladder_kanim",
					200,
					40f,
					MaterialCosts,
					Materials,
					1600f,
					BuildLocationRule.NotInTiles,
					noise: NOISE_POLLUTION.NONE,
					decor: BUILDINGS.DECOR.PENALTY.TIER0);

			//BuildingTemplates.CreateLadderDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront+1; //render ladder above building connected to the right
			//buildingDef.ForegroundLayer = Grid.SceneLayer.FXFront;
			//buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.DefaultAnimState = "off";
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;

			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.CanMove = false;
			buildingDef.UseStructureTemperature = false;
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

			//SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
			//simCellOccupier.doReplaceElement = true;
			//simCellOccupier.notifyOnMelt = true;

			Ladder ladder = go.AddOrGet<Ladder>();
			ladder.offsets =
			[
				CellOffset.none,
				new CellOffset(0,1)
			];
			ladder.upwardsMovementSpeedMultiplier = 1.5f;
			ladder.downwardsMovementSpeedMultiplier = 1.5f;

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
			var anim = go.AddOrGet<AnimTileable>();
			anim.tags = [LoaderLadderAdapterConfig.ID,ReinforcedLadderConfig.ID];
			go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
			go.AddOrGet<RocketPortLadderHider>();
		}
	}
}
