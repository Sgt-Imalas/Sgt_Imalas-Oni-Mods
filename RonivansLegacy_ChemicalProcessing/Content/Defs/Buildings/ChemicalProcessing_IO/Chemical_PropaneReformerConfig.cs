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
using UtilLibs.BuildingPortUtils;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//==== [ CHEMICAL: PROPANE REFORMER CONFIG ] =================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_PropaneReformerConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_PropaneReformer";

		//--[ Identification and DLC stuff ]-----------------------------------
		public static readonly List<Storage.StoredItemModifier> PropaneReformerStoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(2, -1));

		private static readonly PortDisplayOutput pollutedWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, -2));
		private static readonly PortDisplayOutput co2GasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(2, 1));

		static Chemical_PropaneReformerConfig()
		{
			Color? steamPortColor = new Color32(167, 180, 201, 255);
			steamGasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(2, -1), null, steamPortColor);

			Color? pollutedWaterOutputPortColor = new Color32(71, 69, 6, 255);
			pollutedWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, -2), null, pollutedWaterOutputPortColor);

			Color? co2OutputPortColor = new Color32(96, 87, 97, 255);
			co2GasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(2, 1), null, co2OutputPortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			PropaneReformerStoredItemModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [300f, 100f];
			string[] ingredient_types = ["RefinedMetal", SimHashes.Steel.ToString()];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 7, "propane_reformer_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 320f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 6f;
			buildingDef.PowerInputOffset = new CellOffset(1, 1);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(2, -2);
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 1);

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

			ConduitConsumer propaneGasInput = go.AddOrGet<ConduitConsumer>();
			propaneGasInput.conduitType = ConduitType.Gas;
			propaneGasInput.consumptionRate = 10f;
			propaneGasInput.capacityKG = 50f;
			propaneGasInput.capacityTag = SimHashes.Propane.CreateTag();
			propaneGasInput.forceAlwaysSatisfied = true;
			propaneGasInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer steamInput = go.AddComponent<PortConduitConsumer>();
			steamInput.conduitType = ConduitType.Gas;
			steamInput.consumptionRate = 10f;
			steamInput.capacityKG = 100f;
			steamInput.capacityTag = SimHashes.Steam.CreateTag();
			steamInput.forceAlwaysSatisfied = true;
			steamInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			steamInput.AssignPort(steamGasInputPort);

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter propane_reforming = go.AddComponent<ElementConverter>();
			propane_reforming.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Propane.CreateTag(), 0.525f),
				new ElementConverter.ConsumedElement(SimHashes.Steam.CreateTag(), 0.200f) ];
			propane_reforming.outputElements = [
				new ElementConverter.OutputElement(0.435f, SimHashes.Hydrogen, 371.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.2175f, SimHashes.DirtyWater, 312.15f, true, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.0725f, SimHashes.CarbonDioxide, 371.15f, true, true, 0f, 0.5f, 0.25f, 0xff, 0)];
			//--------------------------------------------------------------------

			Storage outputStorage = go.AddOrGet<Storage>();
			outputStorage.SetDefaultStoredItemModifiers(PropaneReformerStoredItemModifiers);
			outputStorage.showInUI = true;

			ConduitDispenser hydrogenGasOutput = go.AddOrGet<ConduitDispenser>();
			hydrogenGasOutput.conduitType = ConduitType.Gas;
			hydrogenGasOutput.storage = outputStorage;
			hydrogenGasOutput.elementFilter = [SimHashes.Hydrogen];

			PipedConduitDispenser pollutedWaterOutput = go.AddComponent<PipedConduitDispenser>();
			pollutedWaterOutput.storage = outputStorage;
			pollutedWaterOutput.conduitType = ConduitType.Liquid;
			pollutedWaterOutput.alwaysDispense = true;
			pollutedWaterOutput.elementFilter = [SimHashes.DirtyWater];
			pollutedWaterOutput.AssignPort(pollutedWaterLiquidOutputPort);

			PipedConduitDispenser co2GasOutput = go.AddComponent<PipedConduitDispenser>();
			co2GasOutput.storage = outputStorage;
			co2GasOutput.conduitType = ConduitType.Gas;
			co2GasOutput.alwaysDispense = true;
			co2GasOutput.elementFilter = [SimHashes.CarbonDioxide];
			co2GasOutput.AssignPort(co2GasOutputPort);

			go.AddOrGet<ElementConversionBuilding>(); //Handles element converter
			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, steamGasInputPort);
			controller.AssignPort(go, pollutedWaterLiquidOutputPort);
			controller.AssignPort(go, co2GasOutputPort);
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
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
			go.AddOrGet<ColliderOffsetHandler>().ColliderOffsetY = -2;
		}
	}
}
