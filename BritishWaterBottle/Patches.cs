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
                global::STRINGS.ELEMENTS.WATER.NAME = "Wo'ah";
                global::STRINGS.ELEMENTS.DIRTYWATER.NAME = "Perloo'ed  Wo'ah";
                global::STRINGS.ELEMENTS.SALTWATER.NAME = "Serl'  Wo'ah";
            }
        }
    }
}
