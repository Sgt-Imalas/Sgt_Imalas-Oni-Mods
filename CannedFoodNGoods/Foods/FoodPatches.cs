using CannedFoods.EmptyCans;
using Database;
using HarmonyLib;
using System.Collections.Generic;
using TUNING;
using static CannedFoods.ModAssets;
using static ComplexRecipe;

namespace CannedFoods.Foods
{

	internal class FoodPatches
	{
		[HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
		public static class Patch_CraftingTableConfig_ConfigureRecipes
		{
			public static void Postfix()
			{
				//AddCanRecipes();
				AddCannedTunaRecipe();
				AddCannedBBQRecipe();
				AddCannedBreadRecipe();
			}

			private static void AddCanRecipes()
			{
				var metalTag = ElementLoader.FindElementByHash(ModAssets.ExportSettings.GetMaterialHashForCans()).tag;
				RecipeElement[] input =
				[
						new RecipeElement(metalTag, 0.5f),
				];

				RecipeElement[] output =
				[
						new RecipeElement(EmptyCanConfig.ID, 0.5f)
				];

				string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				CannedTunaConfig.recipe = new ComplexRecipe(recipeID, input, output)
				{
					time = 15,
					description = STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_EMPTYCAN.DESC,
					nameDisplay = RecipeNameDisplay.IngredientToResult,
					fabricators = new List<Tag> { CraftingTableConfig.ID }
				};

			}


			private static void AddCannedTunaRecipe()
			{
				var metalTag = ElementLoader.FindElementByHash(ModAssets.ExportSettings.GetMaterialHashForCans()).tag;
				RecipeElement[] input =
				[
					new RecipeElement(metalTag, 0.5f),
					new RecipeElement(CookedFishConfig.ID, 0.5f)
				];

				RecipeElement[] output =
				[
					new RecipeElement(CannedTunaConfig.ID, 1f)
				];

				string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				CannedTunaConfig.recipe = new ComplexRecipe(recipeID, input, output)
				{
					time = FOOD.RECIPES.SMALL_COOK_TIME,
					description = STRINGS.ITEMS.FOOD.CF_CANNEDTUNA.DESC,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { CraftingTableConfig.ID }
				};
			}
			private static void AddCannedBBQRecipe()
			{
				var metalTag = ElementLoader.FindElementByHash(ModAssets.ExportSettings.GetMaterialHashForCans()).tag;
				RecipeElement[] input =
				[
					new RecipeElement(metalTag, 0.5f),
					new RecipeElement(CookedMeatConfig.ID, 0.5f)
				];

				RecipeElement[] output =
				[
					new RecipeElement(CannedBBQConfig.ID, 1f)
				];

				string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				CannedBBQConfig.recipe = new ComplexRecipe(recipeID, input, output)
				{
					time = FOOD.RECIPES.SMALL_COOK_TIME,
					description = STRINGS.ITEMS.FOOD.CF_CANNEDBBQ.DESC,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { CraftingTableConfig.ID }
				};
			}
			private static void AddCannedBreadRecipe()
			{
				var metalTag = ElementLoader.FindElementByHash(ModAssets.ExportSettings.GetMaterialHashForCans()).tag;
				RecipeElement[] input =
				[
					new RecipeElement(metalTag, 0.5f),
					new RecipeElement(SpiceBreadConfig.ID, 0.5f)
				];

				RecipeElement[] output =
				[
					new RecipeElement(CannedBreadConfig.ID, 1f)
				];

				string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				CannedBBQConfig.recipe = new ComplexRecipe(recipeID, input, output)
				{
					time = FOOD.RECIPES.SMALL_COOK_TIME,
					description = STRINGS.ITEMS.FOOD.CF_CANNEDBREAD.DESC,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { CraftingTableConfig.ID }
				};
			}
		}

		/// <summary>
		/// Carnivore Achievment: add canned meat
		/// </summary>
		//[HarmonyPatch(typeof(EatXCaloriesFromY), MethodType.Constructor)]
		//[HarmonyPatch(new Type[] { typeof(int), typeof(List<string>) })]

		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch(nameof(Db.Initialize))]
		public static class PatchCarnivoreAchievment
		{
			public static void Postfix(Db __instance)
			{
				var items = __instance.ColonyAchievements.EatkCalFromMeatByCycle100.requirementChecklist;
				foreach (var requirement in items)
				{
					if (requirement is EatXCaloriesFromY foodRequirement)
					{
						foodRequirement.fromFoodType.Add(CannedBBQConfig.ID);
						foodRequirement.fromFoodType.Add(CannedTunaConfig.ID);
						break;
					}
				}
			}
		}


