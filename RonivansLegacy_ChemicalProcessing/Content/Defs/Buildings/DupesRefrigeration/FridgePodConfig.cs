using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesRefrigeration
{
    class FridgePodConfig : IBuildingConfig, IHasConfigurableStorageCapacity, IHasConfigurableWattage
	{
		public static float Wattage = 120;
		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;
		public static float StorageCapacity = 50; 
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;

		public static string ID = "FridgePod";

		public float BaseFridgeMultiplier => GetWattage() / 120f;

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "fridge_pod_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.BONUS.TIER1, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = GetWattage();
			buildingDef.AddLogicPowerPort = false;
			buildingDef.SelfHeatKilowattsWhenActive = 0.125f * BaseFridgeMultiplier;
			buildingDef.ExhaustKilowattsWhenActive = 0f;
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
			{
				LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 0),
				REFRIGERATOR.LOGIC_PORT,
			REFRIGERATOR.LOGIC_PORT_ACTIVE,
			REFRIGERATOR.LOGIC_PORT_INACTIVE,
				false,
				false)
			};
			buildingDef.Floodable = false;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			SoundEventVolumeCache.instance.AddVolume("fridge_pod_kanim", "Refrigerator_open", NOISE_POLLUTION.NOISY.TIER1);
			SoundEventVolumeCache.instance.AddVolume("fridge_pod_kanim", "Refrigerator_close", NOISE_POLLUTION.NOISY.TIER1);
			SoundUtils.CopySoundsToAnim("fridge_pod_kanim", "smartstoragelocker_kanim");
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Refrigerator, false);
			Storage storage = go.AddOrGet<Storage>();
			storage.showInUI = true;
			storage.showDescriptor = true;
			storage.storageFilters = STORAGEFILTERS.FOOD;
			storage.allowItemRemoval = true;
			storage.capacityKg = GetStorageCapacity();
			storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
			storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
			storage.showCapacityStatusItem = true;
			Prioritizable.AddRef(go);
			go.AddOrGet<TreeFilterable>();
			go.AddOrGet<FoodStorage>();
			go.AddOrGet<Refrigerator>();
			RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
			def.powerSaverEnergyUsage = GetWattage() / 6f;
			def.coolingHeatKW = 0.375f * BaseFridgeMultiplier;
			def.steadyHeatKW = 0f;
			def.simulatedInternalHeatCapacity = 1000f;
			def.simulatedInternalTemperature = 252.15f;
			def.simulatedThermalConductivity = 10000f;
			go.AddOrGet<UserNameable>();
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
			go.AddOrGetDef<StorageController.Def>();
			go.AddOrGet<FridgeSaverDescriptor>().Cache();


			PreciseStorageControl.AddComponent(go);
			HysteresisStorage.AddComponent(go);
		}
	}
}
