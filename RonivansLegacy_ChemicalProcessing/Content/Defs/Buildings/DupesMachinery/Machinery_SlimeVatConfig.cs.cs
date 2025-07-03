using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;


namespace Dupes_Machinery.Biological_Vats
{
	public class Machinery_SlimeVatConfig : IBuildingConfig
	{
		public static string ID = "SlimeVat";
		public static readonly Tag EDIBLES_TAG = MushBarConfig.ID;

		public static readonly List<Storage.StoredItemModifier> SlimeVatStorageModifiers;
		static Machinery_SlimeVatConfig()
		{
			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			list1.Add(Storage.StoredItemModifier.Preserve);
			SlimeVatStorageModifiers = list1;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Tag tag = SimHashes.Dirt.CreateTag();
			go.AddOrGet<Desalinator>();

			Storage local1 = go.AddOrGet<Storage>();
			local1.SetDefaultStoredItemModifiers(SlimeVatStorageModifiers);
			local1.showInUI = true;
			local1.allowItemRemoval = false;
			local1.capacityKg = 1000f;

			ElementConverter converter = go.AddComponent<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(EDIBLES_TAG, 0.01f), new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 0.05f)];
			converter.outputElements = [new ElementConverter.OutputElement(0.06f, SimHashes.SlimeMold, 0f, false, true, 0f, 0.5f, 0.75f, Db.Get().Diseases.GetIndex("SlimeLung"), 100)];

			ElementConverter converter3 = go.AddComponent<ElementConverter>();
			converter3.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.ContaminatedOxygen.CreateTag(), 0.025f), new ElementConverter.ConsumedElement(EDIBLES_TAG, 0.005f), new ElementConverter.ConsumedElement(tag, 0.05f)];
			converter3.outputElements = [new ElementConverter.OutputElement(0.05f, SimHashes.SlimeMold, 0f, false, true, 0f, 0.5f, 0.75f, Db.Get().Diseases.GetIndex("SlimeLung"), 100)];

			ElementDropper local2 = go.AddComponent<ElementDropper>();
			local2.emitMass = 5f;
			local2.emitTag = SimHashes.SlimeMold.CreateTag();
			local2.emitOffset = new Vector3(-1f, 1f, 0f);

			ManualDeliveryKG ingredient1 = go.AddComponent<ManualDeliveryKG>();
			ingredient1.SetStorage(local1);
			ingredient1.RequestedItemTag = EDIBLES_TAG;
			ingredient1.capacity = 10f;
			ingredient1.refillMass = 2f;
			ingredient1.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer local4 = go.AddOrGet<ConduitConsumer>();
			local4.conduitType = ConduitType.Liquid;
			local4.consumptionRate = 10f;
			local4.capacityKG = 20f;
			local4.capacityTag = SimHashes.Water.CreateTag();
			local4.forceAlwaysSatisfied = true;
			local4.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ElementConsumer local6 = go.AddOrGet<ElementConsumer>();
			local6.elementToConsume = SimHashes.ContaminatedOxygen;
			local6.consumptionRate = 0.05f;
			local6.consumptionRadius = 6;
			local6.storeOnConsume = true;
			local6.capacityKG = 10f;
			local6.showInStatusPanel = true;
			local6.sampleCellOffset = new Vector3(0f, 1f, 0f);
			local6.isRequired = false;

			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			go.AddOrGet<AnimTileable>();

			Prioritizable.AddRef(go);
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] material_required = [800f, 400f];
			string[] material_type = ["RefinedMetal", "Farmable"];

			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "bio_slime_vat_kanim", 30, 90f, material_required, material_type, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.Floodable = true;
			def1.ViewMode = OverlayModes.Oxygen.ID;
			def1.AudioCategory = "HollowMetal";
			def1.EnergyConsumptionWhenActive = 360f;
			def1.SelfHeatKilowattsWhenActive = 2f;
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.Overheatable = true;
			def1.OverheatTemperature = 328.15f;
			def1.InputConduitType = ConduitType.Liquid;
			def1.UtilityInputOffset = new CellOffset(0, 0);
			def1.PowerInputOffset = new CellOffset(0, 0);
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
		}
	}
}
