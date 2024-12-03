using HarmonyLib;
using RoboRockets.LearningBrain;
using System.Collections.Generic;
using UtilLibs;
using static ComplexRecipe;

namespace RoboRockets.Patches
{
	internal class AddBrainRecipe
	{
		[HarmonyPatch(typeof(SupermaterialRefineryConfig), "ConfigureBuildingTemplate")]
		public static class Patch_CraftingTableConfig_ConfigureRecipes
		{
			public static void Postfix()
			{
				AddBrainRecipe();
			}
			static bool added = false;
			private static void AddBrainRecipe()
			{
				if (added)
				{
					SgtLogger.l("tried adding brain recipe twice");
					return;
				}

				added = true;

				SgtLogger.l("adding brain recipe");

				RecipeElement[] input = BrainConfig.ProductionCosts;

				RecipeElement[] output = new RecipeElement[]
				{
					new RecipeElement(BrainConfig.ID, 1f)
				};

				string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);

				BrainConfig.recipe = new ComplexRecipe(recipeID, input, output)
				{
					time = 120f,
					description = STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.DESC,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
				};
				BrainConfig.recipe.requiredTech = ModAssets.Techs.AiBrainsTech;
			}
		}

		[HarmonyPatch(typeof(RocketModuleCluster), nameof(RocketModuleCluster.UpdateAnimations))]
		public static class SkipAnimSetter
		{
			public static bool Prefix(RocketModuleCluster __instance)
			{
				if (__instance.TryGetComponent<BrainTeacher>(out var brainTeacher))
				{
					return !brainTeacher.PreventAnimChanges;
				}
				return true;
			}

		}

	}
}
