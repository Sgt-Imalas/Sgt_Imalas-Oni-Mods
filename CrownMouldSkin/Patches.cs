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
        /// 

        [HarmonyPatch(typeof(BuildingFacades), MethodType.Constructor, typeof(ResourceSet))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Postfix(BuildingFacades __instance)
            {
                __instance.Add("CMS_corner_moulding_b", STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_B.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_B.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_b_kanim");
                __instance.Add("CMS_corner_moulding_c", STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_C.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_C.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_c_kanim");
                __instance.Add("CMS_corner_moulding_d", STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_D.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_D.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_d_kanim");
                __instance.Add("CMS_corner_moulding_e", STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_E.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_E.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_e_kanim");
                __instance.Add("CMS_corner_moulding_f", STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_F.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_F.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_f_kanim");
                __instance.Add("CMS_corner_moulding_g", STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_G.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_G.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_g_kanim");
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
