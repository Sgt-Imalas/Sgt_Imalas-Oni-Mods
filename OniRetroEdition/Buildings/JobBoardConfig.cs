using OniRetroEdition.Behaviors;
using TUNING;
using UnityEngine;

namespace ShockWormMob
{
	internal class JobBoardConfig : IBuildingConfig
	{
		public const string ID = "RetroOni_JobStation";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] farmable = MATERIALS.ALL_METALS;
			EffectorValues none1 = NOISE_POLLUTION.NONE;
			EffectorValues none2 = BUILDINGS.DECOR.BONUS.TIER1;
			EffectorValues noise = none1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "job_station_kanim", 100, 30f, tieR2, farmable, 1600f, BuildLocationRule.Tile, none2, noise);

			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			SocialGatheringPoint socialGatheringPoint = go.AddOrGet<SocialGatheringPoint>();
			socialGatheringPoint.choreOffsets = new CellOffset[6]
			{
				new CellOffset(-1, 0),
				new CellOffset(-2, 0),
				new CellOffset(2, 0),
				new CellOffset(3, 0),
				new CellOffset(0, 0),
				new CellOffset(1, 0)
			};
			socialGatheringPoint.choreCount = 4;
			socialGatheringPoint.basePriority = RELAXATION.PRIORITY.TIER0;
			go.GetComponent<KPrefabID>().AddTag(GameTags.Experimental);
			RoleStation roleStation = go.AddOrGet<RoleStation>();
			roleStation.overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim((HashedString) "anim_interacts_job_station_kanim")
			};
			go.AddOrGet<JobBoardSkillbutton>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}

