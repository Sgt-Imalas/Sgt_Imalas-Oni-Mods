using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStationBuilderModuleConfig : IBuildingConfig
    {
        public const string ID = "RTB_SpaceStationModuleBuilder";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] cargoMass =
            {
                1250f,
                150f,
            };
            string[] constructionmaterials = new string[]
            {
                MATERIALS.REFINED_METAL,
                ModAssets.Tags.NeutroniumAlloy.ToString(),
            };

            EffectorValues noiseLevel = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 5,
                height: 6,
                anim: "space_station_deployer_kanim",
                hitpoints: 1000,
                construction_time: 60f,
                construction_mass: cargoMass,
                construction_materials: constructionmaterials,
                melting_point: 9999f,
                BuildLocationRule.Anywhere,
                decor: none,
                noise: noiseLevel);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
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
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 6), GameTags.Rocket, (AttachableBuilding) null)
            };
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
        }


        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MONUMENTAL);
            go.AddOrGet<SpaceStationBuilder>();
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new OneModulePerRocket(ID));
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
        }
    }
}

