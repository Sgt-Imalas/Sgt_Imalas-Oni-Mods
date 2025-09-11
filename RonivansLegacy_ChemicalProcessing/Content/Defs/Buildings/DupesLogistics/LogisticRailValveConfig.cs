using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics
{
	class LogisticRailValveConfig : IBuildingConfig
	{
		public static string ID = "LogisticRailValve";
		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = [100f];
			string[] materials1 = [GameTags.Metal.ToString()];
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "logistic_valve_kanim", 100, 50f, quantity1, materials1, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Solid;
			buildingDef.OutputConduitType = ConduitType.Solid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
			buildingDef.PermittedRotations = PermittedRotations.R360;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("logistic_rail_kanim", "filter_material_conveyor_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.showDescriptor = false;
			storage.storageFilters = null;
			storage.capacityKg = HighPressureConduitRegistration.SolidCap_Regular;
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.showCapacityStatusItem = false;
			storage.showCapacityAsMainStatus = false;

			SolidConduitConsumer conduitConsumer = go.AddOrGet<SolidConduitConsumer>();
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.capacityKG = storage.capacityKg;

			ConfigurableSolidConduitDispenser solidDispenser = go.AddOrGet<ConfigurableSolidConduitDispenser>();
			solidDispenser.alwaysDispense = true;
			solidDispenser.elementFilter = null;
			solidDispenser.massDispensed = HighPressureConduitRegistration.SolidCap_Logistic;

		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			var requirement = go.AddOrGet<HPA_SolidConduitRequirement>();
			requirement.RequiresHighPressureOutput = true;
			requirement.IsLogisticRail = true;
			go.AddOrGet<AIO_DecompressionValve>();
		}
	}
}
