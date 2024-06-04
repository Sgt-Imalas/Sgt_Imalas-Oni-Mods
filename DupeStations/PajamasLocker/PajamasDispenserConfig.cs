using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace DupeStations.PajamasLocker
{
    internal class PajamasDispenserConfig : IBuildingConfig
    {
        public const string ID = "DS_PajamasDispenser";
        private const float WORK_TIME = 1.5f;

        public static Tag PajamasMaterialTag = TagManager.Create("DS_PajamasTag");

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR3 = new float[] { 200f, 1f };
            string[] allMetals = new string[] { "Metal", "DS_PajamasTag" };
            EffectorValues none1 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.BONUS.TIER0;
            EffectorValues noise = none1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "gravitas_container_kanim", 30, 10f, tieR3, allMetals, 2400f, BuildLocationRule.OnFloor, none2, noise);
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.Building;
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<PajamasLocker>();
        }
    }
}
