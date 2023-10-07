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
using static NeutroniumTrashCan.ModAssets;

namespace NeutroniumTrashCan
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
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, NeutroniumTrashCanConfig.ID);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, SolidTrashCanConfig.ID);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, LiquidTrashCanConfig.ID);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, GasTrashCanConfig.ID);
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
    }
}
