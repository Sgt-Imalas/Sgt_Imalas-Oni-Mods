using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class ComplexRecipeManager_Patches
	{


		[HarmonyPatch(typeof(ComplexRecipeManager), nameof(ComplexRecipeManager.DeriveRecipiesFromSource))]
		public class ComplexRecipeManager_DeriveRecipiesFromSource_Patch
		{
			public static void Postfix(ComplexRecipe sourceRecipe, List<ComplexRecipe>__result)
			{
				RecipeCondenser.CollectDerivedRecipes(sourceRecipe, __result);
			}
		}
	}
}
