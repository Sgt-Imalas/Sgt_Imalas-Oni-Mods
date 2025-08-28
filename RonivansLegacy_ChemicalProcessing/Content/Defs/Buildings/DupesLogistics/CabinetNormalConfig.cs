using RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
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
    class CabinetNormalConfig : IBuildingConfig, IHasConfigurableStorageCapacity
	{
		public static string ID = "CabinetNormal";
		public static float StorageCapacity = 20000;
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;
		public override BuildingDef CreateBuildingDef()
		{
			float[] cost = [200, 200];
			string[] materials = [GameTags.RefinedMetal.ToString(),GameTags.BuildableRaw.ToString()];
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "cabinet_normal_kanim", 30, 10f, cost, materials, 1600f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, TUNING.NOISE_POLLUTION.NONE);
			def1.Floodable = false;
			def1.AudioCategory = "Metal";
			def1.Overheatable = false;

			def1.InputConduitType = ConduitType.Solid;
			def1.UtilityInputOffset = new CellOffset(0, 0);
			SoundUtils.CopySoundsToAnim("cabinet_normal_kanim", "storagelocker_kanim");
			return def1;			
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			SoundEventVolumeCache.instance.AddVolume("cabinet_normal_kanim", "StorageLocker_Hit_metallic_low", TUNING.NOISE_POLLUTION.NOISY.TIER1);
			Prioritizable.AddRef(go);
			go.AddOrGet<Automatable>();
			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.showInUI = true;
			storage.allowItemRemoval = true;
			storage.showDescriptor = true;
			storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
			storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
			storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			storage.capacityKg = StorageCapacity;
			go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
			go.AddOrGet<StorageLocker>();

			//filtered solid conduit input
			go.AddOrGet<FilteredSolidConduitConsumer>();

			HysteresisStorage.AddComponent(go);
		}
	}
}
