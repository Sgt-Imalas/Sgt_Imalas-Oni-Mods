using System.Collections.Generic;
using UnityEngine;

namespace MugConversionRecipes.Content.Scripts
{
	internal class UncharmingComplexFabricator : ComplexFabricator
	{
		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			var products = base.SpawnOrderProduct(recipe);

			foreach(var item in products)
			{
				if (item.TryGetComponent<SpaceArtifact>(out var artifact))
					artifact.loadCharmed = false;
			}
			return products;
		}
	}
}
