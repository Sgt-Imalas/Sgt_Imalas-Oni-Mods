using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Nosecones
{
    class NoseConeSolarConfig : IBuildingConfig
    {
        public const string ID = "RTB_NoseConeSolar";

        public const float W = 140f;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] mass = new float[] {
                350f,
                250f
            }; ;
            string[] construction_materials_ = new string[2]
            {
                "RefinedMetal",
                "Glass"
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 5,
                height: 2,
                anim: "rocket_nosecone_solar_kanim",
                hitpoints: 1000,
                construction_time: 70f,
                construction_mass: mass,
                construction_materials: construction_materials_,
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


            buildingDef.GeneratorWattageRating = W;
            buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;

            //buildingDef.PowerInputOffset = ModAssets.PLUG_OFFSET_SMALL;
            //buildingDef.PowerOutputOffset = ModAssets.PLUG_OFFSET_SMALL;
            //buildingDef.RequiresPowerOutput = true;
            //buildingDef.UseWhitePowerOutputConnectorColour = true;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.GetComponent<KPrefabID>().AddTag(GameTags.NoseRocketModule);

            go.AddComponent<RequireInputs>();
            go.AddComponent<PartialLightBlocking>();

        }


        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR_PLUS);
            go.GetComponent<ReorderableBuilding>().buildConditions.Add(new TopOnly());

            Prioritizable.AddRef(go);
            var solar = go.AddOrGet<ModuleSolarPanelAdjustable>();
            solar.showConnectedConsumerStatusItems = false;
            solar.Wattage = W;
            go.GetComponent<RocketModule>().operationalLandedRequired = false;
            //WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
            //virtualNetworkLink.visualizeOnly = true;
        }
    }
}

