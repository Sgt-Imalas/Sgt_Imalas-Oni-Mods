using AkisSnowThings.Content.Defs.Buildings;
using AkisSnowThings.Content.Scripts.Buildings;
using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static InventoryOrganization;

namespace AkisSnowThings.Patches.Buildings
{
    internal class BuildingFacadesPatches
    {
        public static void PrefixPatch()
        {
            AddNewSkinsPatch.Patch(Mod.HarmonyInstance);
            SublevelCategoryPatch.Patch(Mod.HarmonyInstance);
        }

        public static List<string> SculptureSkins = new List<string>();
        internal class AddNewSkinsPatch
        {
            // manually patching, because referencing BuildingFacades class will load strings too early
            public static void Patch(Harmony harmony)
            {
                SgtLogger.l("init sculpture patch");
                var targetType = AccessTools.TypeByName("Database.ArtableStages");
                var target = AccessTools.Constructor(targetType, [typeof(ResourceSet)]);
                var postfix = AccessTools.Method(typeof(InitSculptureSkins), "PostfixPatch");

                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }
            public class InitSculptureSkins
            {

                public static string AddSnowStatueStage(ArtableStages __instance, string buildingId, string statueId, string name, string description, string kanim, ArtableStatuses.ArtableStatusType level)
                    => ArtHelper.AddStatueStage(__instance, buildingId, statueId, name, description, kanim, level, "pile");

                public static void PostfixPatch(object __instance)
                {
                    SgtLogger.l("Start Facade Patch");
                    var artableStages = (ArtableStages)__instance;
                    string id = SnowSculptureConfig.ID;

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "RomenCat",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_1.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_1.DESC,
                         "snowsculptures_snowsculpture_amazing_1_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Pip",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_2.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_2.DESC,
                         "snowsculptures_snowsculpture_amazing_2_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "SwoleMeep",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_3.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_3.DESC,
                         "snowsculptures_snowsculpture_amazing_3_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "WorkerSnowman",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_4.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_4.DESC,
                         "snowsculptures_snowsculpture_amazing_4_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "LoafKitten",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_5.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_5.DESC,
                         "snowsculptures_snowsculpture_amazing_5_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Hassan",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_6.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_6.DESC,
                         "snowsculptures_snowsculpture_amazing_6_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Pufts",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_7.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_7.DESC,
                         "snowsculptures_snowsculpture_amazing_7_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Classic",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_8.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_8.DESC,
                         "snowsculptures_snowsculpture_amazing_8_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Pei",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_9.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_9.DESC,
                         "snowsculptures_snowsculpture_amazing_9_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "SnowGolem",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_10.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_10.DESC,
                         "snowsculptures_snowsculpture_amazing_10_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Slicksters",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_11.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_11.DESC,
                         "snowsculptures_snowsculpture_amazing_11_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat));

                    SnowPile.SNOWDOG = AddSnowStatueStage(artableStages,
                        id,
                        "SnowDog",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_DOG.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_DOG.DESC,
                         "snowsculptures_snowsculpture_dog_kanim",
                         ArtableStatuses.ArtableStatusType.LookingGreat);

                    SculptureSkins.Add(SnowPile.SNOWDOG);


                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Muckroot",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_GOOD_1.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_GOOD_1.DESC,
                         "snowsculptures_snowsculpture_good_1_kanim",
                         ArtableStatuses.ArtableStatusType.LookingOkay));

                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "Derp",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_GOOD_2.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_GOOD_2.DESC,
                         "snowsculptures_snowsculpture_good_2_kanim",
                         ArtableStatuses.ArtableStatusType.LookingOkay));



                    SculptureSkins.Add(AddSnowStatueStage(artableStages,
                        id,
                        "PileWithEyes",
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_CRAP_1.NAME,
                        STRINGS.BUILDINGS.PREFABS.SNOWSCULPTURES_SNOWSCULPTURE.FACADES.SNOWSCULPTURES_SNOWSCULPTURE_CRAP_1.DESC,
                         "snowsculptures_snowsculpture_crap_1_kanim",
                         ArtableStatuses.ArtableStatusType.LookingUgly));


                    SgtLogger.l("Patch Executed");
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
                SupplyClosetUtils.AddItemsToSubcategory(PermitSubcategories.BUILDING_SCULPTURE, SculptureSkins.ToArray());
            }
        }
    }
}
