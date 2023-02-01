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
using static CrownMouldSkin.ModAssets;

namespace CrownMouldSkin
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(BuildingFacades),"Load")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Postfix(BuildingFacades __instance)
            {
                __instance.Add("CMS_cornerTileAnkled", (string)global::STRINGS.BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.NAME, (string)global::STRINGS.BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.DESC, PermitRarity.Decent, CornerMouldingConfig.ID, "corner_tile_ankled_kanim");
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
