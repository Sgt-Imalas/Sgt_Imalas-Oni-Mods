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
	public class Machinery_CoralVatConfig : IBuildingConfig
	{
		public static string ID = "CoralVat";
		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = [200f, 300f];
			string[] textArray1 = [GameTags.RefinedMetal.ToString(), GameTags.Filter.ToString()];

			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "bio_coral_vat_kanim", 30, 90f, singleArray1, textArray1, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.Floodable = true;
			def1.ViewMode = OverlayModes.Oxygen.ID;
			def1.AudioCategory = "HollowMetal";
			def1.EnergyConsumptionWhenActive = 360f;
			def1.SelfHeatKilowattsWhenActive = 2f;
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.Overheatable = true;
			def1.OverheatTemperature = 348.15f;
			def1.InputConduitType = ConduitType.Liquid;
			def1.OutputConduitType = ConduitType.Liquid;
			def1.UtilityInputOffset = new CellOffset(-1, 0);
			def1.UtilityOutputOffset = new CellOffset(1, 0);
			def1.PowerInputOffset = new CellOffset(0, 0);
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
			return def1;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Tag tag = SimHashes.Sand.CreateTag();
			go.AddOrGet<Desalinator>();

			Storage local1 = go.AddOrGet<Storage>();
			local1.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			local1.showInUI = true;

			ElementConverter converter = go.AddComponent<ElementConverter>();
			converter.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.SaltWater.CreateTag(), 2f), new ElementConverter.ConsumedElement(tag, 0.05f)];
			converter.outputElements = [new ElementConverter.OutputElement(1.86f, SimHashes.Water, 0f, false, true, 0f, 0.5f, 0.75f, 0xff, 0), new ElementConverter.OutputElement(0.0224f, SimHashes.BleachStone, 0f, false, true, 0f, 0.5f, 0.25f, 0xff, 0)];

			ElementConverter converter2 = go.AddComponent<ElementConverter>();
			converter2.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.Brine.CreateTag(), 2f), new ElementConverter.ConsumedElement(tag, 0.05f)];
			converter2.outputElements = [new ElementConverter.OutputElement(1.4f, SimHashes.Water, 313.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0), new ElementConverter.OutputElement(0.096f, SimHashes.BleachStone, 313.15f, false, true, 0f, 0.5f, 0.25f, 0xff, 0)];

			ElementConverter converter3 = go.AddComponent<ElementConverter>();
			converter3.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.ChlorineGas.CreateTag(), 0.05f), new ElementConverter.ConsumedElement(tag, 0.1f)];
			converter3.outputElements = [new ElementConverter.OutputElement(0.008f, SimHashes.BleachStone, 303.15f, false, true, 0f, 1f, 1f, 0xff, 0)];

			ElementDropper local2 = go.AddComponent<ElementDropper>();
			local2.emitMass = 1f;
			local2.emitTag = SimHashes.BleachStone.CreateTag();
			local2.emitOffset = new Vector3(0f, 1f, 0f);

			ManualDeliveryKG local3 = go.AddComponent<ManualDeliveryKG>();
			local3.SetStorage(local1);
			local3.RequestedItemTag = tag;
			local3.capacity = 500f;
			local3.refillMass = 100f;
			local3.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer local4 = go.AddOrGet<ConduitConsumer>();
			local4.conduitType = ConduitType.Liquid;
			local4.consumptionRate = 10f;
			local4.capacityKG = 20f;
			local4.capacityTag = GameTags.AnyWater;
			local4.forceAlwaysSatisfied = true;
			local4.wrongElementResult = ConduitConsumer.WrongElementResult.Store;

			ConduitDispenser local5 = go.AddOrGet<ConduitDispenser>();
			local5.conduitType = ConduitType.Liquid;
			local5.invertElementFilter = true;
			local5.elementFilter = [SimHashes.SaltWater, SimHashes.Brine];
			Prioritizable.AddRef(go);

			ElementConsumer local6 = go.AddOrGet<ElementConsumer>();
			local6.elementToConsume = SimHashes.ChlorineGas;
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
