using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.Habitats
{
    public class HabitatModulePlatedNoseconeLargeConfig : IBuildingConfig
    {
        public const string ID = "RTB_HabitatModulePlatedLarge";
        private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-2, 0));
        private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(2, 0));
        private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(-2, 1));
        private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(2, 1));
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] denseTieR1 = new float[] { 300f,300 };
            string[] rawMetals = new[] 
            { 
                MATERIALS.REFINED_METAL ,
                ModAssets.Tags.RadiationShielding.ToString()
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 5,
                height: 5,
                anim: "rocket_habitat_plated_nosecone_kanim",
                hitpoints: 1000,
                construction_time: 40f,
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
                RocketryUtils.AddPowerPlugToModule(buildingDef, ModAssets.PLUG_OFFSET_MEDIUM);

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
            go.AddOrGet<PassengerRocketModule>().interiorReverbSnapshot = AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot;
            go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/habitat_nosecone_plated";
            go.AddOrGetDef<SimpleDoorController.Def>();
            go.AddOrGet<NavTeleporter>();
            go.AddOrGet<AccessControl>();
            go.AddOrGet<LaunchableRocketCluster>();
            go.AddOrGet<RocketCommandConditions>();
            go.AddOrGet<RocketProcessConditionDisplayTarget>();
            go.AddOrGet<CharacterOverlay>().shouldShowName = true;
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

            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MAJOR_PLUS);
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
            fakeFloorAdder.initiallyActive = false;
            go.AddOrGet<BuildingCellVisualizer>();
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new LimitOneCommandModule());
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new TopOnly());
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
