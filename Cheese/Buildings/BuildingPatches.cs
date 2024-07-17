using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static InventoryOrganization;

namespace Cheese.Buildings
{
    internal class BuildingPatches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, CheeseSculptureConfig.ID, MarbleSculptureConfig.ID);
            }
        }
        public static List<string> CheeseSculptureSkins = new List<string>();

        //[HarmonyPatch(typeof(BuildingFacades), MethodType.Constructor, typeof(ResourceSet))] 
        //[HarmonyPatch(typeof(BuildingFacades), nameof(BuildingFacades.PostProcess))] 
        public class AddNewSkins
        {
            // manually patching, because referencing BuildingFacades class will load strings too early
            public static void Patch(Harmony harmony)
            {
                SgtLogger.l("init Patch 2");
                var targetType = AccessTools.TypeByName("Database.ArtableStages");
                var target = AccessTools.Constructor(targetType, new[] { typeof(ResourceSet) });
                var postfix = AccessTools.Method(typeof(InitCheeseBlockSkins), "PostfixPatch");

                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }


            public class InitCheeseBlockSkins
            {
                public static void PostfixPatch(object __instance)
                {
                    SgtLogger.l("Start Facade Patch");
                    var artableStages = (ArtableStages)__instance;

                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "Ratatouille",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_1.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_1.DESC,
                         "sculpture_cheese_amazing_1_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "Wallace",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_2.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_2.DESC,
                         "sculpture_cheese_amazing_2_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "CheeseMoon",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_3.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_3.DESC,
                         "sculpture_cheese_amazing_3_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));
                    
                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "Sergal",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_4.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_4.DESC,
                         "sculpture_cheese_amazing_4_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "Spongebob",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_5.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_5.DESC,
                         "sculpture_cheese_amazing_5_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "CheezeKitten",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_6.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_6.DESC,
                         "sculpture_cheese_amazing_6_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "PileOfWheels",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_7.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_AMAZING_7.DESC,
                         "sculpture_cheese_amazing_7_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));



                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "Cheeseception",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_GOOD_1.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_GOOD_1.DESC,
                         "sculpture_cheese_good_1_kanim",
                         ArtableStatuses.ArtableStatusType.LookingOkay));



                    CheeseSculptureSkins.Add(AddStatueStage(artableStages,
                        CheeseSculptureConfig.ID,
                        "HungryBite",
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_CRAP_1.NAME,
                        STRINGS.BUILDINGS.PREFABS.CHEESE_CHEESESCULPTURE.FACADES.SCULPTURE_CHEESE_CRAP_1.DESC,
                         "sculpture_cheese_crap_1_kanim",
                         ArtableStatuses.ArtableStatusType.LookingUgly));


                    SgtLogger.l("Patch Executed");
                }

                private static string AddStatueStage(ArtableStages __instance, string buildingId, string statueId, string name, string description, string kanim, ArtableStatuses.ArtableStatusType level)
                {
                    ArtHelper.GetDefaultDecors(__instance, buildingId, out var greatDecor, out var okayDecor, out var uglyDecor);
                    int decor;
                    switch (level)
                    {
                        default:
                        case ArtableStatuses.ArtableStatusType.LookingUgly: 
                            decor = uglyDecor; break;
                        case ArtableStatuses.ArtableStatusType.LookingOkay: 
                            decor = okayDecor; break;
                        case ArtableStatuses.ArtableStatusType.LookingGreat:
                            decor = greatDecor; break;
                    }

                    string skinID = buildingId + "_" + statueId;
                    __instance.Add(
                        skinID,
                        name,
                        description,
                        PermitRarity.Universal,
                        kanim,
                        statueId, // leftover from when these were one merged animation file
                        decor,
                        true,
                        level.ToString(),
                        buildingId,
                        "ui",
                        DlcManager.AVAILABLE_ALL_VERSIONS
                        );
                    return skinID;
                }
            }
        }
        public static class SublevelCategoryPatch
        {
            public static void Patch(Harmony harmony)
            {
                var targetType = AccessTools.TypeByName("InventoryOrganization");
                var target = AccessTools.Method(targetType, "GenerateSubcategories");
                var postfix = AccessTools.Method(typeof(SublevelCategoryPatch), "PostfixMethod");
                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }
            public static void PostfixMethod()
            {
                SupplyClosetUtils.AddItemsToSubcategory(PermitSubcategories.BUILDING_SCULPTURE, CheeseSculptureSkins.ToArray());
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
    }
}
