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
using static InventoryOrganization;

namespace DrywallPatternColours
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        /// 
        public static readonly string[] DrywallPatternIDs =
        {
            "ExteriorWall_red_deep",
            "ExteriorWall_orange_satsuma",
            "ExteriorWall_yellow_lemon",
            "ExteriorWall_green_kelly",
            "ExteriorWall_blue_cobalt",
            "ExteriorWall_pink_flamingo",
            "ExteriorWall_grey_charcoal",
            };

        //[HarmonyPatch(typeof(BuildingFacades), MethodType.Constructor, typeof(ResourceSet))] 
        //[HarmonyPatch(typeof(BuildingFacades), nameof(BuildingFacades.PostProcess))] 
        public class AddNewSkins
        {
            // manually patching, because referencing BuildingFacades class will load strings too early
            public static void Patch(Harmony harmony)
            {
                SgtLogger.l("init Patch 2");
                var targetType = AccessTools.TypeByName("Database.BuildingFacades");
                var target = AccessTools.Constructor(targetType, new[] { typeof(ResourceSet) });
                var postfix = AccessTools.Method(typeof(InitFacadePatchForDrywalls), "PostfixPatch");

                Debug.Log(targetType);
                Debug.Log(target);
                Debug.Log(postfix);

                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }


            public class InitFacadePatchForDrywalls
            {
                public static void PostfixPatch(object __instance)
                {
                    SgtLogger.l("Start Facade Patch");
                    var resource = (ResourceSet<BuildingFacadeResource>)__instance;
                    AddFacade(resource, DrywallPatternIDs[0], (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.RED_DEEP.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.RED_DEEP.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_red_deep_kanim");
                    AddFacade(resource, DrywallPatternIDs[1], (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.ORANGE_SATSUMA.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.ORANGE_SATSUMA.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_orange_satsuma_kanim");
                    AddFacade(resource, DrywallPatternIDs[2], (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.YELLOW_LEMON.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.YELLOW_LEMON.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_yellow_lemon_kanim");
                    AddFacade(resource, DrywallPatternIDs[3], (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREEN_KELLY.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREEN_KELLY.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_green_kelly_kanim");
                    AddFacade(resource, DrywallPatternIDs[4], (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.BLUE_COBALT.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.BLUE_COBALT.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_blue_cobalt_kanim");
                    AddFacade(resource, DrywallPatternIDs[5], (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.PINK_FLAMINGO.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.PINK_FLAMINGO.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_pink_flamingo_kanim");
                    AddFacade(resource, DrywallPatternIDs[6], (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREY_CHARCOAL.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREY_CHARCOAL.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_grey_charcoal_kanim");
                    
                    //foreach(var skin in resource.resources)
                    //{
                    //    SgtLogger.l( skin.Description, skin.Name);
                    //}
                    SgtLogger.l("Patch Executed");
                }

                public static void AddFacade(
                    ResourceSet<BuildingFacadeResource> set,
                    string id,
                    LocString name,
                    LocString description,
                    PermitRarity rarity,
                    string prefabId,
                    string animFile,
                    Dictionary<string, string> workables = null)
                {
                    set.resources.Add(new BuildingFacadeResource(id, name, description, rarity, prefabId, animFile, workables));
                }
            }
        }
        public static class SublevelCategoryPatch
        {
            public static void Patch(Harmony harmony)
            {
                SgtLogger.l("init category");
                var targetType = AccessTools.TypeByName("InventoryOrganization");
                var target = AccessTools.Method(targetType, "GenerateSubcategories");
                var postfix = AccessTools.Method(typeof(SublevelCategoryPatch), "PostfixMethod");


                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }
            public static void PostfixMethod()
            {
                SupplyClosetUtils.AddItemsToSubcategory( PermitSubcategories.BUILDING_WALLPAPER_PRINTS  ,DrywallPatternIDs);
            }
        }
        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Assets_OnPrefabInit_Patch
        {
            public static void Prefix()
            {
                AddNewSkins.Patch(Mod.harmonyInstance);
                SublevelCategoryPatch.Patch(Mod.harmonyInstance);
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
