using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs.BuildingPortUtils;
using static STRINGS.BUILDINGS.PREFABS;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesRefrigeration
{
    class HightechSmallFridgeConfig : IBuildingConfig, IHasConfigurableStorageCapacity, IHasConfigurableWattage
	{
		public static float Wattage = 240;
		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;
		public static float StorageCapacity = 200; 
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;

		public static string ID = "HightechBigFridge";

		public float BaseFridgeMultiplier => GetWattage() / 120f;

		public PortDisplayInput WaterConsumer = new(ConduitType.Liquid, new CellOffset(1, 0));
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "advanced_techfridge_kanim", 30, 30f, [700,100], [GameTags.RefinedMetal.ToString(),GameTags.Plastic.ToString()], 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.BONUS.TIER1, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = GetWattage();
			buildingDef.AddLogicPowerPort = false;
			buildingDef.SelfHeatKilowattsWhenActive = 0.125f * BaseFridgeMultiplier;
			buildingDef.ExhaustKilowattsWhenActive = 0f;
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
			{
				LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1),
				REFRIGERATOR.LOGIC_PORT,
			REFRIGERATOR.LOGIC_PORT_ACTIVE,
			REFRIGERATOR.LOGIC_PORT_INACTIVE,
				false,
				false)
			};
			buildingDef.Floodable = false;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";

			buildingDef.InputConduitType = ConduitType.Solid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 1);

			SoundEventVolumeCache.instance.AddVolume("advanced_techfridge_kanim", "Refrigerator_open", NOISE_POLLUTION.NOISY.TIER1);
			SoundEventVolumeCache.instance.AddVolume("advanced_techfridge_kanim", "Refrigerator_close", NOISE_POLLUTION.NOISY.TIER1);
			return buildingDef;
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			AttachPort(go);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			AttachPort(go);
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
			def.powerSaverEnergyUsage = GetWattage() / 12f;
			def.coolingHeatKW = 0.375f * BaseFridgeMultiplier;
			def.steadyHeatKW = 0f;
			def.simulatedInternalHeatCapacity = 1000f;
			def.simulatedInternalTemperature = 252.15f;
			def.simulatedThermalConductivity = 10000f;
			go.AddOrGet<UserNameable>();
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
			go.AddOrGetDef<StorageController.Def>();


			go.AddOrGet<FilteredSolidConduitConsumer>();

			var inputs = go.AddOrGet<RequireInputs>();
			inputs.requireConduitHasMass = false;
			inputs.requireConduit = false;

			///water storage
			Storage liquidStorage = BuildingTemplates.CreateDefaultStorage(go, true);
			liquidStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			//liquidStorage.showDescriptor = true;
			liquidStorage.capacityKg = 500f;
			liquidStorage.allowItemRemoval = true;
			liquidStorage.showInUI = true;

			PortConduitConsumer waterInput = go.AddComponent<PortConduitConsumer>();
			waterInput.storage = liquidStorage;
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.ignoreMinMassCheck = true;
			waterInput.forceAlwaysSatisfied = true;
			waterInput.SkipSetOperational = true;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			//waterInput.alwaysConsume = true;
			waterInput.capacityKG = liquidStorage.capacityKg;
			waterInput.AssignPort(WaterConsumer);
			AttachPort(go);

		}
		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, WaterConsumer);
		}
	}
}
