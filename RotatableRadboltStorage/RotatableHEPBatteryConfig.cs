using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RotatableRadboltStorage
{
    internal class RotatableHEPBatteryConfig : IBuildingConfig
    {
        public const string ID = "RRS_RotatableHEPBattery";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR4 = 
            {
                420f 
            };
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "radbolt_battery_kanim", 30, 120f, tieR4, refinedMetals, 800f, BuildLocationRule.OnFloor, tieR2, noise);
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "large";
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.Radiation.ID;
            buildingDef.UseHighEnergyParticleInputPort = true;
            buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 1);
            buildingDef.UseHighEnergyParticleOutputPort = true;
            buildingDef.HighEnergyParticleOutputOffset = new CellOffset(0, 2);
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.ExhaustKilowattsWhenActive = 0.25f;
            buildingDef.SelfHeatKilowattsWhenActive = 1f;
            buildingDef.AddLogicPowerPort = true;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, "HEPBattery");
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort((HashedString) "HEP_STORAGE", new CellOffset(1, 1), (string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE, (string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE_ACTIVE, (string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_STORAGE_INACTIVE)
    };
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.InputPort(HEPBattery.FIRE_PORT_ID, new CellOffset(0, 2), (string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT, (string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_ACTIVE, (string) global::STRINGS.BUILDINGS.PREFABS.HEPBATTERY.LOGIC_PORT_INACTIVE)
    };
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            energyParticleStorage.capacity = 1000f;
            energyParticleStorage.autoStore = true;
            energyParticleStorage.PORT_ID = "HEP_STORAGE";
            energyParticleStorage.showCapacityStatusItem = true;
            energyParticleStorage.showCapacityAsMainStatus = true;
            go.AddOrGet<LoopingSounds>();
            HEPBatteryTwo.Def def = go.AddOrGetDef<HEPBatteryTwo.Def>();
            def.minLaunchInterval = 1f;
            def.minSlider = 0.0f;
            def.maxSlider = 100f;
            def.particleDecayRate = 0.5f;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
