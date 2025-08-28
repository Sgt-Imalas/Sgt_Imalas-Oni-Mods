using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//====[ CHEMICAL: SELECTIVE ARC-FURNACE CONFIG ]=========================================================================
	public class Chemical_SelectiveArcFurnaceConfig : IBuildingConfig
	{
		//--[ Base Information ]---------------------------------------------------------------------------------------------
		public static string ID = "Chemical_SelectiveArcFurnace";

		//--[ Identification and DLC stuff ]---------------------------------------------------------------------------------

		//--[ Building Definitions ]-----------------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 3, "arc_smelter_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 1200f;
			buildingDef.ExhaustKilowattsWhenActive = 24f;
			buildingDef.SelfHeatKilowattsWhenActive = 8f;
			buildingDef.AudioCategory = "Metal";
			buildingDef.PermittedRotations = PermittedRotations.FlipH;
			SoundUtils.CopySoundsToAnim("arc_smelter_kanim", "suit_maker_kanim");
			return buildingDef;
		}

		//--[ Building Operation Definitions ]--------------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			ComplexFabricatorRandomOutput complexFabricator = go.AddOrGet<ComplexFabricatorRandomOutput>();
			complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			complexFabricator.heatedTemperature = 320.15f;
			complexFabricator.duplicantOperated = true;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
			workable.overrideAnims = [Assets.GetAnim("anim_interacts_metalrefinery_kanim")];
			this.ConfigureRecipes();
			Prioritizable.AddRef(go);
		}

		//====[ CHEMICAL: SELECTIVE ARC-FURNACE RECIPES ]========================================================================
		private void ConfigureRecipes()
		{
			int index = 0;

			//---- [ Brass ] ----------------------------------------------------------------------------------------------------
			// Ingredient: Copper    - 70kg
			//             Zinc      - 30kg
			// Result: Brass         - 100kg
			//-------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Copper, 70)
				.Input(ModElements.Zinc_Solid, 30)
				.Output(ModElements.Brass_Solid, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(ARCFURNACE_SMELT_2_1,2,1)
				.SortOrder(index++)
				.Build();

			//---- [ Phosphor Bronze ] --------------------------------------------------------------------------------------------
			// Ingredient: Copper      - 80kg
			//             Lead        - 15kg
			//             Phosphorus  - 5kg
			// Result: Phosphor Bronze - 100kg
			//---------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Copper, 80)
				.Input(SimHashes.Lead, 15)
				.Input(SimHashes.Phosphorus, 5)
				.Output(ModElements.PhosphorBronze, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(ARCFURNACE_SMELT_3_1, 3, 1)
				.SortOrder(index++)
				.Build();

			//---- [ Steel #1 ] -----------------------------------------------------------------------------------------------------
			// Ingredient: Iron             - 70kg
			//             Refined Coal     - 20kg
			//             Lime             - 10kg
			// Result:     Steel            - 100kg  
			//-----------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Iron, 70)
				.Input(SimHashes.RefinedCarbon, 20)
				.Input(SimHashes.Lime, 10)
				.Output(SimHashes.Steel, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(ARCFURNACE_STEEL_1, 3, 1)
				.SortOrder(index++)
				.Build();

			//---- [ Steel #2 ] --------------------------------------------------------------------------------------------------------
			// Ingredient: Iron             - 70kg
			//             Refined Coal     - 20kg
			//             Borax            - 5kg
			//             Lime             - 5kg
			// Result:     Steel            - 100kg  
			//---------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Iron, 70)
				.Input(SimHashes.RefinedCarbon, 20)
				.Input(SimHashes.Lime, 5)
				.Input(ModElements.Borax_Solid, 5)
				.Output(SimHashes.Steel, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(ARCFURNACE_STEEL_2, 4, 1)
				.SortOrder(index++)
				.Build();

			//---- [ Low-Grade Metallic Sand ] --------------------------------------------------------------------------------------------
			// Ingredient: Low-Grade Metallic Sand    - 100kg
			//             Borax                      - 10kg
			// Random Results: Copper, Zinc, Silver, Lead
			// Assured Result: Slag - 20kg, 90kg randoms
			//------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(ModElements.LowGradeSand_Solid, 100)
				.Input(ModElements.Borax_Solid, 10)
				.Output(ModElements.Slag_Solid, 20, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetArcFurnaceRandomResultString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(index++)
				.Build();

			//---- [ Base-Grade Metallic Sand ] ---------------------------------------------------------------------------------------------
			// Ingredient: Base-Grade Metallic Sand    - 100kg
			//             Borax                       - 10kg
			// Random Results: Iron, Aluminum, Gold, Tungsten
			// Assured Result: Slag - 20kg, 90kg randoms
			//-------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(ModElements.BaseGradeSand_Solid, 100)
				.Input(ModElements.Borax_Solid, 10)
				.Output(ModElements.Slag_Solid, 20, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetArcFurnaceRandomResultString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(index++)
				.Build();

			//---- [ High-Grade Metallic Sand ] -------------------------------------------------------------------------------
			// Ingredient: High-Grade Metallic Sand    - 100kg
			//             Borax                       - 10kg
			//  Vanilla :  Lime                        - 10kg
			//or DLC1   :  Graphite					   - 10kg
			// Random Results: Tungsten, Fullerene, Niobium
			// Assured Result: Slag - 30kg
			//-----------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(ModElements.HighGradeSand_Solid, 100)
				.Input(ModElements.Borax_Solid, 10)
				.InputBase(SimHashes.Lime,10).InputSO(SimHashes.Graphite, 10)
				.Output(ModElements.Slag_Solid, 30, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.DescriptionFunc(RandomRecipeProducts.GetArcFurnaceRandomResultString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.SortOrder(index++)
				.Build();



			//---- [ Thermium Seperation ] --------------------------------------------------------------------------------------------
			// Ingredient: Thermium      - 100kg
			// Result:     Niobium       - 100kg             
			//-------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder .Create(ID, 50)
				.Input(SimHashes.TempConductorSolid, 100)
				.Output(SimHashes.Niobium,100)
				.Description1I1O(ARCFURNACE_NIOBIUM)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.SortOrder(index++)
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
