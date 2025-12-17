using Biochemistry.Buildings;
using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.Mining_DrillMk2_Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static Crop;
using static ResearchTypes;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using static STRINGS.CODEX;
using static STRINGS.ITEMS.FOOD;
using static STRINGS.ITEMS.INGREDIENTS;
using static STRINGS.UI.TOOLS.FILTERLAYERS;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public class AdditionalRecipes
	{
		public static void BurnedOilShaleCementRecipe(string ID, bool advKiln)
		{
			float recipeMultiplier = advKiln ? 5f : 1f;

			///Cement from Oilshale
			RecipeBuilder.Create(ID, 40)
				.Input(ModElements.OilShale_Solid, 100f * recipeMultiplier)
				.Output(SimHashes.Cement, 70 * recipeMultiplier)
				.Output(SimHashes.CrudeOil, 5 * recipeMultiplier)
				.Output(ModElements.LowGradeSand_Solid, 10 * recipeMultiplier)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.HEAT_REFINE, 1, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.OILSHALE_CEMENT)
				.Build();
		}
		public static void SlagCementRecipe(string ID)
		{
			///Cement from Slag
			RecipeBuilder.Create(ID, 40)
				.Input(ModElements.Slag_Solid, 100)
				.Input(SimHashes.CrushedRock, 20)
				.Output(SimHashes.Cement, 80)
				.Output(ModElements.BaseGradeSand_Solid, 12.5f)
				.Output(ModElements.HighGradeSand_Solid, 7.5f)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1, 1, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.SLAG_CEMENT)
				.Build();

		}
		public static void BrickRecipes(string ID, bool burnMaterial, bool advKiln)
		{
			if (!Config.Instance.DupesEngineering_Enabled)
				return;

			var burnables = RefinementRecipeHelper.GetCombustableSolidsWithWood();

			if (advKiln)
			{
				//alt recipe for adv kiln
				RecipeBuilder.Create(ID, 30)
					.Input(SimHashes.Clay, 300)
					.Input(SimHashes.Sand, 200)
					.Output(SimHashes.Brick, 500, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description1I1O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.HEAT_REFINE)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
					.Build();
				return;
			}
			///Brick from clay
			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Clay, 100)
				.InputConditional(burnables, 25, burnMaterial)
				.Output(SimHashes.Brick, 50)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.HEAT_REFINE, 1, 1)
				.Build();

		}
		public static void AdditionalnRecipes_ChemicalRefinery(string ID)
		{
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				float oilAmount = ModElements.VeggiOilToWaterRatio * 100f;
				float waterAmount = 100 - oilAmount;

				RecipeBuilder.Create(ID, 40)
					.Input(ModElements.VegetableOil_Liquid, oilAmount)
					.Input(SimHashes.Water, waterAmount)
					.Output(SimHashes.PhytoOil, 100)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CHEMICAL_MIXINGUNIT_2_1, 2, 1)
					.Build();
			}
		}
		public static void AdditionalKilnRecipes(string ID, bool burnMaterial = false, bool advKiln = false)
		{
			BurnedOilShaleCementRecipe(ID, advKiln);

			BrickRecipes(ID, burnMaterial, advKiln);
		}
		public static void RegisterRecipes_Kiln()
		{
			AdditionalKilnRecipes(KilnConfig.ID, true);
		}
		public static void RegisterRecipes_DataMiner()
		{
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				RecipeBuilder.Create(DataMinerConfig.ID, 200)
					.Input(ModElements.BioPlastic_Solid, 5)
					.Output(DatabankHelper.TAG, 1)
					.Description(string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ModElements.BioPlastic_Solid.Tag.ProperName(), DatabankHelper.NAME))
					.Build();
			}
		}
		public static void RegisterRecipes_FabricatedWoodMaker()
		{
			string ID = FabricatedWoodMakerConfig.ID;
			RecipeBuilder.Create(ID, 50)
				.Input(ModElements.BioMass_Solid, 90)
				.Input(SimHashes.NaturalResin, 10)
				.Output(SimHashes.FabricatedWood, 100, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Description(GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.FABRICATEDWOODMAKER.RECIPE_DESC, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.PLANT_FIBER.NAME, ElementLoader.FindElementByHash(SimHashes.NaturalResin).name, (object)Assets.GetPrefab((Tag)"FabricatedWood").GetProperName()))
				.Build();
		}

		private static void RegisterRecipes_RayonLoom()
		{
			string ID = Chemical_RayonLoomConfig.ID;
			RecipeBuilder.Create(ID, 50)
				.Input(RefinementRecipeHelper.GetWoods(), 150)
				.Output(RayonFabricConfig.TAG, 1, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Description(RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INGREDIENTS.RAYONFIBER.RECIPE_DESC, 1, 0)
				.Build();

			if (DlcManager.IsExpansion1Active())
			{
				RecipeBuilder.Create(ID, 50)
				.Input(PlantMeatConfig.ID, 1)
				.Output(RayonFabricConfig.TAG, 1, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Description(RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INGREDIENTS.RAYONFIBER.RECIPE_DESC, 1, 0)
				.Build();
			}
			if (DlcManager.IsContentSubscribed(DlcManager.DLC4_ID))
			{
				RecipeBuilder.Create(ID, 50)
				.Input(KelpConfig.ID, 20)
				.Output(RayonFabricConfig.TAG, 1, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Description(RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INGREDIENTS.RAYONFIBER.RECIPE_DESC, 1, 0)
				.Build();
			}
		}


		public static void RegisterRecipes_RockCrusher()
		{
			string ID = RockCrusherConfig.ID;
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				RecipeBuilder.Create(ID, CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_DESCRIPTION, 40)
				.Input(RefinementRecipeHelper.GetCrushables([SimHashes.Obsidian]).Select(e => e.id.CreateTag()), 100f)
				.Output(SimHashes.CrushedRock, 100f)
				.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_NAME)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.Build();

				SlagCementRecipe(ID);
			}
		}

		public static void RegisterRecipes_CraftingTable()
		{
			SimpleDrillbits_Config.CreateSimpleDrillRecipes(CraftingTableConfig.ID,true);
		}

		public static void RegisterRecipes_SuperMaterialRefinery()
		{
			string ID = SupermaterialRefineryConfig.ID;

			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Steel, 60)
				.Input(RefinementRecipeHelper.GetPlasticIds([ModElements.Plasteel_Solid, ModElements.FiberGlass_Solid, SimHashes.HardPolypropylene]), 25)
				.Input(ModElements.Borax_Solid, 15)
				.Output(ModElements.Plasteel_Solid, 100f)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.SUPERMATERIALREFINERY_3_1, 3, 1)
				.SortOrder(1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();
			}


			foreach (var recipe in ComplexRecipeManager.Get().preProcessRecipes
				.Where(r => r.fabricators.Any() && r.fabricators[0] == "SupermaterialRefinery"
				&& r.ingredients.Any(r => r.possibleMaterials != null && r.possibleMaterials.Contains(BasicFabricConfig.ID) && r.possibleMaterials.Contains(FeatherFabricConfig.ID)
				)))
			{
				var fiberIngredient = recipe.ingredients.FirstOrDefault(r => r.possibleMaterials != null && r.possibleMaterials.Contains(BasicFabricConfig.ID) && r.possibleMaterials.Contains(FeatherFabricConfig.ID));
				if (fiberIngredient != null)
				{
					fiberIngredient.possibleMaterials = fiberIngredient.possibleMaterials.AddItem(RayonFabricConfig.TAG).ToArray();
				}
			}
		}


		public static void RegisterRecipes_PostLoadEntities()
		{
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				//if (Config.Instance.Biochem_ExpellerPressRebalance)
				//	RegisterRecipes_ExpellerPress_Rebalanced();
				//else
				RegisterRecipes_AnaerobicDigester();
				RegisterRecipes_ExpellerPress();
			}
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				RegisterRecipes_RayonLoom();
			}
			if (Config.Instance.MineralProcessing_Mining_Enabled)
			{
				RegisterRecipes_CraftingTable();
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

			/// since this is a pretty low output amount and its other function - dirt creation is better done by the soil mixer, add a multiplyer to the output natgas amount:
			kCalToMethane *= Config.Instance.Biochem_AnaerobicDigesterBuff;


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

				RecipeBuilder.Create(ID, 100)
					.Input(prefabID.PrefabTag, foodAmount)
					.Input(SimHashes.Sand, 99)
					.Input(SimHashes.Water, 1)
					.Output(SimHashes.Methane, methaneAmount)
					.Output(SimHashes.Dirt, 100)
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
			RecipeBuilder.Create(ID, 100)
					.Input(ModElements.BioMass_Solid.Tag, 20)
					.Input(SimHashes.Sand, 79)
					.Input(SimHashes.Water, 1)
					.Output(SimHashes.Methane, 3 * Config.Instance.Biochem_AnaerobicDigesterBuff)
					.Output(SimHashes.Dirt, 100)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.ANAEROBIC_DIGESTER_1_2, 1, 2)
					.Build();

			///Husk Digesting
			///54.45kg methane per 400husks, equivalent to previous gas grass power yield

			RecipeBuilder.Create(ID, 100)
					.Input(PlantFiberConfig.ID, 100)
					.Input(SimHashes.Sand, 99)
					.Input(SimHashes.Water, 1)
					.Output(SimHashes.Methane, 5.445f * Config.Instance.Biochem_AnaerobicDigesterBuff / 4) 
					.Output(SimHashes.Dirt, 200)
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
			RecipeBuilder.Create(ID, 100)
					.Input(SimHashes.Algae, 20)
					.Input(SimHashes.Sand, 79)
					.Input(SimHashes.Water, 1)
					.Output(SimHashes.Methane, 3 * Config.Instance.Biochem_AnaerobicDigesterBuff)
					.Output(SimHashes.Dirt, 100)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.ANAEROBIC_DIGESTER_1_2, 1, 2)
					.Build();

		}

		public static void AddPrefefinedExpellerPressRecipe(Tag PlantProduct, float OilAmount, float BiomassAmount, float InputAmount) => PredefinedExpellerPressRecipes[PlantProduct] = new(OilAmount, BiomassAmount, InputAmount);
		public static void AddPrefefinedExpellerPressRecipe(Tag PlantProduct, float OilAmount, float BiomassAmount) => PredefinedExpellerPressRecipes[PlantProduct] = new(OilAmount, BiomassAmount, 1);

		static Dictionary<Tag, Tuple<float, float, float>> PredefinedExpellerPressRecipes = new();
		public static void RegisterRecipes_ExpellerPress()
		{
			string ID = Biochemistry_ExpellerPressConfig.ID;
			//----[ MEAL LICE PRESSING ]---------------------------------------------
			// Mealwood requires 3 cycles to produce 1kg of Meal Lice, using a total of 30kg of Dirt
			// 50% of the Dirt mass consumed will be turned to Biomass waste of 15kg
			// 10% of the remaining mass will turn to Vegetable Oil of 1,5kg
			// 40% remaining mass is lost.
			// -------------------------------
			// Ingredient: Meal Lice -> 1kg
			// Result:     Vegetable Oil -> 1,5kg
			//             Biomass -> 15kg
			//------------------------------------------------------------------------
			AddPrefefinedExpellerPressRecipe(BasicPlantFoodConfig.ID, 1.5f, 7.5f);
			//----[ SLEET WHEAT GRAIN PRESSING ]---------------------------------------------
			// Sleet Wheat requires 18 cycles to produce 18kg of Sleet Wheat Grain, using a total of 90kg of Dirt and 360kg of Water.
			// 25% of the Dirt mass consumed will be turned to Biomass waste of 1,25kg (22,5 / 18). 
			// 35% of the remaining mass will turn to Vegetable Oil of 1,31kg (23,635 / 18).
			// 40% remaining mass is lost.
			//------------------------------------
			// Ingredient: Sleet Wheat Grain -> 1kg
			// Result:     Vegetable Oil -> 23.6kg / 18 
			//             Biomass -> 22,5kg / 18
			//-------------------------------------------------------------------------------
			AddPrefefinedExpellerPressRecipe(ColdWheatConfig.SEED_ID, 2f, 1.25f);
			//----[ NOSH BEAN PRESSING ]-----------------------------------------------------
			// Nosh Sprout requires 21 cycles to produce 12kg of Nosh Beans, using a total of 105kg of Dirt and 420kg of Ethanol.
			// 20% of the Dirt mass consumed will be turned to Biomass waste of 1,75kg (21 / 12).
			// 40% of the remaining mass will turn to Vegetable Oil of 2,8kg (33,6 / 12).
			// 40% remaining mass is lost.
			//------------------------------------
			// Ingredient: Nosh Bean -> 1kg
			// Result:     Vegetable Oil -> 2,8kg
			//             Biomass -> 1,75
			//-------------------------------------------------------------------------------
			AddPrefefinedExpellerPressRecipe(BeanPlantConfig.SEED_ID, 4f, 1.75f);
			// ---- [PINCHA PEPPERNUT PRESSING]---------------------------------------------------- -
			// Pincha Pepper Plant requires 8 cycles to produce 4kg of Pincha Pepper Nuts, using a total of 8kg of Phosphorite and 280kg of Polluted Water.
			// This recipe will consider the total mass of 288kg / 4 = 72kg.
			// 25% of the mass will be turned to Biomass waste of 4,5kg (18 / 4).
			// 35% of the remaining mass will be turned to Vegetable Oil of 4,72kg (18,9 / 4)  
			// 60% remaining mass is lost.
			//------------------------------------
			// Ingredient: Pincha Pepper Nut -> 1kg
			// Result:     Vegetable Oil -> 4,72kg
			//             Biomass -> 4,5kg
			//---------------------------------------------------------------------------------------
			AddPrefefinedExpellerPressRecipe(SpiceNutConfig.ID, 4.72f, 4.5f);
			//----[ BALM LILY FLOWER PRESSING ]-----------------------------------------------------
			// Balm Lilies require 12 cycles to produce 2kg of Balm Lily Flowers, using nothing.
			// From the mass of 1kg, 65%% will turn to Biomass waste of 650g.
			// Remaining mass will turn to vegetable oil of 350g.
			//------------------------------------
			// Ingredient: Balm Lily Flower -> 1kg
			// Result:     Vegetable Oil -> 950g
			//             Biomass -> 50g
			//---------------------------------------------------------------------------------------
			AddPrefefinedExpellerPressRecipe(SwampLilyFlowerConfig.ID, 0.35f, 6.5f);

			///those are the values for regular grubfruit; moving spindly over to a more oily recipy
			//----[ ~~SPINDLY~~ GRUBFRUIT PRESSING ]-----------------------------------------------------  
			// Spindly Grubfruit Plant require 8 cycles to produce 8kg of Spindly Grubfruits, using a total of 80kg Sulfur.
			// 34% of the mass will be turned to Biomass waste of 3,4kg (27,2 / 8).
			// 26% of the remaining mass will be turned to Vegetable Oil of 1,06kg (8,448 / 8)  

			//------------------------------------
			// Ingredient: Spindly Grubfruit -> 1kg
			// Result:     Vegetable Oil -> 1,06kg
			//             Biomass -> 3,4kg
			//---------------------------------------------------------------------------------------
			AddPrefefinedExpellerPressRecipe(WormSuperFruitConfig.ID, 1.06f, 3.4f);
			//----[ PIKEAPPLE PRESSING ]------------------------------------------------------------
			// Pikeapple Plant require ~12~ -> 3 cycles to produce 1kg of Pikeapple, using a total of ~60kg~ 15kg Phosphorite.
			// 40% of the mass will be turned to Biomass waste of ~24kg~ .
			// 1,7% of the remaining mass will be turned to Vegetable Oil of 1,02kg
			//------------------------------------
			// Ingredient: Pikeapple -> 1kg
			// Result:     Vegetable Oil -> 1,02kg
			//             Biomass -> 24kg
			//---------------------------------------------------------------------------------------  
			AddPrefefinedExpellerPressRecipe(HardSkinBerryConfig.ID, 1.02f, 6f);
			//----[ PLUME SQUASH PRESSING ]---------------------------------------------------------
			// Plume squashes require 9 cycles to produce 1kg of Plume Squash, using a total of 135kg of Ethanol.
			// 20% of the mass will be turned to Biomass waste of 27kg.
			// 1,7% of the remaining mass will be turned to Vegetable Oil of 900g
			//------------------------------------
			// Ingredient: Plume Squash -> 1kg
			// Result:     Vegetable Oil -> 0.9kg
			//             Biomass -> 650g
			//--------------------------------------------------------------------------------------- 
			AddPrefefinedExpellerPressRecipe(CarrotConfig.ID, 0.9f, 38f);


			//^^ those are from the original mod, now to my own additions:

			///Info on All current product plants:



			///GasGrassHarvested,Gas Grass, plant consumes 102kg of fertilizer(solid mass: 100, liquid mass: 2) over 4 cycles, making 1 items
			///Percentages:  20% for biomass, 6% oil 
			///Reason: looks rather seedy/like a nut, pincha has high percentage, very high dirt consumption
			AddPrefefinedExpellerPressRecipe(GasGrassHarvestedConfig.ID, 6.12f, 30.6f);

			///PlantMeat,Plant Meat, plant consumes 300kg of fertilizer(solid mass: 0, liquid mass: 300) over 30 cycles, making 10 items
			///Per Item thats 30kg fertilizer
			///Percentages: 40% for biomass, 3% oil 
			///Reason: very "meaty/fruity, not much oil in that, but lots of biomass
			AddPrefefinedExpellerPressRecipe(PlantMeatConfig.ID, 0.9f, 12.0f);

			///WormBasicFruit,Spindly Grubfruit, plant consumes 40kg of fertilizer(solid mass: 40, liquid mass: 0) over 4 cycles, making 1 items
			///Percentages: 15% for biomass, 18% oil 
			///Reason: winging it to similar values in pincha
			AddPrefefinedExpellerPressRecipe(WormBasicFruitConfig.ID, 7.2f, 6f);

			///GardenFoodPlantFood,Sweatcorn, plant consumes 30kg of fertilizer(solid mass: 30, liquid mass: 0) over 3 cycles, making 1 items
			///Percentages: 25% for biomass, 35% oil 
			///Reason: alternative to sleet wheat - similar properties
			AddPrefefinedExpellerPressRecipe(GardenFoodPlantFoodConfig.ID, 6.25f, 7.5f);

			///Kelp,Seakomb Leaf, plant consumes 50kg of fertilizer(solid mass: 50, liquid mass: 0) over 5 cycles, making 50 items
			///normal Seakomb has a 1-4 conversion with 3/4 of water; 25kg Seakomb + 75kg water become 100kg of phyto oil
			///
			AddPrefefinedExpellerPressRecipe(KelpConfig.ID, 6 * 2.5f, 3.65f * 2.5f, 10 * 2.5f);

			///Dupes Cuisine Integration
			string sunnyGrainId = "SunnyWheatSeed";
			string kakawaId = "KakawaTreeSeed";

			///identical to sleet wheat
			if (Assets.TryGetPrefab(sunnyGrainId))
				AddPrefefinedExpellerPressRecipe(sunnyGrainId, 2f, 1.25f);

			///made into butter, very high fat yield, total wattage ~33% higher than sweatcorn
			if (Assets.TryGetPrefab(kakawaId))
				AddPrefefinedExpellerPressRecipe(kakawaId, 4.9f, 1.4f);
			else
				SgtLogger.warning("Could not find Dupes Cuisine, not adding any of its plants to expeller press.");


			int index = 0;

			foreach (var recipe in PredefinedExpellerPressRecipes)
			{
				Tag ingredient = recipe.Key;
				var data = recipe.Value;
				float oil = data.first * Config.Instance.Biochem_BioOilMultiplier;
				float biomass = data.second;
				float ingredientmass = data.third;

				SgtLogger.l("Creating Expeller Press Recipe: " + ingredient + ": oil->" + oil + " biomass->" + biomass);

				RecipeBuilder.Create(ID, 25)
					.Input(ingredient, ingredientmass)
					.Output(ModElements.VegetableOil_Liquid, oil, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(ModElements.BioMass_Solid, biomass)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.EXPELLER_PRESS_1_2, 1, 2)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
					.NameOverrideFormatIngredient(CHEMICAL_COMPLEXFABRICATOR_STRINGS.EXPELLER_PRESS_FOODTOOIL, 0)
					.IconPrefabIngredient(0)
					.SortOrder(index++)
					.Build();
			}
			ExpellerPress_Seeds(ID);

			HashSet<ComplexRecipe> toConvertRecipes = [];
			foreach(var existingRecipe in ComplexRecipeManager.Get().preProcessRecipes)
			{
				if(!existingRecipe.fabricators.Any() || existingRecipe.fabricators[0] != MilkPressConfig.ID)
				{
					continue;
				}
				toConvertRecipes.Add(existingRecipe);					
			}

			foreach(var existingRecipe in toConvertRecipes)
			{
				var pressRecipe = RecipeBuilder.Create(ID, existingRecipe.time);

				foreach (var ingredient in existingRecipe.ingredients)
				{
					pressRecipe.Input(ingredient.material, ingredient.amount * 2f);
				}
				foreach (var product in existingRecipe.results)
				{
					pressRecipe.Output(product.material, product.amount * 2f);
				}
				pressRecipe.RequiresTech(existingRecipe.requiredTech);
				pressRecipe.Description(existingRecipe.description);
				pressRecipe.NameDisplay(existingRecipe.nameDisplay);
				pressRecipe.NameOverride(existingRecipe.customName);
				pressRecipe
					.SortOrder(index++)
					.Build();
			}
		}
		public static void ExpellerPress_Seeds(string ID)
		{
			HashSet<Tag> seeds = new();
			foreach (var seed in Assets.GetPrefabsWithTag(GameTags.CropSeed))
			{
				var prefabTag = seed.PrefabID();
				//SgtLogger.l("Seed Tag: " + prefabTag);
				if (!PredefinedExpellerPressRecipes.ContainsKey(prefabTag))
					seeds.Add(prefabTag);
			}
			//seeds.Add("ForestTreeSeed");
			//seeds.Add("SpaceTreeSeed");

			//----[ SEEDS PRESSING ]-----------------------------------------------------------------
			// All seeds have 1kg mass.
			// 5% of the mass will be turned to Biomass waste of 50g.
			// 95%% of the remaining mass will be turned to Vegetable Oil of 950g
			//------------------------------------
			// Ingredient: Seeds -> 1kg
			// Result:     Vegetable Oil -> 950g
			//             Biomass -> 50g
			//--------------------------------------------------------------------------------------- 

			RecipeBuilder.Create(ID, 10)
				.Input(seeds.ToArray(), 10, GameTags.Seed)
				.Output(ModElements.VegetableOil_Liquid, 9.5f * Config.Instance.Biochem_BioOilMultiplier, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Output(ModElements.BioMass_Solid, 0.5f)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.NameOverrideFormatIngredient(CHEMICAL_COMPLEXFABRICATOR_STRINGS.EXPELLER_PRESS_SEEDTOOIL, 0)
				.IconPrefabOverride(ModElements.BioMass_Solid.Tag)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.EXPELLER_PRESS_1_2, 1, 2)
				.Build();
		}
		//public static void RegisterRecipes_ExpellerPress_Rebalanced()
		//{

		//	string ID = Biochemistry_ExpellerPressConfig.ID;
		//	foreach (var cropVal in CROPS.CROP_TYPES)
		//	{
		//		Tag product = cropVal.cropId;

		//		///elemental plant products are ignored
		//		if (ElementLoader.GetElement(product) != null)
		//			continue;

		//		var item = Assets.GetPrefab(product);
		//		if (item == null)
		//			continue;

		//		///ignore tree branches
		//		if (item.HasTag(GameTags.PlantBranch))
		//			continue;

		//		///ignore plant grown critters, eg. butterfly from dlc4
		//		if (item.GetComponent<Navigator>() != null)
		//			continue;

		//		if (!PlantProductsConsumption.TryGetValue(product, out var plantProductsConsumption))
		//			continue;

		//		float totalMassConsumedByPlant = plantProductsConsumption.TotalMassPerSecond() * cropVal.cropDuration;
		//		float liquidMassConsumedByPlant = plantProductsConsumption.LiquidMassPerSecond() * cropVal.cropDuration;
		//		float solidMassConsumedByPlant = plantProductsConsumption.SolidMassPerSecond() * cropVal.cropDuration;

		//		//SgtLogger.l(product+","+global::STRINGS.UI.StripLinkFormatting(Assets.GetPrefab(product).GetProperName()) + ", plant consumes " + totalMassConsumedByPlant + "kg of fertilizer (solid mass: " + solidMassConsumedByPlant + ", liquid mass: " + liquidMassConsumedByPlant + ") over " + cropVal.cropDuration / 600f + " cycles, making " + cropVal.numProduced + " items");
		//		Console.WriteLine(cropVal.numProduced + "x items, consumes " + plantProductsConsumption.TotalMassPerSecond() * 600 + "per cycle over cycles: "+cropVal.cropDuration/600f+", makes: " + product + "," + global::STRINGS.UI.StripLinkFormatting(Assets.GetPrefab(product).GetProperName()) + ";");				
		//	}

		//	float oilMultiplier = 0.5f; //filler
		//	float maxAmountBiomass = 1f; //filler

		//	Dictionary<Tag, Tuple<float, float, float>> RecipeAmounts = [];
		//	void AddPercentagedRecipe(Tag PlantProduct, float OilAmountPercentage, float BioMassPercentage, float InputAmount = 1) => RecipeAmounts[PlantProduct] = new(OilAmountPercentage, BioMassPercentage, InputAmount);


		//	///Kelp; Seakomb
		//	AddPercentagedRecipe(KelpConfig.ID, 1f, 0.4f, 10);

		//	///GasGrassHarvested; Gas Grass
		//	AddPrefefinedExpellerPressRecipe(GasGrassHarvestedConfig.ID, 0.4f, 1.8f);

		//	//add a dummy value for balm lily consumption to multiply with
		//	PlantProductsConsumption.Add(SwampLilyFlowerConfig.ID, new() { MassPerSecondPerItem = new() { { GameTags.Void, 0.1f } } });

		//	foreach (var recipe in RecipeAmounts)
		//	{
		//		Tag ingredient = recipe.Key;


		//		if(!PlantProductsConsumption.TryGetValue(ingredient, out PlantConsumptionInfo plantProductsConsumption))
		//		{
		//			SgtLogger.warning("no consumption found for " + ingredient);
		//			continue;
		//		}

		//		var cropVal = CROPS.CROP_TYPES.Find(crop => crop.cropId == ingredient);
		//		float totalMassConsumedByPlant = plantProductsConsumption.TotalMassPerSecond() * cropVal.cropDuration;
		//		float fertilizerConsumedByPlantPerProduct = totalMassConsumedByPlant / ((float)cropVal.numProduced);

		//		var data = recipe.Value;
		//		float oil = data.first * oilMultiplier * fertilizerConsumedByPlantPerProduct;
		//		float biomass = data.second * maxAmountBiomass * fertilizerConsumedByPlantPerProduct;
		//		float ingredientmass = data.third;

		//		RecipeBuilder.Create(ID, 25)
		//			.Input(ingredient, ingredientmass)
		//			.Output(ModElements.VegetableOil_Liquid, oil, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		//			.Output(ModElements.BioMass_Solid, biomass)
		//			.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.EXPELLER_PRESS_1_2, 1, 2)
		//			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
		//			.NameOverrideFormatIngredient(CHEMICAL_COMPLEXFABRICATOR_STRINGS.EXPELLER_PRESS_FOODTOOIL, 0)
		//			.IconPrefabIngredient(0)
		//			.Build();
		//	}
		//	ExpellerPress_Seeds(ID);
		//}

		/// <summary>
		/// unused
		/// </summary>
		//public static void RegisterRecipes_ExpellerPress_GenericTest()
		//{
		//	RegisterRecipes_ExpellerPress();
		//	return;
		//	string ID = Biochemistry_ExpellerPressConfig.ID;
		//	SgtLogger.l("Listing generic crops:");

		//	foreach (var cropVal in CROPS.CROP_TYPES)
		//	{
		//		Tag product = cropVal.cropId;

		//		///elemental plant products are ignored
		//		if (ElementLoader.GetElement(product) != null)
		//			continue;

		//		var item = Assets.GetPrefab(product);
		//		if (item == null)
		//			continue;

		//		///ignore tree branches
		//		if (item.HasTag(GameTags.PlantBranch))
		//			continue;

		//		///ignore plant grown critters, eg. butterfly from dlc4
		//		if (item.GetComponent<Navigator>() != null)
		//			continue;

		//		if (!PlantProductsConsumption.TryGetValue(product, out var plantProductsConsumption))
		//			continue;

		//		float totalMassConsumedByPlant = plantProductsConsumption.TotalMassPerSecond() * cropVal.cropDuration;
		//		float liquidMassConsumedByPlant = plantProductsConsumption.LiquidMassPerSecond() * cropVal.cropDuration;
		//		float solidMassConsumedByPlant = plantProductsConsumption.SolidMassPerSecond() * cropVal.cropDuration;

		//		//SgtLogger.l(product+","+global::STRINGS.UI.StripLinkFormatting(Assets.GetPrefab(product).GetProperName()) + ", plant consumes " + totalMassConsumedByPlant + "kg of fertilizer (solid mass: " + solidMassConsumedByPlant + ", liquid mass: " + liquidMassConsumedByPlant + ") over " + cropVal.cropDuration / 600f + " cycles, making " + cropVal.numProduced + " items");
		//		Console.WriteLine(cropVal.numProduced + "," + plantProductsConsumption.TotalMassPerSecond() * 600 + "," + product + "," + global::STRINGS.UI.StripLinkFormatting(Assets.GetPrefab(product).GetProperName()) + ";");
		//		float massConsumedByPlantPerProduct = totalMassConsumedByPlant / ((float)cropVal.numProduced);




		//		//generic fallbacK: 6% to oil, 9% to biomass
		//		RecipeBuilder.Create(ID, 25)
		//			.Input(product, 1)
		//			.Output(ModElements.VegetableOil_Liquid, massConsumedByPlantPerProduct * 0.06f)
		//			.Output(ModElements.BioMass_Solid, massConsumedByPlantPerProduct * 0.09f)
		//			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
		//			.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.EXPELLER_PRESS_1_2, 1, 2)
		//			.Build();
		//	}
		//	ExpellerPress_Seeds(ID);
		//}
		//public class PlantConsumptionInfo
		//{
		//	public float TotalMassPerSecond()
		//	{
		//		float val = 0;
		//		foreach (var item in MassPerSecondPerItem)
		//		{
		//			val += item.Value;
		//		}
		//		return val;
		//	}
		//	public float SolidMassPerSecond()
		//	{
		//		float val = 0;
		//		foreach (var item in MassPerSecondPerItemSolid)
		//		{
		//			val += item.Value;
		//		}
		//		return val;
		//	}
		//	public float LiquidMassPerSecond()
		//	{
		//		float val = 0;
		//		foreach (var item in MassPerSecondPerItemLiquid)
		//		{
		//			val += item.Value;
		//		}
		//		return val;
		//	}


		//	public Dictionary<Tag, float> MassPerSecondPerItem = new();
		//	public Dictionary<Tag, float> MassPerSecondPerItemLiquid = new();
		//	public Dictionary<Tag, float> MassPerSecondPerItemSolid = new();
		//	public void AddOrIncreasConsumption(Tag tag, float val)
		//	{
		//		if (MassPerSecondPerItem.ContainsKey(tag))
		//			MassPerSecondPerItem[tag] += val;
		//		else MassPerSecondPerItem[tag] = val;


		//		if (ElementLoader.GetElement(tag) != null && ElementLoader.GetElement(tag).IsLiquid)
		//		{
		//			if (MassPerSecondPerItemLiquid.ContainsKey(tag))
		//				MassPerSecondPerItemLiquid[tag] += val;
		//			else MassPerSecondPerItemLiquid[tag] = val;
		//		}
		//		else
		//		{

		//			if (MassPerSecondPerItemSolid.ContainsKey(tag))
		//				MassPerSecondPerItemSolid[tag] += val;
		//			else MassPerSecondPerItemSolid[tag] = val;
		//		}
		//	}
		//}
		//public static PlantConsumptionInfo AddOrGetPlantConsumptionInfo(Tag crop, Tag consumes, float amount)
		//{
		//	if (!PlantProductsConsumption.TryGetValue(crop, out var consumptionInfo))
		//	{
		//		consumptionInfo = new();
		//		PlantProductsConsumption[crop] = consumptionInfo;
		//	}
		//	//SgtLogger.l("registering Consumption rate for "+crop+"; "+consumes+" x "+amount+" per second");
		//	consumptionInfo.AddOrIncreasConsumption(consumes, amount);
		//	return consumptionInfo;
		//}

		//public static Dictionary<Tag, PlantConsumptionInfo> PlantProductsConsumption = new();


		internal static void RegisterTags()
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				GameTags.Fabrics = GameTags.Fabrics.Append(RayonFabricConfig.ID);
		}
	}
}
