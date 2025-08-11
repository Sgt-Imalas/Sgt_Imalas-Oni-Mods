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
	public class LogisticLoaderConfig : IBuildingConfig, IHasConfigurableStorageCapacity, IHasConfigurableWattage
	{
		public static float Wattage = HighPressureConduitRegistration.GetLogisticConduitMultiplier() * 120f;

		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;
	
		public static float StorageCapacity = //HighPressureConduitRegistration.GetLogisticConduitMultiplier() * 
			1000 *0.6f;    // 1/2 of regular loader capacity by default, at least originally, this exposes a bug with unitmass items like suits breaking the deliverable with those remaining 100 capacity (a suit weighs 200). so we give it 600 instead to avoid that
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;
	
		public static string ID = "LogisticLoader";
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "logistic_loader_kanim", 100, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
			def1.RequiresPowerInput = true;
			def1.EnergyConsumptionWhenActive = Wattage;
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.SelfHeatKilowattsWhenActive = HighPressureConduitRegistration.GetLogisticConduitMultiplier()*2f;
			def1.Floodable = false;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.AudioCategory = "Metal";
			def1.OutputConduitType = ConduitType.Solid;
			def1.PowerInputOffset = new CellOffset(0, 1);
			def1.UtilityOutputOffset = new CellOffset(0, 0);
			def1.PermittedRotations = PermittedRotations.R360;
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("logistic_loader_kanim", "conveyorin_kanim");
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			Prioritizable.AddRef(go);
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<Automatable>();
			List<Tag> tagList = [.. STORAGEFILTERS.STORAGE_LOCKERS_STANDARD, .. STORAGEFILTERS.FOOD];
			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = GetStorageCapacity();
			storage.showInUI = true;
			storage.showDescriptor = true;
			storage.storageFilters = tagList;
			storage.allowItemRemoval = false;
			storage.onlyTransferFromLowerPriority = true;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			go.AddOrGet<TreeFilterable>();
			go.AddOrGet<SolidConduitInbox>();
			go.AddOrGet<ConfigurableSolidConduitDispenser>().massDispensed = Config.Instance.Logistic_Rail_Capacity;

			var requirement = go.AddOrGet<HPA_SolidConduitRequirement>();
			requirement.RequiresHighPressureOutput = true;
			requirement.IsLogisticRail = true;
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}
	}
}
