using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing;
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


namespace Biochemistry.Buildings
{
	//===[ BIODIESEL REFINERY CONFIG ]===================================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Biochemistry_BiodieselRefineryConfig : IBuildingConfig
	{
		public static string ID = "Biochemistry_BiodieselRefinery";

		private static readonly PortDisplayInput ethanolLiquidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-3, 3), null, new Color32(0, 255, 235, 255));
		private static readonly PortDisplayOutput pollutedWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(3, 1), null, new Color32(137, 137, 66, 255));

		float multiplier = 1f/8f; //reduce refinery output to be more in line with production amounts

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 5, "biodiesel_refinery_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 480f* multiplier;
			buildingDef.ExhaustKilowattsWhenActive = 2f * multiplier;
			buildingDef.SelfHeatKilowattsWhenActive = 8f * multiplier;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(-3, 1);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(3, 3);
			SoundUtils.CopySoundsToAnim("biodiesel_generator_kanim", "algae_distillery_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go); 

			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			storage.capacityKg = 300f;
			//storage.showCapacityStatusItem = true;
			//storage.showCapacityAsMainStatus = true;
			//storage.showDescriptor = true;

			ConduitConsumer vegOilInput = go.AddOrGet<ConduitConsumer>();
			vegOilInput.conduitType = ConduitType.Liquid;
			vegOilInput.consumptionRate = 10f;
			vegOilInput.capacityKG = 20f;
			vegOilInput.capacityTag = ModElements.VegetableOil_Liquid.Tag;
			vegOilInput.forceAlwaysSatisfied = true;
			vegOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer ethanolInput = go.AddComponent<PortConduitConsumer>();
			ethanolInput.conduitType = ConduitType.Liquid;
			ethanolInput.consumptionRate = 10f;
			ethanolInput.capacityKG = 20f;
			ethanolInput.capacityTag = SimHashes.Ethanol.CreateTag();
			ethanolInput.forceAlwaysSatisfied = true;
			ethanolInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			ethanolInput.AssignPort(ethanolLiquidInputPort);

			// Biodiesel production uses 70% Vegetable Oil and 30% Ethanol
			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(ModElements.VegetableOil_Liquid.Tag, 3.5f* multiplier),
				new ElementConverter.ConsumedElement(SimHashes.Ethanol.CreateTag(), 1.5f* multiplier) ];
			converter.outputElements = [
				new ElementConverter.OutputElement(4.7f* multiplier,ModElements.BioDiesel_Liquid, 325.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.3f* multiplier, SimHashes.DirtyWater, 315.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];
			//-------------------------------------------------------------------

			ConduitDispenser biodieselDispenser = go.AddOrGet<ConduitDispenser>();
			biodieselDispenser.conduitType = ConduitType.Liquid;
			biodieselDispenser.alwaysDispense = true;
			biodieselDispenser.elementFilter = [ModElements.BioDiesel_Liquid];

			PipedConduitDispenser pollutedWaterDispenser = go.AddComponent<PipedConduitDispenser>();
			pollutedWaterDispenser.conduitType = ConduitType.Liquid;
			pollutedWaterDispenser.alwaysDispense = true;
			pollutedWaterDispenser.elementFilter = [SimHashes.DirtyWater];
			pollutedWaterDispenser.AssignPort(pollutedWaterLiquidOutputPort);

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, ethanolLiquidInputPort);
			controller.AssignPort(go, pollutedWaterLiquidOutputPort);
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
