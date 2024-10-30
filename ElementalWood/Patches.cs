using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ElementLoader;

namespace ElementalWood
{
	internal class Patches
	{
		/// <summary>
		/// change wood transition target to coal
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.CollectElementsFromYAML))]
		public static class Patch_ElementLoader_CollectElementsFromYAML
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix(List<ElementEntry> __result)
			{
				var wood = __result.FirstOrDefault(ele => ele.elementId == nameof(SimHashes.WoodLog));
				if (wood != null)
				{
					wood.highTempTransitionTarget = nameof(SimHashes.Carbon);
					wood.highTemp = UtilMethods.GetKelvinFromC(200);
					SgtLogger.l("Changed wood transition target to " + wood.highTempTransitionTarget);
				}
			}
		}

		/// <summary>
		/// add kiln recipe for coal
		/// </summary>
		[HarmonyPatch(typeof(KilnConfig))]
		[HarmonyPatch(nameof(KilnConfig.ConfigureRecipes))]
		public static class Patch_KilnConfig_ConfigureRecipes
		{
			public static void Postfix()
			{
				Tag coalTag = SimHashes.Carbon.CreateTag();
				ComplexRecipe.RecipeElement[] ingredients =
				[
					new ComplexRecipe.RecipeElement(SimHashes.WoodLog.CreateTag(), 100f)
				];
				ComplexRecipe.RecipeElement[] results =
				[
					new ComplexRecipe.RecipeElement(coalTag, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				];
				string obsolete_id3 = ComplexRecipeManager.MakeObsoleteRecipeID(KilnConfig.ID, coalTag);
				string recipeID = ComplexRecipeManager.MakeRecipeID(KilnConfig.ID, ingredients, results);
				new ComplexRecipe(recipeID, ingredients, results)
				{
					time = 40f,
					description = string.Format(STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.WoodLog).name, ElementLoader.FindElementByHash(SimHashes.Carbon).name),
					fabricators = new List<Tag> { TagManager.Create(KilnConfig.ID) },
					nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
					sortOrder = 299
				};
				ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id3, recipeID);
			}
		}
	}
}