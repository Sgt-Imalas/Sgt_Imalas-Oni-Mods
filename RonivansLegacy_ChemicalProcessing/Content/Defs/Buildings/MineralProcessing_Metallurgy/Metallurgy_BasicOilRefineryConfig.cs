using HarmonyLib;
using KSerialization;
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



namespace Metallurgy.Buildings
{
	//==== [ METALLURGY: BASIC OIL REFINERY CONFIG ] =================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Metallurgy_BasicOilRefineryConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Metallurgy_BasicOilRefinery";


		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayOutput CarbonDioxideGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 0));
		private static readonly PortDisplayOutput methaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 1));

		static Metallurgy_BasicOilRefineryConfig()
		{
			Color? co2PortColor = new Color32(71, 71, 71, 255);
			CarbonDioxideGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 0), null, co2PortColor);

			Color? methanePortColor = new Color32(255, 114, 33, 255);
			methaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 1), null, methanePortColor);
		}

		//--[ Building Definitions ]---------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "basic_oil_refinery_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier, 0.2f);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 90f;
			buildingDef.ExhaustKilowattsWhenActive = 16f;
			buildingDef.SelfHeatKilowattsWhenActive = 8f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(1, 0);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 0);
			SoundUtils.CopySoundsToAnim("basic_oil_refinery_kanim", "algae_distillery_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.capacityKg = 4000f;
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			ManualDeliveryKG coalFetch = go.AddComponent<ManualDeliveryKG>();
			coalFetch.SetStorage(storage);
			coalFetch.RequestedItemTag = SimHashes.Carbon.CreateTag();
			coalFetch.capacity = 500f;
			coalFetch.refillMass = 100f;
			coalFetch.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			go.AddOrGet<SolidDeliverySelection>().Options = [SimHashes.Carbon.CreateTag(), SimHashes.Peat.CreateTag()];

			ConduitConsumer crudeOilInput = go.AddComponent<ConduitConsumer>();
			crudeOilInput.capacityTag = SimHashes.CrudeOil.CreateTag();
			crudeOilInput.capacityKG = 50f;
			crudeOilInput.forceAlwaysSatisfied = true;
			crudeOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter crudeoil_refining = go.AddComponent<ElementConverter>();
			crudeoil_refining.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.CrudeOil.CreateTag(), 5f),
				new ElementConverter.ConsumedElement(GameTags.CombustibleSolid, 0.1f) ];
			crudeoil_refining.outputElements = [
				new ElementConverter.OutputElement(2.5f, SimHashes.Petroleum, 371.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.05f, SimHashes.CarbonDioxide, 367.15f, true, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.09f, SimHashes.Methane, 388.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];
			//--------------------------------------------------------------------


			ConduitDispenser petrolOutput = go.AddOrGet<ConduitDispenser>();
			petrolOutput.conduitType = ConduitType.Liquid;
			petrolOutput.storage = storage;
			petrolOutput.alwaysDispense = true;
			petrolOutput.elementFilter = [SimHashes.Petroleum];

			PipedConduitDispenser co2Output = go.AddComponent<PipedConduitDispenser>();
			co2Output.storage = storage;
			co2Output.conduitType = ConduitType.Gas;
			co2Output.alwaysDispense = true;
			co2Output.SkipSetOperational = true;
			co2Output.elementFilter = [SimHashes.CarbonDioxide];
			co2Output.AssignPort(CarbonDioxideGasOutputPort);

			PipedOptionalExhaust exhaustCO2 = go.AddComponent<PipedOptionalExhaust>();
			exhaustCO2.dispenser = co2Output;
			exhaustCO2.elementTag = SimHashes.CarbonDioxide.CreateTag();
			exhaustCO2.capacity = 10f;

			PipedConduitDispenser methaneOutput = go.AddComponent<PipedConduitDispenser>();
			methaneOutput.storage = storage;
			methaneOutput.conduitType = ConduitType.Gas;
			methaneOutput.alwaysDispense = true;
			methaneOutput.SkipSetOperational = true;
			methaneOutput.elementFilter = [SimHashes.Methane];
			methaneOutput.AssignPort(methaneGasOutputPort);

			PipedOptionalExhaust exhaustMethane = go.AddComponent<PipedOptionalExhaust>();
			exhaustMethane.dispenser = methaneOutput;
			exhaustMethane.elementTag = SimHashes.Methane.CreateTag();
			exhaustMethane.capacity = 10f;

			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, CarbonDioxideGasOutputPort);
			controller.AssignPort(go, methaneGasOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}
