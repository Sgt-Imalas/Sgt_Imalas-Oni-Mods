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
	//====[ CHEMICAL: RAW GAS REFINERY CONFIG ]============================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_RawGasRefineryConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_RawGasRefinery";

		//--[ Identification and DLC stuff ]-----------------------------------
		private void ConfigureRecipes() { }
		public static readonly List<Storage.StoredItemModifier> GasRefineryStoredItemModifiers = [
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate,
			];

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(2, 1), null, new Color32(167, 180, 201, 255));  //Steam Input

		private static readonly PortDisplayOutput propaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(2, 3), null, new Color32(3, 44, 252, 255)); //Propane Output
		private static readonly PortDisplayOutput SourWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, 1), null, new Color32(130, 104, 65, 255));


		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [300f, 100f];
			string[] ingredient_types = ["RefinedMetal", SimHashes.Steel.ToString()];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 5, "rawgas_refinery_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 420f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 6f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(2, 0);
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 3);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			Storage standardStorage = go.AddOrGet<Storage>();
			standardStorage.capacityKg = 500f;
			standardStorage.SetDefaultStoredItemModifiers(GasRefineryStoredItemModifiers);
			standardStorage.showInUI = true;

			ConduitConsumer rawGasInput = go.AddOrGet<ConduitConsumer>();
			rawGasInput.conduitType = ConduitType.Gas;
			rawGasInput.consumptionRate = 10f;
			rawGasInput.capacityKG = 100f;
			rawGasInput.capacityTag = ModElements.RawNaturalGas_Gas.Tag;
			rawGasInput.forceAlwaysSatisfied = true;
			rawGasInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer steamInput = go.AddComponent<PortConduitConsumer>();
			steamInput.conduitType = ConduitType.Gas;
			steamInput.consumptionRate = 10f;
			steamInput.capacityKG = 100f;
			steamInput.capacityTag = SimHashes.Steam.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			steamInput.AssignPort(steamGasInputPort);

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter rawGasRefining = go.AddComponent<ElementConverter>();
			rawGasRefining.consumedElements = [
				new (ModElements.RawNaturalGas_Gas.Tag, 1f),
				new (SimHashes.Steam.CreateTag(), 0.5f) ];
			rawGasRefining.outputElements = [
				new (0.75f, SimHashes.Methane, 371.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new (0.525f, SimHashes.Propane, 367.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new (0.225f, ModElements.SourWater_Liquid, 307.15f, true, true, 0f, 0.5f, 0.75f, 0xff, 0) ];
			//--------------------------------------------------------------------

			Storage outputStorage = go.AddOrGet<Storage>();
			outputStorage.SetDefaultStoredItemModifiers(GasRefineryStoredItemModifiers);
			outputStorage.showInUI = true;

			ConduitDispenser petrolOutput = go.AddOrGet<ConduitDispenser>();
			petrolOutput.conduitType = ConduitType.Gas;
			petrolOutput.storage = outputStorage;
			petrolOutput.elementFilter = [SimHashes.Methane];

			PipedConduitDispenser propaneOutput = go.AddComponent<PipedConduitDispenser>();
			propaneOutput.storage = outputStorage;
			propaneOutput.conduitType = ConduitType.Gas;
			propaneOutput.alwaysDispense = true;
			propaneOutput.elementFilter = [SimHashes.Propane];
			propaneOutput.AssignPort(propaneGasOutputPort);

			PipedConduitDispenser sourWaterOutput = go.AddComponent<PipedConduitDispenser>();
			sourWaterOutput.storage = outputStorage;
			sourWaterOutput.conduitType = ConduitType.Liquid;
			sourWaterOutput.alwaysDispense = true;
			sourWaterOutput.elementFilter = [ModElements.SourWater_Liquid];
			sourWaterOutput.AssignPort(SourWaterLiquidOutputPort);

			go.AddOrGet<ElementConversionBuilding>(); //Handles element converter
			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, steamGasInputPort);
			controller.AssignPort(go, propaneGasOutputPort);
			controller.AssignPort(go, SourWaterLiquidOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
			go.AddTag(GameTags.CorrosionProof);
			MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
			def.occupyFoundationLayer = false;
			def.solidOffsets = new CellOffset[4];
			for (int i = 0; i < 4; i++)
			{
				def.solidOffsets[i] = new CellOffset(i - 1, 2);
			}
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
