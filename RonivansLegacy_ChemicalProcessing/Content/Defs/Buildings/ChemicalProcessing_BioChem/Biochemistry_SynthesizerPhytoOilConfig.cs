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
using static UtilLibs.UIcmp.FSlider;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.ChemicalProcessing_BioChem
{
	internal class Biochemistry_SynthesizerPhytoOilConfig : IBuildingConfig
	{

		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Biochemistry_SynthesizerPhytoOil";

		private static readonly PortDisplayInput veggiOilInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(0, 3), color: ModElements.VEGEOIL_COLOR);
		private static readonly PortDisplayInput waterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(0, 2), color: new Color?((Color)new Color32((byte)66, (byte)135, (byte)245, byte.MaxValue)));

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 4, "mixer_phytooil_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 90f;
			buildingDef.ExhaustKilowattsWhenActive = 4f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			SoundUtils.CopySoundsToAnim("mixer_phytooil_kanim", "waterpurifier_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.capacityKg = 300f;
			//storage.showCapacityStatusItem = true;
			//storage.showCapacityAsMainStatus = true;
			//storage.showDescriptor = true;
			go.AddOrGet<ElementConversionBuilding>();
			Prioritizable.AddRef(go);

			PortConduitConsumer waterInput = go.AddOrGet<PortConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 10f;
			waterInput.capacityKG = 50f;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			waterInput.AssignPort(waterInputPort);

			PortConduitConsumer bioOilPort = go.AddComponent<PortConduitConsumer>();
			bioOilPort.conduitType = ConduitType.Liquid;
			bioOilPort.consumptionRate = 10f;
			bioOilPort.capacityKG = 10f;
			bioOilPort.capacityTag = ModElements.VegetableOil_Liquid.Tag;
			bioOilPort.forceAlwaysSatisfied = true;
			bioOilPort.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			bioOilPort.AssignPort(veggiOilInputPort);

			float rate = 1f;
			float oilAmount = rate * ModElements.VeggiOilToWaterRatio;
			float waterAmount = rate - oilAmount;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new(SimHashes.Water.CreateTag(), waterAmount),
				new( ModElements.VegetableOil_Liquid.Tag, oilAmount) ];
			converter.outputElements = [
				new(1f, SimHashes.PhytoOil, UtilMethods.GetKelvinFromC(10), false, true),
				 ];
			//-------------------------------------------------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.alwaysDispense = true;
			dispenser.storage = storage;
			dispenser.elementFilter = [SimHashes.PhytoOil];

			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, waterInputPort);
			controller.AssignPort(go, veggiOilInputPort);
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
