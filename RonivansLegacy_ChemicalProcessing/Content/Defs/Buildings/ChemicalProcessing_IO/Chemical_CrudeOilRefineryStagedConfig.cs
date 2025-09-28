using KSerialization;
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
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_CrudeOilRefineryStagedConfig : IBuildingConfig
	{
		public static string ID = "Chemical_CrudeOilRefineryStaged";

		private static readonly PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 1), color: new Color32(167, 180, 201, byte.MaxValue));
		private static readonly PortDisplayInput hydrogenGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 2), color: new Color32(224, 67, 203, byte.MaxValue));
		private static readonly PortDisplayInput naphthaInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-2, 2), color: new Color32(176, 0, 255, 255));
		private static readonly PortDisplayOutput naphthaLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-2, 1), color: new Color32(176, 0, 255, 255));
		private static readonly PortDisplayOutput methaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(3, 3), color: new Color32(byte.MaxValue, 114, 33, byte.MaxValue));
		private static readonly PortDisplayOutput PetroleumLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-2, 0), null, new Color32(255, 195, 37, 255));
		private static readonly PortDisplayOutput SourWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-2, 3), null, new Color32(130, 104, 65, 255));

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [750f, 250f];
			string[] construction_materials =
			[
				GameTags.RefinedMetal.ToString(),
				SimHashes.Steel.ToString()
			];
			EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 6, 5, "crudeoil_refinery_staged_kanim", 100, 30f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tieR6);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 960f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 6f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(3, 0);
			SoundUtils.CopySoundsToAnim("crudeoil_refinery_staged_kanim", "algae_distillery_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityKG = 50f;
			conduitConsumer.capacityTag = SimHashes.CrudeOil.CreateTag();
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer portConduitConsumer1 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer1.conduitType = ConduitType.Gas;
			portConduitConsumer1.consumptionRate = 10f;
			portConduitConsumer1.capacityKG = 50f;
			portConduitConsumer1.capacityTag = SimHashes.Steam.CreateTag();
			portConduitConsumer1.forceAlwaysSatisfied = true;
			portConduitConsumer1.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer1.AssignPort(steamGasInputPort);

			PortConduitConsumer portConduitConsumer2 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer2.conduitType = ConduitType.Gas;
			portConduitConsumer2.consumptionRate = 10f;
			portConduitConsumer2.capacityKG = 50f;
			portConduitConsumer2.capacityTag = SimHashes.Hydrogen.CreateTag();
			portConduitConsumer2.forceAlwaysSatisfied = true;
			portConduitConsumer2.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer2.AssignPort(hydrogenGasInputPort);

			PortConduitConsumer portConduitConsumer3 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer3.conduitType = ConduitType.Liquid;
			portConduitConsumer3.consumptionRate = 10f;
			portConduitConsumer3.capacityKG = 50f;
			portConduitConsumer3.capacityTag = SimHashes.Naphtha.CreateTag();
			portConduitConsumer3.forceAlwaysSatisfied = true;
			portConduitConsumer3.SkipSetOperational = true;
			portConduitConsumer3.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer3.AssignPort(naphthaInputPort);

			ElementConverter elementConverter1 = go.AddComponent<ElementConverter>();
			elementConverter1.consumedElements =
			[
				new (SimHashes.CrudeOil.CreateTag(), 9.5f),
				new (SimHashes.Steam.CreateTag(), 0.5f)
			];
			elementConverter1.outputElements =
			[
				new (5f, SimHashes.Petroleum, 371.15f, storeOutput: true, diseaseWeight: 0.5f),
				new (2.5f, SimHashes.Naphtha, 367.15f, storeOutput: true, diseaseWeight: 0.25f),
				new (1f, SimHashes.Methane, 388.15f, storeOutput: true, diseaseWeight: 0.1f),
				new (0.5f, SimHashes.Bitumen, 343.15f, storeOutput: true, diseaseWeight: 0.05f),
				new (1f, ModElements.SourWater_Liquid, 343.15f, storeOutput: true, diseaseWeight: 0.1f)
			];
			ElementConverter elementConverter2 = go.AddComponent<ElementConverter>();
			elementConverter2.consumedElements =
			[
				new ElementConverter.ConsumedElement(SimHashes.Naphtha.CreateTag(), 2.5f),
				new ElementConverter.ConsumedElement(SimHashes.Hydrogen.CreateTag(), 0.21f)
			];
			elementConverter2.outputElements =
			[
				new ElementConverter.OutputElement(1.125f, SimHashes.Petroleum, 371.15f, storeOutput: true, diseaseWeight: 0.45f),
				new ElementConverter.OutputElement(0.25f, SimHashes.Methane, 367.15f, storeOutput: true, diseaseWeight: 0.1f),
				new ElementConverter.OutputElement(1.125f, SimHashes.Bitumen, 343.15f, storeOutput: true, diseaseWeight: 0.45f)
			];
			ElementDropper elementDropper = go.AddComponent<ElementDropper>();
			elementDropper.emitMass = 50f;
			elementDropper.emitTag = SimHashes.Bitumen.CreateTag();
			elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);

			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.showInUI = true;

			PipedConduitDispenser conduitDispenser = go.AddComponent<PipedConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.storage = storage;
			conduitDispenser.alwaysDispense = true;
			conduitDispenser.elementFilter = [SimHashes.Petroleum];
			conduitDispenser.AssignPort(PetroleumLiquidOutputPort);

			PipedConduitDispenser naphtaDispenser = go.AddComponent<PipedConduitDispenser>();
			naphtaDispenser.storage = storage;
			naphtaDispenser.conduitType = ConduitType.Liquid;
			naphtaDispenser.alwaysDispense = true;
			naphtaDispenser.elementFilter = [SimHashes.Naphtha];
			naphtaDispenser.SkipSetOperational = true; //handled by threshold
			naphtaDispenser.AssignPort(naphthaLiquidOutputPort);

			var sourGasLimit = go.AddComponent<ElementThresholdOperational>();
			sourGasLimit.Threshold = 100f;
			sourGasLimit.ThresholdTag = SimHashes.Naphtha.CreateTag();

			PipedConduitDispenser methaneDispenser = go.AddComponent<PipedConduitDispenser>();
			methaneDispenser.storage = storage;
			methaneDispenser.conduitType = ConduitType.Gas;
			methaneDispenser.alwaysDispense = true;
			methaneDispenser.elementFilter = [SimHashes.Methane];
			methaneDispenser.AssignPort(methaneGasOutputPort);


			PipedConduitDispenser sWaterDispenser = go.AddComponent<PipedConduitDispenser>();
			sWaterDispenser.conduitType = ConduitType.Liquid;
			sWaterDispenser.storage = storage;
			sWaterDispenser.alwaysDispense = true;
			sWaterDispenser.elementFilter = [ModElements.SourWater_Liquid];
			sWaterDispenser.AssignPort(SourWaterLiquidOutputPort);

			go.AddOrGet<ElementConversionBuilding>(); //Handles element converter
			Prioritizable.AddRef(go);
			this.AttachPort(go);

		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController displayController = go.AddComponent<PortDisplayController>();
			displayController.Init(go);
			displayController.AssignPort(go, hydrogenGasInputPort);
			displayController.AssignPort(go, steamGasInputPort);
			displayController.AssignPort(go, naphthaInputPort);
			displayController.AssignPort(go, naphthaLiquidOutputPort);
			displayController.AssignPort(go, methaneGasOutputPort);
			displayController.AssignPort(go, PetroleumLiquidOutputPort);
			displayController.AssignPort(go, SourWaterLiquidOutputPort);			
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
