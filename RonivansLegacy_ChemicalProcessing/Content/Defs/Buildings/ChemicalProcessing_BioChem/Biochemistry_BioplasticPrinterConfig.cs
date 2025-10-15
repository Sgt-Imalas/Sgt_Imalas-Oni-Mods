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
	//===[ BIOPLASTIC PRINTER CONFIG ]===================================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Biochemistry_BioplasticPrinterConfig : IBuildingConfig
	{
		public static string ID = "Biochemistry_BioplasticPrinter";

		private static readonly PortDisplayInput co2GasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(1, 0), null, new Color32(186, 186, 186, 255));

		private static readonly PortDisplayOutput pWaterOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(2, 1));

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 4, "bioplastic_printer_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 240f;
			buildingDef.ExhaustKilowattsWhenActive = 2f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(2, 0);
			SoundUtils.CopySoundsToAnim("bioplastic_printer_kanim", "plasticrefinery_kanim");
			return buildingDef;
		}

		static float MushbarConsumption = 1 / (5f * 600f);
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Polymerizer polymerizer = go.AddOrGet<Polymerizer>();
			polymerizer.emitMass = 30f;
			polymerizer.emitTag = GameTagExtensions.Create(ModElements.BioPlastic_Solid);
			polymerizer.emitOffset = new Vector3(0f, 1f, 0f);

			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			storage.capacityKg = 300f;
			//storage.showCapacityStatusItem = true;
			//storage.showCapacityAsMainStatus = true;
			//storage.showDescriptor = true;

			Tag oil = SimHashes.PhytoOil.CreateTag();

			ConduitConsumer vegOilInput = go.AddOrGet<ConduitConsumer>();
			vegOilInput.conduitType = ConduitType.Liquid;
			vegOilInput.consumptionRate = 10f;
			vegOilInput.capacityKG = 50f;
			vegOilInput.capacityTag = oil;
			vegOilInput.forceAlwaysSatisfied = true;
			vegOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer ethanolInput = go.AddComponent<PortConduitConsumer>();
			ethanolInput.conduitType = ConduitType.Gas;
			ethanolInput.consumptionRate = 10f;
			ethanolInput.capacityKG = 50f;
			ethanolInput.capacityTag = SimHashes.CarbonDioxide.CreateTag();
			ethanolInput.forceAlwaysSatisfied = true;
			ethanolInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			ethanolInput.AssignPort(co2GasInputPort);


			PipedConduitDispenser pWaterDispenser = go.AddComponent<PipedConduitDispenser>();
			pWaterDispenser.elementFilter = [SimHashes.DirtyWater];
			pWaterDispenser.AssignPort(pWaterOutputPort);
			pWaterDispenser.alwaysDispense = true;
			//pWaterDispenser.SkipSetOperational = true;


			ManualDeliveryKG mushbar_delivery = go.AddOrGet<ManualDeliveryKG>();
			mushbar_delivery.RequestedItemTag = MushBarConfig.ID.ToTag();
			mushbar_delivery.SetStorage(storage);
			mushbar_delivery.capacity = 4f;
			mushbar_delivery.refillMass = 1f;
			mushbar_delivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements =
			[
			new ElementConverter.ConsumedElement(oil, 0.90f, true),
			new ElementConverter.ConsumedElement(SimHashes.CarbonDioxide.CreateTag(), 0.10f, true),
			new ElementConverter.ConsumedElement(MushBarConfig.ID.ToTag(), MushbarConsumption, true)
			];
			elementConverter.outputElements =
			[
			new ElementConverter.OutputElement(0.50f, ModElements.BioPlastic_Solid, 296.15f, false, true, 0f, 0.5f)
			new ElementConverter.OutputElement(0.40f, SimHashes.DirtyWater, UtilMethods.GetKelvinFromC(10), true, true, 0f, 0.5f)
			];

			ElementDropper elementDropper = go.AddComponent<ElementDropper>();
			elementDropper.emitMass = 25f;
			elementDropper.emitTag = ModElements.BioPlastic_Solid.Tag;
			elementDropper.emitOffset = new Vector3(0f, 1f, 0f);

			go.AddOrGet<DropAllWorkable>();
			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, co2GasInputPort);
			controller.AssignPort(go, pWaterOutputPort);
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
