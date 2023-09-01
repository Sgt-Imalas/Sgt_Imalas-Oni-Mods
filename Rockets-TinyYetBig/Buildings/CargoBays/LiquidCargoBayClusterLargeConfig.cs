using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.CargoBays
{
    public class LiquidCargoBayClusterLargeConfig : IBuildingConfig
    {
        public const string ID = "RTB_LiquidCargoBayClusterLarge";
        public static float CAPACITY = !Config.Instance.RebalancedCargoCapacity ? CAPACITY_OFF : CAPACITY_ON;
        public static float CAPACITY_OFF = 50000f;
        public static float CAPACITY_ON = Config.Instance.LiquidCargoBayKgPerUnit * Config.Instance.CollossalCargoBayUnits;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] cargoMass;
            string[] construction_materials;
            if (Config.Instance.NeutroniumMaterial)
            {
                cargoMass = new[]
                 {
                    950f,
                    50f,
                };
                construction_materials = new string[]
                {
                MATERIALS.REFINED_METAL,
                ModAssets.Tags.NeutroniumAlloy.ToString(),
                };
            }
            else
            {
                cargoMass = new[]
                 {
                    1200f
                };
                construction_materials = new string[]
                {
                    SimHashes.Steel.ToString()
                };
            }
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 8, "rocket_cluster_storage_liquid_large_kanim", 1000, 60f, cargoMass, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.Invincible = true;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerInput = false;
            buildingDef.attachablePosition = new CellOffset(0, 0);
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
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 8), GameTags.Rocket, (AttachableBuilding) null)
            };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go = BuildingTemplates.ExtendBuildingToClusterCargoBay(go, CAPACITY, STORAGEFILTERS.LIQUIDS, CargoBay.CargoType.Liquids);
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MEGA);
        }
    }
}
