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

		private static readonly List<Storage.StoredItemModifier> BioRefineryStoredItemModifiers;

		private static readonly PortDisplayInput ethanolLiquidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-3, 3), null, new Color32(185, 239, 185, 255));
		private static readonly PortDisplayOutput pollutedWaterLiquidOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(3, 1), null, new Color32(137, 137, 66, 255));

		static Biochemistry_BiodieselRefineryConfig()
		{

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Preserve);
			list1.Add(Storage.StoredItemModifier.Insulate);
			list1.Add(Storage.StoredItemModifier.Seal);
			BioRefineryStoredItemModifiers = list1;
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 5, "biodiesel_refinery_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 480f;
			buildingDef.ExhaustKilowattsWhenActive = 2f;
			buildingDef.SelfHeatKilowattsWhenActive = 8f;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(-3, 1);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(3, 3);
			SoundUtils.CopySoundsToAnim("biodiesel_generator_kanim", "oilrefinery_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(BioRefineryStoredItemModifiers);
			storage.capacityKg = 300f;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.showDescriptor = true;

			ConduitConsumer vegOilInput = go.AddOrGet<ConduitConsumer>();
			vegOilInput.conduitType = ConduitType.Liquid;
			vegOilInput.consumptionRate = 10f;
			vegOilInput.capacityKG = 50f;
			vegOilInput.capacityTag = ModAssets.Tags.BioOil_Composition;
			vegOilInput.forceAlwaysSatisfied = true;
			vegOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer ethanolInput = go.AddComponent<PortConduitConsumer>();
			ethanolInput.conduitType = ConduitType.Liquid;
			ethanolInput.consumptionRate = 10f;
			ethanolInput.capacityKG = 50f;
			ethanolInput.capacityTag = SimHashes.Ethanol.CreateTag();
			ethanolInput.forceAlwaysSatisfied = true;
			ethanolInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			ethanolInput.AssignPort(ethanolLiquidInputPort);

			// Biodiesel production uses 70% Vegetable Oil and 30% Ethanol
			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(ModAssets.Tags.BioOil_Composition, 3.5f),
				new ElementConverter.ConsumedElement(SimHashes.Ethanol.CreateTag(), 1.5f) ];
			converter.outputElements = [
				new ElementConverter.OutputElement(4.7f,ModElements.BioDiesel_Liquid, 325.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0),
				new ElementConverter.OutputElement(0.3f, SimHashes.DirtyWater, 315.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];
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
