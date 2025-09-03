using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.NuclearProcessing
{
	public class HepCentrifugeConfig : IBuildingConfig
	{
		public static string ID = "HepCentrifuge";
		public static readonly List<Storage.StoredItemModifier> CentrifugeStorageModifiers = new()
		{
		  Storage.StoredItemModifier.Hide,
		  Storage.StoredItemModifier.Seal,
		  Storage.StoredItemModifier.Insulate
		};
		public override string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [400f, 200f];
			string[] construction_materials =
			[
				"RefinedMetal",
				"Plastic"
			];
			EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 6, "hep_centrifuge_kanim", 100, 90f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tieR5);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 800f;
			buildingDef.BaseMeltingPoint = 2400f;
			buildingDef.ExhaustKilowattsWhenActive = 1f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			SoundUtils.CopySoundsToAnim("hep_centrifuge_kanim", "enrichmentCentrifuge_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(HepCentrifugeConfig.CentrifugeStorageModifiers);
			storage.showInUI = true;
			storage.allowItemRemoval = false;
			storage.capacityKg = 1000f;
			go.AddOrGet<ElementConversionBuilding>();

			ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
			manualDeliveryKg.SetStorage(storage);
			manualDeliveryKg.RequestedItemTag = SimHashes.Yellowcake.CreateTag();
			manualDeliveryKg.capacity = 500f;
			manualDeliveryKg.refillMass = 100f;
			manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = [new(SimHashes.Yellowcake.CreateTag(), 0.05f)];
			elementConverter.outputElements = 
			[
				new(0.035f, SimHashes.EnrichedUranium, 0.0f, storeOutput: true, diseaseWeight: 0.75f),
				new(0.015f, SimHashes.DepletedUranium, 0.0f, storeOutput: true, diseaseWeight: 0.25f)
			];


			ElementDropper elementDropper = go.AddComponent<ElementDropper>();
			elementDropper.emitMass = 5f;
			elementDropper.emitTag = SimHashes.EnrichedUranium.CreateTag();
			elementDropper.emitOffset = new Vector3(-1f, 1f);

			ElementDropper elementDropper2 = go.AddComponent<ElementDropper>();
			elementDropper2.emitMass = 5f;
			elementDropper2.emitTag = SimHashes.DepletedUranium.CreateTag();
			elementDropper2.emitOffset = new Vector3(-1f, 1f);
		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
		}
	}
}
