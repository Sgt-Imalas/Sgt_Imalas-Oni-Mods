using Rockets_TinyYetBig.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings
{
	internal class FlightComputerUpgradeInterfaceConfig : IBuildingConfig
	{
		public const string ID = "RTB_FlightComputerUpgradeInterface";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] materialMass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
			string[] materialType = MATERIALS.REFINED_METALS;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 1,
				anim: "rocket_logic_checklist_sender_kanim",
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
			buildingDef.OnePerWorld = true;

			buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.RibbonInputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0),
				 STRINGS.BUILDINGS.PREFABS.RTB_FLIGHTCOMPUTERUPGRADEINTERFACE.LOGIC_PORT,
				 STRINGS.BUILDINGS.PREFABS.RTB_FLIGHTCOMPUTERUPGRADEINTERFACE.LOGIC_PORT_ACTIVE,
				 STRINGS.BUILDINGS.PREFABS.RTB_FLIGHTCOMPUTERUPGRADEINTERFACE.LOGIC_PORT_INACTIVE)
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
				component.AddTag(GameTags.UniquePerWorld);
			}
			go.AddOrGet<RandomizedUserNameable>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
		}
	}
}
