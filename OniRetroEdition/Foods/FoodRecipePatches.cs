using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using static ComplexRecipe;

namespace OniRetroEdition.Foods
{
    internal class FoodRecipePatches
    {
        [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureRecipes")]
        public static class AddGasRangeRecipes
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
    }
}
