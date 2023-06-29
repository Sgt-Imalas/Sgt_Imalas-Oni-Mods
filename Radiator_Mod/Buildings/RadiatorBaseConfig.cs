
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Radiator_Mod
{
    public class RadiatorBaseConfig : IBuildingConfig
    {
        public const string ID = "RadiatorBase";
        

        public static float[] matCosts = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;

        public static string[] construction_materials = MATERIALS.REFINED_METALS;

        public override BuildingDef CreateBuildingDef()
        {
          
            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 6, "heat_radiator_kanim", 100, 120f, matCosts, construction_materials, 1600f, BuildLocationRule.Anywhere, none2, noise);

            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.DefaultAnimState = "on";

            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.ThermalConductivity = 2f;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "small";

            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;

            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));

            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<RadiatorBase>();
            //GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            go.AddOrGet<LoopingSounds>();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());

            go.AddOrGet<LogicOperationalController>();

            MakeBaseSolid.Def solidBase = go.AddOrGetDef<MakeBaseSolid.Def>();
            solidBase.occupyFoundationLayer = false;
            solidBase.solidOffsets = new CellOffset[]
            {
                new CellOffset(0, 0),
                new CellOffset(1, 0)
            };
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
            BuildingTemplates.DoPostConfigure(go);
        }
        //public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => RadiatorBaseConfig.AddVisualPreview(go, true);

        //public override void DoPostConfigureUnderConstruction(GameObject go) => RadiatorBaseConfig.AddVisualPreview(go, false);

    }
}
