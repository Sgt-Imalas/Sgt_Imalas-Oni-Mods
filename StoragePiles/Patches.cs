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
using static StoragePiles.ModAssets;

namespace StoragePiles
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
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, WoodStorageConfig.ID,StorageLockerConfig.ID,ordering:ModUtil.BuildingOrdering.Before);
            }
        }
        [HarmonyPatch(typeof(WoodStorageConfig))]
        [HarmonyPatch(nameof(WoodStorageConfig.CreateBuildingDef))]
        public static class WoodStorageConfig_CreateBuildingDef
        {

            public static void Postfix(BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
            }
        }
    }
}
