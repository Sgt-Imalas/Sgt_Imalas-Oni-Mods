using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ShockWormMob
{
    internal class SmolHydrogenGeneratorConfig : IBuildingConfig
    {
        public const string ID = "SmolHydrogenGenerator";
        public const float ConsumptionRate = 0.1f;
        public const float InternalStorageCapacity = 2f;
        public const float GeneratorWattage = 800f;
        public const float HeatProductionWhenActiveKW = 2f;

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID,
                width: 4,
                height: 3,
                anim: "generatormerc_kanim",
                hitpoints: 100,
                construction_time: 120f,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER5,
                construction_materials: MATERIALS.RAW_METALS,
                melting_point: 2400f,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: BUILDINGS.DECOR.PENALTY.TIER2,
                noise: NOISE_POLLUTION.NOISY.TIER5) ;

            buildingDef.GeneratorWattageRating = GeneratorWattage;
            buildingDef.GeneratorBaseCapacity = 1000f;
            buildingDef.ExhaustKilowattsWhenActive = HeatProductionWhenActiveKW;
            buildingDef.SelfHeatKilowattsWhenActive = HeatProductionWhenActiveKW;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.RequiresPowerOutput = true;
            buildingDef.PowerOutputOffset = new CellOffset(1, 0);
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<Storage>();
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = 1f;
            conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
            conduitConsumer.capacityKG = InternalStorageCapacity;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
            energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(SimHashes.Hydrogen.CreateTag(), ConsumptionRate, InternalStorageCapacity);
            energyGenerator.powerDistributionOrder = 8;
            energyGenerator.ignoreBatteryRefillPercent = true;
            energyGenerator.meterOffset = Meter.Offset.Behind;
            Tinkerable.MakePowerTinkerable(go);
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}
