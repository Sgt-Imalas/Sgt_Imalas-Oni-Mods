using HarmonyLib;
using RoboRockets.LearningBrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;
using static STRINGS.BUILDINGS.PREFABS;

namespace RoboRockets.Patches
{
    internal class AddBrainRecipe
    {
        [HarmonyPatch(typeof(SupermaterialRefineryConfig), "ConfigureBuildingTemplate")]
        public static class Patch_CraftingTableConfig_ConfigureRecipes
        {
            public static void Postfix()
            {
                AddBrainRecipe();
            }
            private static void AddBrainRecipe()
            {
                RecipeElement[] input = BrainConfig.ProductionCosts;

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(BrainConfig.ID, 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);

                BrainConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = 120f,
                    description = STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
                };
                BrainConfig.recipe.requiredTech = ModAssets.Techs.AiBrainsTech;
            }
        }

    }
}
