using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using Mineral_Processing;
using Mineral_Processing_Mining.Buildings;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.MineralProcessing_Metallurgy;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;
using static RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom.RecipeRandomResult;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public static class RandomRecipeProducts
	{
		/// <summary>
		/// random outputs at recipe completion
		/// </summary>
		private static Dictionary<Tag, Dictionary<Tag, RecipeRandomResult>> _randomFabricationByproductsCollection = null;
		/// <summary>
		/// random outputs that spawn during recipe progress
		/// </summary>
		private static Dictionary<Tag, Dictionary<Tag, RecipeRandomResult>> _randomRecipeResultsCollection = null;
		private static void InitRandomResults()
		{
			_randomRecipeResultsCollection = new()
			{
				{Chemical_BallCrusherMillConfig.ID,InitRandomResults_BallCrusher_Chemical() },
				{Chemical_SelectiveArcFurnaceConfig.ID,InitRandomResults_SelectiveArcFurnace() },
				{Metallurgy_BallCrusherMillConfig.ID,InitRandomResults_BallCrusher_Metallurgy() },
				{Mining_AugerDrillConfig.ID,InitRandomResults_AugerDrill() },
			};
			_randomFabricationByproductsCollection = new()
			{
				{Mining_AugerDrillConfig.ID,InitRandomFabricationByproducts_AugerDrill() },
				{Mining_MineralDrillConfig.ID,InitRandomFabricationByproducts_MineralDrill() },
			};
		}

		public static bool GetRandomResultsforRecipe(ComplexRecipe recipe, out RecipeRandomResult randomResult)
		{
			randomResult = null;
			foreach (var fabricator in recipe.fabricators)
			{
				if (recipe.ingredients.Count() == 0)
				{
					SgtLogger.error("GetRandomResultsforRecipe called with a recipe that has no ingredients! " + recipe.id);
					continue; //no ingredients, no results
				}

				if (GetRandomResultList(fabricator, out var results)
					&& results.TryGetValue(recipe.ingredients[0].material, out randomResult))
					return true;
			}
			return false;
		}
		public static bool GetRandomResultList(Tag buildingID, out Dictionary<Tag, RecipeRandomResult> results)
		{
			results = null;
			if (_randomRecipeResultsCollection == null)
				InitRandomResults();

			return _randomRecipeResultsCollection.TryGetValue(buildingID, out results);
		}

		public static bool GetRandomOccurencesforRecipe(ComplexRecipe recipe, out RecipeRandomResult randomResult)
		{
			randomResult = null;
			foreach (var fabricator in recipe.fabricators)
			{
				if (GetRandomOccurenceList(fabricator, out var results)
					&& results.TryGetValue(recipe.ingredients[0].material, out randomResult))
					return true;
			}
			return false;
		}
		public static bool GetRandomOccurenceList(Tag buildingID, out Dictionary<Tag, RecipeRandomResult> results)
		{
			results = null;
			if (_randomFabricationByproductsCollection == null)
				InitRandomResults();

			return _randomFabricationByproductsCollection.TryGetValue(buildingID, out results);
		}

		public static string GetMetallurgyBallCrusherRandomResultsString(ComplexRecipe.RecipeElement[] recipeIngredients, ComplexRecipe.RecipeElement[] recipeResults)
		{
			var inputElement = recipeIngredients[0].material;

			if (!GetRandomResultList(Metallurgy_BallCrusherMillConfig.ID, out var recipes) || !recipes.TryGetValue(inputElement, out RecipeRandomResult result))
			{
				return string.Empty;
			}

			string potentialResults = string.Empty;
			foreach (var potentialResult in result.RandomProductsRange)
			{
				var elementTag = potentialResult.Key.CreateTag();
				potentialResults += "\n• " + elementTag.ProperName();
			}
			return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.BALLCRUSHER_MILLING_1INGREDIENT,
				inputElement.ProperName(),
				GameUtil.GetFormattedMass(result.TotalMass),
				potentialResults);
		}
		public static string GetChemicalBallCrusherRandomResultsString(ComplexRecipe.RecipeElement[] recipeIngredients, ComplexRecipe.RecipeElement[] recipeResults)
		{
			var inputElement = recipeIngredients[0].material;
			var guaranteed = recipeResults[0].material;

			if (!GetRandomResultList(Chemical_BallCrusherMillConfig.ID, out var recipes) || !recipes.TryGetValue(inputElement, out RecipeRandomResult result))
			{
				return string.Empty;
			}

			string potentialResults = string.Empty;
			foreach (var potentialResult in result.RandomProductsRange)
			{
				var elementTag = potentialResult.Key.CreateTag();
				if (elementTag != guaranteed)
				{
					potentialResults += "\n• " + elementTag.ProperName();
				}
			}

			if (recipeIngredients.Length == 4)
				return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.BALLCRUSHER_MILLING_3INGREDIENTS,
					recipeIngredients[0].material.ProperName(),
					recipeIngredients[1].material.ProperName(),
					recipeIngredients[2].material.ProperName(),
					recipeIngredients[3].material.ProperName(),
					GameUtil.GetFormattedMass(result.TotalMass),
					potentialResults,
					guaranteed.ProperName());
			else
				return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.BALLCRUSHER_MILLING_2INGREDIENTS,
					recipeIngredients[0].material.ProperName(),
					recipeIngredients[1].material.ProperName(),
					recipeIngredients[2].material.ProperName(),
					GameUtil.GetFormattedMass(result.TotalMass),
					potentialResults,
					guaranteed.ProperName());
		}
		public static string GetArcFurnaceRandomResultString(ComplexRecipe.RecipeElement[] recipeIngredients, ComplexRecipe.RecipeElement[] recipeResults)
		{
			var inputElement = recipeIngredients[0].material;
			var guaranteed = recipeResults[0].material;

			if (!GetRandomResultList(Chemical_SelectiveArcFurnaceConfig.ID, out var recipes) || !recipes.TryGetValue(inputElement, out RecipeRandomResult result))
			{
				return string.Empty;
			}

			string potentialResults = string.Empty;
			foreach (var potentialResult in result.RandomProductsRange)
			{
				var elementTag = potentialResult.Key.CreateTag();
				if (elementTag != guaranteed)
				{
					potentialResults += "\n• " + elementTag.ProperName();
				}
			}
			return string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.ARCFURNACE_RANDOM_RECIPE,
					recipeIngredients[0].material.ProperName(),
					potentialResults,
					guaranteed.ProperName());
		}

		static StringBuilder sb;
		public static string GetAugerDrillRandomResultString(string drillID, Tag inputElement, string drillbit, bool IsGuidance = false, bool liquid = false, bool dangerousliquid = false)
		{
			if (sb == null)
				sb = new StringBuilder();
			else
				sb.Clear();

			if (!IsGuidance)
				sb.AppendLine(string.Format(STRINGS.UI.MINING_AUGUR_DRILL.RECIPE_1I, drillbit));
			else
				sb.AppendLine(string.Format(STRINGS.UI.MINING_AUGUR_DRILL.RECIPE_3I, Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(inputElement), drillbit, Mining_Drillbits_GuidanceDevice_ItemConfig.GetGuidanceItemName(inputElement)));

			if (liquid)
				sb.AppendLine(STRINGS.UI.MINING_AUGUR_DRILL.RECIPE_LIQUID);
			if (dangerousliquid)
				sb.AppendLine(STRINGS.UI.MINING_AUGUR_DRILL.RECIPE_LIQUID_DANGER);
			string products = string.Empty;
			if (GetRandomResultList(drillID, out var recipes) && recipes.TryGetValue(inputElement, out RecipeRandomResult result))
			{
				sb.Append(STRINGS.UI.MINING_AUGUR_DRILL.RECIPE_RESULTS);
				products = result.GetCompositionDescription(sb.ToString(), null, true);
			}
			else
				products = sb.ToString();


			if (GetRandomOccurenceList(drillID, out var occurenceList) && occurenceList.TryGetValue(inputElement, out RecipeRandomResult occurenceResult))
			{
				products += STRINGS.UI.MINING_AUGUR_DRILL.RECIPE_OCCURENCES;
				return occurenceResult.GetCompositionDescription(products, null, true);
			}
			return products;
		}

		private static Dictionary<Tag, RecipeRandomResult> InitRandomResults_BallCrusher_Metallurgy()
		{
			bool chemproc = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;
			var results = new Dictionary<Tag, RecipeRandomResult>();
			///rates taken from ronivans dictionary solution

			bool soEnabled = DlcManager.IsExpansion1Active();

			//===: SANDSTONE RANDOM RESULTS :============================================================ < 500 kg >
			//---[ Possible Results Elements: ]
			// - Copper Ore
			// - Electrum
			// - Fertilizer
			// - Crushed Rock
			// - Sand
			//-------------------------------------------------------------------------------------
			results.Add(SimHashes.SandStone.CreateTag(),
				new RecipeRandomResult(500, 20, 50)
				.AddProduct(SimHashes.CrushedRock, 150, 310)
				.AddProduct(SimHashes.Sand, 175, 310)
				.AddProduct(SimHashes.Cuprite, 20, 40, 3f / 9f)
				.AddProduct(SimHashes.Electrum, 15, 30, 3f / 9f)
				.AddProductConditional(chemproc, ModElements.Argentite_Solid, 10, 30, 3f / 9f)
				.AddProduct(SimHashes.Fertilizer, 10, 20, 2f / 9f)
				);
			//===: SEDIMENTARY ROCK RANDOM RESULTS :======================================================== < 500 kg >
			//---[ Possible Results Elements: ]
			// - Gold Amalgam
			// - Pyrite
			// - Cobalt Ore
			// - Salt
			// - Crushed Rock
			// - Clay
			//------------------------------------------------------------------------------------------
			results.Add(SimHashes.SedimentaryRock.CreateTag(),
				new RecipeRandomResult(500, 20, 50)
				.AddProduct(SimHashes.CrushedRock, 160, 320)
				.AddProduct(SimHashes.Clay, 150, 310)
				.AddProduct(SimHashes.GoldAmalgam, 10, 30, 2f / 9f)
				.AddProduct(SimHashes.FoolsGold, 20, 40, 2f / 9f)
				.AddProductConditional(soEnabled, SimHashes.Cobaltite, 25, 35, 2f / 9f)
				.AddProduct(SimHashes.Salt, 40, 50, 1f / 9f)
				.AddProductConditional(chemproc, ModElements.Chloroschist_Solid, 40, 50, 1f / 9f)
				.AddProductConditional(chemproc, ModElements.Aurichalcite_Solid, 25, 35, 2f / 9f)
				);
			///Shale: mirror of sedimentary rock bc it is described as sedimentary in its desc
			results.Add(SimHashes.Shale.CreateTag(),
				new RecipeRandomResult(500, 20, 50)
				.AddProduct(SimHashes.CrushedRock, 160, 320)
				.AddProduct(SimHashes.Clay, 150, 310)
				.AddProduct(SimHashes.GoldAmalgam, 10, 30, 2f / 9f)
				.AddProduct(SimHashes.FoolsGold, 20, 40, 2f / 9f)
				.AddProductConditional(soEnabled, SimHashes.Cobaltite, 25, 35, 2f / 9f)
				.AddProduct(SimHashes.Salt, 40, 50, 1f / 9f)
				.AddProductConditional(chemproc, ModElements.Chloroschist_Solid, 40, 50, 1f / 9f)
				.AddProductConditional(chemproc, ModElements.Aurichalcite_Solid, 25, 35, 2f / 9f)
				);
			//===: IGNEOUS ROCK RANDOM RESULTS :========================================================= < 500 kg >
			//---[ Possible Results Elements: ]
			// - Pyrite
			// - Iron Ore
			// - Obsidian
			// - Sulfur
			// - Crushed Rock
			// - Sand
			//--------------------------------------------------------------------------------------
			results.Add(SimHashes.IgneousRock.CreateTag(),
				new RecipeRandomResult(500, 20, 50)
				.AddProduct(SimHashes.CrushedRock, 250, 350)
				.AddProduct(SimHashes.Sand, 50, 150)
				.AddProduct(SimHashes.FoolsGold, 15, 25, 4f / 9f)
				.AddProduct(SimHashes.Sulfur, 25, 75, 4f / 9f)
				.AddProduct(SimHashes.IronOre, 15, 20, 2f / 9f)
				.AddProduct(SimHashes.Obsidian, 65, 95, 3f / 9f)
				.AddProductConditional(chemproc, ModElements.Galena_Solid, 15, 25, 2f / 8f)
				.AddProductConditional(chemproc, ModElements.OilShale_Solid, 25, 75, 2f / 8f)
				);
			//===: GRANITE RANDOM RESULTS :============================================================= < 500 kg >
			//---[ Possible Results Elements: ]
			// - Aluminum 
			// - Iron Ore
			// - Obsidian
			// - Crushed Rock
			// - Sand
			results.Add(SimHashes.Granite.CreateTag(),
				new RecipeRandomResult(500, 20, 50)
				.AddProduct(SimHashes.CrushedRock, 270, 370)
				.AddProduct(SimHashes.Sand, 50, 250)
				.AddProduct(SimHashes.AluminumOre, 15, 25, 4f / 8f)
				.AddProduct(SimHashes.IronOre, 15, 25, 2f / 8f)
				.AddProductConditional(chemproc, ModElements.Chloroschist_Solid, 40, 50, 2f / 8f)
				.AddProductConditional(chemproc, ModElements.Aurichalcite_Solid, 15, 25, 2f / 8f)
				.AddProduct(SimHashes.Obsidian, 65, 95, 2f / 8f)
				);
			//===: MAFIC ROCK RANDOM RESULTS :========================================================== < 500 kg >
			//---[ Possible Results Elements: ]
			// - Aluminum Ore
			// - Phosphorus 
			// - Electrum
			// - Crushed Rock
			// - Sand
			results.Add(SimHashes.MaficRock.CreateTag(),
				new RecipeRandomResult(500, 20, 50)
				.AddProduct(SimHashes.CrushedRock, 250, 350)
				.AddProduct(SimHashes.Sand, 100, 200)
				.AddProduct(SimHashes.AluminumOre, 15, 25, 2f / 8f)
				.AddProduct(SimHashes.Electrum, 25, 35, 2f / 8f)
				.AddProductConditional(chemproc, ModElements.Aurichalcite_Solid, 15, 25, 2f / 8f)
				.AddProduct(SimHashes.Phosphorus, 50, 100, 3f / 8f)
				);
			//===: ABYSSALITE RANDOM RESULTS :========================================================== < 500 kg >
			//---[ Possible Results Elements: ]
			// - Wolframite
			// - Tungsten
			// - Phosphorus
			// - Obsidian
			// - Diamond
			// - Crushed Rock
			// - Sand

			results.Add(SimHashes.Katairite.CreateTag(),
				new RecipeRandomResult(500, 20, 50)
				.AddProduct(SimHashes.Wolframite, 5, 15, 1f / 9f)
				.AddProduct(SimHashes.Tungsten, 5, 15, 1f / 9f)
				.AddProduct(SimHashes.Phosphorus, 70, 110, 4f / 9f)
				.AddProduct(SimHashes.Obsidian, 70, 120, 3f / 9f)
				.AddProduct(SimHashes.Diamond, 20, 25, 1f / 9f)
				.AddProduct(SimHashes.CrushedRock, 200, 420)
				.AddProduct(SimHashes.Sand, 120, 180, 4f / 9f)
				);

			return results;
		}
		private static Dictionary<Tag, RecipeRandomResult> InitRandomResults_BallCrusher_Chemical()
		{
			var results = new Dictionary<Tag, RecipeRandomResult>();
			///rates taken from ronivans dictionary solution
			results.Add(SimHashes.SandStone.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 380)
				.AddProduct(LowGradeSand_Solid, 50, 100)
				.AddProduct(SimHashes.Fertilizer, 5, 15, 2f / 5f)
				);
			results.Add(SimHashes.SedimentaryRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 390)
				.AddProduct(LowGradeSand_Solid, 50, 90)
				.AddProduct(BaseGradeSand_Solid, 20, 50, 3f / 5f)
				);
			results.Add(SimHashes.Granite.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 360, 420)
				.AddProduct(BaseGradeSand_Solid, 30, 90)
				);
			results.Add(SimHashes.IgneousRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 360, 420)
				.AddProduct(BaseGradeSand_Solid, 30, 80)
				.AddProduct(SimHashes.Sulfur, 25, 45)
				);
			results.Add(SimHashes.MaficRock.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 300, 390)
				.AddProduct(LowGradeSand_Solid, 30, 70)
				.AddProduct(BaseGradeSand_Solid, 10, 50)
				.AddProduct(SimHashes.Phosphorus, 20, 55)
				);
			results.Add(SimHashes.Katairite.CreateTag(), //abyssalite
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 180, 270)
				.AddProduct(LowGradeSand_Solid, 50, 100)
				.AddProduct(BaseGradeSand_Solid, 50, 95)
				.AddProduct(SimHashes.Phosphorus, 25, 65)
				.AddProduct(SimHashes.Diamond, 10, 35, 4f / 5f)
				);
			results.Add(SimHashes.Regolith.CreateTag(),
				new RecipeRandomResult(490, 20, 40)
				.AddProduct(ToxicMix_Liquid, 440, 475)
				.AddProduct(LowGradeSand_Solid, 8, 25)
				.AddProduct(BaseGradeSand_Solid, 8, 25)
				.AddProduct(HighGradeSand_Solid, 2, 6)
				);
			results.Add(MeteorOre_Solid.Tag,
				new RecipeRandomResult(490, 20, 40)
				.AddProduct(ToxicMix_Liquid, 320, 420)
				.AddProduct(BaseGradeSand_Solid, 50, 90)
				.AddProduct(HighGradeSand_Solid, 20, 55)
				);
			///added separately

			///Shale: mirror of sedimentary rock bc it is described as sedimentary in its desc
			results.Add(SimHashes.Shale.CreateTag(),
				new RecipeRandomResult(440, 20, 40)
				.AddProduct(ToxicMix_Liquid, 340, 390)
				.AddProduct(LowGradeSand_Solid, 50, 90)
				.AddProduct(BaseGradeSand_Solid, 20, 50, 3f / 5f)
				);
			return results;
		}
		private static Dictionary<Tag, RecipeRandomResult> InitRandomResults_SelectiveArcFurnace()
		{
			var results = new Dictionary<Tag, RecipeRandomResult>();
			results.Add(LowGradeSand_Solid.Tag,
				new RecipeRandomResult(90, 40, 60)
				.AddProduct(SimHashes.Copper, 15, 60)
				.AddProduct(Zinc_Solid, 15, 60)
				.AddProduct(SimHashes.Lead, 15, 60)
				.AddProduct(Silver_Solid, 15, 60)
				.ProductCountRange(2, 4)
				);

			results.Add(BaseGradeSand_Solid.Tag,
				new RecipeRandomResult(90, 40, 60)
				.AddProduct(SimHashes.Aluminum, 15, 60)
				.AddProduct(SimHashes.Iron, 15, 60)
				.AddProduct(SimHashes.Gold, 15, 60)
				.AddProduct(SimHashes.Tungsten, 15, 60)
				.ProductCountRange(2, 4)
				);

			results.Add(HighGradeSand_Solid.Tag,
				new RecipeRandomResult(90, 40, 60)
				.AddProduct(SimHashes.Tungsten, 75, 80)
				.AddProduct(SimHashes.Fullerene, 7, 12)
				.AddProduct(SimHashes.Niobium, 1, 4)
				);
			return results;

		}
		private static Dictionary<Tag, RecipeRandomResult> InitRandomResults_AugerDrill()
		{
			bool chemproc = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;
			var results = new Dictionary<Tag, RecipeRandomResult>();
			//===: BASIC DRILLBITS RANDOM RESULTS :============================================================
			//---[ Possible Results Elements: ]
			// -    Dirt
			// -    Toxic Dirt
			// -    Clay
			// -    Sand
			// -    Coal

			// -    Copper Ore
			// -    Pyrite
			// -    Aluminum Ore
			// -    Gold Amalgam
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_Basic_ItemConfig.TAG,
			new RecipeRandomResult()
				.MinRequiredProducts(3)
				//high chance
				.AddProduct(SimHashes.CrushedRock, 50, 200)
				.AddProduct(SimHashes.Carbon, 50, 300, 4 / 6f)
				.AddProduct(SimHashes.Dirt, 100, 300, 2f / 6f)
				.AddProduct(SimHashes.Clay, 100, 300, 2f / 6f)
				.AddProduct(SimHashes.Sand, 100, 300, 2f / 6f)
				.AddProduct(SimHashes.ToxicSand, 100, 300, 2f / 6f)
				//low chance
				.AddProduct(SimHashes.Cuprite, 25, 200, 1f / 6f)
				.AddProduct(SimHashes.FoolsGold, 25, 200, 2f / 6f)
				.AddProduct(SimHashes.GoldAmalgam, 25, 200, 1f / 6f)
				.AddProduct(SimHashes.AluminumOre, 25, 200, 1f / 6f)
				.AddProductConditional(chemproc, ModElements.Aurichalcite_Solid, 25, 200, 1f / 6f)
				.AddProductConditional(chemproc, ModElements.Argentite_Solid, 25, 200, 1f / 6f)
				);

			//===: STEEL DRILLBITS RANDOM RESULTS :=============================================================
			//---[ Possible Results Elements: ]
			// -    Copper Ore
			// -    Iron Ore
			// -    Aluminum Ore
			// -    Gold Amalgam
			// -    Electrum

			// - Igneous Rock
			// - Granite
			// - Sandstone
			// - Sulfur
			// - Wolframite
			//---------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_Steel_ItemConfig.TAG,
			new RecipeRandomResult()
				.MinRequiredProducts(3)
				//high chance
				.AddProduct(SimHashes.IronOre, 75, 300, 6f / 9f)
				.AddProduct(SimHashes.AluminumOre, 75, 300, 4 / 9f)
				.AddProduct(SimHashes.Cuprite, 75, 300, 4f / 9f)
				.AddProduct(SimHashes.GoldAmalgam, 75, 300, 2f / 9f)
				.AddProduct(SimHashes.Electrum, 75, 300, 2f / 9f)
				.AddProduct(SimHashes.Sulfur, 50, 400, 3f / 9f)
				.AddProductConditional(chemproc, ModElements.Aurichalcite_Solid, 25, 300, 2f / 9f)
				.AddProductConditional(chemproc, ModElements.Argentite_Solid, 25, 300, 2f / 9f)
				//low chance
				.AddProduct(SimHashes.IgneousRock, 25, 400, 2f / 9f)
				.AddProduct(SimHashes.Granite, 25, 400, 3f / 9f)
				.AddProduct(SimHashes.SandStone, 25, 400, 2f / 9f)
				.AddProduct(SimHashes.Wolframite, 25, 100, 1f / 9f)
				);
			//===: TUNGSTEN DRILLBITS RANDOM RESULTS :===========================================================
			//---[ Possible Results Elements: ]
			// -    Wolframite
			// -    Abyssalite
			// -    Obsidian
			// -    Rust
			// -    Salt

			// - Diamond
			// - Fossil
			// - Refined Coal
			// - Lead
			//---------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_Tungsten_ItemConfig.TAG,
				new RecipeRandomResult()
				.MinRequiredProducts(3)
				//high chance
				.AddProduct(SimHashes.Wolframite, 75, 300, 6f / 12f)
				.AddProduct(SimHashes.Katairite, 75, 300, 6f / 12f)
				.AddProduct(SimHashes.Obsidian, 75, 300, 6f / 12f)
				.AddProduct(SimHashes.Rust, 75, 300, 2f / 12f)
				.AddProduct(SimHashes.Salt, 75, 300, 2f / 12f)
				//low chance
				.AddProduct(SimHashes.Fossil, 25, 100, 3f / 12f)
				.AddProduct(SimHashes.RefinedCarbon, 25, 100, 1f / 12f)
				.AddProduct(SimHashes.Diamond, 25, 100, 1f / 12f)
				.AddProduct(SimHashes.Lead, 25, 100, 1f / 12f)
				);

			//===: SMART DRILLBITS SOFT STRATUM RANDOM RESULTS :================================================
			//---[ Possible Results Elements: ]
			// -    Dirt
			// -    Sand
			// -    Phosphorite
			// -    Coal
			// -    Salt
			// -    Clay
			// -    Polluted Dirt
			// -    Algae
			// -    Slime
			//---------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.SoftStratumTag,
			new RecipeRandomResult()
				.MinRequiredProducts(4)
				.AddProduct(SimHashes.Dirt, 100, 500, 2f / 3f)
				.AddProduct(SimHashes.Sand, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Carbon, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Phosphorite, 100, 500, 2f / 3f)
				.AddProduct(SimHashes.Clay, 100, 500, 2f / 3f)
				.AddProduct(SimHashes.Salt, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.ToxicSand, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Algae, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.SlimeMold, 100, 500, 1f / 3f)
				.AddProductConditional(chemproc, ModElements.Chloroschist_Solid, 100, 500, 1f / 3f)
			);
			//===: SMART DRILLBITS AQUIFER RANDOM RESULTS :=========================================================
			//---[ Possible Results Elements: ]
			// -    Sand
			// -    Sedimentary Rock
			// -    Sandstone
			// -    Granite
			// -    Clay
			//-------------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.AquiferTag,
			new RecipeRandomResult()
				.MinRequiredProducts(3)
				.AddProduct(SimHashes.Sand, 50, 200)
				.AddProduct(SimHashes.Clay, 50, 200)
				.AddProduct(SimHashes.SedimentaryRock, 50, 200, 1f / 3f)
				.AddProduct(SimHashes.Granite, 50, 200, 1f / 3f)
				.AddProduct(SimHashes.SandStone, 50, 200, 1f / 3f)
				.AddProductConditional(chemproc, ModElements.Chloroschist_Solid, 50, 200, 1f / 3f)
			);
			//===: SMART DRILLBITS HARD STRATUM RANDOM RESULTS :======================================================
			//---[ Possible Results Elements: ]
			// -    Copper Ore
			// -    Iron Ore
			// -    Aluminum Ore
			// -    Gold Amalgam
			// -    Electrum
			// -    Wolframite
			// -    Igneous Rock
			// -    Granite
			// -    Obsidian
			// -    Rust
			// -    Salt
			// -    Fossil
			//---------------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.HardStratumTag,
			new RecipeRandomResult()
				.MinRequiredProducts(6)
				.AddProduct(SimHashes.IronOre, 100, 500)
				.AddProduct(SimHashes.AluminumOre, 100, 500)
				.AddProduct(SimHashes.Cuprite, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.GoldAmalgam, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.IgneousRock, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Obsidian, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Wolframite, 100, 500, 2f / 3f)
				.AddProduct(SimHashes.Electrum, 100, 500, 2f / 3f)
				.AddProduct(SimHashes.Granite, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Rust, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Fossil, 100, 500, 1f / 3f)
				.AddProduct(SimHashes.Salt, 100, 500, 1f / 3f)
				.AddProductConditional(chemproc, Argentite_Solid, 100, 500, 1f / 3f)
				.AddProductConditional(chemproc, Aurichalcite_Solid, 100, 500, 1f / 3f)
			);
			//===: SMART DRILLBITS OIL RESERVES RANDOM RESULTS :======================================================
			//---[ Possible Results Elements: ]
			// -    Igneous Rock
			// -    Obsidian
			// -    Sulfur
			// -    Granite
			// -    Fossil
			//---------------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.OilReservesTag,
			new RecipeRandomResult()
				.MinRequiredProducts(4)
				.AddProduct(SimHashes.IgneousRock, 50, 200, 1f / 2f)
				.AddProduct(SimHashes.Obsidian, 50, 200)
				.AddProduct(SimHashes.Sulfur, 50, 200, 1f / 2f)
				.AddProduct(SimHashes.Granite, 50, 200, 1f / 2f)
				.AddProduct(SimHashes.Fossil, 50, 200, 1f / 2f)
				.AddProductConditional(chemproc, OilShale_Solid, 100, 300, 1)
				.AddProductConditional(chemproc, Galena_Solid, 50, 200, 1f / 2f)
			);
			//===: SMART DRILLBITS CRYOSPHERE RANDOM RESULTS :================================================
			//---[ Possible Results Elements: ]
			// -    Snow
			// -    Ice
			// -    Polluted Ice
			// -    Brine Ice
			// -    Sand
			// -    Regolith
			// -    Fossil

			// - Solid Crude Oil
			// - Solid Carbon Dioxide
			// - Solid Chlorine
			// - Solid Methane
			// - Solid Mercury
			//---------------------------------------------------------------------------------------------------

			///normal to rare is 4-1 here
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.CryosphereTag,
			new RecipeRandomResult()
				.MinRequiredProducts(3)
				//high chance
				.AddProduct(SimHashes.Ice, 100, 500, 4f / 10f)
				.AddProduct(SimHashes.DirtyIce, 100, 500, 4f / 10f)
				.AddProduct(SimHashes.Sand, 100, 500, 4f / 10f)
				.AddProduct(SimHashes.Fossil, 25, 100, 8f / 10f)
				.AddProduct(SimHashes.BrineIce, 100, 500, 4f / 10f)
				.AddProduct(SimHashes.Snow, 100, 500, 4f / 10f)
				.AddProduct(SimHashes.Regolith, 100, 500, 4f / 10f)
				.AddProductConditional(chemproc, Ammonia_Solid, 100, 500, 4f / 10f) //ammonia snow
																					//low chance
				.AddProduct(SimHashes.SolidCrudeOil, 100, 750, 2f / 10f)
				.AddProduct(SimHashes.SolidCarbonDioxide, 100, 500, 1f / 10f)
				.AddProduct(SimHashes.SolidMercury, 100, 900, 1f / 10f)
				.AddProduct(SimHashes.SolidChlorine, 100, 500, 1f / 10f)
				.AddProduct(SimHashes.SolidMethane, 100, 500, 1f / 10f)
			);
			//===: SMART DRILLBITS MANTLE RANDOM RESULTS :======================================================
			//---[ Possible Results Elements: ]
			// -    Diamond
			// -    Abyssalite
			// -    Obsidian
			// -    Refined Coal
			// -    Fullerene
			//---------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.MantleTag,
			new RecipeRandomResult()
				.MinRequiredProducts(3)
				//high chance
				.AddProduct(SimHashes.Diamond, 100, 200, 1f / 3f)
				.AddProduct(SimHashes.Katairite, 100, 500)
				.AddProduct(SimHashes.Obsidian, 25, 100, 2f / 3f)
				.AddProduct(SimHashes.Fullerene, 10, 40, 1f / 3f)
				.AddProduct(SimHashes.RefinedCarbon, 100, 500, 2f / 3f)
			);
			return results;

		}

		/// <summary>
		/// elements that are thrown out at random every second while the drill is running
		/// </summary>
		/// <returns></returns>
		private static Dictionary<Tag, RecipeRandomResult> InitRandomFabricationByproducts_AugerDrill()
		{
			bool chemproc = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;

			var results = new Dictionary<Tag, RecipeRandomResult>();
			//===: BASIC DRILLBITS RANDOM SPAWN :==============================================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Water
			// -    Polluted Water
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_Basic_ItemConfig.TAG,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 27f / 29f)
				.AddProduct(SimHashes.Water, 250, 600, 1f / 29f)
				.AddProduct(SimHashes.DirtyWater, 250, 600, 1f / 29f)
				);

			//===: STEEL DRILLBITS RANDOM SPAWN :==============================================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Steam
			// -    Carbon Dioxide
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_Steel_ItemConfig.TAG,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 27f / 29f)
				.AddProduct(SimHashes.Steam, 50, 500, 1f / 29f)
				.AddProduct(SimHashes.CarbonDioxide, 50, 500, 1f / 29f)
				);

			//===: TUNGSTEN DRILLBITS RANDOM SPAWN :===========================================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Methane Gas
			// -    Sulfur Gas
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_Tungsten_ItemConfig.TAG,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 27f / 29f)
				.AddProduct(SimHashes.Methane, 50, 500, 1f / 29f)
				.AddProduct(SimHashes.SulfurGas, 50, 500, 1f / 29f)
				);


			//===: SMART DRILLBITS: SOFT STRATUM RANDOM SPAWN :================================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Polluted Oxygen
			// -    Chlorine Gas
			// -    Carbon Dioxide
			// -    Hydrogen
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.SoftStratumTag,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 21f / 25f)
				.AddProduct(SimHashes.ContaminatedOxygen, 25, 90, 1f / 25f)
				.AddProduct(SimHashes.ChlorineGas, 25, 300, 1f / 25f)
				.AddProduct(SimHashes.CarbonDioxide, 25, 300, 1f / 25f)
				.AddProduct(SimHashes.Hydrogen, 25, 90, 1f / 25f)
				);
			//===: SMART DRILLBITS: AQUIFER RANDOM SPAWN :=====================================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Water
			// -    Polluted Water
			// -    Salt Water
			// -    Brine
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.AquiferTag,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 8f / 20f)
				.AddProduct(SimHashes.Water, 25, 100, 4f / 20f)
				.AddProduct(SimHashes.DirtyWater, 25, 100, 3f / 20f)
				.AddProduct(SimHashes.SaltWater, 25, 100, 3f / 20f)
				.AddProduct(SimHashes.Brine, 25, 100, 2f / 20f)
				);
			//===: SMART DRILLBITS: HARD STRATUM RANDOM SPAWN :===============================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Carbon Dioxide
			// -    Liquid Sulfur
			// -    Mercury Gas
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.HardStratumTag,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 11f / 14f)
				.AddProduct(SimHashes.CarbonDioxide, 100, 400, 1f / 14f)
				.AddProduct(SimHashes.LiquidSulfur, 25, 300, 1f / 14f)
				.AddProduct(SimHashes.MercuryGas, 25, 300, 1f / 14f)
				);
			//===: SMART DRILLBITS: OIL DEPOSIT RANDOM SPAWN :===============================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Crude Oil
			// -    Methane
			// -    Sour Gas
			// -    Carbon Dioxide
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.OilReservesTag,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 6f / 15f)
				.AddProduct(SimHashes.CarbonDioxide, 5, 20, 4f / 15f)
				.AddProduct(SimHashes.CrudeOil, 5, 20, 3f / 15f)
				.AddProductConditional(chemproc,ModElements.OilShale_Solid, 25, 100, 3f / 15f)
				.AddProduct(SimHashes.Methane, 5, 20, 1f / 15f)
				.AddProduct(SimHashes.SourGas, 5, 20, 1f / 15f)
				.AddProductConditional(chemproc, RawNaturalGas_Gas, 5, 20, 1f / 15f)
				);

			//===: SMART DRILLBITS: CRYOSPHERE RANDOM SPAWN :=================================================
			//---[ Possible Results Elements: ]
			// -    Crushed Rock
			// -    Crushed Ice
			///adding some other ices....
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.CryosphereTag,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 4f / 7f)
				.AddProduct(SimHashes.CrushedIce, 15, 75, 3f / 7f)
				);

			//===: SMART DRILLBITS: MANTLE RANDOM SPAWN :=================================================
			//---[ Possible Results Elements: ]
			// -      Magma
			// -      Molten Salt
			// -      Molten Iron
			// -      Molten Copper
			// -      Molten Gold
			// -      Molten Niobium
			//-------------------------------------------------------------------------------------------------
			results.Add(Mining_Drillbits_GuidanceDevice_ItemConfig.MantleTag,
			new OccurenceRandomResult(4)
				.ProductCount(1)
				.AddProduct(SimHashes.CrushedRock, 25, 100, 10f / 30f)
				.AddProduct(SimHashes.Magma, 25, 300, 6f / 30f)
				.AddProduct(SimHashes.MoltenSalt, 25, 300, 5f / 30f)
				.AddProduct(SimHashes.MoltenIron, 25, 75, 2f / 30f)
				.AddProduct(SimHashes.MoltenCopper, 25, 75, 2f / 30f)
				.AddProduct(SimHashes.MoltenGold, 25, 75, 2f / 30f)
				.AddProduct(SimHashes.MoltenAluminum, 25, 75, 2f / 30f)
				.AddProduct(SimHashes.MoltenNiobium, 10, 30, 1f / 30f)
				.AddProductConditional(chemproc, Silver_Liquid, 25, 75, 2f / 30f)
				.AddProductConditional(chemproc, Zinc_Liquid, 25, 75, 2f / 30f)
				);

			return results;
		}

		private static Dictionary<Tag, RecipeRandomResult> InitRandomFabricationByproducts_MineralDrill()
		{
			///rates here are a mix of the rates from the recipe descriptions and the actual code found in an older version of the mod where the drill existed

			bool isSO = DlcManager.IsExpansion1Active();
			bool chemproc = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;

			var results = new Dictionary<Tag, RecipeRandomResult>();
			//"Engage a drilling operation using " + SimHashes.Copper.CreateTag().ProperName() + " as drill bits." +
			//"Possible minerals availabe at this layer:\n"
			//+ SimHashes.Sand.CreateTag().ProperName() + ": 37.5%.\n"
			//+ SimHashes.Dirt.CreateTag().ProperName() + ": 22.5%.\n"
			//+ SimHashes.CrushedRock.CreateTag().ProperName() + ": 11%.\n"
			//+ SimHashes.Carbon.CreateTag().ProperName() + ": 11%.\n"
			//+ SimHashes.Sulfur.CreateTag().ProperName() + ": 11%.\n"
			//+ SimHashes.Algae.CreateTag().ProperName() + ": 11%.\n")

			results.Add(SimHashes.Copper.CreateTag(),
			new OccurenceRandomResult(10)
				.MaxRequiredProducts(1)
				.TempRange(20, 50)
				.AddProduct(SimHashes.SedimentaryRock, 200, 500, 0.375f)
				.AddProduct(SimHashes.Dirt, 200, 500, 0.225f)
				.AddProduct(SimHashes.CrushedRock, 200, 500, 0.22f)
				.AddProduct(SimHashes.SandStone, 200, 500, 0.22f)
				.AddProduct(isSO ? SimHashes.ToxicMud : SimHashes.ToxicSand, 200, 500, 0.22f)
				.AddProduct(isSO ? SimHashes.Mud : SimHashes.Sand, 200, 500, 0.22f)
				.AddProduct(SimHashes.Carbon, 200, 500, 0.11f)
				.AddProduct(SimHashes.PhosphateNodules, 200, 500, 0.11f)
				.AddProduct(SimHashes.Sulfur, 200, 500, 0.11f)
				.AddProduct(SimHashes.Algae, 200, 500, 0.11f)
				);
			//"Engage a drilling operation using " + SimHashes.Iron.CreateTag().ProperName() + " as drill bits.
			//Possible minerals availabe at this layer:\n"
			//+ SimHashes.CrushedRock.CreateTag().ProperName() + ": 22%.\n"
			//+ SimHashes.GoldAmalgam.CreateTag().ProperName() + ": 7%.\n"
			//+ SimHashes.IronOre.CreateTag().ProperName() + ": 7%.\n"
			//+ SimHashes.AluminumOre.CreateTag().ProperName() + ": 7%.\n"
			//+ SimHashes.Cuprite.CreateTag().ProperName() + ": 7%.\n"
			//+ SimHashes.Salt.CreateTag().ProperName() + ": 7%.\n"

			results.Add(SimHashes.Iron.CreateTag(),
			new OccurenceRandomResult(10)
				.TempRange(30, 60)
				.MaxRequiredProducts(1)
				.AddProduct(SimHashes.CrushedRock, 200, 500, 0.22f)
				.AddProduct(SimHashes.Granite, 200, 500, 0.22f)
				.AddProduct(SimHashes.Katairite, 200, 500, 0.22f)
				.AddProduct(SimHashes.GoldAmalgam, 200, 500, 0.07f)
				.AddProduct(SimHashes.IronOre, 200, 500, 0.07f)
				.AddProduct(SimHashes.AluminumOre, 200, 500, 0.07f)
				.AddProduct(SimHashes.Cuprite, 200, 500, 0.07f)
				.AddProduct(SimHashes.Salt, 200, 500, 0.07f)
				.AddProductConditional(chemproc, Argentite_Solid, 200, 500, 0.07f)
				.AddProductConditional(chemproc, Aurichalcite_Solid, 200, 500, 0.07f)
				);
			//"Engage a drilling operation using " + SimHashes.Steel.CreateTag().ProperName() + " as drill bits.
			//Possible minerals availabe at this layer:\n" 
			//+ SimHashes.CrushedRock.CreateTag().ProperName() + ": 22.5%.\n"
			//+ SimHashes.Fossil.CreateTag().ProperName() + ": 11%.\n" 
			//+ SimHashes.Phosphorite.CreateTag().ProperName() + ": 11%.\n" 
			//+ SimHashes.Wolframite.CreateTag().ProperName() + ": 11%.\n")
			results.Add(SimHashes.Steel.CreateTag(),
			new OccurenceRandomResult(10)
				.TempRange(40, 70)
				.MaxRequiredProducts(1)
				.AddProduct(SimHashes.CrushedRock, 200, 500, 0.225f)
				.AddProduct(SimHashes.Obsidian, 200, 500, 0.11f)
				.AddProduct(SimHashes.Granite, 200, 500, 0.11f)
				.AddProduct(SimHashes.MaficRock, 200, 500, 0.11f)
				.AddProduct(SimHashes.FoolsGold, 200, 500, 0.11f)
				.AddProduct(SimHashes.Electrum, 200, 500, 0.11f)
				.AddProduct(SimHashes.Fossil, 200, 500, 0.11f)
				.AddProduct(SimHashes.Phosphorite, 200, 500, 0.11f)
				.AddProduct(SimHashes.Wolframite, 200, 500, 0.11f)
				.AddProductConditional(chemproc, Galena_Solid, 200, 500, 0.11f)
				);
			return results;
		}
	}
}
