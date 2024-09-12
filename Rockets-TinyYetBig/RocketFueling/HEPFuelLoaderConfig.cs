using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
	public class HEPFuelLoaderConfig : IBuildingConfig
	{
		public const string ID = "RTB_HEPFuelLoader";
		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 3, "fuel_loader_hep_kanim", 1000, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.RequiresPowerInput = true;
			buildingDef.DefaultAnimState = "idle";
			buildingDef.CanMove = false;
			//buildingDef.UtilityInputOffset = new CellOffset(0, 1);
			buildingDef.PowerInputOffset = new CellOffset(0, 1);
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
			buildingDef.ExhaustKilowattsWhenActive = 0.125f;
			buildingDef.ViewMode = OverlayModes.Radiation.ID;
			buildingDef.UseHighEnergyParticleInputPort = true;
			buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 2);


			if (Config.Instance.EnableRocketLoaderLogicOutputs)
			{
				buildingDef.LogicOutputPorts = new List<LogicPorts.Port>(){
					LogicPorts.Port.OutputPort(RocketLoaderPatches.ROCKETPORTLOADER_ACTIVE, new CellOffset(0, 1),
					STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER,
					STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER_ACTIVE,
					STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER_INACTIVE),

					LogicPorts.Port.OutputPort(HEPEngineConfig.PORT_ID, new CellOffset(0, 2),
					 global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE,
					 global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE,
					 global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE)
				};
			}
			else
			{
				buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
				{
					LogicPorts.Port.OutputPort(HEPEngineConfig.PORT_ID, new CellOffset(0, 2),
					 global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE,
					 global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE,
					global :: STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE)
				};
			}

			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<LoopingSounds>();
			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			component.AddTag(BaseModularLaunchpadPortConfig.LinkTag);
			component.AddTag(GameTags.ModularConduitPort);
			component.AddTag(GameTags.NotRocketInteriorBuilding);
			component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);


			HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
			energyParticleStorage.autoStore = true;
			energyParticleStorage.showInUI = false;
			energyParticleStorage.PORT_ID = HEPEngineConfig.PORT_ID;
			energyParticleStorage.showCapacityStatusItem = true;
			energyParticleStorage.capacity = 500f;

			go.AddOrGetDef<ModularConduitPortController.Def>().mode = (ModularConduitPortController.Mode.Load);
			var LoaderComp = go.AddOrGet<FuelLoaderComponent>();
			LoaderComp.loaderType = FuelLoaderComponent.LoaderType.HEP;
			LoaderComp.HEPStorage = energyParticleStorage;

			ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
			def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
			def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
			def.objectLayer = ObjectLayer.Building;
			go.AddOrGet<LogicOperationalController>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<BuildingCellVisualizer>();
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			go.AddOrGet<BuildingCellVisualizer>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<BuildingCellVisualizer>();
		}
	}
}
