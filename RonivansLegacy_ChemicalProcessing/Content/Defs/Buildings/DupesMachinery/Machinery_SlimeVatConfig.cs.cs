using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;


namespace Dupes_Machinery.Biological_Vats
{
	public class Machinery_SlimeVatConfig : IBuildingConfig
	{
		public static string ID = "SlimeVat";
		// 5 cycles per mush bar; 0.8kg per gram
		static float MushbarConsumption = 1 / (10f * 600f);

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Tag EDIBLES_TAG = MushBarConfig.ID;
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			Storage local1 = go.AddOrGet<Storage>();
			local1.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			local1.showInUI = true;
			local1.allowItemRemoval = false;
			//local1.capacityKg = 1000f;

			ElementConverter mainRecipe = go.AddComponent<ElementConverter>();
			mainRecipe.consumedElements =
				[
				new ElementConverter.ConsumedElement(EDIBLES_TAG, MushbarConsumption),
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 0.05f)];
			mainRecipe.outputElements = [
				new ElementConverter.OutputElement(0.05f, SimHashes.SlimeMold, 0f, false, true, 0f, 0.5f, 1f, Db.Get().Diseases.GetIndex("SlimeLung"), 100)];

			ElementConverter optionalRecipe = go.AddComponent<ElementConverter>();
			optionalRecipe.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.ContaminatedOxygen.CreateTag(), 0.025f),
				new ElementConverter.ConsumedElement(EDIBLES_TAG, MushbarConsumption),
				new ElementConverter.ConsumedElement(SimHashes.Dirt.CreateTag(), 0.05f)];
			optionalRecipe.outputElements = [
				new ElementConverter.OutputElement(0.075f, SimHashes.SlimeMold, 0f, false, true, 0f, 0.5f, 1f, Db.Get().Diseases.GetIndex("SlimeLung"), 150)];

			ElementDropper local2 = go.AddComponent<ElementDropper>();
			local2.emitMass = 5f;
			local2.emitTag = SimHashes.SlimeMold.CreateTag();
			local2.emitOffset = new Vector3(-1f, 1f, 0f);

			ManualDeliveryKG mushBarDelivery = go.AddComponent<ManualDeliveryKG>();
			mushBarDelivery.SetStorage(local1);
			mushBarDelivery.RequestedItemTag = EDIBLES_TAG;
			mushBarDelivery.capacity = 4f;
			mushBarDelivery.refillMass = 1f;
			mushBarDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ManualDeliveryKG dirtDelivery = go.AddComponent<ManualDeliveryKG>();
			dirtDelivery.SetStorage(local1);
			dirtDelivery.RequestedItemTag = SimHashes.Dirt.CreateTag();
			dirtDelivery.capacity = 120f;
			dirtDelivery.refillMass = 30f;
			dirtDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer waterConsumer = go.AddOrGet<ConduitConsumer>();
			waterConsumer.conduitType = ConduitType.Liquid;
			waterConsumer.consumptionRate = 10f;
			waterConsumer.capacityKG = 20f;
			waterConsumer.capacityTag = SimHashes.Water.CreateTag();
			waterConsumer.forceAlwaysSatisfied = true;
			waterConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ElementConsumer pOxConsumer = go.AddOrGet<ElementConsumer>();
			pOxConsumer.elementToConsume = SimHashes.ContaminatedOxygen;
			pOxConsumer.consumptionRate = 0.05f;
			pOxConsumer.consumptionRadius = 6;
			pOxConsumer.storeOnConsume = true;
			pOxConsumer.capacityKG = 10f;
			pOxConsumer.showInStatusPanel = true;
			pOxConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
			pOxConsumer.isRequired = false;

			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			go.AddOrGet<AnimTileable>();
			go.AddOrGet<ElementConversionBuilding>().UsePrimaryConverterOnly = true; //Handles element converter

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
			SoundEventVolumeCache.instance.AddVolume("bio_slime_vat_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("bio_slime_vat_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("bio_slime_vat_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
			SoundUtils.CopySoundsToAnim("bio_slime_vat_kanim", "algae_distillery_kanim");
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
