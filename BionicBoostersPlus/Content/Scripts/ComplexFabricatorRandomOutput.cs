using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BionicBoostersPlus.Content.Scripts
{
	internal class ComplexFabricatorRandomOutput : ComplexFabricator
	{
		[SerializeField] public float minPercentage = 0.4f, maxPercentage = 1f;
		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			var products = base.SpawnOrderProduct(recipe);

			foreach (var prod in products)
			{
				if (prod.TryGetComponent<PrimaryElement>(out var element))
				{
					float randomPercentage = UnityEngine.Random.Range(minPercentage, maxPercentage);
					float units = element.Units;

					//SgtLogger.l("Rolled random amount of products: " + randomPercentage);
					float percentageUnits = Mathf.Clamp(Mathf.RoundToInt(randomPercentage * units), 1, units * maxPercentage);
					element.Units = percentageUnits;
				}
			}
			return products;
		}
		public float MinAmount(ComplexRecipe recipe)
		{
			if (!recipe.results.Any())
				return 0;
			return MinAmount(recipe.results[0]);
		}
		public float MinAmount(ComplexRecipe.RecipeElement element)
		{
			float units = element.amount;
			return Mathf.Clamp(Mathf.RoundToInt(minPercentage * units), 1, units);
		}
		public float MaxAmount(ComplexRecipe recipe)
		{
			if (!recipe.results.Any())
				return 0;
			return MaxAmount(recipe.results[0]);
		}
		public float MaxAmount(ComplexRecipe.RecipeElement element)
		{
			float units = element.amount;
			return Mathf.Clamp(Mathf.RoundToInt(maxPercentage * units), 1, units);
		}
	}
}
