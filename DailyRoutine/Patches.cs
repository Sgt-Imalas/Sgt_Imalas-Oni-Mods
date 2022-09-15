
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
using static DailyRoutine.ModAssets;

namespace DailyRoutine
{
    class Patches
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

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        [HarmonyPriority(Priority.LowerThanNormal)]
        public static class AddNewCmpToComplexFabricators
        {
            public static void Postfix()
            {
                foreach(var building in Assets.BuildingDefs)
                {
                    if (building.BuildingComplete.GetComponent<ComplexFabricator>() != null)
                    {
                        var addon = building.BuildingComplete.AddOrGet<DR_ResetComponent>();
                        addon.fabricator = building.BuildingComplete.GetComponent<ComplexFabricator>();
                        //Debug.Log("Added Reset Cmp to " + building.Name);
                    }
                }

            }
        }


        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_DailyReset
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                UIUtils.AddClonedSideScreen<DailyResetSidescreen>("DailyResetSidescreen", "Timer SideScreen", typeof(TimerSideScreen));
            }
        }
    }
}
