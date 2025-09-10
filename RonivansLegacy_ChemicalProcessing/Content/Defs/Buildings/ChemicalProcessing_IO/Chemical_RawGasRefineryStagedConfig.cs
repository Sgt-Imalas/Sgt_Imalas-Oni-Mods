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
	public class Chemical_RawGasRefineryStagedConfig : IBuildingConfig
	{
		public static string ID = "Chemical_RawGasRefineryStaged";

		private static PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(-2, 0));
		private static PortDisplayInput hydrogenGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 2));
		private static PortDisplayInput propaneGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 1));
		private static PortDisplayInput sourGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 3));
		private static PortDisplayInput nitricInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(4, 0));
		private static PortDisplayOutput propaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(3, 0));
		private static PortDisplayOutput sourGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 2));
		private static PortDisplayOutput ammoniaGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 1));
		private static PortDisplayOutput waterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-3, 0));


		private void ConfigureRecipes()
		{
		}

		static Chemical_RawGasRefineryStagedConfig()
		{
			steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(-2, 0), color: new Color?((Color)new Color32((byte)167, (byte)180, (byte)201, byte.MaxValue)));
			hydrogenGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 2), color: new Color?((Color)new Color32((byte)224, (byte)67, (byte)203, byte.MaxValue)));
			propaneGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 1), color: new Color?((Color)new Color32((byte)3, (byte)44, (byte)252, byte.MaxValue)));
			sourGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 3), color: new Color?((Color)new Color32(byte.MaxValue, (byte)173, (byte)248, byte.MaxValue)));
			nitricInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(4, 0), color: new Color?((Color)new Color32(byte.MaxValue, (byte)68, (byte)0, byte.MaxValue)));
			propaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(3, 0), color: new Color?((Color)new Color32((byte)3, (byte)44, (byte)252, byte.MaxValue)));
			sourGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 2), color: new Color?((Color)new Color32(byte.MaxValue, (byte)173, (byte)248, byte.MaxValue)));
			ammoniaGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 1), color: new Color?((Color)new Color32((byte)215, (byte)227, (byte)252, byte.MaxValue)));
			waterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-3, 0), color: new Color?((Color)new Color32((byte)72, (byte)129, (byte)247, byte.MaxValue)));
			
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [500f, 200f];
			string[] construction_materials =
			[
				"RefinedMetal",
				SimHashes.Steel.ToString()
			];
			EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 8, 5, "rawgas_refinery_staged_kanim", 100, 30f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tieR6);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 640f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 6f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(2, 3);
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.UtilityOutputOffset = new CellOffset(-2, 3);
			SoundUtils.CopySoundsToAnim("rawgas_refinery_staged_kanim", "generatormethane_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			Storage storage1 = go.AddOrGet<Storage>();
			storage1.capacityKg = 500f;
			storage1.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage1.showInUI = true;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityKG = 100f;
			conduitConsumer.capacityTag = ModElements.RawNaturalGas_Gas.Tag;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer portConduitConsumer1 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer1.conduitType = ConduitType.Gas;
			portConduitConsumer1.consumptionRate = 10f;
			portConduitConsumer1.capacityKG = 100f;
			portConduitConsumer1.capacityTag = SimHashes.Steam.CreateTag();
			portConduitConsumer1.forceAlwaysSatisfied = true;
			portConduitConsumer1.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer1.AssignPort(steamGasInputPort);

			PortConduitConsumer portConduitConsumer2 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer2.conduitType = ConduitType.Gas;
			portConduitConsumer2.consumptionRate = 10f;
			portConduitConsumer2.capacityKG = 100f;
			portConduitConsumer2.capacityTag = SimHashes.Hydrogen.CreateTag();
			portConduitConsumer2.forceAlwaysSatisfied = true;
			portConduitConsumer2.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer2.AssignPort(hydrogenGasInputPort);

			PortConduitConsumer portConduitConsumer3 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer3.conduitType = ConduitType.Gas;
			portConduitConsumer3.consumptionRate = 10f;
			portConduitConsumer3.capacityKG = 100f;
			portConduitConsumer3.capacityTag = SimHashes.Propane.CreateTag();
			portConduitConsumer3.forceAlwaysSatisfied = true;
			portConduitConsumer3.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer3.AssignPort(propaneGasInputPort);
			PortConduitConsumer portConduitConsumer4 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer4.conduitType = ConduitType.Gas;
			portConduitConsumer4.consumptionRate = 10f;
			portConduitConsumer4.capacityKG = 100f;
			portConduitConsumer4.capacityTag = SimHashes.SourGas.CreateTag();
			portConduitConsumer4.forceAlwaysSatisfied = true;
			portConduitConsumer4.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer4.AssignPort(sourGasInputPort);

			PortConduitConsumer portConduitConsumer5 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer5.conduitType = ConduitType.Liquid;
			portConduitConsumer5.consumptionRate = 10f;
			portConduitConsumer5.capacityKG = 100f;
			portConduitConsumer5.capacityTag = ModElements.NitricAcid_Liquid.Tag;
			portConduitConsumer5.forceAlwaysSatisfied = true;
			portConduitConsumer5.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer5.AssignPort(nitricInputPort);

			ElementConverter elementConverter1 = go.AddComponent<ElementConverter>();
			elementConverter1.consumedElements = [
				new ElementConverter.ConsumedElement(ModElements.RawNaturalGas_Gas.Tag, 1f),
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.5f)
				];
			elementConverter1.outputElements =
				[
				new (0.5f, SimHashes.Methane, 371.15f, storeOutput: true, diseaseWeight: 0.25f),
				new (0.4f, SimHashes.Propane, 367.15f, storeOutput: true, diseaseWeight: 0.30f),
				new (0.6f, SimHashes.SourGas, 388.15f, storeOutput: true, diseaseWeight: 0.45f)
			];
			ElementConverter elementConverter2 = go.AddComponent<ElementConverter>();
			elementConverter2.consumedElements =
			[
		new ElementConverter.ConsumedElement(SimHashes.Propane.CreateTag(), 0.4f),
		new ElementConverter.ConsumedElement(SimHashes.Hydrogen.CreateTag(), 0.1f)
			];
			elementConverter2.outputElements =
			[
		new (0.5f, SimHashes.Methane, 371.15f, storeOutput: true)
			];
			ElementConverter elementConverter3 = go.AddComponent<ElementConverter>();
			elementConverter3.consumedElements =
			[
		new ElementConverter.ConsumedElement(SimHashes.SourGas.CreateTag(), 0.6f),
		new ElementConverter.ConsumedElement(ModElements.NitricAcid_Liquid.Tag, 0.15f)
			];
			elementConverter3.outputElements =
			[
		new (0.35f, SimHashes.Water, 362.15f, storeOutput: true, diseaseWeight: 0.5f),
		new (0.15f,ModElements.Ammonia_Gas, 367.15f, storeOutput: true, diseaseWeight: 0.15f),
		new (0.25f, SimHashes.Sulfur, 333.15f, storeOutput: true, diseaseWeight: 0.35f)
			];
			ElementDropper elementDropper = go.AddComponent<ElementDropper>();
			elementDropper.emitMass = 25f;
			elementDropper.emitTag = SimHashes.Sulfur.CreateTag();
			elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
			Storage storage2 = go.AddOrGet<Storage>();
			storage2.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage2.showInUI = true;
			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Gas;
			conduitDispenser.storage = storage2;
			conduitDispenser.elementFilter =
			[
		SimHashes.Methane
			];

			PipedConduitDispenser pipedDispenser1 = go.AddComponent<PipedConduitDispenser>();
			pipedDispenser1.storage = storage2;
			pipedDispenser1.conduitType = ConduitType.Gas;
			pipedDispenser1.alwaysDispense = true;
			pipedDispenser1.elementFilter =
			[
		SimHashes.Propane
			];
			pipedDispenser1.AssignPort(propaneGasOutputPort);

			PipedConduitDispenser pipedDispenser2 = go.AddComponent<PipedConduitDispenser>();
			pipedDispenser2.storage = storage2;
			pipedDispenser2.conduitType = ConduitType.Gas;
			pipedDispenser2.alwaysDispense = true;
			pipedDispenser2.elementFilter =
			[
				SimHashes.SourGas
			];
			pipedDispenser2.AssignPort(sourGasOutputPort);

			PipedConduitDispenser pipedDispenser3 = go.AddComponent<PipedConduitDispenser>();
			pipedDispenser3.storage = storage2;
			pipedDispenser3.conduitType = ConduitType.Gas;
			pipedDispenser3.alwaysDispense = true;
			pipedDispenser3.elementFilter =
			[
				ModElements.Ammonia_Gas
			];
			pipedDispenser3.AssignPort(ammoniaGasOutputPort);

			PipedConduitDispenser pipedDispenser4 = go.AddComponent<PipedConduitDispenser>();
			pipedDispenser4.storage = storage2;
			pipedDispenser4.conduitType = ConduitType.Liquid;
			pipedDispenser4.alwaysDispense = true;
			pipedDispenser4.elementFilter =
			[
		SimHashes.Water
			];
			pipedDispenser4.AssignPort(waterLiquidOutputPort);

			Prioritizable.AddRef(go);
			go.AddOrGet<ElementConversionBuilding>(); //Handles element converter
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController displayController = go.AddComponent<PortDisplayController>();
			displayController.Init(go);
			displayController.AssignPort(go, (DisplayConduitPortInfo)hydrogenGasInputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)steamGasInputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)propaneGasInputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)sourGasInputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)nitricInputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)propaneGasOutputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)sourGasOutputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)ammoniaGasOutputPort);
			displayController.AssignPort(go, (DisplayConduitPortInfo)waterLiquidOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go) => go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
			go.AddOrGet<PortPreviewVisualizer>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}
