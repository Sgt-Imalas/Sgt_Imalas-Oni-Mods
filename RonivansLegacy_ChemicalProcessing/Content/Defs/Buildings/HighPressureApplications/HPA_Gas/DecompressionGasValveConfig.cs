using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications
{

	public class DecompressionGasValveConfig : IBuildingConfig
	{
		public static string ID = "DecompressionGasValve";

		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = [50f, 20f];
			string[] materials1 = [GameTags.Steel.ToString(), GameTags.Plastic.ToString()];
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 1, "deco_gas_valve_kanim", 100, 50f, quantity1, materials1, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			buildingDef.PermittedRotations = PermittedRotations.R360;

			buildingDef.SceneLayer = Grid.SceneLayer.GasConduitBridges;
			buildingDef.ObjectLayer = ObjectLayer.GasConduitConnection;

			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			SoundUtils.CopySoundsToAnim("deco_gas_valve_kanim", "valvegas_logic_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.showDescriptor = false;
			storage.storageFilters = STORAGEFILTERS.GASES;
			storage.capacityKg = HighPressureConduitRegistration.GasCap_HP;
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.showCapacityStatusItem = false;
			storage.showCapacityAsMainStatus = false;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.ignoreMinMassCheck = true;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.capacityKG = storage.capacityKg;

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Gas;
			conduitDispenser.elementFilter = null;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<StorageController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			go.AddOrGet<HPA_ConduitRequirement>().RequiresHighPressureInput = true;
			go.AddOrGet<HPA_DecompressionOutput>();
		}
	}
}
