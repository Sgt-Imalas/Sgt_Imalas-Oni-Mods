using Imalas_TwitchChaosEvents.Elements;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Buildings
{
	class InvertedWaterPurifierConfig : IBuildingConfig
	{
		public const string ID = "ITCE_WaterPurifierInverted";
		private const float FILTER_INPUT_RATE = 1f;
		private const float DIRTY_WATER_INPUT_RATE = 5f;
		private const float FILTER_CAPACITY = 1200f;
		private const float USED_FILTER_OUTPUT_RATE = 0.2f;
		private const float CLEAN_WATER_OUTPUT_RATE = 5f;
		private const float TARGET_OUTPUT_TEMPERATURE = 313.15f;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR3_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
			string[] allMetals = TUNING.MATERIALS.ALL_METALS;
			EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
			EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
			EffectorValues noise = tieR3_2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 3, "waterpurifier_inverted_kanim", 100, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnCeiling, tieR2, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 0.0f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(2, 2));
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.PowerInputOffset = new CellOffset(-1, 2);
			buildingDef.UtilityInputOffset = new CellOffset(2, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(-1, 0);
			buildingDef.PermittedRotations = PermittedRotations.FlipH;
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.FILTER);
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.WATER);
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
			defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			go.AddOrGet<WaterPurifier>();
			Prioritizable.AddRef(go);
			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
			{
				new ElementConverter.ConsumedElement(ModElements.InverseWater.Tag, 5f)
			};
			elementConverter.outputElements = new ElementConverter.OutputElement[]
			{
				new ElementConverter.OutputElement(5f, SimHashes.Water, 0.0f, storeOutput: true, diseaseWeight: 0.75f),
				new (0.005f, SimHashes.Unobtanium, 343.15f,  storeOutput: true)
			};
			ElementDropper elementDropper2 = go.AddComponent<ElementDropper>();
			elementDropper2.emitMass = 0.5f;
			elementDropper2.emitTag = SimHashes.Unobtanium.CreateTag();
			elementDropper2.emitOffset = new Vector3(0.0f, 1f, 0.0f);

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityKG = 20f;
			conduitConsumer.capacityTag = GameTags.AnyWater;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.invertElementFilter = true;
			conduitDispenser.elementFilter = new SimHashes[1]
			{
				ModElements.InverseWater.SimHash
			};
			var flip = go.AddOrGet<InvertedBuilding>();
			flip.yOffset = 3;
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			var flip = go.AddOrGet<InvertedBuilding>();
			flip.yOffset = 3;
		}
		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			var flip = go.AddOrGet<InvertedBuilding>();
			flip.yOffset = 3;
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
		}
	}
}
