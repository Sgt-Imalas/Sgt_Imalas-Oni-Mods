using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.MineralProcessing_Metallurgy
{
	//==== [ METALLURGY: BALL CRUSHER MILL CONFIG ] ===================================================================
	public class Metallurgy_BallCrusherMillConfig : IBuildingConfig
	{
		//--[ Base Information ]---------------------------------------------------------------------------------------
		public static string ID = "Metallurgy_BallCrusherMill";
		//--[ Building Definitions ]------------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [1000f];
			string[] ingredient_types = [SimHashes.Steel.ToString()];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "metallurgy_ball_mill_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier, 0.2f);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 800f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(-2, 0);
			buildingDef.AudioCategory = "Metal";
			SoundUtils.CopySoundsToAnim("metallurgy_ball_mill_kanim", "orescrubber_kanim");
			return buildingDef;
		}

		//--[ Building Operation Definitions ]---------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Prioritizable.AddRef(go);
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			//----------------------------- Fabricator Section
			ComplexFabricatorRandomOutput ballMill = go.AddOrGet<ComplexFabricatorRandomOutput>();
			ballMill.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, ballMill);
			ballMill.duplicantOperated = false;
			ballMill.heatedTemperature = 298.15f;
			ballMill.keepExcessLiquids = true;
			ballMill.storeProduced = true;
			ballMill.inStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			ballMill.buildStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			ballMill.outStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			ballMill.outputOffset = new Vector3(1f, 0.5f);
			//-----------------------------

			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			this.ConfigureRecipes();
		}

		//==== [ METALLURGY: BALL CRUSHER MILL RECIPES ] ==================================================================
		private void ConfigureRecipes()
		{
			int sortOrder = 0;
			//---- [ Sandstone Milling ] ----------------------------------------------------------------------------------
			// Ingredient: Sandstone - 500kg
			// Random Results: Copper Ore
			//                 Electrum
			//                 Fertilizer
			//                 Crushed Rock
			//                 Sand
			// Assured Result: Sand - 10kg
			//-------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.SandStone, 500f)
				.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetMetallurgyBallCrusherRandomResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(sortOrder++)
				.Build();

			//---- [ Sedimentary Rock Milling ~ Spaced Out DLC ] -----------------------------------------------------------------
			// Ingredient: Sandstone - 500kg
			// Random Results: Gold Amalgam
			//                 Cobalt Ore - DLC1 Only
			//                 Salt
			//                 Crushed Rock
			//                 Clay
			// Assured Result: Sand - 10kg
			//--------------------------------------------------------------------------------------------------------------------
			//---- [ Sedimentary Rock Milling ~ Vanilla Base Game ] --------------------------------------------------------------
			// Ingredient: Sandstone - 500kg
			// Random Results: Gold Amalgam
			//                 Pyrite
			//                 Salt
			//                 Crushed Rock
			//                 Clay
			// Assured Result: Sand - 10kg
			//--------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.SedimentaryRock, 500f)
				.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetMetallurgyBallCrusherRandomResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(sortOrder++)
				.Build();
			///Shale is a sort of sedimentary rock, so it can be milled in the same way as Sedimentary Rock.
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Shale, 500f)
				.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetMetallurgyBallCrusherRandomResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(sortOrder++)
				.Build();

			//---- [ Igneous Rock Milling ] ----------------------------------------------------------------------------------
			// Ingredient: Igneous Rock - 500kg
			// Randon Results:  - Pyrite
			//                  - Iron Ore
			//                  - Obsidian
			//                  - Sulfur
			//                  - Crushed Rock
			//                  - Sand
			// Assured Result: Sand - 10kg
			//-------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.IgneousRock, 500f)
				.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetMetallurgyBallCrusherRandomResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(sortOrder++)
				.Build();

			//---- [ Granite Milling ] ------------------------------------------------------------------------------------
			// Ingredient: Granite - 500kg
			// Randon Results:  - Aluminum Ore
			//                  - Iron Ore
			//                  - Obsidian
			//                  - Crushed Rock
			//                  - Sand
			// Assured Result: Sand - 10kg
			//-------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Granite, 500f)
				.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetMetallurgyBallCrusherRandomResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(sortOrder++)
				.Build();

			//---- [ Mafic Rock Milling ] ------------------------------------------------------------------------------------
			// Ingredient: Mafic Rock - 500kg
			// Randon Results:  - Aluminum Ore
			//                  - Electrum
			//                  - Phosphorus
			//                  - Crushed Rock
			//                  - Sand
			// Assured Result: Sand - 10kg
			//-------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.MaficRock, 500f)
				.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetMetallurgyBallCrusherRandomResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(sortOrder++)
				.Build();

			//---- [ Abyssalite Milling ] ------------------------------------------------------------------------------------
			// Ingredient: Abyssalite - 500kg
			// Randon Results:  - Tungsten
			//                  - Wolframite
			//                  - Diamond
			//                  - Obsidian
			//                  - Phosphorus
			//                  - Crushed Rock
			//                  - Sand
			// Assured Result: Sand - 10kg
			//-------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Katairite, 500f)
				.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetMetallurgyBallCrusherRandomResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(sortOrder++)
				.Build();			
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
		}
	}
}
