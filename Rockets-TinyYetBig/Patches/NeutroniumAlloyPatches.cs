using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;

namespace Rockets_TinyYetBig.Patches
{
    internal class NeutroniumAlloyPatches
    {
        [HarmonyPatch(typeof(SupermaterialRefineryConfig))]
        [HarmonyPatch(nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
        public static class Patch_CraftingTableConfig_ConfigureRecipes
        {
            public static void Postfix()
            {
                AddAlloyRecipe();
            }
            private static void AddAlloyRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(ELEMENTpatches.UnobtaniumDust.Tag, 10f),
                    new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 90f),
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(ELEMENTpatches.UnobtaniumAlloy.Tag, 100f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
                new ComplexRecipe(recipeID, input, output)
                {
                    time = 40f,
                    description = STRINGS.ELEMENTS.UNOBTANIUMALLOY.RECIPE_DESCRIPTION,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
                };
            }
        }
    }
}
