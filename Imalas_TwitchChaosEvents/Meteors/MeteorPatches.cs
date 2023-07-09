using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Meteors;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using TUNING;
using UnityEngine;
using UtilLibs;
using static ComplexRecipe;

namespace Imalas_TwitchChaosEvents
{
    internal class MeteorPatches
    {
        public const string ITC_TacoMeteorsID = "ITC_TacoMeteorShowerEvent";
        public static GameplayEvent ITC_TacoMeteors;
        public static void Register(GameplayEvents gameplayEvents)
        {
            ITC_TacoMeteors = gameplayEvents.Add(new MeteorShowerEvent(
                ITC_TacoMeteorsID,
                35f,
                0.12f,
                METEORS.BOMBARDMENT_OFF.NONE,
                METEORS.BOMBARDMENT_ON.UNLIMITED,
                null,
                false)
                .AddMeteor(TacoCometConfig.ID, 0.15f));

        }
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                Register(__instance.GameplayEvents);
            }
        }


        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        public static class PatchCarnivoreAchievment
        {
            //public static void Postfix(List<string> fromFoodType)
            //{
            //    if (!fromFoodType.Contains(CannedBBQConfig.ID))
            //    {
            //        fromFoodType.Add(CannedBBQConfig.ID);
            //        fromFoodType.Add(CannedTunaConfig.ID);
            //    }
            //}
            public static void Postfix(Db __instance)
            {
                var items = __instance.ColonyAchievements.EatkCalFromMeatByCycle100.requirementChecklist;
                foreach (var requirement in items)
                {
                    if (requirement is EatXCaloriesFromY foodRequirement)
                    {
                        foodRequirement.fromFoodType.Add(TacoConfig.ID);
                        break;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureRecipes")]
        public static class AddGasRangeRecipes
        {
            public static void Postfix()
            {
                AddTacoRecipe();
            }
            private static void AddTacoRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement((Tag) "ColdWheatSeed", 4f),
                    new RecipeElement( SimHashes.Salt.CreateTag(), 0.2f), 
                    new RecipeElement((Tag) "Lettuce", 1f),
                    new RecipeElement((Tag) "CookedMeat", 1f),
                    new RecipeElement((Tag) SpiceNutConfig.ID, 1f)
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(TacoConfig.ID, 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(GourmetCookingStationConfig.ID, input, output);

                TacoConfig.recipe = new ComplexRecipe(recipeID, input, output)
                {
                    time = FOOD.RECIPES.STANDARD_COOK_TIME,
                    description = STRINGS.ITEMS.FOOD.ICT_TACO.DESC,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { GourmetCookingStationConfig.ID },
                    sortOrder = 900
                };
            }
        }
    }
}
