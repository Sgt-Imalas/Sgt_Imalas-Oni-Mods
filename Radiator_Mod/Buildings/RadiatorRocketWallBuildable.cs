using RoboRockets.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RadiatorMod.Buildings
{
    public class RadiatorRocketWallBuildable : RadiatorBaseConfig
    {
        public new const string ID = "RM_RadiatorRocketWallBuildable";

        public new static float[] matCosts = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;

        public new static string[] construction_materials = MATERIALS.REFINED_METALS;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {

            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "heat_radiator_kanim", 100, 120f, matCosts, construction_materials, 1600f, BuildLocationRule.Anywhere, none2, noise);

            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 1);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
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
            base.ConfigureBuildingTemplate(go, prefab_tag);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            base.DoPostConfigureComplete(go);
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.RocketEnvelopeTile);
            go.GetComponent<Deconstructable>().allowDeconstruction = false;
        }

    }
}
