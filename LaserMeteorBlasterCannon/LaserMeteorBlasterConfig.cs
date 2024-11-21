using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace LaserMeteorBlasterCannon
{
    internal class LaserMeteorBlasterConfig : IBuildingConfig
    {
        public static readonly string ID = "LMB_LaserBallTurret";
        public const string PORT_ID = "HEP_STORAGE_LASERTURRET";

        public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;


        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[]
            {
                800f
            };
            string[] materialType = new string[]
            {
                MATERIALS.REFINED_METAL
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NOISY.TIER5;
            EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 3,
                height: 3,
                anim: "laser_ball_turret_kanim",
                hitpoints: 300,
                construction_time: 40f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 1600f,
                BuildLocationRule.OnFloor,
                decor: decorValue,
                noise: noiseLevel);

            buildingDef.AudioCategory = "Metal";
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 360f;
            buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 0);
            buildingDef.DefaultAnimState = "on";
            buildingDef.RequiresPowerInput = true;
            buildingDef.ExhaustKilowattsWhenActive = 0.5f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;

            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            
            //HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            //energyParticleStorage.capacity = 2000f;
            //energyParticleStorage.autoStore = true;
            //energyParticleStorage.PORT_ID = PORT_ID;
            //energyParticleStorage.showCapacityStatusItem = true;
            go.AddOrGetDef<LaserBlaster.Def>();
            //ColorIntegration(go);
            this.AddVisualizer(go);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            SymbolOverrideControllerUtil.AddToPrefab(go);
        }
        private void AddVisualizer(GameObject go)
        {
            RangeVisualizer rangeVisualizer = go.AddOrGet<RangeVisualizer>();
            rangeVisualizer.RangeMin.x = MissileLauncher.Def.LaunchOffset.x - MissileLauncher.Def.launchRange.x;
            rangeVisualizer.RangeMax.x = MissileLauncher.Def.LaunchOffset.x + MissileLauncher.Def.launchRange.x;
            rangeVisualizer.RangeMin.y = MissileLauncher.Def.LaunchOffset.y;
            rangeVisualizer.RangeMax.y = MissileLauncher.Def.LaunchOffset.y + MissileLauncher.Def.launchRange.y;
        }
    }
}