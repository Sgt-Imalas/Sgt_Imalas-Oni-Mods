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
using static CompressedCritters.ModAssets;

namespace CompressedCritters
{
    internal class Patches
    {
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
        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Postfix()
            {

            }
        }
        [HarmonyPatch(typeof(OvercrowdingMonitor), "Initialize")]
        public static class OvercrowdingMonitor__Patch
        {
            public static void Postfix()
            {

            }
        }
    }
}
