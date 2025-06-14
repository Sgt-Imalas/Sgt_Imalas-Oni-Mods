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
using static BritishWaterBottle.ModAssets;

namespace BritishWaterBottle
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(EntityTemplates))]
        [HarmonyPatch(nameof(EntityTemplates.CreateOreEntity))]
        public static class EntityTemplates_CreateOreEntity
        {
            static SimHashes[] Woters = [SimHashes.Water,SimHashes.DirtyWater,SimHashes.SaltWater];

            public static void Postfix(ref GameObject __result, SimHashes elementID)
            {
                if (!Woters.Contains(elementID))
                    return;

                if(__result.TryGetComponent<KSelectable>(out var selectable))
                {
                    Element element = ElementLoader.FindElementByHash(elementID);
                    selectable.SetName("Bo'oh'  o'  " + element.name);
                }
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
                global::STRINGS.ELEMENTS.WATER.NAME = global::STRINGS.UI.FormatAsLink("Wo'ah", "WATER");
                global::STRINGS.ELEMENTS.DIRTYWATER.NAME = global::STRINGS.UI.FormatAsLink("Perloo'ed  Wo'ah", "DIRTYWATER"); 
                global::STRINGS.ELEMENTS.SALTWATER.NAME = global::STRINGS.UI.FormatAsLink("Serl'  Wo'ah", "SALTWATER");
			}
        }
    }
}
