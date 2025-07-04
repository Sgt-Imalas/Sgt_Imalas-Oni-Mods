using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using UtilLibs;
using RonivansLegacy_ChemicalProcessing;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ Custom: Metal Refinery ]==============================================================================================
	public class Custom_MetalRefineryConfig : IBuildingConfig
	{
		//--[ Base Information ]-------------------------------------------------------------------------------------------------
		public static string ID = "Custom_MetalRefinery";
		//--[ Identification and DLC stuff ]---------------------------------------------------------------------------------------

		//--[ Special Settings ]---------------------------------------------------------------------------------------------------
		private const float INPUT_KG = 100f;
		private const float LIQUID_COOLED_HEAT_PORTION = 0.8f;
		private static readonly Tag COOLANT_TAG = GameTags.Liquid;
		private const float COOLANT_MASS = 400f;

		//--[ Special Settings ]---------------------------------------------------------------------------------------------------
		private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Seal
		};

		//--[ Building Definitions ]------------------------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			string id = ID;
			int width = 3;
			int height = 4;
			string anim = "metalrefinery_kanim";
			int hitpoints = 30;
			float construction_time = 60f;
			float[] tier = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
			string[] all_MINERALS = MATERIALS.ALL_MINERALS;
			float melting_point = 2400f;
			BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
			EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_MINERALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 800f;
			buildingDef.SelfHeatKilowattsWhenActive = 8f;
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			buildingDef.Deprecated = true;
			return buildingDef;
		}

		//--[ Building Operation Definitions ]------------------------------------------------------------------------------------------
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
			liquidCooledRefinery.thermalFudge = 0.6f;
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
			conduitConsumer.capacityTag = GameTags.Liquid;
			conduitConsumer.capacityKG = 1000f;
			conduitConsumer.consumptionRate = 50f;
			conduitConsumer.storage = liquidCooledRefinery.inStorage;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.forceAlwaysSatisfied = true;
			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.storage = liquidCooledRefinery.outStorage;
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.elementFilter = null;
			conduitDispenser.alwaysDispense = true;
			go.AddOrGet<DeprecationTint>();
			Prioritizable.AddRef(go);
			ConfigureRecipes(ID);
		}

		//===[ Custom: Metal Refinery Recipes | Ore to Metal Ratio: 80%]==========================================================================
		public static void ConfigureRecipes(string ID)
		{
			//---- [ Basic Ore Refining ] -----------------------------------------------------------------------------------------------------
			// Ingredient: Ore  - 100kg
			//             Coal        - 20kg
			// Result: Refined Metal   - 80kg
			//         Slag            - 40kg
			//-------------------------------------------------------------------------------------------------------------------------------------
			var specialOres = RefinementRecipeHelper.GetSpecialOres();
			foreach (var element in ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.Ore)))
			{
				if (specialOres.Contains(element.id) || element.HasTag(GameTags.Noncrushable) || element.HasTag(ModAssets.Tags.RandomSand))
				{
					continue;
				}

				Element refinedElement = element.highTempTransition.lowTempTransition;

				RecipeBuilder.Create(ID, 40)
					.Input(element.id, 100f)
					.Input(SimHashes.Carbon, 20f)
					.Output(refinedElement.id, 80f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(ModElements.Slag_Solid, 40f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.METALREFINERY_2_1_1,2,2)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Build();

			}

			//---- [ Basic Electrum Refining ] ----------------------------------------------------------------------------------------------------------
			// Ingredient: Electrum      - 100kg
			//             Coal          - 20kg
			// Result: Gold              - 50kg
			//         Silver            - 30kg
			//         Slag              - 30kg
			//--------------------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
					.Input(SimHashes.Electrum, 100f)
					.Input(SimHashes.Carbon, 20f)
					.Output(SimHashes.Gold, 50, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(ModElements.Silver_Solid, 30, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(ModElements.Slag_Solid, 40f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.METALREFINERY_2_2_1, 2, 3)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Build();

			//---- [ Galena Refining ] -----------------------------------------------------------------------------------------------------------
			// Ingredient: Galena      - 100kg
			//             Coal          - 20kg
			// Result: Silver            - 50kg
			//         Lead              - 30kg
			//         Slag              - 40kg
			//-------------------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
					.Input(ModElements.Galena_Solid, 100f)
					.Input(SimHashes.Carbon, 20f)
					.Output(ModElements.Silver_Solid, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Lead, 30f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(ModElements.Slag_Solid, 40f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.METALREFINERY_2_2_1, 2, 3)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Build();


			//---- [ Pyrite Refining ] -----------------------------------------------------------------------------------------------------------
			// Ingredient: Pyrite        - 100kg
			//             Coal          - 20kg
			// Result: Iron              - 60kg
			//         Slag              - 60kg
			//-------------------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
					.Input(SimHashes.FoolsGold, 100f)
					.Input(SimHashes.Carbon, 20f)
					.Output(SimHashes.Iron, 60f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(ModElements.Slag_Solid, 60f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.METALREFINERY_2_1_1,2,2)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
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

	//===[ Custom: Metal Refinery Building Patch ]===================================================================
	public class Custom_MetalRefinery_BuildingPatches
	{

		[HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
		internal class Custom_MetalRefineryUI
		{
			private static void Prefix()
			{
				ModUtil.AddBuildingToPlanScreen("Refining", Custom_MetalRefineryConfig.ID);
			}
		}
	}
}
