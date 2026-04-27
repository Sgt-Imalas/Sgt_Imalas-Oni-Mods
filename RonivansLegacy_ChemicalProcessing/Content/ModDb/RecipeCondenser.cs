using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public static class RecipeCondenser
	{
		static Dictionary<ComplexRecipe, List<ComplexRecipe>> derivedRecipes = new Dictionary<ComplexRecipe, List<ComplexRecipe>>();
		static Dictionary<ComplexRecipe, ComplexRecipe> derivedRecipesInvertedMap = new Dictionary<ComplexRecipe, ComplexRecipe>();
		internal static void CollectDerivedRecipes(ComplexRecipe sourceRecipe, List<ComplexRecipe> derivedList)
		{
			if(derivedList != null && derivedList.Count > 1)
			{
				derivedRecipes[sourceRecipe] = [.. derivedList];
				foreach(var recipe in derivedList)
				{
					derivedRecipesInvertedMap[recipe] = sourceRecipe;
				}
			}
		}

		internal static bool IsDerivedRecipe(ComplexRecipe recipe, out ComplexRecipe sourceRecipe)
		{
			return derivedRecipesInvertedMap.TryGetValue(recipe, out sourceRecipe);
		}

		public static List<Tuple<Tag,float>>[] GetAllIngredientVariants(ComplexRecipe recipe)
		{
			if(derivedRecipesInvertedMap.TryGetValue(recipe, out var sourceRecipe))
				recipe = sourceRecipe;

			if(derivedRecipes.TryGetValue(recipe, out var recipeList))
			{
				int ingredientCount = recipeList.First().ingredients.Count();
				List<Tuple<Tag, float>>[] ingredientsMap = new List<Tuple<Tag, float>>[ingredientCount];
				foreach(var derivedRecipe in recipeList)
				{
					for (int i = 0; i < derivedRecipe.ingredients.Count(); i++)
					{
						if(ingredientsMap[i] == null)
							ingredientsMap[i] = new List<Tuple<Tag,float>>();
						var ingredient = new Tuple<Tag, float>(derivedRecipe.ingredients[i].material, derivedRecipe.ingredients[i].amount);
						if (!ingredientsMap[i].Contains(ingredient))
							ingredientsMap[i].Add(ingredient);
					}
				}
				return ingredientsMap;
			}
			return null;
		}
	}
}
