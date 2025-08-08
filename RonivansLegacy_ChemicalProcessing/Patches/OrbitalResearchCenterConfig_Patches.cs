using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class OrbitalResearchCenterConfig_Patches
	{

        [HarmonyPatch(typeof(OrbitalResearchCenterConfig), nameof(OrbitalResearchCenterConfig.ConfigureRecipes))]
        public class OrbitalResearchCenterConfig_ConfigureRecipes_Patch
		{
            [HarmonyPrepare]
            public static bool Prepare() => DlcManager.IsExpansion1Active();
            public static void Postfix()
            {
                RecipeBuilder.Create(OrbitalResearchCenterConfig.ID, 33)
                    .Input(ModElements.BioPlastic_Solid,5,true)
                    .Output(OrbitalResearchDatabankConfig.TAG,1)
                    .NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
                    .Description(global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.RECIPE_DESC)
                    .Build();

            }
        }
	}
}
