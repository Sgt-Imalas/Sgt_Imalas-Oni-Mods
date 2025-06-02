using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static Storage;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	public class FridgeModuleAccessHatchConfig : IBuildingConfig
	{
		public const string ID = "RTB_FridgeModuleAccessHatch";

		public static readonly HashedString PULL_MORE_ID = "RTB_PULL_MORE";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] materialMass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] materialType = MATERIALS.REFINED_METALS;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 2,
				height: 1,
				anim: "fridge_interface_kanim",
				hitpoints: 15,
				construction_time: 10f,
				construction_mass: materialMass,
				construction_materials: materialType,
				melting_point: 400f,
				BuildLocationRule.OnRocketEnvelope,
				decor: BUILDINGS.DECOR.PENALTY.TIER1,
				noise: NOISE_POLLUTION.NONE);
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.RequiresPowerInput = false;
			buildingDef.AddLogicPowerPort = true;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.Floodable = false;

			//buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			//{//TODO
			//    LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(1, 0), (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT_INACTIVE)
			//};
			buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
			{//TODO
                LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT_PULL, (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT_ACTIVE_PULL , (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT_INACTIVE_PULL)
			};
			//GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			if (go.TryGetComponent<KPrefabID>(out var component))
			{
				component.AddTag(GameTags.RocketInteriorBuilding);
				component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
				component.AddTag(GameTags.UniquePerWorld);
			}
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			Storage storage = go.AddOrGet<Storage>();
			storage.showInUI = true;
			storage.showDescriptor = true;
			storage.storageFilters = STORAGEFILTERS.FOOD;
			storage.allowItemRemoval = true;
			//storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
			storage.capacityKg = 4.0f;
			storage.showCapacityStatusItem = true;
			storage.SetDefaultStoredItemModifiers(new List<StoredItemModifier>
			{
				StoredItemModifier.Hide,
				StoredItemModifier.Seal,
				StoredItemModifier.Preserve,
				StoredItemModifier.Insulate
			});
			go.AddOrGet<Prioritizable>();
			Prioritizable.AddRef(go);

			go.AddOrGet<TreeFilterable>();
			go.AddOrGet<FridgeModuleHatchGrabber>().maxPullCapacityKG = 2f;

			//go.AddOrGet<FridgeModuleItemDistributor>();
			go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;

			//go.AddOrGetDef<StorageController.Def>();
			go.AddOrGetDef<OperationalController.Def>();
		}
	}
}
