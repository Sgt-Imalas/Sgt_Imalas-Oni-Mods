using Database;
using HarmonyLib;
using OniRetroEdition.Behaviors;
using OniRetroEdition.Entities.Foods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
    internal class MicrowaveFoodsPatches
    {
        [HarmonyPatch(typeof(GammaMushConfig), nameof(GammaMushConfig.CreatePrefab))]
        public static class GammaMushConfig_NewAnim
        {
            public static void Postfix(ref GameObject __result)
            {
                //TODO STRING FIX manual patch (breaks strings in the food tooltip)

                __result = EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("GammaMush", global::STRINGS.ITEMS.FOOD.GAMMAMUSH.NAME, global::STRINGS.ITEMS.FOOD.GAMMAMUSH.DESC, 1f, unitMass: false, Assets.GetAnim("gammamush_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true), TUNING.FOOD.FOOD_TYPES.GAMMAMUSH);
                var animHandler = __result.AddComponent<ItemLoopedAnimHandler>();
                animHandler.LoopBaseAnim = true;
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
                        foodRequirement.fromFoodType.Add(SushiConfig.ID);
                        break;
                    }
                }
            }
        }
    }
}
