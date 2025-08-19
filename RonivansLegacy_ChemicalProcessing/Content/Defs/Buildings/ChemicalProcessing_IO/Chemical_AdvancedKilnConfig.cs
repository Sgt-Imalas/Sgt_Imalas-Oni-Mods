using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
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
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INGREDIENTS;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS;


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
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "fabricator_generic_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
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
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
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
			bool cemprocessingEnabled = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;

			//---- [ Refined Coal ] -------------------------------------------------------------------------------------
			// Ingredient: Coal - 500kg        
			// Result: Refined Coal - 500kg
			//-----------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 30)
				.Input([SimHashes.Carbon, SimHashes.WoodLog, SimHashes.Peat], [500, 800, 1200], GameTags.CombustibleSolid)
				.Output(SimHashes.RefinedCarbon, 500, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description1I1O(HEAT_REFINE)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Build();


			//---- [ Ceramic ] -------------------------------------------------------------------------------------------
			// Ingredient: Clay - 300kg
			//             Sand - 200kg
			// Result: Ceramic  - 500kg
			//------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 30)
				.Input(SimHashes.Clay, 300)
				.Input(SimHashes.Sand, 200)
				.Output(SimHashes.Ceramic, 500, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description1I1O(HEAT_REFINE)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Build();

			//---- [ Concrete Block ] --------------------------------------------------------------------------------------
			// Ingredient: Sand         - 100kg
			//             Crushed Rock - 200kg
			//             Slag         - 200kg
			//             Water        -  25kg
			// Result: Concrete Block   - 500kg
			//---------------------------------------------------------------------------------------------------------------

			if (!Config.Instance.DupesEngineering_Enabled && cemprocessingEnabled) //that mod adds cement mixer with a more realistic concrete recipe
			{
				RecipeBuilder.Create(ID, 30)
					.Input(SimHashes.Sand, 100)
					.Input(SimHashes.CrushedRock, 200)
					.Input(Slag_Solid, 200)
					.Input(SimHashes.Water, 25)
					.Output(ConcreteBlock_Solid, 500, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(THREE_MIXTURE_COMPRESS_COOKING, 3, 1)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
					.Build();
			}

			//---- [ Fiberglass ] --------------------------------------------------------------------------------------------
			// Ingredient: Sand         - 270kg
			//             Plastic      - 100kg
			//             Borax        - 30kg
			// Result: Fiberglass       - 300g
			//-------------------------------------------------------------------------------------------------------------------
			if (cemprocessingEnabled)
				RecipeBuilder.Create(ID, 30)
					.Input(SimHashes.Sand, 270)
					.Input(RefinementRecipeHelper.GetPlasticIds(FiberGlass_Solid), 100f, GameTags.Plastic)
					.Input(Borax_Solid, 30)
					.Output(FiberGlass_Solid, 400, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(THREE_MIXTURE_FUSE, 3, 1)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
					.Build();

			//---- [ Carbon Fibre ] --------------------------------------------------------------------------------------------
			// Ingredient: Bitumen      - 100kg
			//             Fullerene    -  20kg
			//             Rayon Fiber  -  10kg
			//             Isoresin     -  15kg
			// Result: Carbon Fiber     - 100kg
			//-------------------------------------------------------------------------------------------------------------------
			if (cemprocessingEnabled)
				RecipeBuilder.Create(ID, 30)
					.Input(SimHashes.Bitumen, 100)
					.Input(SimHashes.Fullerene, 25f)
					.Input(SimHashes.Isoresin, 15f)
					.Input(RayonFabricConfig.ID.ToTag(), 10f)
					.Output(CarbonFiber_Solid, 150, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(string.Format(THREE_MIXTURE_COMPRESS_COOKING, SimHashes.Polypropylene.CreateTag().ProperName(), SimHashes.Fullerene.CreateTag().ProperName(), RAYONFIBER.NAME_PLURAL, CarbonFiber_Solid.Tag.ProperName()))
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
					.Build();



			///Cement from burning oilshale
			AdditionalRecipes.AdditionalKilnRecipes(ID, false, true);
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
