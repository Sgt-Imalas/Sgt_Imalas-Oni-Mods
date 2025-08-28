using HarmonyLib;
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
	//==== [ CHEMICAL: SOUR WATER STRIPPER CONFIG ] =================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_SourWaterStripperConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_SourWaterStripper";
	

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 0));

		private static readonly PortDisplayOutput sourGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 2));
		private static readonly PortDisplayOutput ammoniaGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 1));


		static Chemical_SourWaterStripperConfig()
		{
			Color? steamInputPortColor = new Color32(167, 180, 201, 255);
			steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 0), null, steamInputPortColor);

			Color? sourGasOutputPortColor = new Color32(255, 173, 248, 255);
			sourGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 2), null, sourGasOutputPortColor);

			Color? ammoniaGasOutputPortColor = new Color32(215, 227, 252, 255);
			ammoniaGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-2, 1), null, ammoniaGasOutputPortColor); //Ammonia Output


		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [400f, 200f];
			string[] ingredient_types = ["BuildableRaw", "RefinedMetal"];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 6, 4, "sourwater_stripper_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = true;
			buildingDef.OverheatTemperature = 348.15f;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 360f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 8f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(3, 2);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(-2, 0);
			SoundUtils.CopySoundsToAnim("sourwater_stripper_kanim", "desalinator_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			Tag filter = SimHashes.Sand.CreateTag();

			ConduitConsumer crudeOilInput = go.AddOrGet<ConduitConsumer>();
			crudeOilInput.conduitType = ConduitType.Liquid;
			crudeOilInput.consumptionRate = 10f;
			crudeOilInput.capacityKG = 50f;
			crudeOilInput.capacityTag = ModElements.SourWater_Liquid.Tag;
			crudeOilInput.forceAlwaysSatisfied = true;
			crudeOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			Storage solidStorage = go.AddOrGet<Storage>();
			solidStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			solidStorage.showInUI = true;

			PortConduitConsumer steamInput = go.AddComponent<PortConduitConsumer>();
			steamInput.conduitType = ConduitType.Gas;
			steamInput.consumptionRate = 10f;
			steamInput.capacityKG = 50f;
			steamInput.capacityTag = SimHashes.Steam.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			steamInput.AssignPort(steamGasInputPort);

			ManualDeliveryKG catalystDelivery = go.AddComponent<ManualDeliveryKG>();
			catalystDelivery.SetStorage(solidStorage);
			catalystDelivery.RequestedItemTag = filter;
			catalystDelivery.capacity = 500f;
			catalystDelivery.refillMass = 90f;
			catalystDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter stripping = go.AddComponent<ElementConverter>();
			stripping.consumedElements = [
				new ElementConverter.ConsumedElement(ModElements.SourWater_Liquid.Tag, 5f),
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.25f),
				new ElementConverter.ConsumedElement(SimHashes.Sand.CreateTag(), 0.1f)];
			stripping.outputElements = [
				new ElementConverter.OutputElement(4.250f, SimHashes.Water, 321.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.750f, SimHashes.SourGas, 367.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.5f,ModElements.Ammonia_Gas, 356.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.35f, SimHashes.ToxicSand, 343.15f, true, true, 0f, 0.5f, 0.25f, 0xff, 0)];
			//--------------------------------------------------------------------

			ElementDropper toxicDropper = go.AddComponent<ElementDropper>();
			toxicDropper.emitMass = 35f;
			toxicDropper.emitTag = SimHashes.ToxicSand.CreateTag();
			toxicDropper.emitOffset = new Vector3(0f, 1f, 0f);

			Storage outputStorage = go.AddOrGet<Storage>();
			outputStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			outputStorage.showInUI = true;

			ConduitDispenser petrolOutput = go.AddOrGet<ConduitDispenser>();
			petrolOutput.conduitType = ConduitType.Liquid;
			petrolOutput.storage = outputStorage;
			petrolOutput.elementFilter = [SimHashes.Water];

			PipedConduitDispenser sourGasOutput = go.AddComponent<PipedConduitDispenser>();
			sourGasOutput.storage = outputStorage;
			sourGasOutput.conduitType = ConduitType.Gas;
			sourGasOutput.alwaysDispense = true;
			sourGasOutput.elementFilter = [SimHashes.SourGas];
			sourGasOutput.AssignPort(sourGasOutputPort);

			PipedConduitDispenser ammoniaGasOutput = go.AddComponent<PipedConduitDispenser>();
			ammoniaGasOutput.storage = outputStorage;
			ammoniaGasOutput.conduitType = ConduitType.Gas;
			ammoniaGasOutput.alwaysDispense = true;
			ammoniaGasOutput.elementFilter = [ModElements.Ammonia_Gas];
			ammoniaGasOutput.AssignPort(ammoniaGasOutputPort);

			go.AddOrGet<ElementConversionBuilding>(); //Handles element converter
			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, steamGasInputPort);
			controller.AssignPort(go, sourGasOutputPort);
			controller.AssignPort(go, ammoniaGasOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

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
