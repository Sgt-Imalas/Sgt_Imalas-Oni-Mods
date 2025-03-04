using Rockets_TinyYetBig.RocketFueling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static Grid;
using static KAnim;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	internal class ReinforcedLadderConfig : IBuildingConfig
	{
		public const string ID = "RTB_ReinforcedLadder";

		public override BuildingDef CreateBuildingDef()
		{			
			var buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 1,
				anim: "ladder_reinforced_kanim",
				hitpoints: BUILDINGS.HITPOINTS.TIER2,
				construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER1,
				construction_mass: [100,25],
				construction_materials: [MATERIALS.BUILDABLERAW, SimHashes.Steel.ToString() ],
				melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER2,
				build_location_rule: BuildLocationRule.Anywhere,
				decor: DECOR.BONUS.TIER0,
				noise: NOISE_POLLUTION.NONE);

			BuildingTemplates.CreateLadderDef(buildingDef);
			buildingDef.SceneLayer = SceneLayer.BuildingFront;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.DragBuild = true;

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);

			var ladder = go.AddOrGet<Ladder>();
			ladder.upwardsMovementSpeedMultiplier = 1.5f;
			ladder.downwardsMovementSpeedMultiplier = 1.5f;

			go.AddOrGet<AnimTileable>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var anim = go.AddOrGet<AnimTileable>();
			anim.tags = [LoaderLadderAdapterConfig.ID, ReinforcedLadderConfig.ID];
			go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
		}
	}
}
