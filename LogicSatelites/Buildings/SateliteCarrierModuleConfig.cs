using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace LogicSatelites.Buildings
{
    class SateliteCarrierModuleConfig : IBuildingConfig
    {
        public const string ID = "LS_SatelliteCarrierModule";
        public const string Name = "Satellite Carrier Module";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] hollowTieR1 = BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1;
            string[] rawMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "satelite_deployer_module_kanim", 1000, 30f, hollowTieR1, rawMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.DefaultAnimState = "satelite_construction";
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerInput = false;
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            Storage storage = go.AddComponent<Storage>();
            storage.showInUI = true;
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            BuildingInternalConstructor.Def def1 = go.AddOrGetDef<BuildingInternalConstructor.Def>();
            def1.constructionMass = 500f;
            def1.outputIDs = new List<string>()
            {
                "LS_ClusterSateliteLogicDeployer",
            };
            def1.spawnIntoStorage = true;
            def1.storage = (DefComponent<Storage>)storage;
            def1.constructionSymbol = "under_construction";
            go.AddOrGet<BuildingInternalConstructorWorkable>().SetWorkTime(30f);
            JettisonableCargoModule.Def def2 = go.AddOrGetDef<JettisonableCargoModule.Def>();
            def2.landerPrefabID = "ScoutLander".ToTag();
            def2.landerContainer = (DefComponent<Storage>)storage;
            def2.clusterMapFXPrefabID = "DeployingScoutLanderFXConfig";
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
            };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Prioritizable.AddRef(go);
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
        }
    }
}
