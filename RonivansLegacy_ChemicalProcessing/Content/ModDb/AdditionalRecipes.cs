using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using UtilLibs;
using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using Biochemistry.Buildings;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public class AdditionalRecipes
	{
		public static void RegisterRecipes_RockCrusher()
		{
			string ID = RockCrusherConfig.ID;
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				RecipeBuilder.Create(ID, CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_DESCRIPTION, 40)
				.Input(RefinementRecipeHelper.GetCrushables().Select(e => e.id.CreateTag()), 100f)
				.Output(SimHashes.CrushedRock, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_NAME)
				.SortOrder(1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.Build();
			}
		}
		public static void RegisterRecipes_SuperMaterialRefinery()
		{
			string ID = SupermaterialRefineryConfig.ID;

			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Steel, 60)
				.Input(SimHashes.Polypropylene, 25)
				.Input(ModElements.Borax_Solid, 15)
				.Output(ModElements.Plasteel_Solid, 100f)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.SUPERMATERIALREFINERY_3_1, 3, 1)
				.SortOrder(1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();


				RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Propane, 50)
				.Input(SimHashes.Petroleum, 45)
				.Input(SimHashes.Isoresin, 5)
				.Output(ModElements.Isopropane_Gas, 100f)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.SUPERMATERIALREFINERY_3_1, 3, 1)
				.SortOrder(1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();
			}
		}


		public static void RegisterDynamicFoodRecipes()
		{
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				RegisterRecipes_AnaerobicDigester();
				RegisterRecipes_ExpellerPress();
			}
		}
		public static void RegisterRecipes_AnaerobicDigester()
		{
			//0.090909  == 1/11

			// Applied formula: ((kcal x 0.0012) / 0.09090909) *0.061 * 0.4 
			//dont ask me how that formula came to be, its whats used for the mods values
			//can be broken down to: kcal*mass * 0.00032208


			string ID = Biochemistry_AnaerobicDigesterConfig.ID;
			float kCalToMethane = 0.00032208f;




			//----[ GENERIC PLANT DIGESTING ]-----------------------------------------------
			//-------------------------
			// Applied formula: ((kcal x 0.0012) / 0.09090909) *0.061
			//-------------------------
			// Ingredient:  Amount of plant for 10k kCals, but maximum x10
			//              Sand -> 50kg
			//              Water -> 1kg
			// Result:      Methane -> cKal per kg amount * units * kCalToMethane
			//              Dirt -> 50kg
			//-------------------------------------------------------------------------


			HashSet<string> FoodPlants = CROPS.CROP_TYPES.Select(croptype => croptype.cropId).ToHashSet();

			///starter foods, need to be added to if theres a new one
			//Muckroot
			FoodPlants.Add("BasicForagePlant");
			//Hexalent
			FoodPlants.Add("ForestForagePlant");
			//Swampchard
			FoodPlants.Add("SwampForagePlant");
			//Sherberry
			FoodPlants.Add("IceCavesForagePlant");
			//Snac Fruit
			FoodPlants.Add("GardenForagePlant");


			int count = 0;
			foreach (var food in Assets.GetPrefabsWithComponent<Edible>())
			{
				count++;
				if (!food.TryGetComponent<KPrefabID>(out var prefabID))
					return;

				if (!food.TryGetComponent<Edible>(out var edible))
					continue;

				if (!FoodPlants.Contains(prefabID.PrefabTag.ToString()))
				{
					//SgtLogger.l(food.GetProperName() + " was not a plant product!");
					continue;
				}

				if (edible.foodInfo == null)
				{
					SgtLogger.warning(food.GetProperName() + " food info was null!");
					continue;
				}

				var kcalsPerKG = edible.foodInfo.CaloriesPerUnit / 1000f; //foods are in calories, not kilo calories

				if (kcalsPerKG <= 0)
					continue;

				//get an amount thats roughtly 10k kCals, but not more than 10 units
				int foodAmount = Mathf.RoundToInt((10000f / kcalsPerKG) + 0.001f);
				if (foodAmount > 10)
					foodAmount = 10;

				float recipeCKals = foodAmount * kcalsPerKG;
				float methaneAmount = recipeCKals * kCalToMethane;

				SgtLogger.l("Adding Anaerobic Digester recipe for " + foodAmount + "x " + global::STRINGS.UI.StripLinkFormatting(food.GetProperName()) + " with " + recipeCKals + "kcals, producing " + methaneAmount + "kg methane");

				RecipeBuilder.Create(ID, 50)
					.Input(prefabID.PrefabTag, foodAmount)
					.Input(SimHashes.Sand, 50)
					.Input(SimHashes.Water, 1)
					.Output(SimHashes.Methane, methaneAmount)
					.Output(SimHashes.Dirt, 50)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.ANAEROBIC_DIGESTER_1_2, 1, 2)
					.Build();
			}

			//----[ BIOMASS DIGESTING ]-----------------------------------------------
			//-------------------------
			// Applied formula: ((kcal x 0.0012) / 0.09090909) *0.061
			//-------------------------
			// Ingredient:  Biomass -> 20kg
			//              Sand -> 30kg
			//              Water -> 1kg
			// Result:      Methane -> 1 kg
			//              Dirt -> 50kg
			//-------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
					.Input(ModElements.BioMass_Solid.Tag, 20)
					.Input(SimHashes.Sand, 30)
					.Input(SimHashes.Water, 1)
					.Output(SimHashes.Methane, 1)
					.Output(SimHashes.Dirt, 50)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.ANAEROBIC_DIGESTER_1_2, 1, 2)
					.Build();

			//----[ ALGAE DIGESTING ]-------------------------------------------------
			//-------------------------
			// Applied formula: ((kcal x 0.0012) / 0.09090909) *0.061
			//-------------------------
			// Ingredient:  Algae -> 20kg
			//              Sand -> 30kg
			//              Water -> 1kg
			// Result:      Methane -> 1 kg
			//              Dirt -> 50kg
			//-------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
					.Input(SimHashes.Algae, 20)
					.Input(SimHashes.Sand, 30)
					.Input(SimHashes.Water, 1)
					.Output(SimHashes.Methane, 1)
					.Output(SimHashes.Dirt, 50)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.ANAEROBIC_DIGESTER_1_2, 1, 2)
					.Build();

		}
		public static void RegisterRecipes_ExpellerPress()
		{
			string ID = Biochemistry_AnaerobicDigesterConfig.ID;

			foreach (var cropVal in CROPS.CROP_TYPES)
			{
				Tag product = cropVal.cropId;

				///elemental plant products are ignored
				if (ElementLoader.GetElement(product) != null)
					continue;

				var item = Assets.GetPrefab(product);
				if (item == null)
					continue;

				///ignore tree branches
				if (item.HasTag(GameTags.PlantBranch))
					continue;

				///ignore plant grown critters, eg. butterfly from dlc4
				if (item.GetComponent<Navigator>() != null)
					continue;


			}

		}
		public class PlantConsumptionInfo
		{
			public float TotalMassPerSecond()
			{
				float val = 0;
				foreach (var item in MassPerSecondPerItem)
				{
					val += item.Value;
				}
				return val;
			}
			public Dictionary<Tag, float> MassPerSecondPerItem = new();
			public void AddOrIncreasConsumption(Tag tag, float val)
			{
				if (MassPerSecondPerItem.ContainsKey(tag))
					MassPerSecondPerItem[tag] += val;
				else MassPerSecondPerItem[tag] = val;
			}

		}
		public static PlantConsumptionInfo AddOrGetPlantConsumptionInfo(Tag crop, Tag consumes, float amount)
		{
			if (!PlantProductsConsumption.TryGetValue(crop, out var consumptionInfo))
			{
				consumptionInfo = new();
				PlantProductsConsumption[crop] = consumptionInfo;
			}
			//SgtLogger.l("registering Consumption rate for "+crop+"; "+consumes+" x "+amount+" per second");
			consumptionInfo.AddOrIncreasConsumption(consumes,amount);
			return consumptionInfo;
		}

		public static Dictionary<Tag, PlantConsumptionInfo> PlantProductsConsumption = new();


		internal static void RegisterTags()
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				GameTags.Fabrics = GameTags.Fabrics.Append(RayonFabricConfig.ID);
		}
	}
}
