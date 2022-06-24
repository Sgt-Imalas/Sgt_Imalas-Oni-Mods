using LogicSatelites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static ComplexRecipe;

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
            go.AddOrGet<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            Storage storage = go.AddComponent<Storage>();
            storage.showInUI = true;
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

            ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
            fabricator.heatedTemperature = 318.15f;
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            fabricator.storeProduced = true;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "anim_interacts_material_research_centre_kanim")
            };


            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);

            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
            };

            this.ConfigureRecipes();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Prioritizable.AddRef(go);
            AddFabricatorSkillInteraction(go);
            AddFakeFloor(go);
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
        }

        private void AddFabricatorSkillInteraction(GameObject go)
        {
            go.GetComponent<KPrefabID>().prefabSpawnFn += (KPrefabID.PrefabFn)(game_object =>
            {
                ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
                component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
                component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
                component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
                component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
                component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            });

        }
        private void AddFakeFloor(GameObject go)
        {
            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[5]
            {
                new CellOffset(-2, -1),
                new CellOffset(-1, -1),
                new CellOffset(0, -1),
                new CellOffset(1, -1),
                new CellOffset(2, -1)
            };
            fakeFloorAdder.initiallyActive = false;
        }


        private void ConfigureRecipes()
        {
            RecipeElement[] logicSatelliteComponents = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Glass.CreateTag(), 400f),
                new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 200f)
            };
            ComplexRecipe.RecipeElement[] logicSatelliteProduct = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SatelliteLogicConfig.ID, 1f)
            };

            string LogicSatellite = ComplexRecipeManager.MakeRecipeID(ID, logicSatelliteComponents, logicSatelliteProduct);

            SatelliteLogicConfig.recipe = new ComplexRecipe(LogicSatellite, logicSatelliteComponents, logicSatelliteProduct)
            {
                time = 30,
                description = "A logic relay to be deployed into space.",
                nameDisplay = RecipeNameDisplay.Result,
                fabricators = new List<Tag>()
                {
                    ID
                },              
            };
            
        }
    }
}
