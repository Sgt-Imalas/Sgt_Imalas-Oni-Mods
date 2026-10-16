using Rockets_TinyYetBig.Behaviours;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.Engines
{
	internal class IonEngineClusterConfig : IBuildingConfig
	{
		public const string ID = "RTB_IonEngineCluster";
		public const string kanim = "ion_thrus_module_kanim"; //rocket_petro_engine_small_kanim
		public const int RocketHeight = 23;
		public static float TankCapacity = 480;
		public const int MaxRange = 32;
		public const SimHashes FUEL = SimHashes.Water;

		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			SoundUtils.CopySoundsToAnim(kanim, "rocket_cluster_hydrogen_engine_kanim");

			float[] constructionMass = new float[] { 500f };
			string[] constructioMaterials = new string[1]
			{
				SimHashes.Steel.ToString()
			};
			EffectorValues noiseval = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues decorval = BUILDINGS.DECOR.NONE;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 5,
				height: 5,
				anim: kanim,
				hitpoints: 1000,
				construction_time: 60f,
				construction_mass: constructionMass,
				construction_materials: constructioMaterials,
				melting_point: 9999f,
				BuildLocationRule.Anywhere,
				decor: decorval,
				noise: noiseval);


			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.RequiresPowerInput = false;
			buildingDef.RequiresPowerOutput = false;
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			buildingDef.ShowInBuildMenu = false;


			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket,  null)
			};
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			ElectricEngineCluster rocketEngineCluster = go.AddOrGet<ElectricEngineCluster>();
			rocketEngineCluster.maxModules = 7;
			rocketEngineCluster.maxHeight = RocketHeight;
			rocketEngineCluster.fuelTag = FUEL.CreateTag();
			rocketEngineCluster.efficiency = ROCKETRY.ENGINE_EFFICIENCY.MEDIUM;
			rocketEngineCluster.requireOxidizer = false;
			rocketEngineCluster.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
			rocketEngineCluster.exhaustElement = SimHashes.Steam;
			rocketEngineCluster.exhaustTemperature = 2300f;
			rocketEngineCluster.exhaustEmitRate = 10f;

			go.AddOrGet<ModuleGenerator>();

			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = TankCapacity;
			storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate
			});
			FuelTank fuelTank = go.AddOrGet<FuelTank>();
			fuelTank.consumeFuelOnLand = false;
			fuelTank.storage = storage;
			fuelTank.FuelType = FUEL.CreateTag();
			fuelTank.targetFillMass = storage.capacityKg;
			fuelTank.physicalFuelCapacity = storage.capacityKg;

			go.AddOrGet<CopyBuildingSettings>();

			go.AddOrGet<ExtendedClusterModuleAnimator>();
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityTag = fuelTank.FuelType;
			conduitConsumer.capacityKG = storage.capacityKg;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, 20, 30f, TankCapacity / MaxRange / 600f);
			go.GetComponent<KPrefabID>().prefabInitFn += inst => { };
		}
	}
}
