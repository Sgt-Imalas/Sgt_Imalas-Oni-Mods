using Rockets_TinyYetBig.Behaviours;
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
        public override BuildingDef CreateBuildingDef()
        {
            string tubeKanim = Config.Instance.CompressInteriors ? "batterysm_kanim" : "batterysm_kanim";


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
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);

            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;

            buildingDef.PermittedRotations = PermittedRotations.FlipH;


            //buildingDef.OnePerWorld = true;

            buildingDef.RequiresPowerInput = false;
            SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.RocketInteriorBuilding);
            //component.AddTag(GameTags.UniquePerWorld);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RocketInterior);
            go.AddOrGet<NavTeleporter>();
            go.AddComponent<DockingDoor>();
        }
    }
}
