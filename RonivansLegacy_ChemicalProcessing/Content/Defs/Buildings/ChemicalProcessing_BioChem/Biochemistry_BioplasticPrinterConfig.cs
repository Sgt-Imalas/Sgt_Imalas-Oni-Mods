using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
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


		private static readonly List<Storage.StoredItemModifier> BioRefineryStoredItemModifiers = [
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Preserve,
				Storage.StoredItemModifier.Insulate,
				Storage.StoredItemModifier.Seal,
			];

		private static readonly PortDisplayInput co2GasInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(1, 0), null, new Color32(110, 110, 110, 255));


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

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Polymerizer polymerizer = go.AddOrGet<Polymerizer>();
			polymerizer.emitMass = 30f;
			polymerizer.emitTag = GameTagExtensions.Create(ModElements.BioPlastic_Solid);
			polymerizer.emitOffset = new Vector3(0f, 1f, 0f);

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
			ethanolInput.conduitType = ConduitType.Gas;
			ethanolInput.consumptionRate = 10f;
			ethanolInput.capacityKG = 50f;
			ethanolInput.capacityTag = SimHashes.CarbonDioxide.CreateTag();
			ethanolInput.forceAlwaysSatisfied = true;
			ethanolInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			ethanolInput.AssignPort(co2GasInputPort);

			ManualDeliveryKG mushbar_delivery = go.AddOrGet<ManualDeliveryKG>();
			mushbar_delivery.RequestedItemTag = MushBarConfig.ID.ToTag();
			mushbar_delivery.SetStorage(storage);
			mushbar_delivery.capacity = 2f;
			mushbar_delivery.refillMass = 1f;
			mushbar_delivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements =
			[
			new ElementConverter.ConsumedElement(ModAssets.Tags.BioOil_Composition, 0.35f, true),
			new ElementConverter.ConsumedElement(SimHashes.CarbonDioxide.CreateTag(), 0.10f, true),
			new ElementConverter.ConsumedElement(MushBarConfig.ID.ToTag(), 0.002f, true)
			];
			elementConverter.outputElements =
			[
			new ElementConverter.OutputElement(0.5f, ModElements.BioPlastic_Solid, 296.15f, false, true, 0f, 0.5f, 1f, byte.MaxValue, 0, true)
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
