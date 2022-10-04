using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    class InvisibleLandingPlatformConfig : IBuildingConfig
    {
        public const string ID = "RTB_InvisibleLandingPlatform";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] buildingCosts =
            {
                100f
            };
            string[] buildingMaterials =
            {
                "Steel"
            };
            EffectorValues tieR2 = TUNING.NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 1, "steam_generator_module_kanim", 1000, 180f, buildingCosts, buildingMaterials, 9999f, BuildLocationRule.Anywhere, none, noise);
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.RequiresPowerInput = false;
            buildingDef.DefaultAnimState = "grounded";
            buildingDef.CanMove = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            //BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            //go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
            LaunchPad launchPad = go.AddOrGet<LaunchPad>();

            go.AddOrGet<LaunchPadConditions>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
        }
    }
}
