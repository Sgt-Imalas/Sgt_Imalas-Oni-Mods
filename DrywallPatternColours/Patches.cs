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
                    AddFacade(resource, "ExteriorWall_red_deep", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.RED_DEEP.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.RED_DEEP.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_red_deep_kanim");
                    AddFacade(resource, "ExteriorWall_orange_satsuma", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.ORANGE_SATSUMA.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.ORANGE_SATSUMA.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_orange_satsuma_kanim");
                    AddFacade(resource, "ExteriorWall_yellow_lemon", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.YELLOW_LEMON.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.YELLOW_LEMON.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_yellow_lemon_kanim");
                    AddFacade(resource, "ExteriorWall_green_kelly", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREEN_KELLY.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREEN_KELLY.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_green_kelly_kanim");
                    AddFacade(resource, "ExteriorWall_blue_cobalt", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.BLUE_COBALT.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.BLUE_COBALT.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_blue_cobalt_kanim");
                    AddFacade(resource, "ExteriorWall_pink_flamingo", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.PINK_FLAMINGO.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.PINK_FLAMINGO.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_pink_flamingo_kanim");
                    AddFacade(resource, "ExteriorWall_grey_charcoal", (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREY_CHARCOAL.NAME, (string)STRINGS.BUILDINGS.PREFABS_MODDEDSKINS.EXTERIORWALL.FACADES.GREY_CHARCOAL.DESC, PermitRarity.Universal, ExteriorWallConfig.ID, "walls_grey_charcoal_kanim");
                    
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

        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Assets_OnPrefabInit_Patch
        {
            public static void Prefix()
            {
                AddNewSkins.Patch(Mod.harmonyInstance);
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
