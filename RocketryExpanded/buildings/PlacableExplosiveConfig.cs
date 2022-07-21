using ExplosiveMaterials.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ExplosiveMaterials.buildings
{
    class PlacableExplosiveConfig : IBuildingConfig
    {
        public const string ID = "RemoteExplosive";
        public const string NAME = "Remote Explosive";
        public const string DESC = "place a explosive to trigger it remotely";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;


        public override BuildingDef CreateBuildingDef()
        {
            string explosives =
                ModAssets.Tags.BuildableExplosive.ToString();
            float[] mass ={5f, 1f };
            string[] material = {
                MATERIALS.REFINED_METALS[0],
                explosives
            };
            EffectorValues noise = TUNING.NOISE_POLLUTION.NONE;
            EffectorValues decor = TUNING.BUILDINGS.DECOR.PENALTY.TIER5;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "radiation_sensor_kanim", 100, 30f, mass, material, 1600f, BuildLocationRule.NotInTiles, decor, noise);
            

            return buildingDef;
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<ExplosiveBomblet>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddComponent<BombSideScreen>();
        }
    }
}