using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;

namespace ShockWormMob.OreDeposits
{
    internal class DrillbitsPatches
    {
        [HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
        public static class AdDrillbitRecipesToCraftingTable
        {
            public static void Postfix()
            {
                AddDrillbitRecipesForEachRefinedMetal();
            }
            private static void AddDrillbitRecipesForEachRefinedMetal()
            {                                                                                                                    //allow both refined metals   &         steel, thermium, niobium
                foreach (Element refinedMetal in ElementLoader.elements.FindAll((Element e) => e.IsSolid && (e.HasTag(GameTags.RefinedMetal))|| e.HasTag(GameTags.Alloy)))
                {

                    RecipeElement[] input = new RecipeElement[1]
                    {
                        new RecipeElement(refinedMetal.tag, 50f, true)
                    };
                    RecipeElement[] output = new RecipeElement[1]
                    {
                        new RecipeElement(TagManager.Create(DrillbitConfig.ID), 5f)
                    };


                    var recipeID = ComplexRecipeManager.MakeRecipeID(MetalRefineryConfig.ID, input, output);

                    ComplexRecipe complexRecipe = new ComplexRecipe(recipeID, input, output)
                    {
                        time = 10f,
                        description = DrillbitConfig.DESC_RECIPE,
                        nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
                        fabricators = new List<Tag>()
                        {
                            TagManager.Create(CraftingTableConfig.ID)
                        }
                    };

                }
            }
        }
    }

}
