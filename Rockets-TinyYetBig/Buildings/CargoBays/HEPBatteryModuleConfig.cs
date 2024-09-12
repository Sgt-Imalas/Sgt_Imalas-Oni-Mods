using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings.Utility;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.CargoBays
{
	public class HEPBatteryModuleConfig : IBuildingConfig
	{
		public const string ID = "RTB_HEPBatteryModule";

		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

		public override BuildingDef CreateBuildingDef()
		{
			float[] MatCosts = {
				1200f,
				400f
			};
			string[] Materials =
			{
				"Steel",
				"Glass"
			};
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "radbolt_battery_module_kanim", 1000, 30f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.DefaultAnimState = "grounded";
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.ViewMode = OverlayModes.Radiation.ID;
			buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;


			buildingDef.UseHighEnergyParticleInputPort = true;
			buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 2);
			buildingDef.UseHighEnergyParticleOutputPort = true;
			buildingDef.HighEnergyParticleOutputOffset = new CellOffset(0, 2);

			buildingDef.AddLogicPowerPort = true;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, ID);

			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.OutputPort((HashedString) ID, new CellOffset(1, 1),
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE,
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE_ACTIVE,
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE_INACTIVE)
			};
			buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.InputPort(HEPBattery.FIRE_PORT_ID, new CellOffset(0, 2),
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT,
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_ACTIVE,
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_INACTIVE)
			};


			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddComponent<RequireInputs>();
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket,  null)
			};

			HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
			energyParticleStorage.capacity = Config.Instance.RadboltStorageCapacity;
			energyParticleStorage.autoStore = true;
			energyParticleStorage.PORT_ID = ID;
			energyParticleStorage.showCapacityStatusItem = true;
			energyParticleStorage.showCapacityAsMainStatus = true;
			go.AddOrGet<LoopingSounds>();
			var HEPBatteryModule = go.AddOrGet<RadiationBatteryOutputHandler>();
			HEPBatteryModule.physicalFuelCapacity = Config.Instance.RadboltStorageCapacity;
			go.AddOrGet<DrillConeAssistentModuleHEP>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Prioritizable.AddRef(go);

			//WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
			//virtualNetworkLink.visualizeOnly = true;
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MODERATE_PLUS);
		}
	}
}
