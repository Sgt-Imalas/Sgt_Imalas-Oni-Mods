using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;
using static Klei.SimUtil;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom
{
	class ComplexFabricatorRandomOutput : ComplexFabricator
	{
		public RecipeRandomResult DefaultOutput = null;
		protected void SpawnRandomProductsFromCurrentRecipe()
		{
			var outputSelection = GetRandomOutputSelection();
			if (outputSelection == null)
			{
				SgtLogger.warning("ComplexFabricatorRandomOutput: GetRandomOutputSelection returned null. This is not expected.");
				return;
			}

			var randomSourceIngredient = this.CurrentWorkingOrder.ingredients[0];
			if(outputSelection.TryGetValue(randomSourceIngredient.material, out var recipeRandomResult))
				SpawnProductsFor(recipeRandomResult);
			else if (DefaultOutput != null)
				SpawnProductsFor(DefaultOutput);
		}
		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			SpawnRandomProductsFromCurrentRecipe();
			return base.SpawnOrderProduct(recipe);
		}

		public void SpawnProductsFor(RecipeRandomResult recipeRandomResult)
		{
			var products = recipeRandomResult.GetRandomProducts();
			foreach (var productInfo in products)
			{

				var element = ElementLoader.FindElementByHash(productInfo.first);
				var pos = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore) + outputOffset;
				var product = element.substance.SpawnResource(pos, productInfo.second, recipeRandomResult.GetRandomOutputTemperature(), 0, 0);

				if (
					 //storeProduced ||
					 !element.IsSolid)
				{
					outStorage.Store(product);
				}
			}
		}
		public void SpawnAdditionalRandomProducts()
		{
			if (CurrentWorkingOrder == null)
				return;
			SpawnRandomProductsFromCurrentRecipe();
		}
		public virtual Dictionary<Tag, RecipeRandomResult> GetRandomOutputSelection()
		{
			return	new();
		}
	}
}
