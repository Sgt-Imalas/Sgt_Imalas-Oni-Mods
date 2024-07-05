
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
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
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


        //[HarmonyPatch(typeof(ComplexFabricatorSideScreen), "Initialize")]
        //public static class AddNOTFractionsToScreen
        //{
        //    public static void Postfix(ComplexFabricatorSideScreen __instance)
        //    {
        //        UIUtils.ListAllChildren(__instance.transform);
        //    }
        //}
        //[HarmonyPatch(typeof(ComplexFabricatorSideScreen), "RefreshQueueCountDisplay")]
        //public static class ManipulateToShowRoutine
        //{
        //    public static void Postfix(ComplexFabricator ___targetFab, GameObject entryGO, ComplexFabricator fabricator, Dictionary<GameObject, ComplexRecipe> ___recipeMap)
        //    {
        //        if(___targetFab.TryGetComponent(out DR_ResetComponent recipes)) 
        //        {
        //            ComplexRecipe recipe = ___recipeMap[entryGO];



        //            if (recipes.StoredRecipes.ContainsKey(recipe)&&recipes.StoredRecipes[recipe] > 0)
        //            {
        //                Debug.Log(recipe.FirstResult.ProperNameStripLink() + "<-target");
        //                HierarchyReferences component = entryGO.GetComponent<HierarchyReferences>();
        //                bool flag = fabricator.GetRecipeQueueCount(___recipeMap[entryGO]) == ComplexFabricator.QUEUE_INFINITE;
        //                component.GetReference<LocText>("CountLabel").text = flag ? "" : fabricator.GetRecipeQueueCount(recipe).ToString() + "\n("+ recipes.StoredRecipes[recipe]+")";
        //            }
        //        }                
        //    }
        //}


        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class AddNewCmpToComplexFabricators
        {
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix()
            {
                foreach(var building in Assets.BuildingDefs)
                {
                    if (building.BuildingComplete.TryGetComponent(out ComplexFabricator fab) ==true)
                    {
                        var addon = building.BuildingComplete.AddOrGet<DR_ResetComponent>();
                        addon.fabricator = fab;
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
