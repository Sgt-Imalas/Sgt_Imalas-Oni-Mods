using AkisSnowThings.Content.Defs.Plants;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;

namespace AkisSnowThings.Patches
{
	internal class SupermaterialRefineryPatch
	{
		[HarmonyPatch(typeof(SupermaterialRefineryConfig), nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
		public class AddSeedRecipes
		{
			static ComplexRecipe AcornToPinecone;
			public static void Postfix(Immigration __instance)
			{
				AddArborAcornConversionRecipe();

				if(DlcManager.IsContentSubscribed(DlcManager.DLC2_ID))
					AddBonBonSeedConversionRecipe();
			}
			public static void AddArborAcornConversionRecipe() {
				//arbor acorn to pine cone
				RecipeElement[] input = [new RecipeElement(ForestTreeConfig.SEED_ID, 1f)];

				RecipeElement[] output = [new RecipeElement(EvergreenTreeConfig.SEED_ID, 1f)];
				string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				AcornToPinecone = new ComplexRecipe(recipeID, input, output)
				{
					time = 40,
					description = STRINGS.CREATURES.SPECIES.SEEDS.SNOWSCULPTURES_EVERGREEN_TREE.DESC,
					nameDisplay = RecipeNameDisplay.IngredientToResult,
					fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
				};
			}
			public static void AddBonBonSeedConversionRecipe()
			{

				//bonbon tree seed to arbor acorn
				RecipeElement[] input = [new RecipeElement(SpaceTreeConfig.SEED_ID, 1f)];

				RecipeElement[] output = [new RecipeElement(EvergreenTreeConfig.SEED_ID, 1f)];
				string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				AcornToPinecone = new ComplexRecipe(recipeID, input, output)
				{
					time = 40,
					description = STRINGS.CREATURES.SPECIES.SEEDS.SNOWSCULPTURES_EVERGREEN_TREE.DESC,
					nameDisplay = RecipeNameDisplay.IngredientToResult,
					fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
				};
			}
		}
	}
}
