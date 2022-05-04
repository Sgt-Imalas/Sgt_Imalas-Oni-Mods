using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RocketryExpanded.buildings
{
    class NuclearPulseEngineConfig : IBuildingConfig
    {
        public const string ID = "NuclearPulseEngine";
        public const string DisplayName = "Project Orion";

        public const SimHashes FUEL = SimHashes.EnrichedUranium;
        public Tag FUEL_TAG = SimHashes.EnrichedUranium.CreateTag();
        public const float FUEL_CAPACITY = 350f;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] engineMassLarge = BUILDINGS.ROCKETRY_MASS_KG.ENGINE_MASS_LARGE;
            string[] construction_materials = new string[1]
            {
      SimHashes.Steel.ToString()
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 5, "rocket_cluster_hydrogen_engine_kanim", 1000, 60f, engineMassLarge, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.InputConduitType = ConduitType.None;
            buildingDef.GeneratorWattageRating = 1400;
            buildingDef.GeneratorBaseCapacity = 100000f;
            buildingDef.RequiresPowerInput = false;
            buildingDef.RequiresPowerOutput = false;
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            buildingDef.ShowInBuildMenu = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery); 
            go.AddOrGet<ExhaustDispenser>();
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
            };
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            RadiationEmitter radiationEmitter = go.AddOrGet<RadiationEmitter>();
            radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
            radiationEmitter.emitRadiusX = (short)15;
            radiationEmitter.emitRadiusY = (short)15;
            radiationEmitter.emitRads = (float)(16800.0 / ((double)radiationEmitter.emitRadiusX / 6.0));
            radiationEmitter.emissionOffset = new Vector3(0.0f, 3f, 0.0f);

            RocketEngineCluster rocketEngineCluster = go.AddOrGet<RocketEngineCluster>();
            rocketEngineCluster.maxModules = 7;
            rocketEngineCluster.maxHeight = ROCKETRY.ROCKET_HEIGHT.VERY_TALL;
            rocketEngineCluster.fuelTag = SimHashes.EnrichedUranium.CreateTag();
            rocketEngineCluster.efficiency = 90f;
            rocketEngineCluster.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
            rocketEngineCluster.requireOxidizer = false;
            rocketEngineCluster.exhaustElement = SimHashes.Fallout;
            rocketEngineCluster.exhaustTemperature = 3000f;
            rocketEngineCluster.exhaustEmitRate = 50f;
            rocketEngineCluster.exhaustDiseaseIdx = Db.Get().Diseases.GetIndex((HashedString)Db.Get().Diseases.RadiationPoisoning.Id);
            rocketEngineCluster.exhaustDiseaseCount = 100000;
            rocketEngineCluster.emitRadiation = true;

            go.AddOrGet<ModuleGenerator>();
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = FUEL_CAPACITY;
            storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Hide,
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate
    });
            FuelTank fuelTank = go.AddOrGet<FuelTank>();
            fuelTank.consumeFuelOnLand = false;
            fuelTank.storage = storage;
            fuelTank.FuelType = FUEL_TAG;
            fuelTank.targetFillMass = storage.capacityKg;
            fuelTank.physicalFuelCapacity = storage.capacityKg;
            go.AddOrGet<CopyBuildingSettings>();
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.requestedItemTag = FUEL_TAG;
            manualDeliveryKg.refillMass = storage.capacityKg;
            manualDeliveryKg.capacity = storage.capacityKg;
            manualDeliveryKg.operationalRequirement = FetchOrder2.OperationalRequirement.None;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MAJOR_PLUS, (float)ROCKETRY.ENGINE_POWER.LATE_VERY_STRONG, ROCKETRY.FUEL_COST_PER_DISTANCE.VERY_LOW);
            go.GetComponent<KPrefabID>().prefabInitFn += (KPrefabID.PrefabFn)(inst => { });
        }
    }
}