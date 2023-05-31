using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.Engines
{
    public class NatGasEngineClusterConfig : IBuildingConfig
    {
        public const string ID = "RTB_NatGasEngineCluster";
        public const string kanim = "rocket_natgas_engine_kanim"; //rocket_petro_engine_small_kanim
        public const SimHashes FUEL = SimHashes.Methane;
        public const float Wattage = 480f;
        public const float TankCapacity = 900f;
        public const int RocketHeight = 23;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            UtilMethods.CopySoundsToAnim(kanim, "rocket_petro_engine_small_kanim");

            float[] constructionMass = new float[] { 300f };
            string[] constructioMaterials = MATERIALS.REFINED_METALS;
            EffectorValues noiseval = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues decorval = BUILDINGS.DECOR.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 5,
                height: 5,
                anim: kanim,
                hitpoints: 1000,
                construction_time: 60f,
                construction_mass: constructionMass,
                construction_materials: constructioMaterials,
                melting_point: 9999f,
                BuildLocationRule.Anywhere,
                decor: decorval,
                noise: noiseval);

            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.GeneratorWattageRating = Wattage;
            buildingDef.GeneratorBaseCapacity = 20000f;
            buildingDef.RequiresPowerInput = false;
            buildingDef.RequiresPowerOutput = false;
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            buildingDef.ShowInBuildMenu = false;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 3);
            buildingDef.InputConduitType = ConduitType.Gas;


            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket,  null)
            };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            RocketEngineCluster rocketEngineCluster = go.AddOrGet<RocketEngineCluster>();
            rocketEngineCluster.maxModules = 7;
            rocketEngineCluster.maxHeight = RocketHeight;
            rocketEngineCluster.fuelTag = GameTags.CombustibleGas;// FUEL.CreateTag();
            rocketEngineCluster.efficiency = ROCKETRY.ENGINE_EFFICIENCY.MEDIUM;
            rocketEngineCluster.requireOxidizer = true;
            rocketEngineCluster.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
            rocketEngineCluster.exhaustElement = SimHashes.CarbonDioxide;
            rocketEngineCluster.exhaustTemperature = 1263.15f;
            go.AddOrGet<ModuleGenerator>();

            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = TankCapacity;
            storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
            {
                Storage.StoredItemModifier.Hide,
                Storage.StoredItemModifier.Seal,
                Storage.StoredItemModifier.Insulate
            });
            FuelTank fuelTank = go.AddOrGet<FuelTank>();
            fuelTank.consumeFuelOnLand = false;
            fuelTank.storage = storage;
            fuelTank.FuelType = GameTags.CombustibleGas;// FUEL.CreateTag();
            fuelTank.targetFillMass = storage.capacityKg;
            fuelTank.physicalFuelCapacity = storage.capacityKg;
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MODERATE_PLUS, 40f, TankCapacity / Config.Instance.EnableNatGasEngineRange / 600f);
            go.GetComponent<KPrefabID>().prefabInitFn += inst => { };
        }
    }
}
