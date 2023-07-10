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
using static DrywallPatternColours.ModAssets;

namespace DrywallPatternColours
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        /// 

        //[HarmonyPatch(typeof(BuildingFacades), MethodType.Constructor, typeof(ResourceSet))] 
        //[HarmonyPatch(typeof(BuildingFacades), nameof(BuildingFacades.PostProcess))] 
        public static class AddNewSkins
        {

            public static void initSkins(BuildingFacades __instance)
            {
                SgtLogger.l("init skins");
                __instance.Add("ExteriorWall_red_deep", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.RED_DEEP.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.RED_DEEP.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_red_deep_kanim");
                __instance.Add("ExteriorWall_orange_satsuma", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.ORANGE_SATSUMA.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.ORANGE_SATSUMA.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_orange_satsuma_kanim");
                __instance.Add("ExteriorWall_yellow_lemon", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.YELLOW_LEMON.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.YELLOW_LEMON.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_yellow_lemon_kanim");
                __instance.Add("ExteriorWall_green_kelly", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREEN_KELLY.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREEN_KELLY.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_green_kelly_kanim");
                __instance.Add("ExteriorWall_blue_cobalt", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.BLUE_COBALT.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.BLUE_COBALT.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_blue_cobalt_kanim");
                __instance.Add("ExteriorWall_pink_flamingo", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.PINK_FLAMINGO.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.PINK_FLAMINGO.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_pink_flamingo_kanim");
                __instance.Add("ExteriorWall_grey_charcoal", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREY_CHARCOAL.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREY_CHARCOAL.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_grey_charcoal_kanim");
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Init_Patch
        {
            // using System; will allow using Type insted of System.Type
            // using System.Reflection; will allow using MethodInfo instead of System.Reflection.MethodInfo
           
            

            public static void Postfix(Db __instance)
            {
            }
        }
        [HarmonyPatch(typeof(BuildingFacades))]
        [HarmonyPatch("PostProcess")]
        public static class BuildingFacadesbats
        {
            // using System; will allow using Type insted of System.Type
            // using System.Reflection; will allow using MethodInfo instead of System.Reflection.MethodInfo



            public static void Prefix(BuildingFacades __instance)
            {
                AddNewSkins.initSkins(Db.Get().Permits.BuildingFacades);
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
