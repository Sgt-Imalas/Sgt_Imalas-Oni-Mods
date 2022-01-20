using TUNING;
using UnityEngine;
namespace Robo_Rockets 
{
    class RoboRocketConfig : IBuildingConfig
    {
        public const string ID = "AiModule";
        public const string DisplayName = "Ai Module";
        public const string Description = "A Module to control your Rockets without dupes.";
        public const string Effect = "Fly far away...";


        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] hollowTieR1 = BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AiModule", 3, 3, "rocket_pioneer_cargo_module_kanim", 1000, 30f, hollowTieR1, refinedMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerInput = true;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery); 
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, (AttachableBuilding) null)
            };
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
           
        }
    }
}
