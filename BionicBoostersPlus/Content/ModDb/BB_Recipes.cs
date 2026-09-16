using BionicBoostersPlus.Content.Defs.Buildings;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_Recipes
	{
		internal static void RegisterRecipes_PostLoadEntities()
		{
			RegisterRecyclingRecipes();
		}


		static float GetMicrochipsForRecipe(ComplexRecipe recipe)
		{
			float circuitsAmount = 0;

			Tag microchipId = PowerStationToolsConfig.tag;
			foreach (var ingredient in recipe.ingredients)
			{
				if (ingredient.material == microchipId)
					circuitsAmount += ingredient.amount;

				if (BoosterCraftingRecipes.TryGetValue(ingredient.material, out var preRequisiteBooster))
					circuitsAmount += GetMicrochipsForRecipe(preRequisiteBooster) * ingredient.amount;
			}
			return circuitsAmount;
		}
		static readonly Dictionary<Tag, ComplexRecipe> BoosterCraftingRecipes = [];

		public static void RegisterRecyclingRecipes()
		{
			string id = BoosterRecyclerConfig.ID;
			Tag microchipId = PowerStationToolsConfig.tag;

			var recipeManager = ComplexRecipeManager.Get();
			if (recipeManager == null)
				return;
			var recipesToCheck = recipeManager.preProcessRecipes.ToList();


			///mapping booster recipes for microchip yield calculation
			foreach (var recipe in recipesToCheck)
			{
				if (!recipe.results.Any())
					continue;
				var boosterId = recipe.results.First().material;

				if (BionicUpgradeComponentConfig.UpgradesData.TryGetValue(boosterId, out BionicUpgradeComponentConfig.BionicUpgradeData data))
				{
					BoosterCraftingRecipes[boosterId] = recipe;
				}
			}


			foreach (var recipe in recipesToCheck)
			{
				if (!recipe.results.Any())
					continue;

				var boosterId = recipe.results.First().material;

				if (BionicUpgradeComponentConfig.UpgradesData.TryGetValue(boosterId, out BionicUpgradeComponentConfig.BionicUpgradeData data))
				{
					var circuitsCount = GetMicrochipsForRecipe(recipe);

					RecipeBuilder.Create(id, 20)
						.Input(boosterId, 1)
						.Output(microchipId, circuitsCount)
						.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
						.Description(STRINGS.BUILDINGS.PREFABS.BBP_BOOSTERRECYCLER.RECIPE_FORMAT, 1, 0)
						.Build();
				}
			}
		}

	}
}
