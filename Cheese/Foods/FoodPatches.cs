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
        [HarmonyPatch(typeof(WaterCoolerChore.States), "Drink")]
        public static class WaterCoolerChore_Drink_Patch
        {
            public static void Postfix(WaterCoolerChore.States __instance, WaterCoolerChore.StatesInstance smi)
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
