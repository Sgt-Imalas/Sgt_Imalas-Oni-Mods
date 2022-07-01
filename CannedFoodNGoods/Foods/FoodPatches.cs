using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
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
                AddCannedTunaRecipe();
                AddCannedBBQRecipe();
            }
            private static void AddCannedTunaRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Copper.CreateTag(), 0.5f),
                    new RecipeElement(CookedFishConfig.ID, 0.5f)
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(CannedTunaConfig.ID, 1f)
                };

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
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Copper.CreateTag(), 0.5f),
                    new RecipeElement(CookedMeatConfig.ID, 0.5f)
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(CannedBBQConfig.ID, 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

                CannedBBQConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = FOOD.RECIPES.SMALL_COOK_TIME,
                    description = STRINGS.ITEMS.FOOD.CF_CANNEDBBQ.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { CraftingTableConfig.ID }
                };
            }
        }

        /// <summary>
        /// Carnivore Achievment: add canned meat
        /// </summary>
        [HarmonyPatch(typeof(EatXCaloriesFromY), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(int), typeof(List<string>) })]
        public static class PatchCarnivoreAchievment
        {
            public static void Postfix(List<string> fromFoodType)
            {
                if (!fromFoodType.Contains(CannedBBQConfig.ID))
                {
                    fromFoodType.Add(CannedBBQConfig.ID);
                    fromFoodType.Add(CannedTunaConfig.ID);
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
                var copperTag = SimHashes.Copper.CreateTag();
                var input = new RecipeElement[]
                {
                    new RecipeElement(TagManager.Create(CanScrapConfig.ID), 10f)
                };

                var output = new RecipeElement[]
                {
                    new RecipeElement(copperTag, 5f)
                };

                var recipeID = ComplexRecipeManager.MakeRecipeID(RockCrusherConfig.ID, input, output);

                ComplexRecipe complexRecipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = 10f,
                    description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, CanScrapConfig.NAME, (object)copperTag.ProperName()),
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
            }
            private static void AddRecyclingRecipeMetalRefinery()
            {
                var copperTag = SimHashes.Copper.CreateTag();
                var input = new RecipeElement[]
                {
                    new RecipeElement(TagManager.Create(CanScrapConfig.ID), 10f)
                };

                var output = new RecipeElement[]
                {
                    new RecipeElement(copperTag, 10f)
                };

                var recipeID = ComplexRecipeManager.MakeRecipeID(MetalRefineryConfig.ID, input, output);

                ComplexRecipe complexRecipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = 10f,
                    description = string.Format(global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, (object)copperTag.ProperName(),CanScrapConfig.NAME),
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
            public static void Prefix(Edible __instance )
            {
                if (__instance.FoodInfo.Id==CannedBBQConfig.ID || __instance.FoodInfo.Id == CannedTunaConfig.ID)
                {
                    float trashMass = 0.5f * __instance.unitsConsumed;
                    DropCan(__instance, trashMass);
                }
            }
            public static void DropCan(Edible gameObject, float mass)
            {
                var element = ElementLoader.FindElementByHash(SimHashes.Copper);
                var temperature = gameObject.GetComponent<PrimaryElement>().Temperature;

                var scrapObject = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"CF_CanScrap"), gameObject.transform.position, Grid.SceneLayer.Ore);
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
