using HarmonyLib;
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
			def1.ViewMode = OverlayModes.None.ID;
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
			SoundEventVolumeCache.instance.AddVolume("bio_coral_vat_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("bio_coral_vat_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("bio_coral_vat_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
			SoundUtils.CopySoundsToAnim("bio_coral_vat_kanim", "algaefarm_kanim");
			return def1;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Tag coralNutrients = SimHashes.Sand.CreateTag();
			float chlorineConversionRate = 0.060f;


			Storage local1 = go.AddOrGet<Storage>();
			local1.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			local1.showInUI = true;

			// taken from Bleachstone Hopper
			float saltToBleachstoneRation = 1f / 3f;
			//7% salt -> 140g salt extracted per 2kg saltwater

			ElementConverter saltWaterConverter = go.AddComponent<ElementConverter>();
			saltWaterConverter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.SaltWater.CreateTag(), 2f)
				, new ElementConverter.ConsumedElement(coralNutrients, 0.05f)
				];

			saltWaterConverter.outputElements = [
				new ElementConverter.OutputElement(1.86f, SimHashes.Water, UtilMethods.GetKelvinFromC(5), false, true),
				new ElementConverter.OutputElement(0.140f * saltToBleachstoneRation, SimHashes.BleachStone, UtilMethods.GetKelvinFromC(5), false, true)
				];

			//30% brine -> 600g salt extracted per 2kg brine

			ElementConverter brineConverter = go.AddComponent<ElementConverter>();
			brineConverter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Brine.CreateTag(), 2f),
				new ElementConverter.ConsumedElement(coralNutrients, 0.05f)
				];
			brineConverter.outputElements = [
				new ElementConverter.OutputElement(1.4f, SimHashes.Water, UtilMethods.GetKelvinFromC(5), false, true),
				new ElementConverter.OutputElement(0.600f * saltToBleachstoneRation, SimHashes.BleachStone, UtilMethods.GetKelvinFromC(5), false, true)];

			ElementConverter chlorineGasCondenser = go.AddComponent<ElementConverter>();
			chlorineGasCondenser.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.ChlorineGas.CreateTag(), chlorineConversionRate)
				, new ElementConverter.ConsumedElement(coralNutrients, 0.1f)
				];
			chlorineGasCondenser.outputElements = [
				new ElementConverter.OutputElement(chlorineConversionRate, SimHashes.BleachStone, UtilMethods.GetKelvinFromC(5), false, true)];

			ElementDropper bleachstoneDropper = go.AddComponent<ElementDropper>();
			bleachstoneDropper.emitMass = 10f;
			bleachstoneDropper.emitTag = SimHashes.BleachStone.CreateTag();
			bleachstoneDropper.emitOffset = new Vector3(0f, 1f, 0f);

			ManualDeliveryKG sandDelivery = go.AddComponent<ManualDeliveryKG>();
			sandDelivery.SetStorage(local1);
			sandDelivery.RequestedItemTag = coralNutrients;
			sandDelivery.capacity = 500f;
			sandDelivery.refillMass = 100f;
			sandDelivery.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

			ConduitConsumer waterConsumer = go.AddOrGet<ConduitConsumer>();
			waterConsumer.conduitType = ConduitType.Liquid;
			waterConsumer.consumptionRate = 10f;
			waterConsumer.capacityKG = 20f;
			waterConsumer.capacityTag = GameTags.AnyWater;
			waterConsumer.forceAlwaysSatisfied = true;
			waterConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;

			ConduitDispenser OutputDispenser = go.AddOrGet<ConduitDispenser>();
			OutputDispenser.conduitType = ConduitType.Liquid;
			OutputDispenser.invertElementFilter = true;
			OutputDispenser.elementFilter = [SimHashes.SaltWater, SimHashes.Brine];
			Prioritizable.AddRef(go);

			ElementConsumer worldChlorineConsumer = go.AddOrGet<ElementConsumer>();
			worldChlorineConsumer.elementToConsume = SimHashes.ChlorineGas;
			worldChlorineConsumer.consumptionRate = chlorineConversionRate;
			worldChlorineConsumer.consumptionRadius = 6;
			worldChlorineConsumer.storeOnConsume = true;
			worldChlorineConsumer.capacityKG = 10f;
			worldChlorineConsumer.showInStatusPanel = true;
			worldChlorineConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
			worldChlorineConsumer.isRequired = false;

			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			go.AddOrGet<AnimTileable>();
			var animHandler = go.AddOrGet<ElementConversionBuilding>();
			animHandler.ConvertersToIgnore = [2]; //Handles element converter
			animHandler.ShowWorkingStatus = true;

			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
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
