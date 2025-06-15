using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using UtilLibs;
using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
   public class AdditionalRecipes
    {
        public static void RegisterRecipes_RockCrusher()
		{
			string ID = RockCrusherConfig.ID;
			RecipeBuilder.Create(ID, CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_DESCRIPTION, 40)
			.Input(RefinementRecipeHelper.GetCrushables().Select(e => e.id.CreateTag()), 100f)
			.Output(SimHashes.CrushedRock, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_FROM_RAW_MINERAL_NAME)
			.SortOrder(1)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.Build();
		}
		public static void RegisterRecipes_SuperMaterialRefinery()
		{
			string ID = SupermaterialRefineryConfig.ID;

			RecipeBuilder.Create(ID, 50)
			.Input(SimHashes.Steel,60)
			.Input(SimHashes.Polypropylene,25)
			.Input(ModElements.Borax_Solid,15)
			.Output(ModElements.Plasteel_Solid, 100f)
			.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.SUPERMATERIALREFINERY_3_1,3,1)
			.SortOrder(1)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
			.Build();


			RecipeBuilder.Create(ID, 50)
			.Input(SimHashes.Propane, 50)
			.Input(SimHashes.Petroleum, 45)
			.Input(SimHashes.Isoresin, 5)
			.Output(ModElements.Isopropane_Gas, 100f)
			.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.SUPERMATERIALREFINERY_3_1, 3, 1)
			.SortOrder(1)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
			.Build();
		}

		internal static void RegisterTags()
		{
			GameTags.Fabrics = GameTags.Fabrics.Append(RayonFabricConfig.ID);
		}
	}
}
