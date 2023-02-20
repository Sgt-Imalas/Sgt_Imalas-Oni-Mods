using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace MineralizerReborn
{
    internal class MineralizerConfig : IBuildingConfig
    {
        public const string ID = "Mineralizer";

        private const float SALT_INPUT_RATE = 0.35f;
        private const float WATER_WITH_SALT_INPUT_RATE = 4.65f;
        private const float OUTPUT_RATE = 5.0f;

        public override BuildingDef CreateBuildingDef()
        {

            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 4,
                height: 2,
                anim: "mineralizer_kanim",
                hitpoints: TUNING.BUILDINGS.HITPOINTS.TIER2,
                construction_time: TUNING.BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                construction_mass: TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                construction_materials: MATERIALS.ALL_METALS,
                melting_point: TUNING.BUILDINGS.MELTING_POINT_KELVIN.TIER0,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: TUNING.BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NOISY.TIER2,
                0.2f
                );
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 360f;
            buildingDef.ExhaustKilowattsWhenActive = 8f;
            buildingDef.SelfHeatKilowattsWhenActive = 0f;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            Storage SaltStorage = go.AddComponent<Storage>();
            SaltStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            SaltStorage.showInUI = true;
            SaltStorage.capacityKg = 600 * SALT_INPUT_RATE+20f;

            go.AddOrGet<LoopingSounds>();
            ElementConverter elementConverter1 = go.AddComponent<ElementConverter>();
            elementConverter1.consumedElements = new ElementConverter.ConsumedElement[2]
            {
                new ElementConverter.ConsumedElement("Salt", SALT_INPUT_RATE),
                new ElementConverter.ConsumedElement("Water", WATER_WITH_SALT_INPUT_RATE)
            };
            elementConverter1.outputElements = new ElementConverter.OutputElement[1]
            {
              new ElementConverter.OutputElement(OUTPUT_RATE, SimHashes.SaltWater, 0.0f, false, true, 0.0f, 0.5f, 0.75f, byte.MaxValue, 0),
            };

            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(SaltStorage);
            manualDeliveryKg.RequestedItemTag = new Tag("Salt");
            manualDeliveryKg.capacity = 600 * SALT_INPUT_RATE;
            manualDeliveryKg.refillMass = 100 * SALT_INPUT_RATE;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 10f;
            conduitConsumer.capacityKG = 20f;
            conduitConsumer.capacityTag = GameTags.Water;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.storage = SaltStorage;

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.elementFilter = new SimHashes[1] { SimHashes.SaltWater };
            conduitDispenser.storage = SaltStorage;
            Prioritizable.AddRef(go);
            go.AddOrGet<Mineralizer>().saltStorage = SaltStorage;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {

        }
    }
}

