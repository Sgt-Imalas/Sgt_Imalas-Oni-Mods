using HarmonyLib;
using OniRetroEdition.Entities;
using OniRetroEdition.Entities.Foods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using static ComplexRecipe;

namespace OniRetroEdition.ModPatches
{
    internal class RecipePatches
    {
        [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureRecipes")]
        public static class AddGasRangeRecipes
        {
            public static void Postfix()
            {
                //AddSushiRecipe();
            }
            private static void AddSushiRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement((Tag) LettuceConfig.ID, 1f),
                    new RecipeElement((Tag) FishMeatConfig.ID, 1f),
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(SushiConfig.ID, 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(GourmetCookingStationConfig.ID, input, output);

                SushiConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = FOOD.RECIPES.STANDARD_COOK_TIME,
                    description = global::STRINGS.ITEMS.FOOD.SUSHI.RECIPEDESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { GourmetCookingStationConfig.ID },
                    sortOrder = 600
                };
            }
        }
        [HarmonyPatch(typeof(CookingStationConfig), "ConfigureRecipes")]
        public static class AddCookingStationRecipes
        {
            public static void Postfix()
            {
                AddSushiRecipe();
            }
            private static void AddSushiRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement((Tag) LettuceConfig.ID, 1f),
                    new RecipeElement((Tag) FishMeatConfig.ID, 1f),
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(SushiConfig.ID, 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(GourmetCookingStationConfig.ID, input, output);

                SushiConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = FOOD.RECIPES.STANDARD_COOK_TIME,
                    description = global::STRINGS.ITEMS.FOOD.SUSHI.RECIPEDESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { GourmetCookingStationConfig.ID },
                    sortOrder = 600
                };
            }
        }

        [HarmonyPatch(typeof(RockCrusherConfig), "ConfigureBuildingTemplate")]
        public static class Add_RockCrusher_Recipes
        {
            public static void Postfix()
            {
                AddBoneCrushingRecipe();
            }
            private static void AddBoneCrushingRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement((Tag) BonesConfig.ID, 1f),
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Lime.CreateTag(), 5f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(RockCrusherConfig.ID, input, output);

                BonesConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = FOOD.RECIPES.STANDARD_COOK_TIME,
                    description = STRINGS.ITEMS.RETROONI_BONES.RECIPEDESC,
                    nameDisplay = RecipeNameDisplay.ResultWithIngredient,
                    fabricators = new List<Tag> { RockCrusherConfig.ID },
                    sortOrder = 600
                };
            }
        }
    }
}
