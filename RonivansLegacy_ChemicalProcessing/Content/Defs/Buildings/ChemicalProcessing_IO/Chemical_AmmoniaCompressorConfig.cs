using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	public class Chemical_AmmoniaCompressorConfig : IBuildingConfig
	{
		public static string ID = "Chemical_AmmoniaCompressor";
		public static readonly List<Storage.StoredItemModifier> CompressorStorage = new List<Storage.StoredItemModifier>()
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Preserve
		};

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [400f, 200f];
			string[] construction_materials =
			[
				SimHashes.Ceramic.ToString(),
				"RefinedMetal"
			];
			EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Chemical_AmmoniaCompressor", 3, 3, "ammonia_compressor_kanim", 30, 10f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tieR0);
			buildingDef.RequiresPowerInput = true;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.EnergyConsumptionWhenActive = 480f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.ThermalConductivity = 0.01f;
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(1, 0);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 0);
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(1, 0), (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
			};
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage inputStorage = BuildingTemplates.CreateDefaultStorage(go);
			inputStorage.SetDefaultStoredItemModifiers(CompressorStorage);
			inputStorage.capacityKg = 3000f;
			inputStorage.showCapacityStatusItem = true;
			inputStorage.showCapacityAsMainStatus = true;
			inputStorage.showDescriptor = true;

			go.AddOrGet<Reservoir>();
			go.AddOrGet<SmartReservoir>();
			go.AddOrGet<WaterPurifier>();
			Prioritizable.AddRef(go);
			RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
			def.powerSaverEnergyUsage = 60f;
			def.coolingHeatKW = 4f;
			def.steadyHeatKW = 0.0f;
			def.simulatedInternalTemperature = 212.15f;
			def.simulatedThermalConductivity = 90000f;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityKG = 10f;
			conduitConsumer.capacityTag = ModElements.Ammonia_Gas.Tag;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = [new ElementConverter.ConsumedElement(ModElements.Ammonia_Gas.Tag, 0.5f)];
			elementConverter.outputElements = [new ElementConverter.OutputElement(0.5f, ModElements.Ammonia_Liquid, 212.15f, storeOutput: true)];

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.storage = inputStorage;
			conduitDispenser.elementFilter = [ModElements.Ammonia_Liquid];
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
		}
	}
}
