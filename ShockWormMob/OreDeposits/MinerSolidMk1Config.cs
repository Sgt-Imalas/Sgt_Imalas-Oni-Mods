using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ShockWormMob.OreDeposits
{
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

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
            Storage standardStorage = go.AddOrGet<Storage>();
            standardStorage.capacityKg = 100f;
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(standardStorage);
            manualDeliveryKg.RequestedItemTag = GameTags.RefinedMetal;
            manualDeliveryKg.refillMass = 120f;
            manualDeliveryKg.capacity = 480f;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<Miner>().baseMiningEff = this.baseMiningEff;
        }

    }
}
