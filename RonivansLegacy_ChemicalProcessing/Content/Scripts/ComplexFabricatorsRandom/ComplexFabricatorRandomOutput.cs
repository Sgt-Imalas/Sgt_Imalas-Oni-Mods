using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;
using static Klei.SimUtil;
using UnityEngine;
using UtilLibs;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom
{
	class ComplexFabricatorRandomOutput : ComplexFabricator
	{
		public enum OutputType
		{
			None,
			SpawnOnOrderCompletion,
			SpawnDuringProduction,
			SpawnOnOrderCompletionAndDuringProduction
		}

		[SerializeField] int ByproductSpawnChancePerSecond = 100;

		[MyCmpGet] Building building;

		[SerializeField] public bool StoreRandomOutputs = false;

		public RecipeRandomResult DefaultOutput = null;

		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			SpawnAdditionalRandomProducts();
			return base.SpawnOrderProduct(recipe);
		}

		public override void Sim1000ms(float dt)
		{			
			base.Sim1000ms(dt);
			SpawnFabricationByproducts();
		}


		public void SpawnProductsFor(RecipeRandomResult recipeRandomResult)
		{
			var products = recipeRandomResult.GetRandomProducts();
			foreach (var productInfo in products)
			{

				var element = ElementLoader.FindElementByHash(productInfo.first);
				var pos = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore) + outputOffset;
				var product = element.substance.SpawnResource(pos, productInfo.second, recipeRandomResult.GetRandomOutputTemperature(element), 0, 0);

				if (StoreRandomOutputs || !element.IsSolid)
				{
					outStorage.Store(product, true);
				}
			}
		}
		#region onRecipeCompleted
		public void SpawnAdditionalRandomProducts()
		{
			if (CurrentWorkingOrder == null)
				return;
			SpawnRandomProductsFromCurrentRecipe();
		}
		public Dictionary<Tag, RecipeRandomResult> GetRandomOutputSelection()
		{
			if (RandomRecipeProducts.GetRandomResultList(building.Def.PrefabID, out var recipeSelection))
				return recipeSelection;
			return new();
		}
		protected void SpawnRandomProductsFromCurrentRecipe()
		{
			var outputSelection = GetRandomOutputSelection();
			if (outputSelection == null)
			{
				SgtLogger.warning("ComplexFabricatorRandomOutput: GetRandomOutputSelection returned null. This is not expected.");
				return;
			}

			var randomSourceIngredient = this.CurrentWorkingOrder.ingredients[0];
			if (outputSelection.TryGetValue(randomSourceIngredient.material, out var recipeRandomResult))
				SpawnProductsFor(recipeRandomResult);
			else if (DefaultOutput != null)
				SpawnProductsFor(DefaultOutput);
		}
		#endregion
		#region RandomByproductsDuringRecipeProcess
		public void SpawnFabricationByproducts()
		{
			if (CurrentWorkingOrder == null || !operational.IsActive)
				return;

			var rollSpawnChance = UnityEngine.Random.Range(1, 101);
			if (rollSpawnChance > ByproductSpawnChancePerSecond)
				return;

			SpawnProgressByproductsFromCurrentRecipe();
		}

		public Dictionary<Tag, RecipeRandomResult> GetRandomOccurenceSelection()
		{
			if (RandomRecipeProducts.GetRandomOccurenceList(building.Def.PrefabID, out var recipeSelection))
				return recipeSelection;
			return new();
		}
		protected void SpawnProgressByproductsFromCurrentRecipe()
		{
			if (CurrentWorkingOrder == null)
				return;

			var outputSelection = GetRandomOccurenceSelection();
			if (outputSelection == null)
			{
				SgtLogger.warning("ComplexFabricatorRandomOutput: GetRandomOutputSelection returned null. This is not expected.");
				return;
			}

			var randomSourceIngredient = this.CurrentWorkingOrder.ingredients[0];
			if (outputSelection.TryGetValue(randomSourceIngredient.material, out var recipeRandomResult))
				SpawnProductsFor(recipeRandomResult);
			else if (DefaultOutput != null)
				SpawnProductsFor(DefaultOutput);
		}
		#endregion

		internal void DestroyFragileIngredientsOnCancel()
		{
			var items = buildStorage.DropHasTags([ModAssets.Tags.RandomRecipeIngredient_DestroyOnCancel]);
			for(int i = items.Length - 1; i >= 0; i--)
			{
				var item = items[i];
				if (item != null)
				{
					Util.KDestroyGameObject(item);
				}
			}
		}
	}
}
