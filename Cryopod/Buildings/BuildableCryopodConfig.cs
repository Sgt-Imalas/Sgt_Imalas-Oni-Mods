using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Cryopod.Buildings
{
    class BuildableCryopodConfig : IBuildingConfig
    {
        public const string ID = "CRY_BuildableCryoTank";

        public override BuildingDef CreateBuildingDef()
        {
            float[] mass = {
                800f,
                200f,
                200f,
            };
            string[] material = {
                "RefinedMetal"
                ,"Glass"
                ,"Plastic"
            };
            EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER1;
            EffectorValues decor = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 3, "cryo_chamber_buildable_kanim", 100, 30f, mass, material, 1600f, BuildLocationRule.OnFloor, decor, noise);

            buildingDef.RequiresPowerInput = true;
            buildingDef.AddLogicPowerPort = false;
            buildingDef.OverheatTemperature = 348.15f;
            buildingDef.EnergyConsumptionWhenActive = 960f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Floodable = false;
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
            buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
            buildingDef.PowerInputOffset = new CellOffset(0, 0); 
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>(){
                LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), (string) global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, (string) global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, (string) global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE)
            };
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
            //go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<Overheatable>().baseOverheatTemp = 398.15f;
            var ownable = go.AddOrGet<Ownable>();
            ownable.tintWhenUnassigned = false;
            ownable.slotID = Db.Get().AssignableSlots.WarpPortal.Id;
            go.AddOrGet<EnergyConsumer>();
            go.AddOrGet<MinionStorage>();
            var cryopod = go.AddOrGet<CryopodReusable>();
            cryopod.dropOffset = new CellOffset(1, 0);
            cryopod.InternalTemperatureKelvin = CryopodReusable.InternalTemperatureKelvinUpperLimit;
            cryopod.buildingeMode = CryopodReusable.BuildingeMode.Standalone;
            cryopod.powerSaverEnergyUsage = 240f;
            go.AddOrGet<CryopodFreezeWorkable>(); 
            go.AddOrGet<OpenCryopodWorkable>(); 
            go.AddOrGet<Prioritizable>();
        }
        
    }    
}
