using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class RockCrusherConfig_Patches
    {
		[HarmonyPatch(typeof(RockCrusherConfig), nameof(RockCrusherConfig.ConfigureBuildingTemplate))]
        public class RockCrusherConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyPriority(Priority.VeryHigh)]
			public static void Postfix()
			{
				FixNegativeCrusherRecipes();
				AdditionalRecipes.RegisterRecipes_RockCrusher();
			}

			/// <summary>
			/// rock crusher has a faulty implementation of secondary byproducts that breaks down if the secondary element is >50%.
			/// fixing that.
			/// </summary>
			static void FixNegativeCrusherRecipes()
			{
				var recipeManager = ComplexRecipeManager.Get();
				if (recipeManager == null)
					return;

				var wrongAmountOfByproducts = recipeManager.preProcessRecipes
					.Where(r => r.fabricators.Contains(RockCrusherConfig.ID))
					.Where(r => r.ingredients.Any(i => ElementLoader.GetElement(i.material)?.HasTag(GameTags.UseSmeltingByproducts) ?? false));

				foreach (var recipe in wrongAmountOfByproducts)
				{
					var byproductIngredient = recipe.ingredients.First(i => ElementLoader.GetElement(i.material)?.HasTag(GameTags.UseSmeltingByproducts) ?? false);
					SgtLogger.l("Fixing rock crusher recipe for " + byproductIngredient.material);

					var properAmounts = RecipeBuilder.Create(RockCrusherConfig.ID,40)
						.OutputOreTransition(byproductIngredient.material, 50)
						.Output(SimHashes.Sand, 50);
					var element = ElementLoader.GetElement(byproductIngredient.material);

					recipe.results = properAmounts.GetOutputs();
				}
			}
        }
    }
}
