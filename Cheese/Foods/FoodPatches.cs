using Cheese.ModElements;
using Cheese.Traits;
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
using static ComplexRecipe;

namespace Cheese.Foods
{
    internal class FoodPatches
    {
        [HarmonyPatch(typeof(MicrobeMusherConfig), nameof(MicrobeMusherConfig.ConfigureRecipes))]
        public static class MicrobeMusherConfig_ConfigureRecipes
        {
            public static void Postfix()
            {
                AddCheeseRecipe();
            }
            private static void AddCheeseRecipe()
            {
                var fabricatorID = MicrobeMusherConfig.ID;

                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.SlimeMold.CreateTag(), 0.5f),
                    new RecipeElement(SimHashes.Milk.CreateTag(),100)
                };
                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(ModElementRegistration.Cheese.SimHash.CreateTag(), 25f,ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(fabricatorID, input, output);

                ModAssets.Foods.CheeseRecipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = TUNING.FOOD.RECIPES.SMALL_COOK_TIME,
                    description = STRINGS.ITEMS.FOOD.CHEESE.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { fabricatorID },
                    sortOrder = 5
                };
                ModAssets.Foods.CheeseRecipe.FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(CheeseDebris.GetPrefabForRecipe());
            }
        }
        [HarmonyPatch(typeof(GourmetCookingStationConfig), nameof(GourmetCookingStationConfig.ConfigureRecipes))]
        public static class GourmetCookingStationConfig_ConfigureRecipes
        {
            public static void Postfix()
            {
                //AddGrilledCheeseRecipe();
                //AddCheeseBurgerRecipe();
            }
            private static void AddGrilledCheeseRecipe()
            {
                var fabricatorID = MicrobeMusherConfig.ID;

                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.SlimeMold.CreateTag(), 0.5f),
                    new RecipeElement(SimHashes.Milk.CreateTag(),100)
                };
                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(ModElementRegistration.Cheese.SimHash.CreateTag(), 25f,ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(fabricatorID, input, output);

                ModAssets.Foods.CheeseRecipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = TUNING.FOOD.RECIPES.SMALL_COOK_TIME,
                    description = STRINGS.ITEMS.FOOD.CHEESE.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { fabricatorID },
                    sortOrder = 5
                };
            }
        }


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
                        //TODO add cheeseburger

                        //foodRequirement.fromFoodType.Add(CannedBBQConfig.ID); 
                        //foodRequirement.fromFoodType.Add(CannedTunaConfig.ID);
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(EntityTemplates), "ExtendEntityToFood", new Type[] { typeof(GameObject), typeof(EdiblesManager.FoodInfo), typeof(bool) })]
        public static class ExtendEntityToFood_AddEncrustable
        {
            public static void Postfix(ref GameObject __result)
            {
                if (__result)
                    __result.AddOrGet<CheeseEncrustable>();
            }
        }

        [HarmonyPatch(typeof(Edible), nameof(Edible.CanAbsorb))]
        public static class Edible_CheesedMerging
        {
            public static void Postfix(ref bool __result, Edible __instance, Edible other)
            {
                if (!__result)
                    return;

                if (__instance.TryGetComponent<CheeseEncrustable>(out var own)
                    && other.TryGetComponent<CheeseEncrustable>(out var target))
                {
                    __result = (own.CheeseEncrusted == target.CheeseEncrusted);
                }

            }
        }

        /// <summary>
        /// Bractose Intolerance food
        /// </summary>
        [HarmonyPatch(typeof(Edible), "OnStopWork")]
        public static class Edible_OnStopWork_Patch
        {
            public static void Prefix(Edible __instance)
            {
                if (__instance.HasTag(ModAssets.Tags.BrackeneProduct))
                {
                    BracktoseIntolerant.HandleDupeEffect(__instance.worker);
                }
            }
        }
        /// <summary>
        /// Bractose Intolerance watercooler
        /// </summary>
        [HarmonyPatch(typeof(WaterCoolerChore.States), "TriggerDrink")]
        public static class WaterCoolerChore_Drink_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(WaterCoolerChore.States __instance, WaterCoolerChore.StatesInstance smi)
            {
                var worker = __instance.drinker.Get<Worker>(smi);
                if (worker.TryGetComponent<Effects>(out var effects) && effects.HasEffect("DuplicantGotMilk"))
                {
                    BracktoseIntolerant.HandleDupeEffect(worker);
                }
            }
        }
    }
}
