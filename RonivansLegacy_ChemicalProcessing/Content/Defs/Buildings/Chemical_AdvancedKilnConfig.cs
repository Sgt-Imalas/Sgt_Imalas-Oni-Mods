using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INGREDIENTS;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: ADVANCED KILN CONFIG ]=====================================================================
	public class Chemical_AdvancedKilnConfig : IBuildingConfig
	{
		//--[ Base Information ]--------------------------------------------------------------------------------
		public static string ID = "Chemical_AdvancedKiln";

		//--[ Storage Modifier Settings ]--------------------------------------------------------------------------------
		private static readonly List<Storage.StoredItemModifier> KilnStoredItemModifiers = [
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Seal];

		//--[ Building Definitions ]-----------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "fabricator_generic_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier, 0.2f);
			def1.Overheatable = false;
			def1.RequiresPowerInput = true;
			def1.EnergyConsumptionWhenActive = 800f;
			def1.ExhaustKilowattsWhenActive = 16f;
			def1.SelfHeatKilowattsWhenActive = 4f;
			def1.AudioCategory = "HollowMetal";
			def1.PowerInputOffset = new CellOffset(1, 0);
			return def1;
		}

		//--[ Building Operation Definitions ]---------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Prioritizable.AddRef(go);
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
			fabricator.heatedTemperature = 320.15f;
			fabricator.duplicantOperated = true;
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.inStorage.SetDefaultStoredItemModifiers(KilnStoredItemModifiers);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(KilnStoredItemModifiers);
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = [Assets.GetAnim("anim_interacts_fabricator_generic_kanim")];

			fabricator.fetchChoreTypeIdHash = Db.Get().ChoreTypes.FabricateFetch.IdHash;
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			this.ConfigureRecipes();
		}

		//===[ CHEMICAL: ADVANCED KILN RECIPES ]=========================================================================
		private void ConfigureRecipes()
		{
			//---- [ Refined Coal ] -------------------------------------------------------------------------------------
			// Ingredient: Coal - 500kg        
			// Result: Refined Coal - 500kg
			//-----------------------------------------------------------------------------------------------------------
			{
				ComplexRecipe.RecipeElement[] inputs =
				[
				new ComplexRecipe.RecipeElement(SimHashes.Carbon.CreateTag(), 500f)
				];
				ComplexRecipe.RecipeElement[] results =
				[
				new ComplexRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
				];
				var recipe_1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, inputs, results), inputs, results)
				{
					time = 30f,
					description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.HEAT_REFINE, SimHashes.Carbon.CreateTag().ProperName(), SimHashes.RefinedCarbon.CreateTag().ProperName()),
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
					fabricators = new List<Tag> { ID },
					sortOrder = 1
				};
			}

			//---- [ Ceramic ] -------------------------------------------------------------------------------------------
			// Ingredient: Clay - 300kg
			//             Sand - 200kg
			// Result: Ceramic  - 500kg
			//------------------------------------------------------------------------------------------------------------
			{
				ComplexRecipe.RecipeElement[] inputs =
				[
				new ComplexRecipe.RecipeElement(SimHashes.Clay.CreateTag(), 300f),
				new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 200f)
				];
				ComplexRecipe.RecipeElement[] results =
				[
				new ComplexRecipe.RecipeElement(SimHashes.Ceramic.CreateTag(), 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
				];
				var recipe_2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, inputs, results), inputs, results)
				{
					time = 30f,
					description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.HEAT_REFINE,
					SimHashes.Clay.CreateTag().ProperName(), SimHashes.Ceramic.CreateTag().ProperName()),
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
					fabricators = new List<Tag> { ID },
					sortOrder = 2
				};
			}

			//---- [ Concrete Block ] --------------------------------------------------------------------------------------
			// Ingredient: Sand         - 100kg
			//             Crushed Rock - 200kg
			//             Slag         - 200kg
			//             Water        -  25kg
			// Result: Concrete Block   - 500kg
			//---------------------------------------------------------------------------------------------------------------
			{
				ComplexRecipe.RecipeElement[] inputs =
				[
					new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 100f),
				new ComplexRecipe.RecipeElement(SimHashes.CrushedRock.CreateTag(), 200f),
				new ComplexRecipe.RecipeElement(Slag_Solid.Tag, 200f),
				new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 25f)
				];
				ComplexRecipe.RecipeElement[] results =
				[
				new ComplexRecipe.RecipeElement(ConcreteBlock_Solid.Tag, 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
				];
				var recipe_3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, inputs, results), inputs, results)
				{
					time = 30f,
					description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.THREE_MIXTURE_COMPRESS_COOKING,
						SimHashes.Sand.CreateTag().ProperName(), 
						SimHashes.CrushedRock.CreateTag().ProperName(),
						Slag_Solid.Tag.ProperName(),
						ConcreteBlock_Solid.Tag.ProperName()),					
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
					fabricators = new List<Tag> { ID },
					sortOrder = 3
				};
			}

			//---- [ Fiberglass ] --------------------------------------------------------------------------------------------
			// Ingredient: Sand         - 270kg
			//             Plastic      - 100kg
			//             Borax        - 30kg
			// Result: Fiberglass       - 300g
			//-------------------------------------------------------------------------------------------------------------------
			ComplexRecipe.RecipeElement[] array7 =
			[
				new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 270f),
				new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 100f),
				new ComplexRecipe.RecipeElement(ModElements.Borax_Solid.Tag, 30f)
			];
			ComplexRecipe.RecipeElement[] array8 =
			[
			new ComplexRecipe.RecipeElement(ModElements.FiberGlass_Solid.Tag, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			];
			var recipe_4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array7, array8), array7, array8)
			{
				time = 30f,
				description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.THREE_MIXTURE_FUSE,
					SimHashes.Sand.CreateTag().ProperName(),
					Borax_Solid.Tag.ProperName(),
					SimHashes.Polypropylene.CreateTag().ProperName(),
					FiberGlass_Solid.Tag.ProperName()),
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { ID },
				sortOrder = 4
			};

			//---- [ Carbon Fibre ] --------------------------------------------------------------------------------------------
			// Ingredient: Bitumen      - 100kg
			//             Fullerene    -  20kg
			//             Rayon Fiber  -  10kg
			//             Isoresin     -  15kg
			// Result: Carbon Fiber     - 100kg
			//-------------------------------------------------------------------------------------------------------------------
			ComplexRecipe.RecipeElement[] array9 =
			[
				new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 100f),
				new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 20f),
				new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 15f),
				new ComplexRecipe.RecipeElement(RayonFabricConfig.ID.ToTag(), 10f)
			];
			ComplexRecipe.RecipeElement[] array10 =
			[
			new ComplexRecipe.RecipeElement(CarbonFiber_Solid.Tag, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			];
			var recipe_7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array9, array10), array9, array10)
			{
				time = 30f,
				description = string.Format(RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.THREE_MIXTURE_COMPRESS_COOKING,
					SimHashes.Polypropylene.CreateTag().ProperName(),
					SimHashes.Fullerene.CreateTag().ProperName(),
					RAYONFIBER.NAME_PLURAL,
					CarbonFiber_Solid.Tag.ProperName()),
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { ID },
				sortOrder = 7
			};
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
