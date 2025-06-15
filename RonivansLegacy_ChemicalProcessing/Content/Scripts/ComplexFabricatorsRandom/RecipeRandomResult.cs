using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom
{
	public class RecipeRandomResult
	{
		public float TotalMass { get; private set; }
		public float MinTemp { get; private set; }
		public float MaxTemp { get; private set; }

		public int RequiredProductsMin = -1, RequiredProductsMax = -1;

		public Dictionary<SimHashes, Tuple<float, float, float>> RandomProductsRange;

		public RecipeRandomResult(float _totalMass, float minTempC, float maxTempC)
		{
			TotalMass = _totalMass;
			RandomProductsRange = new();
			MaxTemp = UtilMethods.GetKelvinFromC(maxTempC);
			MinTemp = UtilMethods.GetKelvinFromC(minTempC);
		}
		public RecipeRandomResult ProductCount(int count) => ProductCountRange(count, count);

		public RecipeRandomResult ProductCountRange(int countMin, int countMax) 
		{
			RequiredProductsMin = countMin;
			RequiredProductsMax = countMax;
			return this;
		}

		StringBuilder sb = null;
		public string GetProductCompositionName(bool massOnly = false)
		{
			if (massOnly)
				return GameUtil.GetFormattedMass(TotalMass, massFormat: GameUtil.MetricMassFormat.Kilogram);
			return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.NAME, GameUtil.GetFormattedMass(TotalMass, massFormat: GameUtil.MetricMassFormat.Kilogram));
		}
		public string GetProductCompositionDescription()
		{
			if(sb == null)		
				sb = new StringBuilder();			
			else			
				sb.Clear();
			

			string totalMass = GameUtil.GetFormattedMass(TotalMass, massFormat: GameUtil.MetricMassFormat.Kilogram);

			if (RequiredProductsMax > 0)
			{
				string count = RequiredProductsMax == RequiredProductsMin
					? RequiredProductsMax.ToString()
					: string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.DESC_RANGE, RequiredProductsMin, RequiredProductsMax);
				sb.AppendLine(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.DESC_MAX_COUNT, totalMass, count));
			}
			else
			{
				sb.AppendLine(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.DESC, totalMass));
			}


			foreach (var product in RandomProductsRange)
			{
				var element = product.Key.CreateTag().ProperName();
				var range = product.Value;
				var minAmount = GameUtil.GetFormattedMass(range.first, massFormat: GameUtil.MetricMassFormat.Kilogram);
				var maxAmount = GameUtil.GetFormattedMass(range.second, massFormat: GameUtil.MetricMassFormat.Kilogram);
				var chance = GameUtil.GetFormattedPercent(range.third * 100);
				if(range.third < 1)
				{
					sb.AppendLine(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.COMPOSITION_ENTRY_CHANCE, element,minAmount,maxAmount,chance));
				}
				else
				{
					sb.AppendLine(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.COMPOSITION_ENTRY, element, minAmount, maxAmount));
				}
			}
			return sb.ToString();
		}

		public RecipeRandomResult AddProduct(SimHashes product, float minAmount, float maxAmount, float chanceToAppear = 1)
		{
			if (RandomProductsRange.ContainsKey(product))
			{
				throw new ArgumentException($"Product {product} already exists in the recipe.");
			}
			RandomProductsRange[product] = new Tuple<float, float, float>(minAmount, maxAmount, chanceToAppear);
			return this;
		}
		public bool HasMaxProductCount => RequiredProductsMax > 0;

		public List<Tuple<SimHashes, float>> GetRandomProducts()
		{
			List<Tuple<SimHashes, float>> products = new();

			HashSet<SimHashes> alreadyAdded = new HashSet<SimHashes>();
			int numRequiredProducts = UnityEngine.Random.Range(RequiredProductsMin,RequiredProductsMax+1);

			do
			{
				var randomProducts = this.RandomProductsRange.Keys.Shuffle();
				foreach (var product in randomProducts)
				{
					if (alreadyAdded.Contains(product))
						continue; // Skip if product is already added

					var range = RandomProductsRange[product];
					float minAmount = range.first;
					float maxAmount = range.second;
					float chanceToAppear = range.third;
					float randomAmount = UnityEngine.Random.Range(minAmount, maxAmount);

					if (randomAmount <= 0)
						continue;

					if (HasMaxProductCount && products.Count >= numRequiredProducts)
					{
						break; // Limit the number of products to TotalProducts
					}
					if (chanceToAppear < 1 && UnityEngine.Random.value > chanceToAppear)
					{
						continue; // Skip this product based on chance
					}

					alreadyAdded.Add(product); // Mark product as added
					products.Add(new Tuple<SimHashes, float>(product, randomAmount));
				}
			} while (HasMaxProductCount && products.Count() < numRequiredProducts);


			var totalMassProducts = products.Sum(x => x.second);
			float multiplier = TotalMass / totalMassProducts;
			foreach (var entry in products)
			{
				entry.second *= multiplier;
			}
			return products;
		}

		internal float GetRandomOutputTemperature() => UnityEngine.Random.Range(MinTemp, MaxTemp);
	}
}
