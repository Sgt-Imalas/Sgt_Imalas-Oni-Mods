using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	public class Chemical_AmmoniaCompressorConfig : IBuildingConfig
	{

		public static string ID = "Chemical_AmmoniaCompressor";

		private static readonly PortDisplayOutput LiquidPipeOutput = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, 0));

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [400f, 200f];
			string[] construction_materials =
			[
				SimHashes.Ceramic.ToString(),
				"RefinedMetal"
			];
			EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "ammonia_compressor_kanim", 30, 10f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tieR0);
			buildingDef.RequiresPowerInput = true;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.EnergyConsumptionWhenActive = 480f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.ThermalConductivity = 0.01f;
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(1, 0);
			//buildingDef.OutputConduitType = ConduitType.Liquid;
			//buildingDef.UtilityOutputOffset = new CellOffset(-1, 0);
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(1, 0), (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
			};
			SoundUtils.CopySoundsToAnim("ammonia_compressor_kanim", "airconditioner_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

			Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
			defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			defaultStorage.capacityKg = 3000f;
			defaultStorage.showCapacityStatusItem = true;
			defaultStorage.showCapacityAsMainStatus = true;
			defaultStorage.showDescriptor = true;

			go.AddOrGet<Reservoir>();
			go.AddOrGet<SmartReservoir>();
			go.AddOrGet<ElementCompressorBuilding>();
			Prioritizable.AddRef(go);

			RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
			def.powerSaverEnergyUsage = 60f;
			def.coolingHeatKW = 8f;
			def.steadyHeatKW = 0.2f;
			def.simulatedInternalTemperature = 212.15f;
			def.simulatedThermalConductivity = 8000f;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.consumptionRate = 1f;
			conduitConsumer.capacityKG = 10f;
			conduitConsumer.capacityTag = ModElements.Ammonia_Gas.Tag;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = [new ElementConverter.ConsumedElement(ModElements.Ammonia_Gas.Tag, 1f)];
			elementConverter.outputElements = [new ElementConverter.OutputElement(1f, ModElements.Ammonia_Liquid, 212.15f, storeOutput: true)];

			PipedConduitDispenser conduitDispenser = go.AddOrGet<PipedConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.storage = defaultStorage;
			conduitDispenser.elementFilter = [ModElements.Ammonia_Liquid];
			conduitDispenser.SkipSetOperational = true;
			conduitDispenser.AssignPort(LiquidPipeOutput);
		}
		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, LiquidPipeOutput);
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			this.AttachPort(go);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			this.AttachPort(go);
		}		
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
			this.AttachPort(go);
		}
	}
}
