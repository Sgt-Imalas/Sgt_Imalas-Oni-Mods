using Cryopod.Buildings;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Cryopod
{
    class Patches
    {
        //needs Changes
        //[HarmonyPatch(typeof(WorldContainer))]
        //[HarmonyPatch(nameof(WorldContainer.SpacePodAllDupes))]
        //public static class ThrowOutFrozenDupesInsideRocket_ToWorld_Patch
        //{
        //    public static void Prefix(Vector3 spawn_pos, WorldContainer __instance)
        //    {
        //        foreach (var worldItem in ModAssets.CryoPods.GetWorldItems(__instance.id))
        //        {
        //            Debug.Log("PATCH !" + worldItem + spawn_pos);
        //            worldItem.ThrowOutDupe(true, spawn_pos);
        //        }
        //    }
        //}

        /// <summary>
        /// Sickness should be Stored
        /// </summary>
        
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, BuildableCryopodConfig.ID);
            }
        }

        
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        [HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
        public static class Database_BuildingStatusItems_CreateStatusItems_Patch
        {
            public static void Postfix()
            {
                ModAssets.StatusItems.Register();
            }
        }
    }
}
