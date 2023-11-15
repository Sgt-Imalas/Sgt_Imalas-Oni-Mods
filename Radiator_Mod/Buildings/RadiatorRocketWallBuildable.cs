
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Radiator_Mod
{
    public class RadiatorRocketWallBuildable : IBuildingConfig
    {
        public const string ID = "RM_RadiatorRocketWallBuildable";

        public static float[] matCosts = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;

        public static string[] construction_materials = MATERIALS.REFINED_METALS;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {

            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "heat_radiator_rocket_module_kanim", 100, 120f, matCosts, construction_materials, 1600f, BuildLocationRule.OnWall, none2, noise);

            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 1);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.DefaultAnimState = "on";

            buildingDef.PermittedRotations = PermittedRotations.FlipH;
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
            go.AddOrGet<RadiatorBase>().SetRocketInternal();

            go.AddOrGet<LoopingSounds>();
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.RocketInteriorBuilding);
            component.AddTag(RoomConstraints.ConstraintTags.RocketInterior);
            component.AddTag(RocketInteriorOnlyBuilding);
        }
        public static Tag RocketInteriorOnlyBuilding = TagManager.Create("RTB_RocketInteriorOnly");
        public override void DoPostConfigureComplete(GameObject go)
        {
            UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());

            go.AddOrGet<LogicOperationalController>();

            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
            BuildingTemplates.DoPostConfigure(go);
        }

    }
}
