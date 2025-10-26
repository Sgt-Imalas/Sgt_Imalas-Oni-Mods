using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom
{
	class ComplexFabricatorRandomOutput : CustomComplexFabricatorBase
	{
		[SerializeField] int ByproductSpawnChancePerSecond = 100;
		//evry x seconds, spawn a byproduct if the chance above is rolled ^
		[SerializeField] public float ByproductSpawnIntervalSeconds = 4;

		[MyCmpGet] Building building;

		[SerializeField] public bool StoreRandomOutputs = false;

		public RecipeRandomResult DefaultOutput = null;

		public float currentByproductSpawnTimer = 0f;

		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			SpawnAdditionalRandomProducts();
			return base.SpawnOrderProduct(recipe);
		}

		public override void Sim1000ms(float dt)
		{			
			base.Sim1000ms(dt);
			SpawnFabricationOccurences(dt);
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
		public void SpawnFabricationOccurences(float dt)
		{
			if (CurrentWorkingOrder == null || !operational.IsActive)
				return;

			currentByproductSpawnTimer -= dt;
			if (currentByproductSpawnTimer > 0)
			{
				return;
			}
			currentByproductSpawnTimer += ByproductSpawnIntervalSeconds;

			var rollSpawnChance = UnityEngine.Random.Range(1, 101);
			if (rollSpawnChance > ByproductSpawnChancePerSecond)
				return;
			//SgtLogger.l("Spawning Random Products in "+this.GetProperName()+", time; "+Time.time);
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
