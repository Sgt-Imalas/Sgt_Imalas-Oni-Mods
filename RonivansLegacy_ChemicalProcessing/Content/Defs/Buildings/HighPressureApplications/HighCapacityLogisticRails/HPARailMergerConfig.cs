//using RonivansLegacy_ChemicalProcessing.Content.Scripts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TUNING;
//using UnityEngine;
//using UtilLibs;

//namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HighCapacityLogisticRails
//{
//    class HPARailMergerConfig : IBuildingConfig
//	{
//		public static string ID = "HPA_RailMerger";
//		public override BuildingDef CreateBuildingDef()
//		{
//			float[] quantity1 = [250f, 150f];
//			string[] materials1 = [GameTags.Steel.ToString(), GameTags.Transparent.ToString()];
//			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "hpa_rail_merger", 100, 60f, quantity1, materials1, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
//			buildingDef.Floodable = false;
//			buildingDef.Overheatable = false;
//			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
//			buildingDef.AudioCategory = "HollowMetal";
//			buildingDef.InputConduitType = ConduitType.Liquid;
//			buildingDef.OutputConduitType = ConduitType.Liquid;
//			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
//			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
//			buildingDef.PermittedRotations = PermittedRotations.R360;
//			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
//			SoundUtils.CopySoundsToAnim("deco_gas_valve_kanim", "valveliquid_logic_kanim");
//			return buildingDef;
//		}

//		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
//		{
//			go.AddOrGet<Reservoir>();
//			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
//			storage.showDescriptor = false;
//			storage.storageFilters = STORAGEFILTERS.LIQUIDS;
//			storage.capacityKg = Config.Instance.HPA_Capacity_Liquid;
//			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
//			storage.showCapacityStatusItem = false;
//			storage.showCapacityAsMainStatus = false;

//			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
//			conduitConsumer.conduitType = ConduitType.Liquid;
//			conduitConsumer.ignoreMinMassCheck = true;
//			conduitConsumer.forceAlwaysSatisfied = true;
//			conduitConsumer.alwaysConsume = true;
//			conduitConsumer.capacityKG = storage.capacityKg;

//			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
//			conduitDispenser.conduitType = ConduitType.Liquid;
//			conduitDispenser.elementFilter = null;
//		}

//		public override void DoPostConfigureComplete(GameObject go)
//		{
//			go.AddOrGetDef<StorageController.Def>();
//			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
//			go.AddOrGet<HPA_ConduitRequirement>().RequiresHighPressureInput = true;
//		}
//	}
//}
