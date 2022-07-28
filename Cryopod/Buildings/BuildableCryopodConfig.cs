using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Cryopod.Buildings
{
    class BuildableCryopodConfig : IBuildingConfig
    {
        public const string ID = "CRY_BuildableCryoTank";

        public override BuildingDef CreateBuildingDef()
        {
            float[] mass = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] material = MATERIALS.REFINED_METALS;
            EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER4;
            EffectorValues decor = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "cryo_chamber_kanim", 100, 30f, mass, material, 1600f, BuildLocationRule.OnFloor, decor, noise);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 1200f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.125f;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            var ownable = go.AddOrGet<Ownable>();
            ownable.tintWhenUnassigned = false;
            ownable.slotID = Db.Get().AssignableSlots.WarpPortal.Id;
            go.AddOrGet<MinionStorage>();
            go.AddOrGet<CryopodReusable>();
            go.AddOrGet<CryopodFreezeWorkable>();
            go.AddOrGet<Prioritizable>();
        }
    }    
}
