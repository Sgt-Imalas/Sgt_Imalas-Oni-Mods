using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Cheese.Buildings
{
    internal class CheeseSculptureConfig : IBuildingConfig
    {
        public const string ID = "Cheese_CheeseSculpture";

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR4 = new float[] { 400f };
            string[] preciousRocks = new string[] { ModAssets.Tags.CheeseMaterial.ToString() };
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues decor = new EffectorValues()
            {
                amount = 20,
                radius = 8
            };
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "sculpture_cheese_kanim", 10, 120f, tieR4, preciousRocks, 1600f, BuildLocationRule.OnFloor, decor, noise);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.ViewMode = OverlayModes.Decor.ID;
            buildingDef.DefaultAnimState = "slab";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<BuildingComplete>().isArtable = true;
            go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
        }

        public override void DoPostConfigureComplete(GameObject go) => go.AddComponent<Sculpture>().defaultAnimName = "slab";
    }

}
