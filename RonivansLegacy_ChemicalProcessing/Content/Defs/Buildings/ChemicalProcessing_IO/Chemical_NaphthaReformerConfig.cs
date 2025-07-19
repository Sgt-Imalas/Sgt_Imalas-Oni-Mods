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

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//==== [ CHEMICAL: NAPHTHA REFORMER CONFIG ] =================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_NaphthaReformerConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_NaphthaReformer";

		//--[ Identification and DLC stuff ]-----------------------------------

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput HydrogenGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(2, 1), null, new Color32(197, 31, 139, 255));
		private static readonly PortDisplayOutput methaneGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(-1, 1), null, new Color32(255, 114, 33, 255));
		private static readonly PortDisplayInput naphthaLiquidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(2, -3), null, new Color32(176, 0, 255, 255));
		private static readonly PortDisplayOutput PetroleumLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, -3), null, new Color32(255, 195, 37, 255));

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [300f, 100f];
			string[] ingredient_types = ["RefinedMetal", SimHashes.Steel.ToString()];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 7, "naphtha_reformer_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 480f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 6f;
			buildingDef.PowerInputOffset = new CellOffset(1, 1);
			buildingDef.AudioCategory = "Metal";

			ColliderOffsetHandler.GenerateBuildingDefOffsets(buildingDef, -2, 0);

			//for (int i = 0; i < buildingDef.PlacementOffsets.Length; i++)
			//{
			//	buildingDef.PlacementOffsets[i] = new CellOffset(buildingDef.PlacementOffsets[i].x, buildingDef.PlacementOffsets[i].y - 2);
			//}
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;


			//-----[ Element Converter Section ]---------------------------------
			ElementConverter naphtha_reforming = go.AddComponent<ElementConverter>();
			naphtha_reforming.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Naphtha.CreateTag(), 2.5f),
				new ElementConverter.ConsumedElement(SimHashes.Hydrogen.CreateTag(), 0.210f) ];
			naphtha_reforming.outputElements = [
				new ElementConverter.OutputElement(1.125f, SimHashes.Petroleum, 371.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.25f, SimHashes.Methane, 388.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(1.125f, SimHashes.Bitumen, 343.15f, true, true, 0f, 0.5f, 0.25f, 0xff, 0)];
			//--------------------------------------------------------------------

			ElementDropper bitumenDropper = go.AddComponent<ElementDropper>();
			bitumenDropper.emitMass = 50f;
			bitumenDropper.emitTag = SimHashes.Bitumen.CreateTag();
			bitumenDropper.emitOffset = new Vector3(0f, 1f, 0f);

			Storage outputStorage = go.AddOrGet<Storage>();
			outputStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			outputStorage.showInUI = true;


			PortConduitConsumer crudeOilInput = go.AddOrGet<PortConduitConsumer>();
			crudeOilInput.conduitType = ConduitType.Liquid;
			crudeOilInput.consumptionRate = 10f;
			crudeOilInput.capacityKG = 50f;
			crudeOilInput.capacityTag = SimHashes.Naphtha.CreateTag();
			crudeOilInput.forceAlwaysSatisfied = true;
			crudeOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			crudeOilInput.AssignPort(naphthaLiquidInputPort);

			PortConduitConsumer HydrogenInput = go.AddComponent<PortConduitConsumer>();
			HydrogenInput.conduitType = ConduitType.Gas;
			HydrogenInput.consumptionRate = 10f;
			HydrogenInput.capacityKG = 50f;
			HydrogenInput.capacityTag = SimHashes.Hydrogen.CreateTag();
			HydrogenInput.forceAlwaysSatisfied = true;
			HydrogenInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			HydrogenInput.AssignPort(HydrogenGasInputPort);


			PipedConduitDispenser petrolOutput = go.AddOrGet<PipedConduitDispenser>();
			petrolOutput.conduitType = ConduitType.Liquid;
			petrolOutput.storage = outputStorage;
			petrolOutput.alwaysDispense = true;
			petrolOutput.elementFilter = [SimHashes.Petroleum];
			petrolOutput.AssignPort(PetroleumLiquidOutputPort);

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
			controller.AssignPort(go, HydrogenGasInputPort);
			controller.AssignPort(go, methaneGasOutputPort);
			controller.AssignPort(go, PetroleumLiquidOutputPort);
			controller.AssignPort(go, naphthaLiquidInputPort);
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
				def.solidOffsets[i] = new CellOffset(i - 1, 0);
			}
			go.AddOrGet<ColliderOffsetHandler>().ColliderOffsetY = -2;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			go.AddOrGet<ColliderOffsetHandler>().ColliderOffsetY = -2;
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}
