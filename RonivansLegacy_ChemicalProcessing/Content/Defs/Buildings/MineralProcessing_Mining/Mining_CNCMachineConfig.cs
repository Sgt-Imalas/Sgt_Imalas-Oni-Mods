using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.Mining_DrillMk2_Consumables;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;

namespace Mineral_Processing_Mining.Buildings
{
	//==== [ MINING: CNC MACHINE CONFIG ] =================================================================
	public class Mining_CNCMachineConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Mining_CNCMachine";
		private HashedString[] dupeInteractAnims;

		//--[ Building Definitions ]---------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 4, "cnc_machine_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = true;
			buildingDef.OverheatTemperature = 348.15f;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 1000f;
			buildingDef.ExhaustKilowattsWhenActive = 6f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			SoundUtils.CopySoundsToAnim("cnc_machine_kanim", "supermaterial_refinery_kanim");
			return buildingDef;
		}

		//--[ Building Operation Definitions ]---------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
			complexFabricator.heatedTemperature = 313.15f;
			complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			complexFabricator.duplicantOperated = false;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
			Prioritizable.AddRef(go);
			this.ConfigureRecipes();
		}

		private void ConfigureRecipes()
		{
			var combustibles = ElementLoader.elements.FindAll(e => e.HasTag(GameTags.CombustibleLiquid)).Select(element => element.id);
			var plastics = RefinementRecipeHelper.GetPlasticIds();

			int pos = SimpleDrillbits_Config.CreateSimpleDrillRecipes(ID,false);
			//===[ Basic Drill Bits ]===========================================================================================================================
			// Ingredients: Iron - 250kg
			//              Copper - 50kg
			//              Petroleum - 20kg
			// Result: Basic Drill Bits 2x
			//==================================================================================================================================================
			RecipeBuilder.Create(ID,50)
				.Input(SimHashes.Iron, 250)
				.Input(RefinementRecipeHelper.GetStarterMetals(), 50)
				.Input(combustibles, 20)
				.Output(Mining_Drillbits_Basic_ItemConfig.TAG, 2, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
				.Description(MINING_DRILLBITS_BASIC_ITEM.RECIPE_DESC)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.SortOrder(pos++)
				.Build();

			//===[ Steel Drill Bits ]===========================================================================================================================
			// Ingredients: Steel - 250kg
			//              Iron - 50kg
			//              Petroleum - 20kg
			// Result: Steel Drill Bits 2x
			//==================================================================================================================================================
			RecipeBuilder.Create(ID, 50)
				.Input(RefinementRecipeHelper.GetSteelLikes(), 250)
				.Input(SimHashes.Iron, 50)
				.Input(combustibles, 20)
				.Output(Mining_Drillbits_Steel_ItemConfig.TAG, 2, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
				.Description(MINING_DRILLBITS_STEEL_ITEM.RECIPE_DESC)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.SortOrder(pos++)
				.Build();

			//===[ Tungsten Drill Bits ]===========================================================================================================================
			// Ingredients: Tungsten - 200kg
			//              Steel - 100kg
			//              Petroleum - 20kg
			// Result: Steel Drill Bits 2x
			//==================================================================================================================================================
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Tungsten, 200)
				.Input(RefinementRecipeHelper.GetSteelLikes(), 100)
				.Input(combustibles, 20)
				.Output(Mining_Drillbits_Tungsten_ItemConfig.TAG, 2, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
				.Description(MINING_DRILLBITS_TUNGSTEN_ITEM.RECIPE_DESC)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.SortOrder(pos++)
				.Build();

			//===[ Guidance Device ]===========================================================================================================================
			// Ingredients: Steel - 50kg
			//              Gold - 20kg
			//              Niobium - 10kg 
			///code did not match with description, using code version here since its a balancing change.
			// Result: Guidance Device
			//==================================================================================================================================================
			
			bool isBioChemistryEnabled = Config.Instance.ChemicalProcessing_BioChemistry_Enabled;

			RecipeBuilder.Create(ID, 50)
				.Input(RefinementRecipeHelper.GetSteelLikes(), 50)
				.Input([SimHashes.Gold, ModElements.Silver_Solid], 20)
				.Input(SimHashes.Glass, 10)
				.Input(plastics, 10)
				.Output(Mining_Drillbits_GuidanceDevice_ItemConfig.TAG, 1, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
				.Description(MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.RECIPE_DESC)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.SortOrder(pos++)
				.Build();

			foreach(var programmable in Mining_Drillbits_GuidanceDevice_ItemConfig.ProgrammedGuidanceModules)
			{
				///Make an automatic recipe for each programmed guidance module from the base one,
				///they can be changed manually in the game, but this allows full automation
				RecipeBuilder.Create(ID,10)
					.Input(Mining_Drillbits_GuidanceDevice_ItemConfig.TAG, 1)
					.Output(programmable,1)
					.Description(string.Format(MINING_DRILLBITS_GUIDANCEDEVICE_ITEM.RECIPE_DESC_PROGRAM,Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(programmable)))
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
					.SortOrder(pos++)
					.Build();
			}
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
		}
	}
}
