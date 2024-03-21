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
using static WeebDupe.ModAssets;

namespace WeebDupe
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
            }
        }
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
       

        static class AccessoryPatch
        {
            [HarmonyPatch(typeof(Db), "Initialize")]
            public class Db_Initialize_Patch
            {
                public static void Postfix(Db __instance)
                {
                    WSkillPerks.Register(__instance.SkillPerks);
                    WSkills.Register(__instance.Skills);
                    WAccessories.Register(__instance.AccessorySlots, __instance.Accessories);
                }

            }
        }
    }
}
