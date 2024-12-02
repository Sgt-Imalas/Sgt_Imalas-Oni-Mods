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

namespace AkisSnowThings.Content.Defs.Plants
{
	internal class PineTreeConfig : IEntityConfig
	{
		public const string ID = "SnowSculptures_PineTree";
		public const string SEED_ID = "SnowSculptures_PineCone";
		public const float FERTILIZER_PER_CYCLE = 5f / 600f;
		public const float WATER_PER_CYCLE = 15f / 600;
		public const float GROWTH_TIME = 48f * 600f;
		public const int HARVEST_MASS = 2000;

		public GameObject CreatePrefab()
		{
			var prefab = EntityTemplates.CreatePlacedEntity(
				ID,
				SNOWSCULPTURES_PINETREE.NAME,
				SNOWSCULPTURES_PINETREE.DESC,
				750f,
				Assets.GetAnim("sm_pine_tree_plant_kanim"),
				"idle_full",
				Grid.SceneLayer.Building,
				3,
				4,
				TUNING.DECOR.BONUS.TIER4);

			EntityTemplates.ExtendEntityToBasicPlant(
				prefab,
				UtilMethods.GetKelvinFromC(-90),
				UtilMethods.GetKelvinFromC(-40),
				UtilMethods.GetKelvinFromC(50),
				UtilMethods.GetKelvinFromC(90),			
				crop_id: PineTreeRemainsConfig.ID,
				should_grow_old:false,
				max_radiation: TUNING.PLANTS.RADIATION_THRESHOLDS.TIER_5,
				baseTraitId: ID + "Original",
				baseTraitName: SNOWSCULPTURES_PINETREE.NAME);


			var fertilizerConsumption = new PlantElementAbsorber.ConsumeInfo()
			{
				tag = GameTags.Fertilizer,
				massConsumptionRate = FERTILIZER_PER_CYCLE
			};
			var waterConsumption = new PlantElementAbsorber.ConsumeInfo()
			{
				tag = SimHashes.Water.CreateTag(),
				massConsumptionRate = WATER_PER_CYCLE
			};

			EntityTemplates.ExtendPlantToFertilizable(prefab, [fertilizerConsumption]);
			EntityTemplates.ExtendPlantToIrrigated(prefab,[waterConsumption]);

			prefab.AddOrGet<StandardCropPlant>();

			var seed = EntityTemplates.CreateAndRegisterSeedForPlant(
				prefab,
				SeedProducer.ProductionType.Harvest,
				SEED_ID,
				SEEDS.SNOWSCULPTURES_PINETREE.NAME,
				SEEDS.SNOWSCULPTURES_PINETREE.DESC,
				Assets.GetAnim("pine_cone_kanim"),
				additionalTags: new List<Tag> { GameTags.CropSeed },
				sortOrder: 2,
				domesticatedDescription: SNOWSCULPTURES_PINETREE.DOMESTICATEDDESC,
				width:0.35f,
				height:0.35f);

			EntityTemplates.CreateAndRegisterPreviewForPlant(
				seed,
				ID+"_preview",
				Assets.GetAnim("sm_pine_tree_plant_kanim"),
				"place",
				3,
				4);

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
		}
	}
}
