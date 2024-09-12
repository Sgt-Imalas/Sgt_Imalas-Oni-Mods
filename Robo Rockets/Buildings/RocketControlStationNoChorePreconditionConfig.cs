using System.Collections.Generic;
using TUNING;
using UnityEngine;


namespace KnastoronOniMods
{
	class RocketControlStationNoChorePreconditionConfig : IBuildingConfig
	{
		public static string ID = "RocketControlStationNoChorePrecondition";
		public const string NAME = "Rocket Brain Controller";
		public const string DESCR = "A brain in a vat, somehow it knows how to fly this bucket.";
		public const string EFFECT = "Grown artificially, this brain will fly your rocket everywhere.";
		public const float CONSOLE_WORK_TIME = 30f;
		public const float CONSOLE_IDLE_TIME = 120f;
		public const float WARNING_COOLDOWN = 30f;
		public const float DEFAULT_SPEED = 1f;
		public const float SLOW_SPEED = 0.5f;
		public const float DEFAULT_PILOT_MODIFIER = 1f;

		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

		public override BuildingDef CreateBuildingDef()
		{
			string id = RocketControlStationNoChorePreconditionConfig.ID;
			float[] tieR2_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] rawMetals = MATERIALS.RAW_METALS;
			EffectorValues tieR3 = TUNING.NOISE_POLLUTION.NOISY.TIER3;
			EffectorValues tieR2_2 = TUNING.BUILDINGS.DECOR.BONUS.TIER2;
			EffectorValues noise = tieR3;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 2, 2, "Brain_Controller_kanim", 30, 60f, tieR2_1, rawMetals, 1600f, BuildLocationRule.OnFloor, tieR2_2, noise);
			buildingDef.Overheatable = false;
			buildingDef.Repairable = false;
			buildingDef.Floodable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "large";
			buildingDef.DefaultAnimState = "on";
			buildingDef.OnePerWorld = true;
			//buildingDef.AlwaysOperational = true;
			buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
	{
	  LogicPorts.Port.InputPort(RocketControlStation.PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.ROCKETCONTROLSTATION.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.ROCKETCONTROLSTATION.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.ROCKETCONTROLSTATION.LOGIC_PORT_INACTIVE)
	};
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(GameTags.RocketInteriorBuilding);
			component.AddTag(GameTags.UniquePerWorld);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			go.AddOrGet<RocketControlStationIdleWorkableAI>().workLayer = Grid.SceneLayer.BuildingUse;
			go.AddOrGet<RocketControlStationLaunchWorkableAI>().workLayer = Grid.SceneLayer.BuildingUse;
			go.AddOrGet<RocketControlStationNoChorePrecondition>();
			go.AddOrGetDef<PoweredController.Def>();
			// go.AddOrGet<DebugSideScreen>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RocketInterior);
		}
	}
}
