
using TUNING;
using UnityEngine;
namespace RoboPilot
{
    class PilotRoboStationConfig : IBuildingConfig
    {
        public const string ID = "RoboDock";
        public const float POWER_USAGE = 240f;

        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[1]
            {
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0] - PilotRoboConfig.MASS
            };
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RoboDock", 2, 2, "sweep_bot_base_station_kanim", 100, 30f, construction_mass, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 240f;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.SelfHeatKilowattsWhenActive = 0f;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            go.AddOrGet<CharacterOverlay>().shouldShowName = true;
        }

        public override void DoPostConfigureComplete(GameObject go) => go.AddOrGetDef<StorageController.Def>();
        }
    }
