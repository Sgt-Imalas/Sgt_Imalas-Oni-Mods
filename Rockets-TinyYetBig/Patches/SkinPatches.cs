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
using static InventoryOrganization;

namespace Rockets_TinyYetBig.Patches
{
    internal class Patches
    {
        static List<string> RocketSkins=new List<string>(16);
        public class AddNewSkins
        {
            // manually patching, because referencing BuildingFacades class will load strings too early
            public static void Patch(Harmony harmony)
            {
                SgtLogger.l("init Patch 2");
                var targetType = AccessTools.TypeByName("Database.BuildingFacades");
                var target = AccessTools.Constructor(targetType, new[] { typeof(ResourceSet) });
                var postfix = AccessTools.Method(typeof(InitFacadePatchForRockets), "PostfixPatch");


                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }

            public class InitFacadePatchForRockets
            {
                public static void PostfixPatch(object __instance)
                {
                    SgtLogger.l("Start Skin Patch");
                    var resource = (ResourceSet<BuildingFacadeResource>)__instance;
                    //AddFacade(resource, "steamRocketTest", "Skin Test", "skin Test", PermitRarity.Universal, SteamEngineClusterConfig.ID, "rocket_natgas_engine_kanim");

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
                    set.resources.Add(new BuildingFacadeResource(id, name, description, rarity, prefabId, animFile, DlcManager.AVAILABLE_ALL_VERSIONS, workables));
                    RocketSkins.Add(id);
                }
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Assets_OnPrefabInit_Patch
        {
            public static void Prefix()
            {
                AddNewSkins.Patch(Mod.haromy);
                SublevelCategoryPatch.Patch(Mod.haromy);
            }
        }

       // [HarmonyPatch(typeof(InventoryOrganization), "GenerateSubcategories")]
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
                SupplyClosetUtils.AddSubcategory(InventoryPermitCategories.BUILDINGS, "BUILDING_CORNER_MOULDING", Def.GetUISprite(CornerMouldingConfig.ID).first, 132, RocketSkins.ToArray());
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
