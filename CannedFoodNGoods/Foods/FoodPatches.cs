using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using static ComplexRecipe;

namespace CannedFoods.Foods
{
    
    internal class FoodPatches
    {
        [HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
        public static class Patch_CookingStationConfig_ConfigureRecipes
        {
            public static void Postfix()
            {
                AddCannedTunaRecipe();
                AddCannedBBQRecipe();
            }

            private static void AddCannedTunaRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Iron.CreateTag(), 0.5f),
                    new RecipeElement(CookedFishConfig.ID, 0.5f)
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(cannedTunaConfig.ID, 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(GourmetCookingStationConfig.ID, input, output);

                cannedTunaConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = FOOD.RECIPES.SMALL_COOK_TIME,
                    description = STRINGS.ITEMS.FOOD.CF_CANNEDFISH.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { CraftingTableConfig.ID }
                };
            }

            private static void AddCannedBBQRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Iron.CreateTag(), 0.5f),
                    new RecipeElement(CookedMeatConfig.ID, 0.5f)
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(cannedTunaConfig.ID, 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(GourmetCookingStationConfig.ID, input, output);

                CookedMeatConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = FOOD.RECIPES.SMALL_COOK_TIME,
                    description = STRINGS.ITEMS.FOOD.CF_CANNEDBBQ.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { CookingStationConfig.ID }
                };
            }
        }
    }
}
