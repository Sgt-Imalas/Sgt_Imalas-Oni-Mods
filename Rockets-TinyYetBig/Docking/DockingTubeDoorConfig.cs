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
    class DockingTubeDoorConfig : IBuildingConfig
    {
        public const string ID = "RTB_DockingTubeDoor";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;


        private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 0));
        private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));
        private ConduitPortInfo solidInputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(0, 0));

        private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));
        private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 1));
        private ConduitPortInfo solidOutputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(0, 1));

        public override BuildingDef CreateBuildingDef()
        {
            string tubeKanim = Config.Instance.CompressInteriors ? "rtb_docking_tube_kanim" : "rtb_docking_tube_kanim";


            float[] materialMass = new float[2]
            {
                200f,
                550f
            };
            string[] materialType = new string[2]
            {
                "RefinedMetal",
                "Transparent"
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues _decor = BUILDINGS.DECOR.BONUS.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 2,
                anim: tubeKanim,
                hitpoints: 1000,
                construction_time: 60f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 9999f,
                BuildLocationRule.OnWall,
                decor: _decor,
                noise: noiseLevel);

            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;

            buildingDef.PermittedRotations = PermittedRotations.FlipH;


            //buildingDef.OnePerWorld = true;

            buildingDef.RequiresPowerInput = false;
            //SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            //SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.RocketInteriorBuilding);
            component.AddTag(RoomConstraints.ConstraintTags.RocketInterior);
            component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            component.AddTag(GameTags.UniquePerWorld);
            IntitializeStorageConnections();
        }

        void IntitializeStorageConnections()
        {

        }



        public override void DoPostConfigureComplete(GameObject go)
        {
            UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());

            var ownable = go.AddOrGet<Ownable>();
            ownable.tintWhenUnassigned = false;
            ownable.slotID = Db.Get().AssignableSlots.WarpPortal.Id;
            go.AddOrGet<MoveToDocked>();
            go.AddOrGet<NavTeleporter>();
            go.AddComponent<DockingDoor>();

            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[]
            {
                new CellOffset(0, -1)
            };
            fakeFloorAdder.initiallyActive = true;
        }

        private void AttachPorts(GameObject go)
        {
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.solidInputPort;

            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.liquidOutputPort;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.gasOutputPort;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.solidOutputPort;
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
