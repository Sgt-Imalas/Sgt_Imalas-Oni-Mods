using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HighCapacityLogisticRails
{
    class HPAInBoxConfig : IBuildingConfig, IHasConfigurableStorageCapacity, IHasConfigurableWattage
	{
		static float multiplier => HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Solid);

		public static float Wattage = multiplier * 120f;

		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;

		public static float StorageCapacity = multiplier * 1000; //10 tons at default
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;

		public static string ID = "HPA_InBox";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
			string[] refinedMetals = MATERIALS.REFINED_METALS;
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 3, "hpa_rail_inbox_kanim", 100, 60f, [300, 100], [GameTags.Steel.ToString(), GameTags.Plastic.ToString()], 1600f, BuildLocationRule.Anywhere, tieR1, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = GetWattage();
			buildingDef.ExhaustKilowattsWhenActive = 0.0f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f* multiplier;
			buildingDef.Floodable = false;
			buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.OutputConduitType = ConduitType.Solid;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("hpa_rail_inbox_kanim", "conveyorin_kanim");
			return buildingDef;
		}

		public override void DoPostConfigureUnderConstruction(GameObject go) => go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			Prioritizable.AddRef(go);
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<Automatable>();
			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = GetStorageCapacity();
			storage.showInUI = true;
			storage.showDescriptor = true;
			storage.storageFilters = [..STORAGEFILTERS.STORAGE_LOCKERS_STANDARD,..STORAGEFILTERS.FOOD];
			storage.allowItemRemoval = false;
			storage.onlyTransferFromLowerPriority = true;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			go.AddOrGet<TreeFilterable>();
			go.AddOrGet<SolidConduitInbox>();
			go.AddOrGet<ConfigurableSolidConduitDispenser>().massDispensed = Config.Instance.HPA_Capacity_Solid;
			go.AddOrGet<HPA_SolidConduitRequirement>().RequiresHighPressureOutput = true;
		}
	}
}
