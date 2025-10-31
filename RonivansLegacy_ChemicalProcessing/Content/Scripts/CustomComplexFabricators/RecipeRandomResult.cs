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
		public class OccurenceRandomResult : RecipeRandomResult
		{
			public OccurenceRandomResult(int rateInSeconds) : base()
			{
				OccurenceRateInSeconds = rateInSeconds;
			}
		}


		public int OccurenceRateInSeconds;

		public float TotalMass { get; private set; }
		public float MinTemp { get; private set; } = -1;
		public float MaxTemp { get; private set; } = -1;

		public int RequiredProductsMin = -1, RequiredProductsMax = -1;

		public bool HasUnifiedProductMass => TotalMass > 0;
		public bool HasRandomTemperatureRange => MinTemp >= 0 && MaxTemp >= 0;

		public Dictionary<SimHashes, Tuple<float, float, float>> RandomProductsRange;

		public RecipeRandomResult(float _totalMass)
		{
			TotalMass = _totalMass;
			RandomProductsRange = new();
		}
		public RecipeRandomResult(float _totalMass, float minTempC, float maxTempC)
		{
			TotalMass = _totalMass;
			RandomProductsRange = new();
			MaxTemp = UtilMethods.GetKelvinFromC(maxTempC);
			MinTemp = UtilMethods.GetKelvinFromC(minTempC);
		}
		public RecipeRandomResult TempRange(float minTempC, float maxTempC)
		{
			MaxTemp = UtilMethods.GetKelvinFromC(maxTempC);
			MinTemp = UtilMethods.GetKelvinFromC(minTempC);
			return this;
		}
		public RecipeRandomResult Multiplier(float multiplier)
		{
			foreach (var key in RandomProductsRange.Keys)
			{
				RandomProductsRange[key].first *= multiplier;
				RandomProductsRange[key].second *= multiplier;
			}
			return this;
		}
		public RecipeRandomResult(float minTempC, float maxTempC) : this(-1, minTempC, maxTempC)
		{
			// This constructor is used when no specific total mass is set.
		}

		public RecipeRandomResult() : this(-1)
		{
		}
		public RecipeRandomResult ProductCount(int count) => ProductCountRange(count, count);

		public RecipeRandomResult ProductCountRange(int countMinInclusive, int countMaxInclusive)
		{
			RequiredProductsMin = countMinInclusive;
			RequiredProductsMax = countMaxInclusive;
			return this;
		}
		public RecipeRandomResult MinRequiredProducts(int countMinInclusive)
		{
			RequiredProductsMin = countMinInclusive;
			return this;
		}
		public RecipeRandomResult MaxRequiredProducts(int countMaxInclusive)
		{
			RequiredProductsMax = countMaxInclusive;
			return this;
		}

		public float GetChanceMultiplier()
		{

			float chanceMultiplier = 1;
			float totalChance = 0;
			foreach (var product in RandomProductsRange)
			{
				var data = product.Value;
				totalChance += data.third;
			}

			if (RequiredProductsMin > 0)
			{
				if ((float)RequiredProductsMin > totalChance)
				{
					chanceMultiplier *= (float)RequiredProductsMin / totalChance;
				}
			}
			if (RequiredProductsMax > 0)
			{
				//if ((float)RequiredProductsMax < totalChance)
				//{
				chanceMultiplier *= (float)RequiredProductsMax / totalChance;
				//}
			}
			return chanceMultiplier;
		}

		public string GetMassString()
		{
			if (HasUnifiedProductMass)
				return GameUtil.GetFormattedMass(TotalMass, massFormat: GameUtil.MetricMassFormat.Kilogram);

			float min = 0, max = 0;
			float chanceMultiplier = GetChanceMultiplier();
			foreach (var product in RandomProductsRange)
			{
				var range = product.Value;
				float chance = range.third * chanceMultiplier;

				min += range.first * chance;
				max += range.second * chance;
			}

			float mean = (min + max) / 2f;
			if (min == max)
				return GameUtil.GetFormattedMass(min, massFormat: GameUtil.MetricMassFormat.Kilogram);
			else
				return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.DESC_RANGE, GameUtil.GetFormattedMass(mean, massFormat: GameUtil.MetricMassFormat.Kilogram));

		}

		StringBuilder sb = null;
		public string GetProductCompositionName(bool massOnly = false)
		{
			if (massOnly)
				return GetMassString();
			return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.NAME, GetMassString());
		}
		public string GetOccurenceCompositionName(bool massOnly = false)
		{
			var massAmount = string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.OCCURENCE_RANDOM_AMOUNT, GetMassString(), OccurenceRateInSeconds);
			if (massOnly)
				return massAmount;
			return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.NAME_OCCURENCE_FORMAT, massAmount);
		}
		public string GetOccurenceCompositionDescription() => GetCompositionDescription(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.DESC_OCCURENCE, OccurenceRateInSeconds), null, true);
		public string GetProductCompositionDescription() => GetCompositionDescription(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.DESC, STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.DESC_MAX_COUNT);

		public string GetCompositionDescription(string single, string maxCount, bool ignoreTotal = false)
		{
			if (sb == null)
				sb = new StringBuilder();
			else
				sb.Clear();


			string totalMass = GetMassString();
			if (!ignoreTotal)
			{
				sb.AppendLine(string.Format(single, totalMass));
			}
			else
			{
				sb.AppendLine(single);
			}

			float chanceMultiplier = GetChanceMultiplier();
			foreach (var product in RandomProductsRange)
			{
				var element = product.Key.CreateTag().ProperName();
				var range = product.Value;
				var minAmount = GameUtil.GetFormattedMass(range.first, massFormat: GameUtil.MetricMassFormat.Kilogram);
				var maxAmount = GameUtil.GetFormattedMass(range.second, massFormat: GameUtil.MetricMassFormat.Kilogram);
				var chancenum = range.third * chanceMultiplier;

				var chance = GameUtil.GetFormattedPercent(chancenum * 100);
				if (chancenum < 1)
				{
					sb.AppendLine(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.COMPOSITION_ENTRY_CHANCE, element, minAmount, maxAmount, chance));
				}
				else
				{
					sb.AppendLine(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.COMPOSITION_ENTRY, element, minAmount, maxAmount));
				}
			}
			return sb.ToString();
		}

		public RecipeRandomResult AddProductConditional(bool condition, SimHashes product, float minAmount, float maxAmount, float chanceToAppear = 1)
		{
			if (condition)
			{
				return AddProduct(product, minAmount, maxAmount, chanceToAppear);
			}
			return this;
		}
		public RecipeRandomResult OccurenceRateSecsUIOnly(int rate)
		{
			OccurenceRateInSeconds = rate;
			return this;
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
		public bool HasMinProductCount => RequiredProductsMin > 0;

		public List<Tuple<SimHashes, float>> GetRandomProducts()
		{
			List<Tuple<SimHashes, float>> products = new();

			HashSet<SimHashes> alreadyAdded = new HashSet<SimHashes>();

			int numRequiredProducts = RequiredProductsMin < RequiredProductsMax ? UnityEngine.Random.Range(RequiredProductsMin, RequiredProductsMax + 1) : RequiredProductsMin;
			bool hasMaxRequirement = HasMaxProductCount;
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

					if (hasMaxRequirement && products.Count >= numRequiredProducts)
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
				if (HasMinProductCount && !HasMaxProductCount && products.Count() < numRequiredProducts)
				{
					///if we dont have the minimum count yet and the maximum count isnt set, we loop only until the min number is satisfied, then abort
					hasMaxRequirement = true;
				}

			} while (HasMinProductCount && products.Count() < numRequiredProducts);

			if (HasUnifiedProductMass)
			{
				var totalMassProducts = products.Sum(x => x.second);
				float multiplier = TotalMass / totalMassProducts;
				foreach (var entry in products)
				{
					entry.second *= multiplier;
				}
			}
			return products;
		}

		internal float GetRandomOutputTemperature(Element element)
		{
			if (HasRandomTemperatureRange)
			{
				return UnityEngine.Random.Range(MinTemp, MaxTemp);
			}
			else
			{
				var temp = element.defaultValues.temperature;
				if (temp <= 0)
				{
					SgtLogger.warning(element.name + " tried spawning with <0°!");
					temp = UtilMethods.GetKelvinFromC(40);
				}
				return element.defaultValues.temperature;
			}
		}

		internal bool HasTagInList(Tag tag)
		{
			foreach (var element in RandomProductsRange.Keys)
			{
				if (element.CreateTag() == tag)
					return true;
			}
			return false;
		}
	}
}
