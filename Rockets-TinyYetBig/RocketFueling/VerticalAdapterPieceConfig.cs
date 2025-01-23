using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
	internal class VerticalAdapterPieceConfig : IBuildingConfig
	{
		public const string ID = "RTB_VerticalAdapterPiece";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{

			string[] Materials = new string[]
			{
				MATERIALS.REFINED_METAL
			};
			float[] MaterialCosts = new float[] { 150f };

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
					ID,
					1,
					1,
					"conduit_link_piece_kanim",
					30,
					15f,
					MaterialCosts,
					Materials,
					1600f,
					BuildLocationRule.Anywhere,
					noise: NOISE_POLLUTION.NONE,
					decor: BUILDINGS.DECOR.PENALTY.TIER0);

			//BuildingTemplates.CreateLadderDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			//buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
			//buildingDef.ForegroundLayer = Grid.SceneLayer.FXFront;
			//buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.DefaultAnimState = "on";
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;

			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.CanMove = false;
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);


			go.AddOrGet<VerticalPortAttachment>();
			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(ModAssets.Tags.VerticalPortAttachementPoint);
			component.AddTag(BaseModularLaunchpadPortConfig.LinkTag);
			component.AddTag(GameTags.ModularConduitPort);

			Ladder ladder = go.AddOrGet<Ladder>();
			ladder.upwardsMovementSpeedMultiplier = 0.75f; //same as ladder bed
			ladder.downwardsMovementSpeedMultiplier = 0.75f;

			go.AddOrGet<AnimTileable>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}
