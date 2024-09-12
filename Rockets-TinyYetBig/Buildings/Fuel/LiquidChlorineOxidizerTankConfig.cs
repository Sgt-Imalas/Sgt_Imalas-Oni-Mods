using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Fuel
{
	public class LiquidChlorineOxidizerTankConfig : IBuildingConfig
	{
		public const string ID = "RTB_LiquidChlorineOxidizerTank";
		public const float FuelCapacity = 450f;

		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

		public override BuildingDef CreateBuildingDef()
		{
			float[] fuelTankDryMass = new[] { 200f, 100f };
			string[] construction_materials = new string[]
			{
				SimHashes.Steel.ToString(),
				SimHashes.Katairite.ToString(),
			};
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				ID,
				5,
				2,
				"liquid_chlorine_oxidizer_kanim",
				1000,
				60f,
				fuelTankDryMass,
				construction_materials,
				9999f,
				BuildLocationRule.Anywhere,
				none,
				noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.DefaultAnimState = "grounded";
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.UtilityInputOffset = new CellOffset(1, 1);
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.RequiresPowerInput = false;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			buildingDef.ShowInBuildMenu = false;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, (AttachableBuilding) null)
			};
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = 450f;
			storage.storageFilters = new List<Tag>()
			{
				ModAssets.Tags.CorrosiveOxidizer
			};
			storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate
			});

			OxidizerTank oxidizerTank = go.AddOrGet<OxidizerTank>();
			oxidizerTank.supportsMultipleOxidizers = false;
			oxidizerTank.consumeOnLand = false;
			oxidizerTank.storage = storage;
			oxidizerTank.targetFillMass = FuelCapacity;
			oxidizerTank.maxFillMass = FuelCapacity;
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<DropToUserCapacity>();
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityTag = ModAssets.Tags.CorrosiveOxidizer;
			conduitConsumer.capacityKG = storage.capacityKg;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MAJOR_PLUS);
			storage.showUnreachableStatus = false;



			//go.GetComponent<KPrefabID>().prefabInitFn += (KPrefabID.PrefabFn)(inst =>
			//{
			//    Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Chlorine);
			//    if (DiscoveredResources.Instance.IsDiscovered(elementByHash.tag))
			//        return;
			//    DiscoveredResources.Instance.Discover(elementByHash.tag, elementByHash.GetMaterialCategoryTag());
			//});
		}
	}

}