		[HarmonyPatch(typeof(RockCrusherConfig), "ConfigureBuildingTemplate")]
		public static class PatchRecyclingRockCrusher
		{
			public static void Postfix()
			{
				AddRecyclingRecipeRockCrusher();
			}
			private static void AddRecyclingRecipeRockCrusher()
			{
				Tag sandTag = SimHashes.Sand.CreateTag();
				var metalTag = ElementLoader.FindElementByHash(ExportSettings.GetMaterialHashForCans()).tag;
				var input = new RecipeElement[]
				{
					new(TagManager.Create(CanScrapConfig.ID), 10f)
				};

				var output = new RecipeElement[]
				{
					new(metalTag, 5f),
					new(sandTag, 5f, RecipeElement.TemperatureOperation.AverageTemperature)
				};

				var recipeID = ComplexRecipeManager.MakeRecipeID(RockCrusherConfig.ID, input, output);

				ComplexRecipe complexRecipe = new ComplexRecipe(recipeID, input, output)
				{
					time = 10f,
					description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_CANSCRAP.NAME, metalTag.ProperName()),
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient,
					fabricators = new List<Tag>()
					{
						TagManager.Create("RockCrusher")
					}
				};
			}
		}


		[HarmonyPatch(typeof(MetalRefineryConfig), "ConfigureBuildingTemplate")]
		public static class PatchRecyclingMetalRefinery
		{
			public static void Postfix()
			{
				AddRecyclingRecipeMetalRefinery();
				AddRecyclingRecipeMetalRefineryEmptyCan();
			}
			private static void AddRecyclingRecipeMetalRefinery()
			{
				var metalTag = ElementLoader.FindElementByHash(Config.Instance.GetCanElement()).tag;
				var input = new RecipeElement[]
				{
					new RecipeElement(TagManager.Create(CanScrapConfig.ID), 10f)
				};

				var output = new RecipeElement[]
				{
					new RecipeElement(metalTag, 10f)
				};

				var recipeID = ComplexRecipeManager.MakeRecipeID(MetalRefineryConfig.ID, input, output);

				ComplexRecipe complexRecipe = new ComplexRecipe(recipeID, input, output)
				{
					time = 10f,
					description = string.Format(global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, metalTag.ProperName(), STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_CANSCRAP.NAME),
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient,
					fabricators = new List<Tag>()
					{
						TagManager.Create("MetalRefinery")
					}
				};
			}
			private static void AddRecyclingRecipeMetalRefineryEmptyCan()
			{
				var metalTag = ElementLoader.FindElementByHash(Config.Instance.GetCanElement()).tag;
				var input = new RecipeElement[]
				{
						new RecipeElement(TagManager.Create(EmptyCanConfig.ID), 10f)
				};

				var output = new RecipeElement[]
				{
						new RecipeElement(metalTag, 10f)
				};

				var recipeID = ComplexRecipeManager.MakeRecipeID(MetalRefineryConfig.ID, input, output);

				ComplexRecipe complexRecipe = new ComplexRecipe(recipeID, input, output)
				{
					time = 10f,
					description = string.Format(global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, metalTag.ProperName(), STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_EMPTYCAN.NAME),
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient,
					fabricators = new List<Tag>()
						{
							TagManager.Create("MetalRefinery")
						}
				};

			}
		}

		/// <summary>
		/// Drops Can at the end of eating.
		/// </summary>
		[HarmonyPatch(typeof(Edible), "OnStopWork")]
		public static class PatchDroppingOfTincans
		{
			public static void Prefix(Edible __instance)
			{
				if (__instance.HasTag(ModAssets.Tags.DropCanOnEat)
					|| __instance.FoodInfo.Id == CannedBBQConfig.ID || __instance.FoodInfo.Id == CannedTunaConfig.ID //compatiblitiy
					)
				{
					float trashMass = 0.5f * __instance.unitsConsumed;
					DropCan(__instance, trashMass);
				}
			}
			public static void DropCan(Edible gameObject, float mass)
			{
				var element = gameObject.GetComponent<PrimaryElement>();
				var temperature = element.Temperature;

				var scrapObject = GameUtil.KInstantiate(Assets.GetPrefab(EmptyCanConfig.ID), gameObject.transform.position, Grid.SceneLayer.Ore);
				scrapObject.SetActive(true);
				var scrapObjectElement = scrapObject.GetComponent<PrimaryElement>();
				scrapObjectElement.Mass = mass;
				scrapObjectElement.Temperature = temperature;
				//Debug.Log(scrapObjectElement.ElementID);
				// var pos = Grid.CellToPosCCC(Grid.PosToCell(gameObject.transform.GetPosition()), Grid.SceneLayer.Ore);
				//element.substance.SpawnResource(pos, mass, temperature, 0, 0);
			}
		}
	}
}
