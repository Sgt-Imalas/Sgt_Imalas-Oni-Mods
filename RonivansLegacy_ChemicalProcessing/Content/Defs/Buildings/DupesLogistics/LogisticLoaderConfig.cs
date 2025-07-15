using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics
{
	public class LogisticLoaderConfig : IBuildingConfig, IHasConfigurableStorageCapacity
	{
		public static float StorageCapacity = 250; // 1/4 of regular loader capacity
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;
	
		public static string ID = "LogisticLoader";
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "logistic_loader_kanim", 100, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
			def1.RequiresPowerInput = true;
			def1.EnergyConsumptionWhenActive = 20f;
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.SelfHeatKilowattsWhenActive = 0f;
			def1.Overheatable = false;
			def1.Floodable = false;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.AudioCategory = "Metal";
			def1.OutputConduitType = ConduitType.Solid;
			def1.PowerInputOffset = new CellOffset(0, 1);
			def1.UtilityOutputOffset = new CellOffset(0, 0);
			def1.PermittedRotations = PermittedRotations.R360;
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			Prioritizable.AddRef(go);
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<Automatable>();
			List<Tag> tagList = [.. STORAGEFILTERS.NOT_EDIBLE_SOLIDS, .. STORAGEFILTERS.FOOD];
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
			go.AddOrGet<SolidConduitDispenser>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}
	}
}
