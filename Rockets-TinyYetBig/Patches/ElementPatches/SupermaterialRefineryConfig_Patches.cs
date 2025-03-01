using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;
using UtilLibs;
using Rockets_TinyYetBig.Elements;

namespace Rockets_TinyYetBig.Patches.ElementPatches
{
	internal class SupermaterialRefineryConfig_Patches
	{
		/// <summary>
		/// adding the neutronium alloy recipe to the supermaterial refinery
		/// </summary>
		[HarmonyPatch(typeof(SupermaterialRefineryConfig), nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
		public static class Patch_SupermaterialRefineryConfig_ConfigureRecipes
		{
			public static void Postfix()
			{
				AddAlloyRecipe();
			}
			private static void AddAlloyRecipe()
			{
				RecipeElement[] input = new RecipeElement[]
				{
					new RecipeElement(ModElements.UnobtaniumDust.Tag, 10f),
					new RecipeElement(SimHashes.Steel.CreateTag(), 90f),
				};

				RecipeElement[] output = new RecipeElement[]
				{
					new RecipeElement(ModElements.UnobtaniumAlloy.Tag, 100f)
				};

				string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
				new ComplexRecipe(recipeID, input, output)
				{
					time = 40f,
					description = STRINGS.ELEMENTS.UNOBTANIUMALLOY.RECIPE_DESCRIPTION,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
				}.requiredTech = GameStrings.Technology.ColonyDevelopment.DurableLifeSupport;
			}
		}
	}
}
