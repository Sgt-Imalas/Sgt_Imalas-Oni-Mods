using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: JAW CRUSHER MILL CONFIG ]===========================================================================
	public class Chemical_SmallCrusherMillConfig : IBuildingConfig, IHasConfigurableWattage
	{
		//--[ Base Information ]-----------------------------------------------------------------------------------------
		public static string ID = "Chemical_SmallCrusherMill";

		public static float Wattage = 240f;
		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;


		//--[ Building Definitions ]---------------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "jawcrusher_mill_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = GetWattage();
			buildingDef.ExhaustKilowattsWhenActive = 16f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.AudioCategory = "Metal";
			SoundUtils.CopySoundsToAnim("jawcrusher_mill_kanim", "rockrefinery_kanim");
			return buildingDef;
		}

		//--[ Building Operation Definitions ]------------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
			complexFabricator.heatedTemperature = 298.15f;
			complexFabricator.duplicantOperated = false;
			complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
			ConfigureRecipes(ID);
			Prioritizable.AddRef(go);
		}

		//===[ CHEMICAL: SMALL CRUSHER MILL RECIPES ]====================================================================
		public static void ConfigureRecipes(string ID)
		{
			bool chemproc = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;
			float recipeDuration = 45;
			int index = 0;
			//---- [ Egg Shell Milling ] --------------------------------------------------------------------------------
			// Ingredient: Eggshell  - 5kg
			// Result: Lime          - 5kg
			//-----------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
				.Input(EggShellConfig.ID, 5f)
				.Output(SimHashes.Lime, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION,
				global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.EGG_SHELL.NAME,
				global::STRINGS.ELEMENTS.LIME.NAME))
				.SortOrder(index++)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			//---- [ Pokeshell Molt Milling ] -----------------------------------------------------------------------------
			// Ingredient: Pokeshell Molt    - 1 full grown crab (10kg)
			// Result: Lime                  - 10kg
			//-------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, recipeDuration)
				.Input(CrabShellConfig.ID, 10f)
				.Output(SimHashes.Lime, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION,
				global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME,
				global::STRINGS.ELEMENTS.LIME.NAME))
				.SortOrder(index++)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			//---- [ Oakshell Molt Milling ] -----------------------------------------------------------------------------
			// Ingredient: Oakshell Molt    - 5kg
			// Result: Wood                  - 500kg
			//-------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
				.Input(CrabWoodShellConfig.ID, 500f)
				.Output(SimHashes.WoodLog, 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION,
				global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.VARIANT_WOOD.NAME,
				global::STRINGS.ELEMENTS.LIME.NAME))
				.SortOrder(index++)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			//---- [ Fossil Milling ] ----------------------------------------------------------------------------------------
			// Ingredient: Fossil     - 100kg
			// Result: Lime           - 5kg
			//         Crushed Rock   - 70g
			//         Bitumen        - 25kg
			//-----------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
			.Input(SimHashes.Fossil, 100)
			.Output(SimHashes.Lime, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			.Output(SimHashes.CrushedRock, 70f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			.Output(chemproc ? SimHashes.Bitumen : SimHashes.Sand, 25f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			.Description1I3O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1_2)
			.SortOrder(index++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
			.Build();

			//---- [ Salt Milling ] --------------------------------------------------------------------------------------------
			// Ingredient: Salt     - 100kg
			// Result: Borax        - 5kg
			//         Table Salt   - 5g
			//         Sand         - 95kg
			//------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
			.Input(SimHashes.Salt, 100)
			.Output(ModElements.Borax_Solid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			.Output(SimHashes.Sand, 89.95f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			.Output(TableSaltConfig.ID.ToTag(), 0.05f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			.Description(
				string.Format(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1_2,
				SimHashes.Salt.CreateTag().ProperName(),
				ModElements.Borax_Solid.Tag.ProperName(),
				SimHashes.Sand.CreateTag().ProperName(),
				global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME
				))
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
			.SortOrder(index++)
			.Build();

			//---- [ Phosphate Nodules Milling ] ----------------------------------------------------------------------------------
			// Ingredient: Phosphate Nodules    - 100kg
			// Result: Phosphorus               - 70kg
			//         Crushed Rock             - 30kg
			//---------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
				.Input(SimHashes.PhosphateNodules, 100f)
				.Output(SimHashes.Phosphorus, 70f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Output(SimHashes.CrushedRock, 30f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description1I1O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.SortOrder(index++)
				.Build();

			//---- [ Crushed Rock Milling ] ------------------------------------------------------------------------------------------
			// Ingredient: Crushed Rock - 100kg
			// Result: Sand - 100kg
			//------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
				.Input(SimHashes.CrushedRock, 100f)
				.Output(SimHashes.Sand, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description1I1O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.SortOrder(index++)
				.Build();

			//---- [ Obsidian Milling ] ------------------------------------------------------------------------------------------------
			// Ingredient: Obsidian - 100kg
			// Result: Sand - 100kg
			//--------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
				.Input(SimHashes.Obsidian, 100f)
				.Output(SimHashes.Sand, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description1I1O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.SortOrder(index++)
				.Build();

			//---- [ Chloroschist Milling ] ----------------------------------------------------------------------------------------------
			// Ingredient: Chloroschist - 100kg
			// Result: Crushed Rock - 65kg
			//         Sand - 20kg
			//         Bleachstone - 0,5kg
			//         Salt - 14,5kg
			//----------------------------------------------------------------------------------------------------------------------------
			if (chemproc)
			{
				RecipeBuilder.Create(ID, recipeDuration)
					.Input(ModElements.Chloroschist_Solid, 100f)
					.Output(SimHashes.CrushedRock, 55f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Sand, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.BleachStone, 15f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Salt, 20f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description1I4O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_4)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.SortOrder(index++)
					.Build();
			}



			//---- [ Crushable to Crushed Rock ] --------------------------------------------------------------------------------------------
			// Ingredient: Crushable - 100kg
			// Result: Crushed Rock - 100kg
			//-------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_DESCRIPTION, recipeDuration)
				.Input(RefinementRecipeHelper.GetCrushables([SimHashes.Obsidian]).Select(e => e.id.CreateTag()), 100f)
				.Output(SimHashes.CrushedRock, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_NAME)
				.SortOrder(0)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.SortOrder(index++)
				.Build();

			//---- [ Ore Crushing ] -----------------------------------------------------------------------------------------------------
			// Ingredient: Ore - 100kg
			// Result: RefinedMetal - 50kg
			//         Sand - 50g
			//----------------------------------------------------------------------------------------------------------------------------------
			foreach (var element in RefinementRecipeHelper.GetNormalOres())
			{

				Element refinedElement = element.highTempTransition.lowTempTransition;
				RecipeBuilder.Create(ID, recipeDuration)
					.Input(element.id, 100f)
					.Output(refinedElement.id, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Sand, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description1I1O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1_BREAK)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.SortOrder(index++)
					.Build();

			}

			///Chemproc:
			//---- [ Electrum Crushing ] ---------------------------------------------------------------------------------------------------------
			// Ingredient: Electrum - 100kg
			// Result: Gold - 25kg
			//         Silver - 15kg
			//         Sand - 50g
			//------------------------------------------------------------------------------------------------------------------------------------
			///Metallurgy:
			/// //---- [ Electrum Crushing ] ---------------------------------------------------------------------------------------------------------
			// Ingredient: Electrum - 100kg
			// Result: Gold - 25kg
			//         Copper - 15kg
			//         Sand - 50g
			//------------------------------------------------------------------------------------------------------------------------------------


			RecipeBuilder.Create(ID, recipeDuration)
					.Input(SimHashes.Electrum, 100f)
					.Output(SimHashes.Gold, 25f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(chemproc ? ModElements.Silver_Solid : SimHashes.Copper, 15f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Sand, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description1I2O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_2)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.SortOrder(index++)
					.Build();

			//---- [ Galena Crushing ] ---------------------------------------------------------------------------------------------------------
			// Ingredient: Galena - 100kg
			// Result: Silver - 25kg
			//         Lead - 15kg
			//         Sand - 50g
			//------------------------------------------------------------------------------------------------------------------------------------
			if (chemproc)
			{
				RecipeBuilder.Create(ID, recipeDuration)
						.Input(ModElements.Galena_Solid, 100f)
						.Output(ModElements.Silver_Solid, 25f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
						.Output(SimHashes.Lead, 15f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
						.Output(SimHashes.Sand, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
						.Description1I2O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_2)
						.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
						.SortOrder(index++)
						.Build();
			}

			//---- [ Pyrite Crushing ] -----------------------------------------------------------------------------------------------------
			// Ingredient: Pyrite - 100kg
			// Result: Iron - 30kg
			//         Sand - 70g
			//-------------------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, recipeDuration)
					.Input(SimHashes.FoolsGold, 100f)
					.Output(SimHashes.Iron, 30f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Output(SimHashes.Sand, 70f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description1I1O(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.SortOrder(index++)
					.Build();

			
			//bammoth patty:
			float phosphoriteRate = 4f / 15f;
			float poopMass = 120f;

			RecipeBuilder.Create(ID, recipeDuration)
					.Input("IceBellyPoop", poopMass)
					.Output(SimHashes.Phosphorite, poopMass * phosphoriteRate)
					.Output(SimHashes.Clay, poopMass * (1f - phosphoriteRate))
					.Description(string.Format(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_2, STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ICE_BELLY_POOP.NAME, SimHashes.Phosphorite.CreateTag().ProperName(), SimHashes.Clay.CreateTag().ProperName()))
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.SortOrder(index++)
					.Build();

			//regal bammoth crown
			RecipeBuilder.Create(ID, recipeDuration)
					.Input("GoldBellyCrown", 1)
					.Output(SimHashes.GoldAmalgam, 250)
					.Description(string.Format(CHEMICAL_COMPLEXFABRICATOR_STRINGS.JAWCRUSHERMILL_MILLING_1_1, STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.GOLD_BELLY_CROWN.NAME, SimHashes.GoldAmalgam.CreateTag().ProperName()))
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.SortOrder(index++)
					.Build();

			//Cement from crushing slag
			AdditionalRecipes.SlagCementRecipe(ID);
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
