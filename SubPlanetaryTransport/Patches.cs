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
using static SubPlanetaryTransport.ModAssets;

namespace SubPlanetaryTransport
{
    internal class Patches
    {

        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public class PlayerController_OnPrefabInit_Patch
        {
            public static void Postfix(PlayerController __instance)
            {
                var go = new GameObject(nameof(TargetGunTool));
                var tool = go.AddComponent<TargetGunTool>();

                tool.transform.SetParent(__instance.gameObject.transform);
                tool.gameObject.SetActive(true);
                tool.gameObject.SetActive(false);

                __instance.tools = __instance.tools.AddToArray(tool);
            }
        }
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, SubPlanetaryItemRailgunConfig.ID);
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
    }
}
