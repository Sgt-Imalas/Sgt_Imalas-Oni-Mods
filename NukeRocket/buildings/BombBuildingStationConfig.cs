using ExplosiveMaterials.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static ComplexRecipe;

namespace ExplosiveMaterials.buildings
{
    class BombBuildingStationConfig : IBuildingConfig
    {
        public static string ID = "BombBuildStation";
        public const string NAME = "Explosives Workbench";
        public const string DESC = "Create all kinds of explosives";

        public override BuildingDef CreateBuildingDef()
        {
            float[] mass = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] material = MATERIALS.REFINED_METALS;
            EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER4;
            EffectorValues decor = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "material_research_centre_kanim", 100, 30f, mass, material, 1600f, BuildLocationRule.OnFloor, decor, noise);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PowerInputOffset = new CellOffset(2, 0);

            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<Prioritizable>();
            BombBuilderStation fabricator = go.AddOrGet<BombBuilderStation>();
            fabricator.heatedTemperature = 318.15f;
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            fabricator.storeProduced = false;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
            {
      Assets.GetAnim((HashedString) "anim_interacts_material_research_centre_kanim")
            };
            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
            Prioritizable.AddRef(go);


            this.ConfigureRecipes();
        }
        public override void DoPostConfigureComplete(GameObject go)
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
        private void ConfigureRecipes()
        {
            RecipeElement[] bombletEnrichedRecipe = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.EnrichedUranium.CreateTag(), 8f),
                new ComplexRecipe.RecipeElement(SimHashes.Hydrogen.CreateTag(), 24f),
                new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 8f)
            };
            ComplexRecipe.RecipeElement[] bombletNuclearProduct = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(BombletNuclearConfig.ID, 4f)
            };

            string recipeIDEnriched = ComplexRecipeManager.MakeRecipeID(ID, bombletEnrichedRecipe, bombletNuclearProduct);

            BombletNuclearConfig.recipe = new ComplexRecipe(recipeIDEnriched, bombletEnrichedRecipe, bombletNuclearProduct)
            {
                time = 30,
                description = "Provides a very explosive way of propulsion for the Orion Project.\n\nWARNING\nDO NOT DETONATE NEAR HUMANS!",
                nameDisplay = RecipeNameDisplay.ResultWithIngredient,
                fabricators = new List<Tag>()
                {
                    ID
                },
                //requiredTech = Db.Get().TechItems.atmoSuit.parentTechId,                
            };

            RecipeElement[] bombletDepletedRecipe = new ComplexRecipe.RecipeElement[]
{
                new ComplexRecipe.RecipeElement(SimHashes.DepletedUranium.CreateTag(), 32f),
                new ComplexRecipe.RecipeElement(SimHashes.Hydrogen.CreateTag(), 24f),
                new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 8f)
};

            string recipeID2 = ComplexRecipeManager.MakeRecipeID(ID, bombletDepletedRecipe, bombletNuclearProduct);

            BombletNuclearConfig.recipe = new ComplexRecipe(recipeID2, bombletDepletedRecipe, bombletNuclearProduct)
            {
                time = 50,
                description = "Provides a very explosive way of propulsion for the Orion Project.\n\nWARNING\nDO NOT DETONATE NEAR HUMANS!",
                nameDisplay = RecipeNameDisplay.ResultWithIngredient,
                fabricators = new List<Tag>()
                {
                    ID
                },
                //requiredTech = Db.Get().TechItems.atmoSuit.parentTechId,                
            };
        }
    }
}
