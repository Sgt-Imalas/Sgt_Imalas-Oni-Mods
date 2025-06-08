using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public static class RandomRecipeResults
	{
		private static Dictionary<Tag, Dictionary<Tag, RecipeRandomResult>> _randomRecipeResultsCollection = null;
		private static void InitRandomResults()
		{
			_randomRecipeResultsCollection = new()
			{
				{Chemical_BallCrusherMillConfig.ID,InitRandomResults_BallCrusher() },
				{Chemical_SelectiveArcFurnaceConfig.ID,InitRandomResults_SelectiveArcFurnace() },
			};
		}

		public static bool GetRandomResultsforRecipe(ComplexRecipe recipe, out RecipeRandomResult randomResult)
		{
			randomResult = null;
			foreach (var fabricator in recipe.fabricators)
			{
				if (GetRandomResultList(fabricator, out var results)
					&& results.TryGetValue(recipe.ingredients[0].material, out randomResult))
					return true;
			}
			return false;
		}
		public static bool GetRandomResultList(Tag buildingID, out Dictionary<Tag, RecipeRandomResult> results)
		{
			results = null;
			if (_randomRecipeResultsCollection == null)
				InitRandomResults();

			return _randomRecipeResultsCollection.TryGetValue(buildingID, out results);
		}
		public static string GetBallCrusherRandomResultsString(ComplexRecipe.RecipeElement[] recipeIngredients, ComplexRecipe.RecipeElement[] recipeResults)
		{
			var inputElement = recipeIngredients[0].material;
			var guaranteed = recipeResults[0].material;

			if (!GetRandomResultList(Chemical_BallCrusherMillConfig.ID, out var recipes)|| !recipes.TryGetValue(inputElement, out RecipeRandomResult result))
			{
				return string.Empty;
			}

			string potentialResults = string.Empty;
			foreach (var potentialResult in result.RandomProductsRange)
			{
				var elementTag = potentialResult.Key.CreateTag();
				if (elementTag != guaranteed)
				{
					potentialResults += "\n• " + elementTag.ProperName();
				}
			}

			if (recipeIngredients.Length == 4)
				return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.BALLCRUSHER_MILLING_3INGREDIENTS,
					recipeIngredients[0].material.ProperName(),
					recipeIngredients[1].material.ProperName(),
					recipeIngredients[2].material.ProperName(),
					recipeIngredients[3].material.ProperName(),
					potentialResults,
					guaranteed.ProperName());
			else
				return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.BALLCRUSHER_MILLING_2INGREDIENTS,
					recipeIngredients[0].material.ProperName(),
					recipeIngredients[1].material.ProperName(),
					recipeIngredients[2].material.ProperName(),
					potentialResults,
					guaranteed.ProperName());
		}
		public static string GetArcFurnaceRandomResultString(ComplexRecipe.RecipeElement[] recipeIngredients, ComplexRecipe.RecipeElement[] recipeResults)
		{
			var inputElement = recipeIngredients[0].material;
			var guaranteed = recipeResults[0].material;

			if (!GetRandomResultList(Chemical_SelectiveArcFurnaceConfig.ID, out var recipes) || !recipes.TryGetValue(inputElement, out RecipeRandomResult result))
			{
				return string.Empty;
			}

			string potentialResults = string.Empty;
			foreach (var potentialResult in result.RandomProductsRange)
			{
				var elementTag = potentialResult.Key.CreateTag();
				if (elementTag != guaranteed)
				{
					potentialResults += "\n• " + elementTag.ProperName();
				}
			}
			return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.ARCFURNACE_RANDOM_RECIPE,
					recipeIngredients[0].material.ProperName(),
					potentialResults,
					guaranteed.ProperName());
		}

		private static Dictionary<Tag, RecipeRandomResult> InitRandomResults_BallCrusher()
		{
			var results = new Dictionary<Tag, RecipeRandomResult>();
			///rates taken from ronivans dictionary solution
			results.Add(SimHashes.SandStone.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 380)
				.AddProduct(LowGradeSand_Solid, 50, 100)
				.AddProduct(SimHashes.Fertilizer, 5, 15, 2f / 5f)
				);
			results.Add(SimHashes.SedimentaryRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 390)
				.AddProduct(LowGradeSand_Solid, 50, 90)
				.AddProduct(BaseGradeSand_Solid, 20, 50, 3f / 5f)
				);
			results.Add(SimHashes.Granite.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 360, 420)
				.AddProduct(BaseGradeSand_Solid, 30, 90)
				);
			results.Add(SimHashes.IgneousRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 360, 420)
				.AddProduct(BaseGradeSand_Solid, 30, 80)
				.AddProduct(SimHashes.Sulfur, 25, 45)
				);
			results.Add(SimHashes.MaficRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 300, 390)
				.AddProduct(LowGradeSand_Solid, 30, 70)
				.AddProduct(BaseGradeSand_Solid, 10, 50)
				.AddProduct(SimHashes.Phosphorus, 20, 55)
				);
			results.Add(SimHashes.Katairite.CreateTag(), //abyssalite
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 180, 270)
				.AddProduct(LowGradeSand_Solid, 50, 100)
				.AddProduct(BaseGradeSand_Solid, 50, 95)
				.AddProduct(SimHashes.Phosphorus, 25, 65)
				.AddProduct(SimHashes.Diamond, 10, 35, 4f / 5f)
				);
			results.Add(SimHashes.Regolith.CreateTag(),
				new RecipeRandomResult(490, 20, 40)
				.AddProduct(ToxicMix_Liquid, 440, 475)
				.AddProduct(LowGradeSand_Solid, 8, 25)
				.AddProduct(BaseGradeSand_Solid, 8, 25)
				.AddProduct(HighGradeSand_Solid, 2, 6)
				);
			results.Add(MeteorOre_Solid.Tag,
				new RecipeRandomResult(490, 20, 40)
				.AddProduct(ToxicMix_Liquid, 320, 420)
				.AddProduct(BaseGradeSand_Solid, 50, 90)
				.AddProduct(HighGradeSand_Solid, 20, 55)
				);
			///added separately

			///Shale: mirror of sedimentary rock bc it is described as sedimentary in its desc
			results.Add(SimHashes.Shale.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 390)
				.AddProduct(LowGradeSand_Solid, 50, 90)
				.AddProduct(BaseGradeSand_Solid, 20, 50, 3f / 5f)
				);
			return results;
		}
		private static Dictionary<Tag, RecipeRandomResult> InitRandomResults_SelectiveArcFurnace()
		{
			var results = new Dictionary<Tag, RecipeRandomResult>();
			results.Add(LowGradeSand_Solid.Tag,
				new RecipeRandomResult(90, 40, 60)
				.AddProduct(SimHashes.Copper, 15, 60)
				.AddProduct(Zinc_Solid, 15, 60)
				.AddProduct(SimHashes.Lead, 15, 60)
				.AddProduct(Silver_Solid, 15, 60)
				.ProductCountRange(2, 4)
				);

			results.Add(BaseGradeSand_Solid.Tag,
				new RecipeRandomResult(90, 40, 60)
				.AddProduct(SimHashes.Aluminum, 15, 60)
				.AddProduct(SimHashes.Iron, 15, 60)
				.AddProduct(SimHashes.Gold, 15, 60)
				.AddProduct(SimHashes.Tungsten, 15, 60)
				.ProductCountRange(2, 4)
				);

			results.Add(HighGradeSand_Solid.Tag,
				new RecipeRandomResult(90, 40, 60)
				.AddProduct(SimHashes.Tungsten, 75, 80)
				.AddProduct(SimHashes.Fullerene, 7, 12)
				.AddProduct(SimHashes.Niobium, 1, 4)
				);
			return results;

		}
	}
}
