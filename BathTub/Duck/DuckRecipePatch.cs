using HarmonyLib;
using System.Collections.Generic;
using static ComplexRecipe;

namespace BathTub.Duck
{
	internal class DuckRecipePatch
	{
		[HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
		public static class Patch_CraftingTableConfig_ConfigureRecipes
		{
			public static void Postfix()
			{
				AddDuckRecipe();
			}

			private static void AddDuckRecipe()
			{
				var plasticTag = ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag;
				RecipeElement[] input = new RecipeElement[]
				{
						new RecipeElement(plasticTag, 10f),
				};

				RecipeElement[] output = new RecipeElement[]
				{
						new RecipeElement(RubberDuckieConfig.ID, 1f)
				};

				string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);
				RubberDuckieConfig.recipe = new ComplexRecipe(recipeID, input, output)
				{
					time = 15,
					description = STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.BT_RUBBERDUCKIE.DESC,
					nameDisplay = RecipeNameDisplay.IngredientToResult,
					fabricators = new List<Tag> { CraftingTableConfig.ID }
				};

			}
		}
	}
}
