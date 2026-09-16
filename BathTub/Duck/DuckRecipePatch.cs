using HarmonyLib;
using UtilLibs;
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
				RecipeBuilder.Create(CraftingTableConfig.ID, 15)
					.Input([SimHashes.Rubber, SimHashes.Polypropylene], 10)
					.Output(RubberDuckieConfig.ID, 1f)
					.NameDisplay(RecipeNameDisplay.Result)
					.Description(STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.BT_RUBBERDUCKIE.DESC)
					.Build();
			}
		}
	}
}
