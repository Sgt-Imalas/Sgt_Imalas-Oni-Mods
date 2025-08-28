using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesRefrigeration
{
    class SpaceBoxConfig : IBuildingConfig, IHasConfigurableStorageCapacity
	{
		public static float StorageCapacity = 50;
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;
		public static string ID = "SpaceBox";
		
		public static readonly List<Storage.StoredItemModifier> SpaceBoxStorage = [
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate,
				Storage.StoredItemModifier.Preserve,
			];

		public override string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>();
		}
		public override BuildingDef CreateBuildingDef()
		{
			float[] material_required = [100f, 50f];
			string[] material_type = ["RefinedMetal", "Plastic"];

			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "space_box_kanim", 30, 10f, material_required, material_type, 800f, BuildLocationRule.OnRocketEnvelope, TUNING.BUILDINGS.DECOR.BONUS.TIER1, noise);
			buildingDef.RequiresPowerInput = false;
			buildingDef.AddLogicPowerPort = false;
			buildingDef.LogicOutputPorts =
			[
				LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 0),
				global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT,
				global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE,
				global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE,
				false,
				false)
			];
			buildingDef.Floodable = false;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			SoundUtils.CopySoundsToAnim("simple_techfridge_kanim", "rationbox_kanim");
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Refrigerator, false);
			Prioritizable.AddRef(go);
			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(SpaceBoxStorage);
			storage.capacityKg = GetStorageCapacity();
			storage.showInUI = true;
			storage.showDescriptor = true;
			storage.storageFilters = STORAGEFILTERS.FOOD;
			storage.allowItemRemoval = true;
			storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
			storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			go.AddOrGet<TreeFilterable>();
			go.AddOrGet<RationBox>();
			go.AddOrGet<UserNameable>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.RocketInteriorBuilding, false);
			HysteresisStorage.AddComponent(go);
		}
	}
}
