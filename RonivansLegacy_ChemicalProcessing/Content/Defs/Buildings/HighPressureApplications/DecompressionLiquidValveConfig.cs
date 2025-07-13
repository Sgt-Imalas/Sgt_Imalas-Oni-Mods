using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications
{
	public class DecompressionLiquidValveConfig : IBuildingConfig
	{
		public static string ID = "DecompressionLiquidValve";
		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = new float[] { 50f, 20f };
			string[] materials1 = new string[] { GameTags.Steel.ToString(), GameTags.Plastic.ToString() };
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 1, "deco_liquid_valve_kanim", 100, 50f, quantity1, materials1, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			buildingDef.PermittedRotations = PermittedRotations.R360;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<Reservoir>();
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.showDescriptor = false;
			storage.storageFilters = STORAGEFILTERS.LIQUIDS;
			storage.capacityKg = Config.Instance.HPA_Capacity_Liquid;
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.showCapacityStatusItem = false;
			storage.showCapacityAsMainStatus = false;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.ignoreMinMassCheck = true;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.capacityKG = storage.capacityKg;

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.elementFilter = null;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<StorageController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}
	}
}
