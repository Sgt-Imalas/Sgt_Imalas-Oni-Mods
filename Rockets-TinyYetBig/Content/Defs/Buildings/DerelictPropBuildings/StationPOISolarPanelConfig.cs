using Rockets_TinyYetBig.RocketFueling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static Grid;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.DerelictPropBuildings
{
	internal class StationPOISolarPanelConfig : IBuildingConfig
	{
		public const string ID = "RTB_POI_SolarPanel";

		public override BuildingDef CreateBuildingDef()
		{
			var buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 1,
				anim: "rtb_poi_solar_panel_kanim",
				hitpoints: BUILDINGS.HITPOINTS.TIER2,
				construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER1,
				construction_mass: [100, 25],
				construction_materials: [MATERIALS.ALL_GLASSES, SimHashes.Steel.ToString()],
				melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER2,
				build_location_rule: BuildLocationRule.Anywhere,
				decor: DECOR.BONUS.TIER0,
				noise: NOISE_POLLUTION.NONE);

			buildingDef.SceneLayer = SceneLayer.LogicGatesFront;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.Invincible = true;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.ShowInBuildMenu = false;

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			go.AddOrGet<AnimTileable>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var anim = go.AddOrGet<AnimTileable>();
			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(GameTags.Bunker);
			component.AddTag(GameTags.NoRocketRefund);
			go.GetComponent<Deconstructable>().allowDeconstruction = false;
		}
	}
}
