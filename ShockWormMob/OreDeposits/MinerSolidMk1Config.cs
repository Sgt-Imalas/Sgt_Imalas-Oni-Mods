using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ShockWormMob.OreDeposits
{
    //Building definitions should always end with [...]Config for clarity
    public class MinerSolidMk1Config : IBuildingConfig
    {
        public const string ID = "SolidMinerMK1";
        float baseMiningEff = 10f;

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 4, "geyser_oil_cap_kanim", 100, 120f, tieR3, refinedMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
            BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;
            buildingDef.PowerInputOffset = new CellOffset(1, 1);
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
            buildingDef.AttachmentSlotTag = OreDepositsConfig.DepositSolidAttachmentTag;
            buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
            buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();

            Storage drillbitStorage = go.AddOrGet<Storage>();
            drillbitStorage.capacityKg = 100f;
            drillbitStorage.showInUI = true;

            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(drillbitStorage);
            manualDeliveryKg.RequestedItemTag = Miner.DrillbitMaterial;
            manualDeliveryKg.refillMass = 40f;
            manualDeliveryKg.capacity = 100f;
            manualDeliveryKg.MinimumMass = 20;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

            var miner = go.AddOrGet<Miner>();

            miner.BaseMiningSpeed = this.baseMiningEff;
            miner.DumpMaterialToWorld = true;
            miner.drillbitStorage = drillbitStorage;
            miner.outputCellOffset = new CellOffset(2, 1);
        }

    }
}
