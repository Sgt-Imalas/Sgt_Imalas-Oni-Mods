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
    class RadiatorBaseConfig : IBuildingConfig
    {
        public const string ID = "RadiatorBase";
        public const string NAME = "Space Radiator";

        public static float[] matCosts = { 1200f
                    //, 400f 
            };

        public static string[] construction_materials = new string[]
            {
                    "RefinedMetal"
                // ,"KATAIRITE"
            };


        public override BuildingDef CreateBuildingDef()
        {
          
            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 6, "heat_radiator_kanim", 100, 120f, matCosts, construction_materials, 1600f, BuildLocationRule.Anywhere, none2, noise);
            BuildingTemplates.CreateFoundationTileDef(buildingDef);

            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);

            buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
            buildingDef.TileLayer = ObjectLayer.Building;

            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = true;

            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));

            //GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            //go.AddOrGet<LoopingSounds>();
            go.AddOrGet<RadiatorBase>();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());

            MakeBaseSolid.Def solidBase = go.AddOrGetDef<MakeBaseSolid.Def>();
            solidBase.occupyFoundationLayer = true;
            solidBase.solidOffsets = new CellOffset[]
            {
                new CellOffset(0, 0),
                new CellOffset(1, 0)
            };

            BuildingTemplates.DoPostConfigure(go);
        }
        //public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => RadiatorBaseConfig.AddVisualPreview(go, true);

        //public override void DoPostConfigureUnderConstruction(GameObject go) => RadiatorBaseConfig.AddVisualPreview(go, false);

        private static void AddVisualPreview(GameObject go, bool movable) { 
        
            var vis = go.AddOrGet<StationaryChoreRangeVisualizer>();
            vis.y = 1;
            vis.width = 2;
            vis.height = 5;
            vis.vision_offset = new CellOffset(0, 1);
            vis.blocking_tile_visible = false;
            vis.movable = movable;
        }
    }
}
