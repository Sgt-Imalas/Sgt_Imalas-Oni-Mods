using Rockets_TinyYetBig.RocketFueling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.RocketPortAdapters
{
	public class ConnectorBypassAdapterConfig : IBuildingConfig
	{
		public const string ID = "RTB_ConnectorBypassAdapter";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public override BuildingDef CreateBuildingDef()
		{

			string[] Materials = [MATERIALS.REFINED_METAL];
			float[] MaterialCosts = [400f];

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
					ID,
					3,
					2,
					"conduit_link_cross_kanim",
					200,
					60f,
					MaterialCosts,
					Materials,
					1600f,
					BuildLocationRule.Anywhere,
					noise: NOISE_POLLUTION.NONE,
					decor: BUILDINGS.DECOR.PENALTY.TIER0);

			//BuildingTemplates.CreateLadderDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
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

			buildingDef.PlacementOffsets = buildingDef.PlacementOffsets.Where(offset => offset.x != 0).ToArray();

			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

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
		}
	}
}
