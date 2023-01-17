using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    class AdvancedLaunchPadConfig : IBuildingConfig
    {
        public const string ID = "RTB_AdvancedLaunchPad";
        public const string LAUNCH_CHECKLIST_ID = "LaunchCheckList";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] buildingCosts = { 900f };
            string[] buildingMaterials = {MATERIALS.REFINED_METAL};
            EffectorValues tieR2 = TUNING.NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 2, "rocket_launchpad_advanced_kanim", 1000, 180f, buildingCosts, buildingMaterials, 9999f, BuildLocationRule.Anywhere, none, noise);
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.RequiresPowerInput = false;
            buildingDef.DefaultAnimState = "idle";
            buildingDef.CanMove = false;
            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.InputPort((HashedString) "TriggerLaunch",
                new CellOffset(-3, 0),
                (string)
                global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_ACTIVE,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_INACTIVE)
            };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort((HashedString) "LaunchReady", new CellOffset(3, 0),
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_ACTIVE,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_INACTIVE),

                LogicPorts.Port.RibbonOutputPort((HashedString) LAUNCH_CHECKLIST_ID, new CellOffset(3, 1),
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY,
                (string) STRINGS.BUILDINGS.PREFABS.RTB_ADVANCEDLAUNCHPAD.LOGIC_PORT_CATEGORY_READY_ACTIVE,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_INACTIVE),

                LogicPorts.Port.OutputPort((HashedString) "LandedRocket", new CellOffset(-3, 1)
                ,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET_ACTIVE,
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET_INACTIVE)
            };
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
            go.GetComponent<KPrefabID>().AddTag(ModAssets.Tags.RocketPlatformTag);
            go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
            {
                Storage.StoredItemModifier.Hide,
                Storage.StoredItemModifier.Seal,
                Storage.StoredItemModifier.Insulate
            });
            LaunchPad launchPad = go.AddOrGet<LaunchPad>();

            launchPad.triggerPort = (HashedString)"TriggerLaunch";
            launchPad.statusPort = (HashedString)"LaunchReady";
            launchPad.landedRocketPort = (HashedString)"LandedRocket";
            AdvancedRocketStatusProvider advStatus = go.AddOrGet<AdvancedRocketStatusProvider>();
            advStatus.rocketStateOutput = (HashedString)LAUNCH_CHECKLIST_ID;

            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[7];
            for (int index = 0; index < 7; ++index)
                fakeFloorAdder.floorOffsets[index] = new CellOffset(index - 3, 1);

            go.AddOrGet<LaunchPadConditions>();
            ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
            def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
            def.objectLayer = ObjectLayer.Building;
            go.AddOrGetDef<LaunchPadMaterialDistributor.Def>();
            go.AddOrGet<UserNameable>();
            go.AddOrGet<CharacterOverlay>().shouldShowName = true;

            ModularConduitPortTiler conduitPortTiler = go.AddOrGet<ModularConduitPortTiler>();
            conduitPortTiler.manageRightCap = true;
            conduitPortTiler.manageLeftCap = false;
            conduitPortTiler.leftCapDefaultSceneLayerAdjust = 1;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
