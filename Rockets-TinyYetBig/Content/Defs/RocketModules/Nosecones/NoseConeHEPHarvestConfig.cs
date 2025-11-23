using Rockets_TinyYetBig.Behaviours;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Nosecones
{
	class NoseConeHEPHarvestConfig : IBuildingConfig
	{
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;
		public const string ID = "RYB_NoseConeHEPHarvest";

		public override BuildingDef CreateBuildingDef()
		{
			float[] hollowTieR2 = new[] { 800f, 400f };
			string[] construction_materials = new string[2]
			{
				"Steel",
				"Plastic"
			};
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "rocket_nosecone_laser_harvest_kanim", 1000, 60f, hollowTieR2, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
			buildingDef.RequiresPowerInput = false;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			buildingDef.ShowInBuildMenu = false;
			buildingDef.UseHighEnergyParticleInputPort = true;
			buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 1);

			buildingDef.AddLogicPowerPort = true;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, ID);

			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.OutputPort((HashedString) ID, new CellOffset(1, 1),
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE,
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE_ACTIVE,
				(string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE_INACTIVE)
			};
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.TryGetComponent<KPrefabID>(out var id);
			id.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			id.AddTag(GameTags.NoseRocketModule);
			id.AddTag(ModAssets.Tags.SpaceHarvestModule);
			id.AddTag(TagManager.Create(NoseconeHarvestConfig.ID));

			HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
			energyParticleStorage.capacity = Config.Instance.LaserDrillconeCapacity;
			energyParticleStorage.autoStore = true;
			energyParticleStorage.PORT_ID = ID;
			energyParticleStorage.showCapacityStatusItem = true;
			energyParticleStorage.showCapacityAsMainStatus = true;



		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR);
			go.GetComponent<ReorderableBuilding>().buildConditions.Add(new TopOnly());
			go.AddOrGet<ExtendedClusterModuleAnimator>();
			go.AddOrGetDef<ResourceHarvestModuleHEP.Def>().harvestSpeed = Config.Instance.LaserDrillconeSpeed;
		}
	}
}

