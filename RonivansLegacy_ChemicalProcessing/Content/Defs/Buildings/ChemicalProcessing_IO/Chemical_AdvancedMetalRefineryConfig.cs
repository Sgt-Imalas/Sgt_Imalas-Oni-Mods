using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static LogicGate.LogicGateDescriptions;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS;
using static STRINGS.ELEMENTS;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: ADVANCED METAL REFINERY CONFIG ]================================================================
	public class Chemical_AdvancedMetalRefineryConfig : IBuildingConfig
	{
		//--[ Base Information ]-------------------------------------------------------------------------------------
		public const string ID = "Chemical_AdvancedMetalRefinery";

		private const float INPUT_KG = 500f;
		private const float LIQUID_COOLED_HEAT_PORTION = 0.8f;
		private static readonly Tag COOLANT_TAG = SimHashes.SuperCoolant.CreateTag();
		private const float COOLANT_MASS = 1000f;


		//--[ Building Definitions ]--------------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = [1000f, 600f];
			string[] materials1 = [SimHashes.Steel.ToString(), SimHashes.Ceramic.ToString()];

			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 5, "adv_metalrefinery_kanim", 30, 60f, quantity1, materials1, 2400f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.EnergyConsumptionWhenActive = 3000f;
			def1.SelfHeatKilowattsWhenActive = 16f;
			def1.InputConduitType = ConduitType.Liquid;
			def1.UtilityInputOffset = new CellOffset(-1, 1);
			def1.OutputConduitType = ConduitType.Liquid;
			def1.UtilityOutputOffset = new CellOffset(1, 0);
			def1.ViewMode = OverlayModes.Power.ID;
			def1.AudioCategory = "HollowMetal";
			def1.AudioSize = "large";
			SoundUtils.CopySoundsToAnim("adv_metalrefinery_kanim", "metalrefinery_kanim");
			return def1;
		}

		//--[ Building Operation Definitions ]------------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			LiquidCooledRefinery liquidCooledRefinery = go.AddOrGet<LiquidCooledRefinery>();
			liquidCooledRefinery.duplicantOperated = true;
			liquidCooledRefinery.heatedTemperature = 320.15f;
			liquidCooledRefinery.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			liquidCooledRefinery.keepExcessLiquids = true;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, liquidCooledRefinery);
			liquidCooledRefinery.coolantTag = COOLANT_TAG;
			liquidCooledRefinery.minCoolantMass = 400f;
			liquidCooledRefinery.outStorage.capacityKg = 2000f;
			liquidCooledRefinery.thermalFudge = 0.9f;
			liquidCooledRefinery.inStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			liquidCooledRefinery.buildStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			liquidCooledRefinery.outStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			liquidCooledRefinery.outputOffset = new Vector3(1f, 0.5f);
			workable.overrideAnims =
			[
			Assets.GetAnim("anim_interacts_metalrefinery_kanim")
			];
			go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.capacityTag = COOLANT_TAG;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			conduitConsumer.capacityKG = 1000f;
			conduitConsumer.consumptionRate = 500f;
			conduitConsumer.storage = liquidCooledRefinery.inStorage;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.forceAlwaysSatisfied = true;

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.storage = liquidCooledRefinery.outStorage;
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.elementFilter = null;
			conduitDispenser.alwaysDispense = true;

			Prioritizable.AddRef(go);
			this.ConfigureRecipes();
		}

		//==== [ CHEMICAL: ADVANCED METAL REFINERY RECIPES | Ore to Metal Ratio: 92,5% ] =========================================================== 
		private void ConfigureRecipes()
		{
			bool chemicalProcessingEnabled = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;

			int index = 0;
			//CHEMPROC
			//---- [ Advanced Generic Ore Refining CHEMPROC ] --------------------------------------------------------------------------------------------------------- 
			// Ingredient: Ore		           - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Refined Metal   - 370kg
			//         Slag            - 130kg
			//-------------------------------------------------------------------------------------------------------------------------------------------

			//METALLURGY
			//===[eneric Ore Refining ]=======================================================================================================================
			// Ingredients: Ore - 500kg
			// Result: Refined Metal - 500kg
			//==================================================================================================================================================
			foreach (var element in RefinementRecipeHelper.GetNormalOres())
			{
				Element refinedElement = element.highTempTransition.lowTempTransition;
				if (chemicalProcessingEnabled)
				{
					RecipeBuilder.Create(ID, 40)
						.Input(element.tag, 400f)
						.Input(SimHashes.RefinedCarbon.CreateTag(), 50f)
						.Input(SimHashes.Sand.CreateTag(), 50f)
						.Output(refinedElement.tag, 370f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
						.Output(Slag_Solid.Tag, 130f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
						.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
						.Description3I2O(THREE_MIXTURE_SMELT_WASTE)
						.SortOrder(index++)
						.Build();
				}
				else
				{
					RecipeBuilder.Create(ID, 40)
						.Input(element.tag, 400f)
						.Output(refinedElement.tag, 400f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
						.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
						.Description1I1O(ARCFURNACE_SMELT)
						.SortOrder(index++)
						.Build();
				}
			}

			//CHEMPROC
			//---- [ Advanced Electrum Refining ] ----------------------------------------------------------------------------------------------------------
			//==== [ Ore to Metal Ratio: 92,5% ]   
			// Ingredient: Electrum            - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Gold            - 250kg
			//         Silver          - 120kg
			//         Slag            - 130kg
			//----------------------------------------------------------------------------------------------------------------------------------------------

			//METALLURGY
			//===[ Electrum to Gold and Copper ]================================================================================================================
			// Ingredients: Electrum - 500kg
			// Result: Gold - 300kg
			//         Copper - 200kg "There is no silver in the game, so yeah going to be copper here"
			//==================================================================================================================================================

			if (chemicalProcessingEnabled)
			{
				RecipeBuilder.Create(ID, 40)
					.Input(SimHashes.Electrum.CreateTag(), 400f)
					.Input(SimHashes.RefinedCarbon.CreateTag(), 50f)
					.Input(SimHashes.Sand.CreateTag(), 50f)
					.Output(SimHashes.Gold.CreateTag(), 250f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(Silver_Solid.Tag, 120f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(Slag_Solid.Tag, 130f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description(THREE_MIXTURE_TWO_PRODUCTS_SMELT_WASTE,3,3)
					.SortOrder(index++)
					.Build();
			}
			else
			{
				RecipeBuilder.Create(ID, 40)
					.Input(SimHashes.Electrum.CreateTag(), 400f)
					.Output(SimHashes.Gold.CreateTag(), 250f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Copper.CreateTag(), 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description1I2O(PLASMAFURNACE_1_2)
					.SortOrder(index++)
					.Build();
			}

			//---- [ Advanced Galena Refining ] -----------------------------------------------------------------------------------------------------------
			// Ingredient: Galena              - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Lead            - 150kg
			//         Silver          - 200kg
			//         Slag            - 150kg
			//---------------------------------------------------------------------------------------------------------------------------------------------

			if (chemicalProcessingEnabled)
			{
				RecipeBuilder.Create(ID, 40)
					.Input(Galena_Solid.Tag, 400f)
					.Input(SimHashes.RefinedCarbon.CreateTag(), 50f)
					.Input(SimHashes.Sand.CreateTag(), 50f)
					
					.Output(SimHashes.Lead.CreateTag(), 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(Silver_Solid.Tag, 200f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(Slag_Solid.Tag, 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description(THREE_MIXTURE_TWO_PRODUCTS_SMELT_WASTE,3,3)
					.SortOrder(index++)
					.Build();
			}

			//CHEMPROC
			//---- [ Advanced Iron Refining with Pyrite ] --------------------------------------------------------------------------------------------------
			// Ingredient: Pyrite              - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Iron            - 300kg
			//         Slag            - 200kg
			//----------------------------------------------------------------------------------------------------------------------------------------------

			//METALLURGY
			//===[ Pyrite to Iron ]=============================================================================================================================
			// Ingredients: Pyrite - 400kg
			// Result: Iron - 400kg
			//         Sulfur - 100kg
			//==================================================================================================================================================
			if (chemicalProcessingEnabled)
			{
				RecipeBuilder.Create(ID, 40)
					.Input(SimHashes.FoolsGold.CreateTag(), 400f)
					.Input(SimHashes.RefinedCarbon.CreateTag(), 50f)
					.Input(SimHashes.Sand.CreateTag(), 50f)

					.Output(SimHashes.Iron.CreateTag(), 300f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(Slag_Solid.Tag, 200f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)

					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description(THREE_MIXTURE_SMELT_WASTE, 3, 2)
					.SortOrder(index++)
					.Build();
			}
			else
			{
				RecipeBuilder.Create(ID, 40)
					.Input(SimHashes.FoolsGold.CreateTag(), 400f)

					.Output(SimHashes.Iron.CreateTag(), 300f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Sulfur.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)

					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description1I1O(ARCFURNACE_SMELT)
					.SortOrder(index++)
					.Build();
			}


			///METALLURGY exclusive recipes:
			if (chemicalProcessingEnabled)
				return;

			//===[ Abyssalite Smelting ]========================================================================================================================
			// Ingredients: Abyssalite - 500kg
			//              Refined Coal - 100kg
			//              Lime - 20kg
			// Result: Tungsten - 50kg
			//         Phosphorus - 100kg
			//         Sand - 350kg
			//==================================================================================================================================================


			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Katairite.CreateTag(), 400f)
				.Input(SimHashes.RefinedCarbon.CreateTag(), 100f)
				.Input(SimHashes.Lime.CreateTag(), 20f)

				.Output(SimHashes.Tungsten.CreateTag(), 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Output(SimHashes.Phosphorus.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
				.Output(SimHashes.Sand.CreateTag(), 350f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)

				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Description(ADVANCED_REFINERY_TRACE_EXTRACTION_1_2,1,2)
				.SortOrder(index++)
				.Build();

			//===[ Iron to Steel ]==============================================================================================================================
			// Ingredients: Iron - 350kg
			//              Refined Coal - 100kg
			//              Lime - 50kg
			// Result: Steel - 500kg
			//==================================================================================================================================================

			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Iron, 280)
				.Input(SimHashes.RefinedCarbon, 80)
				.Input(SimHashes.Lime, 40)
				.Output(SimHashes.Steel, 400, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(ARCFURNACE_STEEL_1, 3, 1)
				.SortOrder(index++)
				.Build();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			SymbolOverrideControllerUtil.AddToPrefab(go);
			go.AddOrGetDef<PoweredActiveStoppableController.Def>();
			go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject game_object)
			{
				ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
				component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
				component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
				component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
				component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
				component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
			};
		}
	}
}
