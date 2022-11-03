using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings
{
    class SpaceStationDockingDoorConfig : IBuildingConfig
    {
        public const string ID = "RTB_SpaceStationDockingDoor";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            string tubeKanim = Config.Instance.CompressInteriors ? "rocket_control_station_kanim" : "rocket_control_station_kanim";


            float[] materialMass = new float[]
            {
                400f,
                //550f
            };
            string[] materialType = new string[]
            {
                "RefinedMetal"
                //,"Transparent"
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues _decor = BUILDINGS.DECOR.PENALTY.TIER2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 2,
                height: 2,
                anim: tubeKanim,
                hitpoints: 100,
                construction_time: 60f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 9999f,
                BuildLocationRule.NotInTiles,
                decor: _decor,
                noise: noiseLevel);

            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = true;
            buildingDef.Entombable = true;

            buildingDef.PermittedRotations = PermittedRotations.Unrotatable;


            //buildingDef.OnePerWorld = true;

            buildingDef.RequiresPowerInput = false;
            //SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            //SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.RocketInteriorBuilding);
            component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
            component.AddTag(ModAssets.Tags.RocketPlatformTag);
            //component.AddTag(GameTags.UniquePerWorld);
            //IntitializeStorageConnections();
            go.AddOrGetDef<DockedRocketMaterialDistributor.Def>();

            ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
            def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
            def.objectLayer = ObjectLayer.Building;

            ModularConduitPortTiler conduitPortTiler = go.AddOrGet<ModularConduitPortTiler>();
            conduitPortTiler.manageRightCap = true;
            conduitPortTiler.manageLeftCap = false;
            conduitPortTiler.leftCapDefaultSceneLayerAdjust = 1;
        }

        void IntitializeStorageConnections()
        {

        }



        public override void DoPostConfigureComplete(GameObject go)
        {
            var ownable = go.AddOrGet<Ownable>();
            ownable.tintWhenUnassigned = false;
            ownable.slotID = Db.Get().AssignableSlots.WarpPortal.Id;
            go.AddOrGet<MoveToDocked>();
            go.AddOrGet<NavTeleporter>();
            go.AddComponent<DockingDoor>(); AddFakeFloor(go);
        }

        //private void AttachPorts(GameObject go)
        //{
        //    go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
        //    go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
        //    go.AddComponent<ConduitSecondaryInput>().portInfo = this.solidInputPort;

        //    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.liquidOutputPort;
        //    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.gasOutputPort;
        //    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.solidOutputPort;
        //}

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddOrGet<BuildingCellVisualizer>();
            //this.AttachPorts(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<BuildingCellVisualizer>();
            //this.AttachPorts(go);
        }
        private void AddFakeFloor(GameObject go)
        {
            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[]
            {
                new CellOffset(0, -1),
                new CellOffset(1, -1)
            };
            fakeFloorAdder.initiallyActive = false;
        }
    }
}
