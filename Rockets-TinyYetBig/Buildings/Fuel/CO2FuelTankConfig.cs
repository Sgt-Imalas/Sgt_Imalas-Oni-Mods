using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Fuel
{
	public class CO2FuelTankConfig : IBuildingConfig
	{
		public const string ID = "RTB_Co2FuelTank";

		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

		public override BuildingDef CreateBuildingDef()
		{
			float[] MatCosts = BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER0;
			string[] Materials = MATERIALS.RAW_METALS;
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER1;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "rocket_co2_tank_kanim", 1000, 30f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.DefaultAnimState = "grounded";
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.UtilityInputOffset = new CellOffset(0, 1);
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.RequiresPowerInput = false;
			buildingDef.RequiresPowerOutput = false;

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket,  null)
			};

		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = 50f;
			storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate
			});
			FuelTank fuelTank = go.AddOrGet<FuelTank>();
			fuelTank.consumeFuelOnLand = false;
			fuelTank.storage = storage;
			fuelTank.FuelType = SimHashes.CarbonDioxide.CreateTag();
			fuelTank.targetFillMass = storage.capacityKg;
			fuelTank.physicalFuelCapacity = storage.capacityKg;
			go.AddOrGet<CopyBuildingSettings>();
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityTag = fuelTank.FuelType;
			conduitConsumer.capacityKG = storage.capacityKg;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR);
		}

	}
}
