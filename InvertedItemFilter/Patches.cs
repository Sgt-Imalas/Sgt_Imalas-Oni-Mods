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
using static InvertedItemFilter.ModAssets;
using static STRINGS.BUILDINGS.PREFABS;

namespace InvertedItemFilter
{
    internal class Patches
    {
        [HarmonyPatch(typeof(PermitResource), "IsUnlocked")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }
        [HarmonyPatch(typeof(ThermalBlockConfig), "CreateBuildingDef")]
        public static class FasterTempShifts
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ConstructionTime = 10f;
            }
        }
    }
}
