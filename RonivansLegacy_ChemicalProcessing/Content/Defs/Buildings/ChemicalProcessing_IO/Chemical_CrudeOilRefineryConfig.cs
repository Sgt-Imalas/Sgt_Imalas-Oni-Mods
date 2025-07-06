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
	//==== [ CHEMICAL: CRUDE OIL REFINERY CONFIG ] =================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_CrudeOilRefineryConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_CrudeOilRefinery";

		//--[ Identification and DLC stuff ]-----------------------------------
		public static readonly List<Storage.StoredItemModifier> OilRefineryStoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(2, 1));
		private static readonly PortDisplayOutput SourWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, 0));
		private static readonly PortDisplayOutput naphthaLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(2, 3));
		private static readonly PortDisplayOutput methaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-1, 3));



		static Chemical_CrudeOilRefineryConfig()
		{
			Color? steamInputPortColor = new Color32(167, 180, 201, 255);
			steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(2, 1), null, steamInputPortColor);

			Color? SourWaterOutputPortColor = new Color32(130, 104, 65, 255);
			SourWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, 0), null, SourWaterOutputPortColor);

			Color? naphthaPortColor = new Color32(92, 81, 90, 255);
			naphthaLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(2, 3), null, naphthaPortColor);

			Color? methanePortColor = new Color32(255, 114, 33, 255);
			methaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-1, 3), null, methanePortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			OilRefineryStoredItemModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [300f, 100f];
			string[] ingredient_types = ["RefinedMetal", SimHashes.Steel.ToString()];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 5, "crudeoil_refinery_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 640f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 6f;
			buildingDef.PowerInputOffset = new CellOffset(1, 1);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(2, 0);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 1);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			go.AddOrGet<Desalinator>();

			ConduitConsumer crudeOilInput = go.AddOrGet<ConduitConsumer>();
			crudeOilInput.conduitType = ConduitType.Liquid;
			crudeOilInput.consumptionRate = 10f;
			crudeOilInput.capacityKG = 50f;
			crudeOilInput.capacityTag = SimHashes.CrudeOil.CreateTag();
			crudeOilInput.forceAlwaysSatisfied = true;
			crudeOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer steamInput = go.AddComponent<PortConduitConsumer>();
			steamInput.conduitType = ConduitType.Gas;
			steamInput.consumptionRate = 10f;
			steamInput.capacityKG = 50f;
			steamInput.capacityTag = SimHashes.Steam.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			steamInput.AssignPort(steamGasInputPort);

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter crudeoil_refining = go.AddComponent<ElementConverter>();
			crudeoil_refining.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.CrudeOil.CreateTag(), 10f),
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.5f) ];
			crudeoil_refining.outputElements = [
				new ElementConverter.OutputElement(5f, SimHashes.Petroleum, 371.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(2.5f, SimHashes.Naphtha, 367.15f, true, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(1f, SimHashes.Methane, 388.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.5f, SimHashes.Bitumen, 343.15f, true, true, 0f, 0.5f, 0.25f, 0xff, 0),
				new ElementConverter.OutputElement(1f, ModElements.SourWater_Liquid, 307.15f, true, true, 0f, 0.5f, 0.75f, 0xff, 0)];
			//--------------------------------------------------------------------

			ElementDropper bitumenDropper = go.AddComponent<ElementDropper>();
			bitumenDropper.emitMass = 50f;
			bitumenDropper.emitTag = SimHashes.Bitumen.CreateTag();
			bitumenDropper.emitOffset = new Vector3(0f, 1f, 0f);

			Storage outputStorage = go.AddOrGet<Storage>();
			outputStorage.SetDefaultStoredItemModifiers(OilRefineryStoredItemModifiers);
			outputStorage.showInUI = true;

			ConduitDispenser petrolOutput = go.AddOrGet<ConduitDispenser>();
			petrolOutput.conduitType = ConduitType.Liquid;
			petrolOutput.storage = outputStorage;
			petrolOutput.elementFilter = [SimHashes.Petroleum];

			PipedConduitDispenser sourWaterOutput = go.AddComponent<PipedConduitDispenser>();
			sourWaterOutput.storage = outputStorage;
			sourWaterOutput.conduitType = ConduitType.Liquid;
			sourWaterOutput.alwaysDispense = true;
			sourWaterOutput.elementFilter = [ModElements.SourWater_Liquid];
			sourWaterOutput.AssignPort(SourWaterLiquidOutputPort);

			PipedConduitDispenser naphthaOuput = go.AddComponent<PipedConduitDispenser>();
			naphthaOuput.storage = outputStorage;
			naphthaOuput.conduitType = ConduitType.Liquid;
			naphthaOuput.alwaysDispense = true;
			naphthaOuput.elementFilter = [SimHashes.Naphtha];
			naphthaOuput.AssignPort(naphthaLiquidOutputPort);

			PipedConduitDispenser methaneOutput = go.AddComponent<PipedConduitDispenser>();
			methaneOutput.storage = outputStorage;
			methaneOutput.conduitType = ConduitType.Gas;
			methaneOutput.alwaysDispense = true;
			methaneOutput.elementFilter = [SimHashes.Methane];
			methaneOutput.AssignPort(methaneGasOutputPort);

			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, steamGasInputPort);
			controller.AssignPort(go, SourWaterLiquidOutputPort);
			controller.AssignPort(go, naphthaLiquidOutputPort);
			controller.AssignPort(go, methaneGasOutputPort);
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
