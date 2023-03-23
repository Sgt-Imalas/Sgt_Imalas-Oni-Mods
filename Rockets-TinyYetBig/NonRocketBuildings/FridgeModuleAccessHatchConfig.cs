using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static KAnimBatchGroup;
using static TUNING.BUILDINGS.UPGRADES;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    internal class FridgeModuleAccessHatchConfig : IBuildingConfig
    {
        public const string ID = "RTB_FridgeModuleAccessHatch";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] materialType = MATERIALS.REFINED_METALS;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
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
            buildingDef.RequiresPowerInput = true;
            buildingDef.AddLogicPowerPort = false;
            buildingDef.EnergyConsumptionWhenActive = 60f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.Floodable = false;

            //buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            //{
            //    LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.RTB_FRIDGEMODULEACCESSHATCH.LOGIC_PORT_INACTIVE)
            //};
            //GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {

            go.GetComponent<KPrefabID>().AddTag(GameTags.RocketInteriorBuilding);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.FOOD;
            storage.allowItemRemoval = true;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.capacityKg = 1f;
            storage.showCapacityStatusItem = true;
            go.AddOrGet<TreeFilterable>(); 
            go.AddOrGet<FridgeModuleHatchGrabber>();

            RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
            def.powerSaverEnergyUsage = 10f;
            def.coolingHeatKW = 0.375f;
            def.steadyHeatKW = 0.0f;

            go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
            go.AddOrGet<FoodStorage>();
            go.AddOrGetDef<StorageController.Def>();
            go.AddOrGetDef<OperationalController.Def>();
        }
    }
}
