using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RoboRockets.Buildings
{
    class RadiatorBaseConfig : IBuildingConfig
    {
        public const string ID = "RadiatorBase";
        public const string NAME = "Space Radiator";

        private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>()
        {
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Seal
        };

        public override BuildingDef CreateBuildingDef()
        {
            float[] matCosts = { 600f
                   // , 300f 
            };

            string[] construction_materials = new string[]
                {
                    "RefinedMetal"
                   // ,"KATAIRITE"
                };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "utilitygasbridge_kanim", 100, 120f, matCosts, construction_materials, 1600f, BuildLocationRule.NotInTiles, none2, noise);
            BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
            buildingDef.EnergyConsumptionWhenActive = 320f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);

            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;

            buildingDef.OverheatTemperature = 398.15f;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            AirConditioner airConditioner = go.AddOrGet<AirConditioner>();
            airConditioner.temperatureDelta = -14f;
            airConditioner.maxEnvironmentDelta = -50f;
            airConditioner.isLiquidConditioner = true;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 10f;
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
            defaultStorage.showInUI = true;
            defaultStorage.capacityKg = 2f * conduitConsumer.consumptionRate;
            defaultStorage.SetDefaultStoredItemModifiers(RadiatorBaseConfig.StoredItemModifiers);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
        }
    }
}
