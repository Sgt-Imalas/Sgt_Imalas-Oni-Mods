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

namespace SpaceMapExpansionTest
{
    class Patches
    {
        [HarmonyPatch(typeof(ClusterGrid), "GenerateGrid")]
        public static class TestPatchForSize
        {
            public static void Prefix(ref int rings)
            {
                //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"); //Working :D

                Debug.Log("Space Expansion patched"+ rings.ToString());
                rings = rings * 2;
            }
            
        }

        [HarmonyPatch(typeof(ClusterGrid), "GenerateGrid")]
        public static class DoubleAsteroidsTest
        {
            public static void Prefix(ref int rings)
            {
                //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"); //Working :D

                //Debug.Log("Space Expansion patched" + rings.ToString());
               // rings = rings * 2;
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
    }
}
