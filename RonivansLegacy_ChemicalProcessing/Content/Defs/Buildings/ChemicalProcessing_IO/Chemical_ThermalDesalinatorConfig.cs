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
using UtilLibs.BuildingPortUtils;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//====[ CHEMICAL: THERMAL DESALINATOR CONFIG ]===================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_ThermalDesalinatorConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_ThermalDesalinator";

		//--[ Identification and DLC stuff ]-----------------------------------
		public static readonly List<Storage.StoredItemModifier> DesalinatorStoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput ammoniumWaterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(3, 0));
		private static readonly PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 2));

		private static readonly PortDisplayOutput waterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-3, 1));
		private static readonly PortDisplayOutput ammoniaGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-3, 2));


		static Chemical_ThermalDesalinatorConfig()
		{
			Color? ammoniumPortColor = new Color32(186, 245, 255, 255);
			ammoniumWaterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(3, 0), null, ammoniumPortColor);

			Color? steamPortColor = new Color32(233, 236, 242, 255);
			steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 2), null, steamPortColor);

			Color? waterOutputPortColor = new Color32(72, 129, 247, 255);
			waterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-3, 1), null, waterOutputPortColor);

			Color? ammoniaGasOutputPortColor = new Color32(215, 227, 252, 255);
			ammoniaGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-3, 2), null, ammoniaGasOutputPortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			DesalinatorStoredItemModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [600f, 400f];
			string[] ingredient_types = [SimHashes.Ceramic.ToString(), "RefinedMetal"];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 3, "thermal_desalinator_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(3, 1);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(-3, 0);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			go.AddOrGet<Desalinator>();

			Storage standardStorage = go.AddOrGet<Storage>();
			standardStorage.capacityKg = 500f;
			standardStorage.SetDefaultStoredItemModifiers(DesalinatorStoredItemModifiers);
			standardStorage.showInUI = true;

			PortConduitConsumer steamInput = go.AddComponent<PortConduitConsumer>();
			steamInput.conduitType = ConduitType.Gas;
			steamInput.consumptionRate = 10f;
			steamInput.capacityKG = 50f;
			steamInput.capacityTag = SimHashes.Steam.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			steamInput.AssignPort(steamGasInputPort);

			ConduitConsumer saltWaterInput = go.AddOrGet<ConduitConsumer>();
			saltWaterInput.conduitType = ConduitType.Liquid;
			saltWaterInput.consumptionRate = 10f;
			saltWaterInput.capacityKG = 50f;
			saltWaterInput.capacityTag = SimHashes.SaltWater.CreateTag();
			saltWaterInput.forceAlwaysSatisfied = true;
			saltWaterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer ammoniumWaterInput = go.AddComponent<PortConduitConsumer>();
			ammoniumWaterInput.conduitType = ConduitType.Liquid;
			ammoniumWaterInput.consumptionRate = 10f;
			ammoniumWaterInput.capacityKG = 50f;
			ammoniumWaterInput.capacityTag = ModElements.AmmoniumWater_Liquid.Tag;
			ammoniumWaterInput.forceAlwaysSatisfied = true;
			ammoniumWaterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			ammoniumWaterInput.AssignPort(ammoniumWaterInputPort);

			//--- Element Converter Section --------------------------------------
			ElementConverter saltWaterTreatment = go.AddComponent<ElementConverter>();
			saltWaterTreatment.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.SaltWater.CreateTag(), 5f),
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.5f) ];
			saltWaterTreatment.outputElements = [
				new ElementConverter.OutputElement(4.35f, SimHashes.Water, 327.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(1.15f, SimHashes.Brine, 347.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];

			ElementConverter ammoniumWaterTreatment = go.AddComponent<ElementConverter>();
			ammoniumWaterTreatment.consumedElements = [
				new ElementConverter.ConsumedElement(ModElements.AmmoniumWater_Liquid.Tag, 5f),
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.5f) ];
			ammoniumWaterTreatment.outputElements = [
				new ElementConverter.OutputElement(2.5f, SimHashes.Water, 327.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(2.45f, SimHashes.Brine, 347.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.55f, ModElements.Ammonia_Gas, 321.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];
			//--------------------------------------------------------------------

			Storage outputStorage = go.AddOrGet<Storage>();

			outputStorage.SetDefaultStoredItemModifiers(DesalinatorStoredItemModifiers);
			outputStorage.showCapacityStatusItem = true;
			outputStorage.showCapacityAsMainStatus = true;
			outputStorage.showDescriptor = true;

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.storage = outputStorage;
			dispenser.elementFilter = [SimHashes.Brine];
			dispenser.alwaysDispense = true;

			PipedConduitDispenser waterOuput = go.AddComponent<PipedConduitDispenser>();
			waterOuput.storage = outputStorage;
			waterOuput.conduitType = ConduitType.Liquid;
			waterOuput.alwaysDispense = true;
			waterOuput.elementFilter = [SimHashes.Water];
			waterOuput.AssignPort(waterLiquidOutputPort);

			PipedConduitDispenser ammoniaGasOutput = go.AddComponent<PipedConduitDispenser>();
			ammoniaGasOutput.storage = outputStorage;
			ammoniaGasOutput.conduitType = ConduitType.Gas;
			ammoniaGasOutput.alwaysDispense = true;
			ammoniaGasOutput.elementFilter = [ModElements.Ammonia_Gas];
			ammoniaGasOutput.AssignPort(ammoniaGasOutputPort);

			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			//controller.AssignPort(go, saltWaterInputPort);
			controller.AssignPort(go, ammoniumWaterInputPort);
			controller.AssignPort(go, steamGasInputPort);
			controller.AssignPort(go, waterLiquidOutputPort);
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
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}
