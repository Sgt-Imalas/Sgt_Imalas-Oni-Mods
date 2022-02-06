using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace KnastoronOniMods
{
    class RocketAiControlstationConfig : IBuildingConfig
    {
        public static string ID = "RocketAiControlstation";
        public const float POWER_USAGE = 240f;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            string id = RocketAiControlstationConfig.ID;
            float[] tieR2_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] rawMetals = MATERIALS.RAW_METALS;
            EffectorValues tieR3 = TUNING.NOISE_POLLUTION.NOISY.TIER3;
            EffectorValues tieR2_2 = TUNING.BUILDINGS.DECOR.BONUS.TIER2;
            EffectorValues noise = tieR3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, "airbornecreaturetrap_kanim", 30, 60f, tieR2_1, rawMetals, 1600f, BuildLocationRule.OnFloor, tieR2_2,  noise);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "large";
            buildingDef.DefaultAnimState = "off";
            buildingDef.EnergyConsumptionWhenActive = 240f;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            //buildingDef.RequiresPowerInput = true;
            buildingDef.OnePerWorld = true;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<RocketAiControlStation>();
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(GameTags.RocketInteriorBuilding);
            component.AddTag(GameTags.UniquePerWorld);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
