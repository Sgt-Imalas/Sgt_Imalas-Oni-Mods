using Cheese.Traits;
using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheese.Foods
{
    internal class FoodPatches
    {
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
        [HarmonyPatch(typeof(WaterCoolerChore.States), "Drink")]
        public static class WaterCoolerChore_Drink_Patch
        {
            public static void Postfix(WaterCoolerChore.States __instance,WaterCoolerChore.StatesInstance smi)
            {
                var worker = __instance.stateTarget.Get<Worker>(smi);
                if (worker.TryGetComponent<Effects>(out var effects) && effects.HasEffect("DuplicantGotMilk"))
                {
                    BracktoseIntolerant.HandleDupeEffect(worker);
                }
            }
        }
    }
}
