using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Meteors;
using Klei.AI;
using ONITwitchLib;
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
    internal class TacoMeteorPatches
    {
        public const string ITC_TacoMeteorsID = "ITC_TacoMeteorShowerEvent";
        public const string ITC_FakeTacoMeteorsID = "ITC_FakeTacoMeteorShowerEvent";
        public static GameplayEvent ITC_TacoMeteors;
        public static GameplayEvent ITC_FakeTacoMeteors;
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

            ITC_FakeTacoMeteors = gameplayEvents.Add(new MeteorShowerEvent(
                ITC_FakeTacoMeteorsID,
                Config.Instance.FakeTacoEventDuration,
                0.12f,
                METEORS.BOMBARDMENT_OFF.NONE,
                METEORS.BOMBARDMENT_ON.UNLIMITED,
                null,
                false)
                .AddMeteor(GhostlyTacoCometConfig.ID, 0.15f));

        }
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                Register(__instance.GameplayEvents);
            }
        }

        [HarmonyPatch(typeof(PlayerController), "OnKeyDown")]
        public class PlayerController_OnKeyDown_Patch
        {
            public static void Prefix(KButtonEvent e)
            {
                if (ClusterManager.Instance == null)
                    return;

                if (e.TryConsume(ModAssets.HotKeys.UnlockTacoRecipe.GetKAction()))
                {
                    ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe = true;
                    ToastManager.InstantiateToast(STRINGS.HOTKEYACTIONS.UNLOCK_TACO_RECIPE_TITLE, STRINGS.HOTKEYACTIONS.UNLOCK_TACO_RECIPE_BODY);
                }
                else if ( e.TryConsume(ModAssets.HotKeys.TriggerTacoRain.GetKAction()))
                {
                    TriggerGhostTacoMeteors();
                }
            }
        }

        static void TriggerGhostTacoMeteors()
        {
            int activeWorld = ClusterManager.Instance.activeWorldId;
            if (ClusterManager.Instance.activeWorld.IsModuleInterior)
            {
                activeWorld = 0;
            }
            GameplayEventInstance eventInstance = GameplayEventManager.Instance.StartNewEvent(TacoMeteorPatches.ITC_FakeTacoMeteors, activeWorld);
            if (Config.Instance.FakeTacoEventMusic)
                SoundUtils.PlaySound(ModAssets.SOUNDS.TACORAIN, SoundUtils.GetSFXVolume() * 0.3f, true);
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
