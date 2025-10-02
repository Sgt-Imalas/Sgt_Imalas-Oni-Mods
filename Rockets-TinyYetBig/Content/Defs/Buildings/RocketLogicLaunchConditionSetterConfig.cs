using Rockets_TinyYetBig.Content.Scripts.Buildings;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static Storage;

namespace Rockets_TinyYetBig.Content.Defs.Buildings
{
	internal class RocketLogicLaunchConditionSetterConfig : IBuildingConfig
	{
		public const string ID = "RTB_RocketLogicLaunchConditionSetter";

		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] materialMass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
			string[] materialType = MATERIALS.REFINED_METALS;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 1,
				anim: "critter_sensor_kanim",
				hitpoints: 30,
				construction_time: 30f,
				construction_mass: materialMass,
				construction_materials: materialType,
				melting_point: 1600f,
				BuildLocationRule.Anywhere,
				decor: BUILDINGS.DECOR.PENALTY.TIER0,
				noise: NOISE_POLLUTION.NONE);
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.RequiresPowerInput = false;
			buildingDef.AddLogicPowerPort = true;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.Floodable = false;

			buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
			{
                LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), 
				(string) STRINGS.BUILDINGS.PREFABS.RTB_ROCKETLOGICLAUNCHCONDITIONSETTER.LOGIC_PORT,
				(string) STRINGS.BUILDINGS.PREFABS.RTB_ROCKETLOGICLAUNCHCONDITIONSETTER.LOGIC_PORT_ACTIVE,
				(string) STRINGS.BUILDINGS.PREFABS.RTB_ROCKETLOGICLAUNCHCONDITIONSETTER.LOGIC_PORT_INACTIVE)
			};
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			if (go.TryGetComponent<KPrefabID>(out var component))
			{
				component.AddTag(GameTags.RocketInteriorBuilding);
				component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
			}
			go.AddOrGet<RandomizedUserNameable>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<RocketLogicLaunchCondition>();

			go.AddOrGet<LogicOperationalController>();			
			//go.AddOrGetDef<StorageController.Def>();
			go.AddOrGetDef<OperationalController.Def>();
		}
	}
}
