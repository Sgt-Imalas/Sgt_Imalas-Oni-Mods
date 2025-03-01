using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;

namespace Cheese.ModElements
{

	public class ElementPatches
	{
		/// <summary>
		/// Credit: akis beached 
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
			{
				var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
				ModElementRegistration.RegisterSubstances(list);
			}
		}

		[HarmonyPatch(typeof(SaveGame), nameof(SaveGame.OnPrefabInit))]
		public class SaveGame_OnPrefabInit_Patch
		{
			public static void Postfix(SaveGame __instance)
			{
				__instance.gameObject.AddOrGet<RandomTickManager>();
			}
		}

		/// <summary>
		/// Credit: akis extra twitch events 
		/// </summary>
		public class CodexEntryGeneratorPatch
		{
			public static HashSet<string> edibleElement = new HashSet<string>()
			{
				"Cheese"
			};

			[HarmonyPatch(typeof(CodexEntryGenerator), "GenerateFoodEntries")]
			public class CodexEntryGenerator_GenerateFoodEntries_Patch
			{
				public static void Postfix(ref Dictionary<string, CodexEntry> __result)
				{
					foreach (var element in edibleElement)
						__result.Remove(element);
				}
			}

			[HarmonyPatch(typeof(CodexCache), "AddEntry")]
			public class CodexCache_AddEntry_Patch
			{
				public static bool Prefix(string id, CodexEntry entry)
				{
					// skip adding to foods it would be a duplicate entry with element)
					return entry.parentId != CodexEntryGenerator.FOOD_CATEGORY_ID || !edibleElement.Contains(id);
				}
			}
		}

		[HarmonyPatch(typeof(ElementsAudio), "LoadData")]
		public class ElementsAudio_LoadData_Patch
		{
			public static void Postfix(ElementsAudio __instance, ref ElementsAudio.ElementAudioConfig[] ___elementAudioConfigs)
			{
				___elementAudioConfigs = ___elementAudioConfigs.AddRangeToArray(ModElementRegistration.CreateAudioConfigs(__instance));
			}
		}
		// Credit: Heinermann (Blood mod)
		public static class EnumPatch
		{
			[HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
			public class SimHashes_ToString_Patch
			{
				public static bool Prefix(ref Enum __instance, ref string __result)
				{
					if (__instance is SimHashes hashes)
					{
						return !SgtElementUtil.SimHashNameLookup.TryGetValue(hashes, out __result);
					}

					return true;
				}
			}
		}

		[HarmonyPatch(typeof(FoodGerms), "PopulateElemGrowthInfo")]
		public static class FoodGerms_Dwell_On_Poop
		{
			public static void Postfix(FoodGerms __instance)
			{
				//ElementGrowthRule poopRule = new ElementGrowthRule(ModElements.LiquidPoop.SimHash);
				//poopRule.populationHalfLife = -24000f;
				//poopRule.overPopulationHalfLife = 24000f;
				//poopRule.maxCountPerKG = 5000f;
				//__instance.AddGrowthRule(poopRule);
			}
		}

		[HarmonyPatch(typeof(SupermaterialRefineryConfig))]
		[HarmonyPatch(nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
		public static class Patch_SupermaterialRefineryConfig_ConfigureRecipes
		{
			//public static void Postfix()
			//{
			//    AddCheeseBacteriaRecipe();
			//}
			//private static void AddCheeseBacteriaRecipe()
			//{
			//    RecipeElement[] input = new RecipeElement[]
			//    {
			//        new RecipeElement(SimHashes.SlimeMold.CreateTag(), 10f),
			//    };

			//    RecipeElement[] output = new RecipeElement[]
			//    {
			//        new RecipeElement(SimHashes.Water.CreateTag(), 999f),
			//        new RecipeElement(SimHashes.Unobtanium.CreateTag(), 1f)
			//    };

			//    string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
			//    new ComplexRecipe(recipeID, input, output)
			//    {
			//        time = 30f,
			//        description = STRINGS.ELEMENTS.ITCE_INVERSE_WATER.RECIPE_DESCRIPTION,
			//        nameDisplay = RecipeNameDisplay.Result,
			//        fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
			//    };

			//}
		}
	}
}
