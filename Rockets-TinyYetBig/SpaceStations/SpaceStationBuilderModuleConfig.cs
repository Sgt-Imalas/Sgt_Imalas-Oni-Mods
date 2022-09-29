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
        public const string ID = "RTB_SpaceStationHabitatModuleSmall";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] denseTieR0 = new float[] { 350f }; ;
            string[] rawMetals = MATERIALS.RAW_METALS;
            EffectorValues noiseLevel = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 3,
                height: 4,
                anim: "rocket_nosecone_small_extended_kanim",
                hitpoints: 1000,
                construction_time: 40f,
                construction_mass: denseTieR0,
                construction_materials: rawMetals,
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
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.GetComponent<KPrefabID>().AddTag(GameTags.NoseRocketModule);
            go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule);
            go.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;
            go.AddOrGet<PassengerRocketModule>().interiorReverbSnapshot = AudioMixerSnapshots.Get().SmallRocketInteriorReverbSnapshot;
            go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/habitat_small_expanded";

            go.AddOrGet<LaunchableRocketCluster>();
            
        }


        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
            Ownable ownable = go.AddOrGet<Ownable>();
            ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;
            ownable.canBePublic = false;
            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[3]
            {
      new CellOffset(-1, -1),
      new CellOffset(0, -1),
      new CellOffset(1, -1)
            };
            fakeFloorAdder.initiallyActive = false;
            go.AddOrGet<BuildingCellVisualizer>();
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new LimitOneCommandModule());
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new TopOnly());
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddOrGet<BuildingCellVisualizer>();
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<BuildingCellVisualizer>();
        }
    }
}

