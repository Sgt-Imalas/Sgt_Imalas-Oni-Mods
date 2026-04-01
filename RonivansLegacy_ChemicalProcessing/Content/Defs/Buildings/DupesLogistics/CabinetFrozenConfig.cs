using PeterHan.PLib.Options;
using ProcGen;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics
{
	class CabinetFrozenConfig : IBuildingConfig, IHasConfigurableStorageCapacity
	{
		public static float StorageCapacity = 20000; 
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;

		public static string ID = "CabinetFrozen";
		public override BuildingDef CreateBuildingDef()
		{
			float[] cost = [200, 200];
			string[] materials = [GameTags.RefinedMetal.ToString(), GameTags.BuildableRaw.ToString()];

			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "cabinet_frozen_kanim", 30, 60f, cost, materials, 1600f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, TUNING.NOISE_POLLUTION.NONE);
			def1.Floodable = false;
			def1.AudioCategory = "Metal";
			def1.Overheatable = false;
			def1.ViewMode = OverlayModes.Logic.ID;
			def1.RequiresPowerInput = true;
			def1.AddLogicPowerPort = false;
			def1.EnergyConsumptionWhenActive = 480f;
			def1.ExhaustKilowattsWhenActive = 0.125f;
			def1.LogicOutputPorts = [LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1),
				global::STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT, 
				global::STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_ACTIVE, 
				global::STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_INACTIVE, false, true),
				LogicPorts.Port.OutputPort(EnergySaverLogicOutput.PORT_ID, new CellOffset(0, 0),
				STRINGS.UI.LOGIC_PORTS.ENERGYSAVER_ACTIVE.LOGIC_PORT,
				STRINGS.UI.LOGIC_PORTS.ENERGYSAVER_ACTIVE.LOGIC_PORT_ACTIVE,
				STRINGS.UI.LOGIC_PORTS.ENERGYSAVER_ACTIVE.LOGIC_PORT_INACTIVE, false, true)

			];

			def1.InputConduitType = ConduitType.Solid;
			def1.UtilityInputOffset = new CellOffset(0, 0);
			SoundUtils.CopySoundsToAnim("cabinet_frozen_kanim", "storagelocker_kanim");
			return def1;
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			SoundEventVolumeCache.instance.AddVolume("cabinet_frozen_kanim", "StorageLocker_Hit_metallic_low", TUNING.NOISE_POLLUTION.NOISY.TIER1);
			Prioritizable.AddRef(go);
			go.AddOrGet<Automatable>();
			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			storage.showInUI = true;
			storage.allowItemRemoval = true;
			storage.showDescriptor = true;
			storage.storageFilters = ModAssets.GetNonLiquifiableSolids();
			storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
			storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.capacityKg = StorageCapacity;
			go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
			go.AddOrGet<StorageLockerSmart>();
			go.AddOrGet<UserNameable>();
			RefrigeratorController.Def fridgeController = go.AddOrGetDef<RefrigeratorController.Def>();
			fridgeController.powerSaverEnergyUsage = 40f;
			fridgeController.coolingHeatKW = 8;
			fridgeController.steadyHeatKW = 0f;
			fridgeController.simulatedInternalTemperature = UtilMethods.GetKelvinFromC(24);
			//fridgeController.simulatedInternalHeatCapacity = 500f;
			fridgeController.simulatedThermalConductivity = 500f;

			//filtered solid conduit input
			go.AddOrGet<FilteredSolidConduitConsumer>();
			go.AddOrGet<EnergySaverLogicOutput>();

			HysteresisStorage.AddComponent(go);
		}
	}
}
