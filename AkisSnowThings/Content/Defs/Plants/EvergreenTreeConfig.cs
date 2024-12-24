using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static AkisSnowThings.STRINGS.CREATURES.SPECIES;
using UtilLibs;
using static AkisSnowThings.STRINGS;
using AkisSnowThings.Content.Defs.Entities;
using AkisSnowThings.Content.Scripts.Entities;

namespace AkisSnowThings.Content.Defs.Plants
{
	internal class EvergreenTreeConfig : IEntityConfig
	{
		public const string ID = "SnowSculptures_EverGreenTree";
		public const string SEED_ID = "SnowSculptures_PineCone";
		public const float FERTILIZER_PER_CYCLE = 2f / 600f;
		public const float WATER_PER_CYCLE = 30f / 600;
		public const float GROWTH_TIME = 24f * 600f;
		public const int HARVEST_MASS = 4000;
		public const float SAP_PER_SECOND = 10f / 600f;
		public const float SAP_CAPACITY = 20f;
		public static CellOffset OUTPUT_CONDUIT_CELL_OFFSET = new CellOffset(0, 1);

		public GameObject CreatePrefab()
		{
			var prefab = EntityTemplates.CreatePlacedEntity(
				ID,
				SNOWSCULPTURES_EVERGREEN_TREE.NAME,
				SNOWSCULPTURES_EVERGREEN_TREE.DESC,
				750f,
				Assets.GetAnim("sm_pine_tree_plant_kanim"),
				"idle_full",
				Grid.SceneLayer.Building,
				3,
				4,
				TUNING.DECOR.BONUS.TIER3);

			EntityTemplates.ExtendEntityToBasicPlant(
				prefab,
				UtilMethods.GetKelvinFromC(-90),
				UtilMethods.GetKelvinFromC(-40),
				UtilMethods.GetKelvinFromC(50),
				UtilMethods.GetKelvinFromC(90),
				crop_id: TreeRemainsConfig.ID,
				should_grow_old: false,
				
				max_radiation: TUNING.PLANTS.RADIATION_THRESHOLDS.TIER_2,
				baseTraitId: ID + "Original",
				baseTraitName: SNOWSCULPTURES_EVERGREEN_TREE.NAME);

			UnityEngine.Object.DestroyImmediate(prefab.GetComponent<HarvestDesignatable>());
			prefab.AddComponent<EvergreenHarvestDesignatable>().SetHarvestWhenReady(false);

			//var fertilizerConsumption = new PlantElementAbsorber.ConsumeInfo()
			//{
			//	tag = SimHashes.Fertilizer.CreateTag(),
			//	massConsumptionRate = FERTILIZER_PER_CYCLE
			//};
			var waterConsumption = new PlantElementAbsorber.ConsumeInfo()
			{
				tag = SimHashes.DirtyWater.CreateTag(),
				massConsumptionRate = WATER_PER_CYCLE
			};

			//EntityTemplates.ExtendPlantToFertilizable(prefab, [fertilizerConsumption]);
			EntityTemplates.ExtendPlantToIrrigated(prefab, [waterConsumption]);

			prefab.AddOrGet<StandardCropPlant>();

			var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
				prefab,
				SeedProducer.ProductionType.Harvest,
				SEED_ID,
				SEEDS.SNOWSCULPTURES_EVERGREEN_TREE.NAME,
				SEEDS.SNOWSCULPTURES_EVERGREEN_TREE.DESC,
				Assets.GetAnim("pine_cone_kanim"),
				additionalTags: new List<Tag> { GameTags.CropSeed },
				sortOrder: 2,
				domesticatedDescription: SNOWSCULPTURES_EVERGREEN_TREE.DOMESTICATEDDESC,
				width: 0.35f,
				height: 0.35f);

			EntityTemplates.CreateAndRegisterPreviewForPlant(
				seed,
				ID + "_preview",
				Assets.GetAnim("sm_pine_tree_plant_kanim"),
				"place",
				3,
				4);


			prefab.AddOrGet<BuildingAttachPoint>().points = [new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), ModAssets.TreeAttachmentTag, null)];
			prefab.AddOrGet<TreeAttachment>();


			var sapStorage = prefab.AddComponent<Storage>();
			sapStorage.capacityKg = SAP_CAPACITY;
			sapStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

			var sapProducer = prefab.AddComponent<TreeSapProducer>();
			sapProducer.SapStorage = sapStorage;
			sapProducer.sapProductionRatePerSecond = SAP_PER_SECOND;

			ConduitDispenser conduitDispenser = prefab.AddOrGet<ConduitDispenser>();
			conduitDispenser.noBuildingOutputCellOffset = OUTPUT_CONDUIT_CELL_OFFSET;
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.alwaysDispense = true;
			//conduitDispenser.SetOnState(true);
			conduitDispenser.storage =	sapStorage;
			return prefab;
		}

		public string[] GetDlcIds()
		{
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst)
		{
			inst.GetComponent<KBatchedAnimController>().randomiseLoopedOffset = true;

		}

		public void OnSpawn(GameObject inst)
		{
			EntityCellVisualizer entityCellVisualizer = inst.AddOrGet<EntityCellVisualizer>();
			entityCellVisualizer.AddPort(EntityCellVisualizer.Ports.LiquidOut, OUTPUT_CONDUIT_CELL_OFFSET, (Color)entityCellVisualizer.Resources.liquidIOColours.output.connected);
		}
	}
}
