using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using FMODUnity;

namespace Rockets_TinyYetBig
{
    class HabitatModuleMediumExpandedConfig : IBuildingConfig
    {
        public const string ID = "RTB_HabitatModuleMediumExpanded";
        private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-2, 0));
        private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(2, 0));
        private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(-2, 5));
        private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(2, 5));

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] denseTieR1;
            string[] rawMetals;

            if (Config.Instance.NeutroniumMaterial)
            {
                denseTieR1 = new float[] { 750f, 100f };
                rawMetals = new[]
                {
                MATERIALS.REFINED_METAL,
                ModAssets.Tags.NeutroniumAlloy.ToString()
                };
            }
            else
            {
                denseTieR1 = new float[] { 950f };
                rawMetals = new[]
                {
                 SimHashes.Steel.ToString()
                };
            }
            EffectorValues noiseLevel = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 5,
                height: 6,
                anim: "rocket_habitat_medium_module_extended_kanim",
                hitpoints: 1000,
                construction_time: 70f,
                construction_mass: denseTieR1,
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

            if (Config.Instance.HabitatPowerPlug)
                UtilLibs.RocketryUtils.AddPowerPlugToModule(buildingDef, ModAssets.PLUG_OFFSET_MEDIUM);

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule);
            go.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;
            go.AddOrGet<PassengerRocketModule>().interiorReverbSnapshot = AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot;
            go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/habitat_medium_expanded";
            go.AddOrGetDef<SimpleDoorController.Def>();
            go.AddOrGet<NavTeleporter>();
            go.AddOrGet<AccessControl>();
            go.AddOrGet<LaunchableRocketCluster>();
            go.AddOrGet<RocketCommandConditions>();
            go.AddOrGet<RocketProcessConditionDisplayTarget>();
            go.AddOrGet<CharacterOverlay>().shouldShowName = true;
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 6), GameTags.Rocket, (AttachableBuilding) null)
            };
            Storage storage1 = go.AddComponent<Storage>();
            storage1.showInUI = false;
            storage1.capacityKg = 10f;
            RocketConduitSender rocketConduitSender1 = go.AddComponent<RocketConduitSender>();
            rocketConduitSender1.conduitStorage = storage1;
            rocketConduitSender1.conduitPortInfo = this.liquidInputPort;
            go.AddComponent<RocketConduitReceiver>().conduitPortInfo = this.liquidOutputPort;

            Storage storage2 = go.AddComponent<Storage>();
            storage2.showInUI = false;
            storage2.capacityKg = 1f;
            RocketConduitSender rocketConduitSender2 = go.AddComponent<RocketConduitSender>();
            rocketConduitSender2.conduitStorage = storage2;
            rocketConduitSender2.conduitPortInfo = this.gasInputPort;
            go.AddComponent<RocketConduitReceiver>().conduitPortInfo = this.gasOutputPort;
        }

        private void AttachPorts(GameObject go)
        {
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.liquidOutputPort;
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.gasOutputPort;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            if (Config.Instance.HabitatPowerPlug)
            {
                WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
                virtualNetworkLink.link1 = ModAssets.PLUG_OFFSET_MEDIUM;
                virtualNetworkLink.visualizeOnly = true;
            }

            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MEGA);
            Ownable ownable = go.AddOrGet<Ownable>();
            ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;
            ownable.canBePublic = false;
            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[5]
            {
      new CellOffset(-2, -1),
      new CellOffset(-1, -1),
      new CellOffset(0, -1),
      new CellOffset(1, -1),
      new CellOffset(2, -1)
            };
            fakeFloorAdder.initiallyActive = false;
            go.AddOrGet<BuildingCellVisualizer>();
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new LimitOneCommandModule());
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddOrGet<BuildingCellVisualizer>();
            this.AttachPorts(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<BuildingCellVisualizer>();
            this.AttachPorts(go);
        }
    }
}
