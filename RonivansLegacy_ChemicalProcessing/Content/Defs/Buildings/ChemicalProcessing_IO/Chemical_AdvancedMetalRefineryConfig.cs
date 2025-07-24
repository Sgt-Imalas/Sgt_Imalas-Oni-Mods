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
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INGREDIENTS;
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

		//--[ Special Settings ]-----------------------------------------------------------------------------------------
		private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Seal
		};

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
			liquidCooledRefinery.inStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
			liquidCooledRefinery.buildStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
			liquidCooledRefinery.outStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
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

			//---- [ Advanced Generic Ore Refining ] --------------------------------------------------------------------------------------------------------- 
			// Ingredient: Ore		           - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Refined Metal   - 370kg
			//         Slag            - 130kg
			//-------------------------------------------------------------------------------------------------------------------------------------------
			foreach (var element in RefinementRecipeHelper.GetNormalOres())
			{

				Element refinedElement = element.highTempTransition.lowTempTransition;

				RecipeBuilder.Create(ID, 40)
					.Input(element.tag, 400f)
					.Input(SimHashes.RefinedCarbon.CreateTag(), 50f)
					.Input(SimHashes.Sand.CreateTag(), 50f)
					.Output(refinedElement.tag, 370f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(Slag_Solid.Tag, 130f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description3I2O(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.THREE_MIXTURE_SMELT_WASTE)
					.Build();
			}


			//---- [ Advanced Electrum Refining ] ----------------------------------------------------------------------------------------------------------
			//==== [ Ore to Metal Ratio: 92,5% ]   
			// Ingredient: Electrum            - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Gold            - 250kg
			//         Silver          - 120kg
			//         Slag            - 130kg
			//----------------------------------------------------------------------------------------------------------------------------------------------
			ComplexRecipe.RecipeElement[] ElectrumRefiningIngredients =
			[
				new ComplexRecipe.RecipeElement(SimHashes.Electrum.CreateTag(), 400f),
				new ComplexRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 50f),
				new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 50f)
			];
			ComplexRecipe.RecipeElement[] ElectrumRefiningProducts =
			[
				new ComplexRecipe.RecipeElement(SimHashes.Gold.CreateTag(), 250f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
				new ComplexRecipe.RecipeElement(Silver_Solid.Tag, 120f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
				new ComplexRecipe.RecipeElement(Slag_Solid.Tag, 130f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			];
			var recipe_7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, ElectrumRefiningIngredients, ElectrumRefiningProducts), ElectrumRefiningIngredients, ElectrumRefiningProducts)
			{
				time = 40f,
				description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.THREE_MIXTURE_TWO_PRODUCTS_SMELT_WASTE,
				SimHashes.Electrum.CreateTag().ProperName(),
				SimHashes.RefinedCarbon.CreateTag().ProperName(),
				SimHashes.Sand.CreateTag().ProperName(),
				SimHashes.Gold.CreateTag().ProperName(),
				Silver_Solid.Tag.ProperName(),
				Slag_Solid.Tag.ProperName()),
				nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
				fabricators = new List<Tag> { ID },
			};

			//---- [ Advanced Galena Refining ] -----------------------------------------------------------------------------------------------------------
			// Ingredient: Galena              - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Lead            - 150kg
			//         Silver          - 200kg
			//         Slag            - 150kg
			//---------------------------------------------------------------------------------------------------------------------------------------------
			ComplexRecipe.RecipeElement[] array15 =
			[
				new ComplexRecipe.RecipeElement(Galena_Solid.Tag, 400f),
				new ComplexRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 50f),
				new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 50f)
			];
			ComplexRecipe.RecipeElement[] array16 =
			[
				new ComplexRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
				new ComplexRecipe.RecipeElement(Silver_Solid.Tag, 200f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
				new ComplexRecipe.RecipeElement(Slag_Solid.Tag, 150f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			];
			var recipe_8 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array15, array16), array15, array16)
			{
				time = 40f,
				description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.THREE_MIXTURE_TWO_PRODUCTS_SMELT_WASTE,
				Galena_Solid.Tag.ProperName(),
				SimHashes.RefinedCarbon.CreateTag().ProperName(),
				SimHashes.Sand.CreateTag().ProperName(),
				SimHashes.Lead.CreateTag().ProperName(),
				Silver_Solid.Tag.ProperName(),
				Slag_Solid.Tag.ProperName()),
				nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
				fabricators = new List<Tag> { ID },
			};

			//---- [ Advanced Iron Refining with Pyrite ] --------------------------------------------------------------------------------------------------
			// Ingredient: Pyrite              - 400kg
			//             Refined Coal        - 50kg
			//             Sand                - 50kg
			// Result: Iron            - 300kg
			//         Slag            - 200kg
			//----------------------------------------------------------------------------------------------------------------------------------------------
			ComplexRecipe.RecipeElement[] array17 =
			[
				new ComplexRecipe.RecipeElement(SimHashes.FoolsGold.CreateTag(), 400f),
				new ComplexRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 50f),
				new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 50f)
			];
			ComplexRecipe.RecipeElement[] array18 =
			[
				new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 300f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
				new ComplexRecipe.RecipeElement(Slag_Solid.Tag, 200f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			];
			var recipe_9 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array17, array18), array17, array18)
			{
				time = 40f,
				description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.THREE_MIXTURE_SMELT_WASTE,
				SimHashes.FoolsGold.CreateTag().ProperName(),
				SimHashes.RefinedCarbon.CreateTag().ProperName(),
				SimHashes.Sand.CreateTag().ProperName(),
				SimHashes.Iron.CreateTag().ProperName(),
				Slag_Solid.Tag.ProperName()),
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { ID },
			};
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
