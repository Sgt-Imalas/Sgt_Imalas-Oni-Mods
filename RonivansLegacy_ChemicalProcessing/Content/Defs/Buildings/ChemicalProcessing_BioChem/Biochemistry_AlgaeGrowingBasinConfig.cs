using HarmonyLib;
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


namespace Biochemistry.Buildings
{
	//===[ ALGAE GROWING BASIN CONFIG ]===================================================================================
	public class Biochemistry_AlgaeGrowingBasinConfig : IBuildingConfig
	{
		public static string ID = "Biochemistry_AlgaeGrowingBasin";

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 9, 3, "algaegrower_basin_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = true;
			buildingDef.OverheatTemperature = 313.15f;
			buildingDef.Floodable = true;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 90f;
			buildingDef.ExhaustKilowattsWhenActive = 0.01f;
			buildingDef.SelfHeatKilowattsWhenActive = 1f;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			SoundEventVolumeCache.instance.AddVolume("algaegrower_basin_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("algaegrower_basin_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("algaegrower_basin_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
			SoundUtils.CopySoundsToAnim("algaegrower_basin_kanim", "algaefarm_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<ElementConversionBuilding>();

			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			storage.capacityKg = 1000f;
			storage.showInUI = true;

			Storage storage2 = go.AddOrGet<Storage>();
			storage2.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			storage2.showInUI = true;
			storage2.capacityKg = 1000f;
			storage2.allowItemRemoval = false;
			storage2.storageFilters = [ModElements.VegetableOil_Liquid.Tag];

			ConduitConsumer vegOilInput = go.AddOrGet<ConduitConsumer>();
			vegOilInput.conduitType = ConduitType.Liquid;
			vegOilInput.consumptionRate = 10f;
			vegOilInput.capacityKG = 100f;
			vegOilInput.capacityTag = ModElements.VegetableOil_Liquid.Tag;
			vegOilInput.forceAlwaysSatisfied = true;
			vegOilInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ManualDeliveryKG dirt_delivery = go.AddOrGet<ManualDeliveryKG>();
			dirt_delivery.RequestedItemTag = GameTags.Dirt;
			dirt_delivery.SetStorage(storage);
			dirt_delivery.capacity = 100f;
			dirt_delivery.refillMass = 20f;
			dirt_delivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(ModElements.VegetableOil_Liquid.Tag, 0.1f),
				new ElementConverter.ConsumedElement(SimHashes.Dirt.CreateTag(), 0.4f) ];
			converter.outputElements = [
				new ElementConverter.OutputElement(0.5f, SimHashes.Algae, 296.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];
			//-------------------------------------------------------------------

			ElementDropper elementDropper = go.AddComponent<ElementDropper>();
			elementDropper.emitMass = 25f;
			elementDropper.emitTag = new Tag(SimHashes.Algae.CreateTag());
			elementDropper.emitOffset = new Vector3(0f, 1f, 0f);

			go.AddOrGet<DropAllWorkable>();
			Prioritizable.AddRef(go);

		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			AddVisualizer(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			AddVisualizer(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.AddOrGet<LightEfficiencyConverter>();
			AddVisualizer(go);
		}
		private static void AddVisualizer(GameObject go1)
		{
			RangeVisualizer rangeVisualizer = go1.AddOrGet<RangeVisualizer>();
			rangeVisualizer.RangeMin.x = -4;
			rangeVisualizer.RangeMin.y = 1;
			rangeVisualizer.RangeMax.x = 4;
			rangeVisualizer.RangeMax.y = 1;
			rangeVisualizer.TestLineOfSight = false;
		}
	}
}
