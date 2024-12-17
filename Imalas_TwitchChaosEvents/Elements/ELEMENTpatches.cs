using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static ComplexRecipe;

namespace Imalas_TwitchChaosEvents.Elements
{

	public class ELEMENTpatches
	{



		/// <summary>
		/// akis beached 
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
			{
				// Add my new elements
				var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
				ModElements.RegisterSubstances(list);

				//SgtLogger.l("ElementList length after that method; " + substanceTablesByDlc[DlcManager.VANILLA_ID].GetList().Count);
				//SgtLogger.l("ElementList SO length; " + substanceTablesByDlc[DlcManager.EXPANSION1_ID].GetList().Count);
			}
			public static void Postfix(ElementLoader __instance)
			{
				//SgtLogger.l("ElementList length in postfix; " + ElementLoader.elementTable.Count);
				SgtElementUtil.FixTags();
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

		//public static class  DangerousCreeperPatch
		//{
		//    [HarmonyPatch(typeof(SafeCellQuery), "GetFlags")]
		//    public class SimHashes_ToString_Patch
		//    {
		//        public static void Postfix(int cell, ref SafeCellQuery.SafeFlags __result)
		//        {                  
		//            int headCell = Grid.CellAbove(cell);

		//            if (
		//                (Grid.Element[cell].id == ModElements.Creeper.SimHash || Grid.Element[headCell].id == ModElements.Creeper.SimHash))
		//            {
		//                SgtLogger.l(Grid.Element[cell].tag.ToString(),"DANGER");
		//                SgtLogger.l(Grid.CellToPos(cell).ToString(),"DANGER2");

		//                __result  = (SafeCellQuery.SafeFlags)0;
		//            }
		//        }
		//    }
		//}

		// Credits to Aki
		// because YAML will try to parse from string that wont work, so I assign it manually
		[HarmonyPatch(typeof(ElementLoader), "CopyEntryToElement")]
		public static class ElementLoader_CopyEntryToElement_Patch
		{
			public static void Postfix(Element elem)
			{
				if (elem.id == ModElements.LiquidPoop)
					elem.sublimateFX = Game_InitializeFXSpawners_Patch.ITCE_PoopyLiquidFX;
			}
		}

		[HarmonyPatch(typeof(FoodGerms), "PopulateElemGrowthInfo")]
		public static class FoodGerms_Dwell_On_Poop
		{
			public static void Postfix(FoodGerms __instance)
			{
				ElementGrowthRule poopRule = new ElementGrowthRule(ModElements.LiquidPoop.SimHash);
				poopRule.populationHalfLife = -24000f;
				poopRule.overPopulationHalfLife = 24000f;
				poopRule.maxCountPerKG = 5000f;
				__instance.AddGrowthRule(poopRule);
			}
		}
		[HarmonyPatch(typeof(Game), nameof(Game.InitializeFXSpawners))]
		public static class Game_InitializeFXSpawners_Patch
		{
			public static SpawnFXHashes ITCE_PoopyLiquidFX = (SpawnFXHashes)1024551740;
			public static void Prefix(Game __instance)
			{
				if (__instance.fxSpawnData == null)
				{
					SgtLogger.warning("No spawn data");
					return;
				}
				//SgtLogger.l("Fields of spawners");
				//foreach(var entry in __instance.fxSpawnData)
				//{
				//    UtilMethods.ListAllFieldValues(entry);
				//}

				var spawnData = new List<Game.SpawnPoolData>(__instance.fxSpawnData);
				var prefab = spawnData.Find(d => d.id == SpawnFXHashes.ContaminatedOxygenBubbleWater).fxPrefab;

				if (prefab == null)
				{
					SgtLogger.warning("FX prefab not found.");
					return;
				}

				var pWater = spawnData.FirstOrDefault(d => d.id == SpawnFXHashes.ContaminatedOxygenBubbleWater);
				spawnData.Add(new Game.SpawnPoolData()
				{
					id = ITCE_PoopyLiquidFX,
					initialCount = pWater.initialCount,
					spawnOffset = Vector3.zero,
					spawnRandomOffset = pWater.spawnRandomOffset,
					colour = new Color(128f / 255f, 61f / 255f, 43f / 255f),
					fxPrefab = GetNewPrefab(prefab),
					initialAnim = pWater.initialAnim
				});


				__instance.fxSpawnData = spawnData.ToArray();
			}

			private static GameObject GetNewPrefab(GameObject original, string newAnim = null, float scale = 1f)
			{
				var prefab = UnityEngine.Object.Instantiate(original);
				var kbac = prefab.GetComponent<KBatchedAnimController>();

				if (!newAnim.IsNullOrWhiteSpace())
					kbac.AnimFiles[0] = Assets.GetAnim(newAnim);

				kbac.animScale *= scale;

				return prefab;
			}

		}



		[HarmonyPatch(typeof(WaterCoolerChore.States), "TriggerDrink")]
		public class WaterCoolerChore_States_Drink_Patch
		{
			public static void Prefix(WaterCoolerChore.States __instance, WaterCoolerChore.StatesInstance smi)
			{
				var storage = __instance.masterTarget.Get<Storage>(smi);

				if (storage.IsEmpty())
					return;

				Tag tag = storage.items[0].PrefabID();

				if (ModAssets.WaterCoolerDrinks.Beverages.TryGetValue(tag, out var effect))
					__instance.stateTarget
						.Get<WorkerBase>(smi)?
						.GetComponent<Effects>()?
						.Add(effect, true);
			}
		}
		[HarmonyPatch(typeof(SupermaterialRefineryConfig))]
		[HarmonyPatch(nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
		public static class Patch_CraftingTableConfig_ConfigureRecipes
		{
			public static void Postfix()
			{
				AddRetawRecipe();
			}
			private static void AddRetawRecipe()
			{
				RecipeElement[] input = new RecipeElement[]
				{
					new RecipeElement(ModElements.InverseWater.Tag, 1000f),
				};

				RecipeElement[] output = new RecipeElement[]
				{
					new RecipeElement(SimHashes.Water.CreateTag(), 999f),
					new RecipeElement(SimHashes.Unobtanium.CreateTag(), 1f)
				};

				string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
				new ComplexRecipe(recipeID, input, output)
				{
					time = 30f,
					description = STRINGS.ELEMENTS.ITCE_INVERSE_WATER.RECIPE_DESCRIPTION,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
				};

			}
			private static void AddRetawReverseRecipe()
			{
				RecipeElement[] input = new RecipeElement[]
				{
					new RecipeElement(SimHashes.Water.CreateTag(), 499.5f),
					new RecipeElement(ModElements.InverseWater.Tag, 499.5f),
				};

				RecipeElement[] output = new RecipeElement[]
				{
					new RecipeElement(ModElements.InverseWater.Tag, 1000f),
				};

				string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
				new ComplexRecipe(recipeID, input, output)
				{
					time = 30f,
					description = STRINGS.ELEMENTS.ITCE_INVERSE_WATER.RECIPE_DESCRIPTION_CREATE,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
				};

			}
		}
	}
}
