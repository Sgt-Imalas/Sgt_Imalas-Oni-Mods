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
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_AmmoniaBreakerConfig : IBuildingConfig
	{
		public static string ID = "Chemical_AmmoniaBreaker";
		public static readonly List<Storage.StoredItemModifier> AmmoniaBreakerItemModifiers = new List<Storage.StoredItemModifier>()
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate
			};
		private static readonly PortDisplayOutput NitrogenGasOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 2), color: new Color?((Color)new Color32((byte)205, (byte)194, byte.MaxValue, byte.MaxValue)));



		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [200f, 100f];
			string[] construction_materials =
			[
				"BuildableRaw",
				"RefinedMetal"
			];
			EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Chemical_AmmoniaBreaker", 3, 3, "ammonia_breaker_kanim", 100, 30f, construction_mass, construction_materials, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tieR6);
			buildingDef.Overheatable = true;
			buildingDef.OverheatTemperature = 348.15f;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 840f;
			buildingDef.ExhaustKilowattsWhenActive = 24f;
			buildingDef.SelfHeatKilowattsWhenActive = 12f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(0, 2);
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 2);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			go.AddOrGet<ElementConversionBuilding>();


			Tag tag = SimHashes.Iron.CreateTag();
			Storage storage = BuildingTemplates.CreateDefaultStorage(go);
			storage.SetDefaultStoredItemModifiers(AmmoniaBreakerItemModifiers);

			ElementConverter elementConverter = go.AddComponent<ElementConverter>();
			elementConverter.consumedElements =
			[
				new(ModElements.Ammonia_Gas.Tag, 1f),
				new(SimHashes.Iron.CreateTag(), 0.01f)
			];
			elementConverter.outputElements =
			[
				new(0.75f, SimHashes.Hydrogen, 371.15f, storeOutput: true, diseaseWeight: 0.75f),
				new(0.25f, ModElements.Nitrogen_Gas, 371.15f, storeOutput: true, diseaseWeight: 0.25f),
				new(0.01f, SimHashes.Rust, 307.15f, storeOutput: true, diseaseWeight: 0.01f)
			];

			ElementDropper elementDropper = go.AddComponent<ElementDropper>();
			elementDropper.emitMass = 10f;
			elementDropper.emitTag = SimHashes.Rust.CreateTag();
			elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);

			ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
			manualDeliveryKg.SetStorage(storage);
			manualDeliveryKg.RequestedItemTag = tag;
			manualDeliveryKg.capacity = 300f;
			manualDeliveryKg.refillMass = 50f;
			manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityKG = 30f;
			conduitConsumer.capacityTag = ModElements.Ammonia_Gas.Tag;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			storage.showInUI = true;

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Gas;
			conduitDispenser.storage = storage;
			conduitDispenser.elementFilter = [SimHashes.Hydrogen];

			PipedConduitDispenser pipedDispenser = go.AddComponent<PipedConduitDispenser>();
			pipedDispenser.storage = storage;
			pipedDispenser.conduitType = ConduitType.Gas;
			pipedDispenser.alwaysDispense = true;
			pipedDispenser.elementFilter = [ModElements.Nitrogen_Gas];
			pipedDispenser.AssignPort(NitrogenGasOutputPort);

			Prioritizable.AddRef(go);
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController displayController = go.AddComponent<PortDisplayController>();
			displayController.Init(go);
			displayController.AssignPort(go, NitrogenGasOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			this.AttachPort(go);
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
