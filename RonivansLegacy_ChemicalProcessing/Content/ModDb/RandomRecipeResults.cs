using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public static class RandomRecipeResults
	{
		public static bool GetRandomResultsforRecipe(ComplexRecipe recipe, out RecipeRandomResult randomResult)
		{
			randomResult = null;
			if (recipe.fabricators.Contains(Chemical_BallCrusherMillConfig.ID))
			{
				return BallCrusher_RandomResults.TryGetValue(recipe.ingredients[0].material, out randomResult);
			}
			return false;
		}


		private static Dictionary<Tag, RecipeRandomResult> _ballCrusher_RandomResults = null;
		public static Dictionary<Tag, RecipeRandomResult> BallCrusher_RandomResults
		{
			get
			{
				if (_ballCrusher_RandomResults == null)
				{
					InitRandomResults_BallCrusher();
				}
				return _ballCrusher_RandomResults;
			}
		}

		public static string GetBallCrusherResultsString(ComplexRecipe.RecipeElement[] recipeIngredients, ComplexRecipe.RecipeElement[] recipeResults)
		{
			var inputElement = recipeIngredients[0].material;
			var guaranteed = recipeResults[0].material;


			if (BallCrusher_RandomResults == null || !BallCrusher_RandomResults.ContainsKey(inputElement))
			{
				return string.Empty;
			}
			RecipeRandomResult result = BallCrusher_RandomResults[inputElement];

			string potentialResults=string.Empty;
			foreach (var potentialResult in result.RandomProductsRange)
			{
				var elementTag = potentialResult.Key.CreateTag();
				if (elementTag != guaranteed)
				{
					potentialResults += "\n• "+ elementTag.ProperName();
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

		public static void InitRandomResults_BallCrusher()
		{
			_ballCrusher_RandomResults = new();
			///rates taken from ronivans dictionary solution
			_ballCrusher_RandomResults.Add(SimHashes.SandStone.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 380)
				.AddProduct(LowGradeSand_Solid, 50, 100)
				.AddProduct(SimHashes.Fertilizer, 5, 15, 2f / 5f)
				);
			_ballCrusher_RandomResults.Add(SimHashes.SedimentaryRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 390)
				.AddProduct(LowGradeSand_Solid, 50, 90)
				.AddProduct(BaseGradeSand_Solid, 20, 50, 3f / 5f)
				);
			_ballCrusher_RandomResults.Add(SimHashes.Granite.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 360, 420)
				.AddProduct(BaseGradeSand_Solid, 30, 90)
				);
			_ballCrusher_RandomResults.Add(SimHashes.IgneousRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 360, 420)
				.AddProduct(BaseGradeSand_Solid, 30, 80)
				.AddProduct(SimHashes.Sulfur, 25, 45)
				);
			_ballCrusher_RandomResults.Add(SimHashes.MaficRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 300, 390)
				.AddProduct(LowGradeSand_Solid, 30, 70)
				.AddProduct(BaseGradeSand_Solid, 10, 50)
				.AddProduct(SimHashes.Phosphorus, 20, 55)
				);
			_ballCrusher_RandomResults.Add(SimHashes.Katairite.CreateTag(), //abyssalite
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 180, 270)
				.AddProduct(LowGradeSand_Solid, 50, 100)
				.AddProduct(BaseGradeSand_Solid, 50, 95)
				.AddProduct(SimHashes.Phosphorus, 25, 65)
				.AddProduct(SimHashes.Diamond, 10, 35, 4f / 5f)
				);
			_ballCrusher_RandomResults.Add(SimHashes.Regolith.CreateTag(),
				new RecipeRandomResult(490, 20, 40)
				.AddProduct(ToxicMix_Liquid, 440, 475)
				.AddProduct(LowGradeSand_Solid, 8, 25)
				.AddProduct(BaseGradeSand_Solid, 8, 25)
				.AddProduct(HighGradeSand_Solid, 2, 6)
				);
			_ballCrusher_RandomResults.Add(MeteorOre_Solid.Tag,
				new RecipeRandomResult(490, 20, 40)
				.AddProduct(ToxicMix_Liquid, 320, 420)
				.AddProduct(BaseGradeSand_Solid, 50, 90)
				.AddProduct(HighGradeSand_Solid, 20, 55)
				);
			///added separately

			///Shale: mirror of sedimentary rock bc it is described as sedimentary in its desc
			_ballCrusher_RandomResults.Add(SimHashes.Shale.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 390)
				.AddProduct(LowGradeSand_Solid, 50, 90)
				.AddProduct(BaseGradeSand_Solid, 20, 50, 3f / 5f)
				);
		}
	}
}
