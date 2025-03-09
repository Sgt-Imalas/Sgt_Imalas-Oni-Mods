using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OniRetroEdition.ModPatches
{
	internal class DeepFryerConfigPatch
	{

        [HarmonyPatch(typeof(DeepfryerConfig), nameof(DeepfryerConfig.ConfigureRecipes))]
        public class DeepfryerConfig_ConfigureRecipes_Patch
        {
            public static void Postfix(DeepfryerConfig __instance)
			{
				ComplexRecipe.RecipeElement[] ingredients = [
					new ComplexRecipe.RecipeElement((Tag) "Meat", 1f),
					new ComplexRecipe.RecipeElement((Tag) "Tallow", 2f)	];
				ComplexRecipe.RecipeElement[] products = [new ComplexRecipe.RecipeElement((Tag) "DeepFriedMeat", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)];
				DeepFriedMeatConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("Deepfryer", (IList<ComplexRecipe.RecipeElement>)ingredients, (IList<ComplexRecipe.RecipeElement>)products), ingredients, products, [DlcManager.DLC2_ID])
				{
					time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME,
					description = global::STRINGS.ITEMS.FOOD.DEEPFRIEDMEAT.RECIPEDESC,
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
					fabricators = new List<Tag>() { (Tag)"Deepfryer"},
					sortOrder = 300
				};
			}
        }
	}
}
