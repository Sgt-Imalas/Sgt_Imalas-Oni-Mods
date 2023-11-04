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
using static CrownMouldSkin.Patches.AddNewSkins;
using static InventoryOrganization;

namespace CrownMouldSkin
{
    internal class Patches
    {
        public static readonly string[] CornerMouldingIDs =
        {
            "CMS_corner_moulding_b",
            "CMS_corner_moulding_c",
            "CMS_corner_moulding_d",
            "CMS_corner_moulding_e",
            "CMS_corner_moulding_f",
            "CMS_corner_moulding_g",

        };
        public class AddNewSkins
        {
            // manually patching, because referencing BuildingFacades class will load strings too early
            public static void Patch(Harmony harmony)
            {
                SgtLogger.l("init Patch 2");
                var targetType = AccessTools.TypeByName("Database.BuildingFacades");
                var target = AccessTools.Constructor(targetType, new[] { typeof(ResourceSet) });
                var postfix = AccessTools.Method(typeof(InitFacadePatchForDrywalls), "PostfixPatch");


                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }

            public class InitFacadePatchForDrywalls
            {
                public static void PostfixPatch(object __instance)
                {
                    SgtLogger.l("Start Skin Patch");
                    var resource = (ResourceSet<BuildingFacadeResource>)__instance;
                    AddFacade(resource, CornerMouldingIDs[0], STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_B.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_B.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_b_kanim");
                    AddFacade(resource, CornerMouldingIDs[1], STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_C.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_C.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_c_kanim");
                    AddFacade(resource, CornerMouldingIDs[2], STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_D.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_D.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_d_kanim");
                    AddFacade(resource, CornerMouldingIDs[3], STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_E.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_E.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_e_kanim");
                    AddFacade(resource, CornerMouldingIDs[4], STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_F.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_F.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_f_kanim");
                    AddFacade(resource, CornerMouldingIDs[5], STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_G.NAME, STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.FACADES.CORNER_TILE_G.DESC, PermitRarity.Universal, CornerMouldingConfig.ID, "corner_tile_g_kanim");

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
                }
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Assets_OnPrefabInit_Patch
        {
            public static void Prefix()
            {
                AddNewSkins.Patch(Mod.HarmonyInstance);
                SublevelCategoryPatch.Patch(Mod.HarmonyInstance);
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
                SupplyClosetUtils.AddSubcategory(InventoryPermitCategories.BUILDINGS, "BUILDING_CORNER_MOULDING", Def.GetUISprite(CornerMouldingConfig.ID).first, 132, CornerMouldingIDs);
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
