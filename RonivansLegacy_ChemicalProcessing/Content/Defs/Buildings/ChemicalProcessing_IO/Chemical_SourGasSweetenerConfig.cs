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

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.ChemicalProcessing_IO
{
	internal class Chemical_SourGasSweetenerConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_SourGasSweetener";

		//private static readonly PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(0, 2), null, new Color32(167, 180, 201, byte.MaxValue));
		private static readonly PortDisplayInput sourGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(-1, 0), null, new Color32(255, 173, 248, byte.MaxValue));
		private static readonly PortDisplayOutput methaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 1), null, new Color32(255, 110, 15, byte.MaxValue));
		//private static readonly PortDisplayOutput sulphuricLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 0), null, new Color32(252, 252, 3, byte.MaxValue));

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [400f, 400f];
			string[] ingredient_types = [GameTags.RefinedMetal.ToString(), SimHashes.RefinedCarbon.ToString()];
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "chemical_sourgas_sweetener_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, noise);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 1f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			//buildingDef.InputConduitType = ConduitType.Gas;
			//buildingDef.UtilityInputOffset = new CellOffset(0, 3);
			//buildingDef.OutputConduitType = ConduitType.Liquid;
			//buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			SoundUtils.CopySoundsToAnim("chemical_sourgas_sweetener_kanim", "electrolyzer_kanim");
			return buildingDef;

		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			//storage.capacityKg = 300f;
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			//-----[ Conduit Consumers/Dispenser Section ]---------------------------------
			PortConduitConsumer sourGasConsumer = go.AddComponent<PortConduitConsumer>();
			sourGasConsumer.conduitType = ConduitType.Gas;
			sourGasConsumer.consumptionRate = 10f;
			sourGasConsumer.capacityKG = 20f;
			sourGasConsumer.capacityTag = SimHashes.SourGas.CreateTag();
			sourGasConsumer.forceAlwaysSatisfied = true;
			sourGasConsumer.SkipSetOperational = true;
			sourGasConsumer.alwaysConsume = true;
			sourGasConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			sourGasConsumer.AssignPort(sourGasInputPort);

			//PortConduitConsumer steamConsumer = go.AddComponent<PortConduitConsumer>();
			//steamConsumer.conduitType = ConduitType.Gas;
			//steamConsumer.consumptionRate = 5f;
			//steamConsumer.capacityKG = 5f;
			//steamConsumer.capacityTag = SimHashes.Steam.CreateTag();
			//steamConsumer.forceAlwaysSatisfied = true;
			//steamConsumer.SkipSetOperational = true;
			//steamConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			//steamConsumer.AssignPort(steamGasInputPort);

			PipedConduitDispenser methaneDispenser = go.AddComponent<PipedConduitDispenser>();
			methaneDispenser.storage = storage;
			methaneDispenser.elementFilter = [SimHashes.Methane];
			methaneDispenser.AssignPort(methaneGasOutputPort);
			methaneDispenser.alwaysDispense = true;
			methaneDispenser.SkipSetOperational = true;

			//PipedConduitDispenser acidDispenser = go.AddComponent<PipedConduitDispenser>();
			//acidDispenser.storage = storage;
			//acidDispenser.elementFilter = [ModElements.SulphuricAcid_Liquid];
			//acidDispenser.AssignPort(sulphuricLiquidOutputPort);
			//acidDispenser.alwaysDispense = true;
			//acidDispenser.SkipSetOperational = true;


			var methaneLimit = go.AddComponent<ElementThresholdOperational>();
			methaneLimit.Threshold = 100f;
			methaneLimit.ThresholdTag = SimHashes.Methane.CreateTag();
			methaneLimit.CreateMeter = true;


			//var acidLimit = go.AddComponent<ElementThresholdOperational>();
			//methaneLimit.Threshold = 30f;
			//methaneLimit.ThresholdTag = ModElements.SulphuricAcid_Liquid.CreateTag();

			float converterMultiplier = 1f;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.SourGas.CreateTag(), 1f * converterMultiplier),
				//new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.22f* converterMultiplier),
				new ElementConverter.ConsumedElement(SimHashes.RefinedCarbon.CreateTag(), 0.1f* converterMultiplier),

				];
			converter.outputElements = [
				new ElementConverter.OutputElement(0.6f* converterMultiplier, SimHashes.Methane, 335.15f, false, true, 0f, 0.5f),
				new ElementConverter.OutputElement(0.4f* converterMultiplier, SimHashes.Sulfur, 345.15f, false, true, 0f, 0.5f),
				new ElementConverter.OutputElement(0.1f* converterMultiplier, SimHashes.Carbon, 345.15f, true, true, 0f, 0.5f)

				];
			//-------------------------------------------------------------------

			ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
			manualDeliveryKg.SetStorage(storage);
			manualDeliveryKg.RequestedItemTag = SimHashes.RefinedCarbon.CreateTag();
			manualDeliveryKg.capacity = 600f;
			manualDeliveryKg.refillMass = 60f;
			manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ElementDropper carbonDropper = go.AddComponent<ElementDropper>();
			carbonDropper.emitMass = 10f;
			carbonDropper.emitTag = SimHashes.Carbon.CreateTag();
			carbonDropper.emitOffset = new Vector3(1.0f, 1f, 0.0f);

			ElementDropper sulphurDropper = go.AddComponent<ElementDropper>();
			sulphurDropper.emitMass = 10f;
			sulphurDropper.emitTag = SimHashes.Sulfur.CreateTag();
			sulphurDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			AttachPort(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			AttachPort(go);
		}


		private void AttachPort(GameObject go)
		{
			PortDisplayController displayController = go.AddComponent<PortDisplayController>();
			displayController.Init(go);
			//displayController.AssignPort(go, steamGasInputPort);
			displayController.AssignPort(go, sourGasInputPort);
			displayController.AssignPort(go, methaneGasOutputPort);
			//displayController.AssignPort(go, sulphuricLiquidOutputPort);
		}
	}
}
