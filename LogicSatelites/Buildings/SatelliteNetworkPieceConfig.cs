using LogicSatellites.Behaviours;
using TUNING;
using UnityEngine;
using static LogicSatellites.Behaviours.ModAssets;

namespace LogicSatellites.Buildings
{
	internal class SatelliteNetworkPieceConfig : IBuildingConfig
	{
		public const string ID = "LS_SatelliteNetworkGroundPiece";
		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
		public override BuildingDef CreateBuildingDef()
		{
			float[] materialMass = new float[2]
			{
				250f,
				50f
			};
			string[] materialType = new string[2]
			{
				"Metal",
				"Plastic"
			};
			EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
			EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 3,
				anim: "meteor_detector_kanim",
				hitpoints: 100,
				construction_time: 40f,
				construction_mass: materialMass,
				construction_materials: materialType,
				melting_point: 1600f,
				BuildLocationRule.OnFloor,
				decor: decorValue,
				noise: noiseLevel);

			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.SceneLayer = Grid.SceneLayer.Building;

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{

		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			var entity = go.AddOrGet<SatelliteGridEntity>();
			entity.satelliteType = (int)SatType.GroundTarget;
			entity.clusterAnimName = "space_satellite_kanim";
			entity.ShowOnMap = false;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{

		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{

		}
	}
}
