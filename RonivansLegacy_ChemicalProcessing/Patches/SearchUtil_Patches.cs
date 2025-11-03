using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDING.STATUSITEMS;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SearchUtil_Patches
	{

        [HarmonyPatch(typeof(SearchUtil), nameof(SearchUtil.CacheTechs))]
        public class SearchUtil_CacheTechs_Patch
        {
            public static void Prefix()
            {
				foreach (ComplexRecipe recipe in ComplexRecipeManager.Get().recipes)
				{
					if(recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Custom && recipe.customName.IsNullOrWhiteSpace())
						recipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
				}
			}
        }
	}
}
