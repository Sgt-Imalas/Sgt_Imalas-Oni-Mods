using RadiatorMod.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RoboRockets.Buildings
{
    class RadiatorPanelConfig : IBuildingConfig
    {
        public const string ID = "RadiatorPart";
        public const string NAME = "Radiator Panel";
        public override BuildingDef CreateBuildingDef()
        {
            float[] matCosts = { 100f, 100f };

            string[] construction_materials= new string[2]
                {
                    "RefinedMetal"
                    ,"KATAIRITE"
                };
            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "heavywatttile_kanim", 100, 20f, matCosts, construction_materials, 1600f, BuildLocationRule.NotInTiles, none2, noise);
            buildingDef.Floodable = false;
            buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<panel>();
            
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
