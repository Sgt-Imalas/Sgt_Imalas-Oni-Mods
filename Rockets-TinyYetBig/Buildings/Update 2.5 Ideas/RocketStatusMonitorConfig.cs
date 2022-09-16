using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Update_2._5_Ideas
{
    class RocketStatusMonitorConfig : IBuildingConfig
    {
        public const string ID = "RTB_DockingTube";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[2]
            {
                250f,
                500f
            };
            string[] materialType = new string[2]
            {
                "Metal",
                "Transparent"
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues _decor = BUILDINGS.DECOR.BONUS.TIER1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 2,
                anim: "door_poi_internal_kanim",
                hitpoints: 1000,
                construction_time: 60f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 9999f,
                BuildLocationRule.Anywhere,
                decor: _decor,
                noise: noiseLevel);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);

            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;

            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.ObjectLayer = ObjectLayer.Building;

            buildingDef.PermittedRotations = PermittedRotations.FlipH;


            buildingDef.OnePerWorld = true;

            buildingDef.RequiresPowerInput = false;   
            SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.RocketInteriorBuilding);
            component.AddTag(GameTags.UniquePerWorld);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RocketInterior);
        }
    }
}
