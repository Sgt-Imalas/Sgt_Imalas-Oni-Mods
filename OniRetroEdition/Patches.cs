using Database;
using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using OniRetroEdition.Behaviors;
using Satsuma;
using ShockWormMob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static OniRetroEdition.ModAssets;
using static STRINGS.CREATURES.STATS;

namespace OniRetroEdition
{
    internal class Patches
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
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Stations, RoleStationConfig.ID, ResetSkillsStationConfig.ID,ordering:ModUtil.BuildingOrdering.Before);
            }
        }// <summary>
        /// Register Buildings to existing Technologies (newly added techs are in "ResearchTreePatches" class
        /// </summary>
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.Employment, RoleStationConfig.ID);
            }
        }

        [HarmonyPatch(typeof(RoleStationConfig))]
        [HarmonyPatch(nameof(RoleStationConfig.ConfigureBuildingTemplate))]
        public static class ReviveOldJobStation
        {
            public static void Postfix(GameObject go)
            {
                go.GetComponent<KPrefabID>().AddTag(GameTags.Experimental);
                RoleStation roleStation = go.AddOrGet<RoleStation>();
                roleStation.overrideAnims = new KAnimFile[1]
                {
                Assets.GetAnim((HashedString) "anim_interacts_job_station_kanim")
                };
                go.AddOrGet<JobBoardSkillbutton>();
            }
        }

        [HarmonyPatch(typeof(ManagementMenu))]
        [HarmonyPatch(nameof(ManagementMenu.AddToggleTooltip))]
        public static class SwapTooltipForSkillStation
        {

            public static void Prefix(ManagementMenu __instance,ManagementMenu.ManagementMenuToggleInfo toggleInfo, ref string disabledTooltip)
            {
                if(toggleInfo == __instance.skillsInfo)
                {
                    disabledTooltip = STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_SKILL_STATION_RETRO;
                }
            }
        }

        /// <summary>
        /// fixing crash on end
        /// </summary>
        [HarmonyPatch(typeof(RoleStation))]
        [HarmonyPatch(nameof(RoleStation.OnStopWork))]
        public static class FixCrash
        {

            public static bool Prefix(RoleStation __instance)
            {
                Telepad.StatesInstance sMI = __instance.GetSMI<Telepad.StatesInstance>();
                return sMI != null;
            }
        }

        /// <summary>
        /// Adjusting work time
        /// </summary>
        [HarmonyPatch(typeof(RoleStation))]
        [HarmonyPatch(nameof(RoleStation.OnSpawn))]
        public static class RoleStationFix
        {
            public static void Postfix(RoleStation __instance)
            {
                __instance.SetWorkTime(3f);
            }
        }

        [HarmonyPatch(typeof(RoleStationConfig))]
        [HarmonyPatch(nameof(RoleStationConfig.CreateBuildingDef))]
        public static class ReviveOldJobStation2
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
                __result.Overheatable = false;

            }
        }



        [HarmonyPatch(typeof(ExobaseHeadquartersConfig))]
        [HarmonyPatch(nameof(ExobaseHeadquartersConfig.ConfigureBuildingTemplate))]
        public static class RemoveSkillUpFromPrinterMini
        {
            public static void Postfix(GameObject go)
            {
                if(go.TryGetComponent<RoleStation>(out var station))
                {
                    UnityEngine.Object.Destroy(station);
                }
            }
        }

        [HarmonyPatch(typeof(HeadquartersConfig))]
        [HarmonyPatch(nameof(HeadquartersConfig.ConfigureBuildingTemplate))]
        public static class RemoveSkillUpFromPrinterMain
        {
            public static void Postfix(GameObject go)
            {
                if (go.TryGetComponent<RoleStation>(out var station))
                {
                    UnityEngine.Object.Destroy(station);
                }
            }
        }

        [HarmonyPatch(typeof(TelepadSideScreen))]
        [HarmonyPatch(nameof(TelepadSideScreen.OnSpawn))]
        public static class RemoveSkillsButtonFromPrinterScreen
        {
            public static void Postfix(TelepadSideScreen __instance)
            {
                __instance.openRolesScreenButton.ClearOnClick();
                __instance.openRolesScreenButton.gameObject.SetActive(false);

            }
        }
        [HarmonyPatch(typeof(TelepadSideScreen))]
        [HarmonyPatch(nameof(TelepadSideScreen.UpdateSkills))]
        public static class RemoveSkillsButtonFromPrinterScreen2
        {
            public static bool Prefix(TelepadSideScreen __instance)
            {
                __instance.skillPointsAvailable.gameObject.SetActive(false);
                return false;
            }
        }

        [HarmonyPatch(typeof(FlyingCreatureBaitConfig))]
        [HarmonyPatch(nameof(FlyingCreatureBaitConfig.CreateBuildingDef))]
        public static class AirborneCritterBait_CeilingOnly
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.BuildLocationRule = BuildLocationRule.OnCeiling;
            }
        }

        [HarmonyPatch(typeof(TravelTubeEntranceConfig))]
        [HarmonyPatch(nameof(TravelTubeEntranceConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_TravelTubeEntranceConfig
        {

            public static void Postfix(ref BuildingDef __result)
            {
                __result.HeightInCells = 2;
                __result.WidthInCells = 2;
                __result.GenerateOffsets();
            }
        }
        [HarmonyPatch(typeof(HydrogenGeneratorConfig))]
        [HarmonyPatch(nameof(HydrogenGeneratorConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_HydrogenGen
        {

            public static void Postfix(ref BuildingDef __result)
            {
                __result.HeightInCells = 2;
                __result.WidthInCells = 4;
                __result.GenerateOffsets();
            }
        }
        [HarmonyPatch(typeof(WaterPurifierConfig))]
        [HarmonyPatch(nameof(WaterPurifierConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_WaterSieve
        {

            public static void Postfix(ref BuildingDef __result)
            {

                __result.UtilityInputOffset = new CellOffset(-1, 1);
                __result.UtilityOutputOffset = new CellOffset(1, 1);
                __result.PowerInputOffset = new CellOffset(1, 0);
                __result.HeightInCells = 2;
                __result.WidthInCells = 3;
                __result.GenerateOffsets();
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


        //[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        //public class Assets_OnPrefabInit_Patch
        //{
        //    public static void Prefix(Assets __instance)
        //    {
        //        InjectionMethods.AddSpriteToAssets(__instance, "conveyor_box",true);
        //    }
        //}


        [HarmonyPatch(typeof(ElementLoader), "Load")]
        public static class Patch_ElementLoader_Load
        {
            public static void Postfix()
            {
                
                //var metalMaterial = ElementLoader.GetElement(SimHashes.Steel.CreateTag()).substance.material;

                // fix lead specular
                //var lead = ElementLoader.FindElementByHash(SimHashes.Lead);
                //lead.substance.material.SetTexture("_ShineMask", AssetUtils.LoadTexture("lead_mask_fixed", texturePath));


                var aluminium = ElementLoader.GetElement(SimHashes.Aluminum.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(aluminium, "aluminium_retro");
                SgtElementUtil.SetTexture_ShineMask(aluminium, "aluminum_retro_ShineMask");

                var co2Solid = ElementLoader.GetElement(SimHashes.SolidCarbonDioxide.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(co2Solid, "solid_carbon_dioxide_retro");


                var depletedU = ElementLoader.GetElement(SimHashes.DepletedUranium.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(depletedU, "depleted_uranium_retro");

                var enrichedU = ElementLoader.GetElement(SimHashes.EnrichedUranium.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(enrichedU, "enriched_uranium_retro");

                var ironORe = ElementLoader.GetElement(SimHashes.IronOre.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(ironORe, Config.Instance.IronOreTexture == Config.EarlierVersion.Beta ? "hematite_(t)_retro" : "hematite_(alpha)_retro");

                var bleachstone = ElementLoader.GetElement(SimHashes.BleachStone.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(bleachstone, "bleach_stone_retro");

                var radEle = ElementLoader.GetElement(SimHashes.Radium.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(radEle, "radium_retro");
            }

        }

    }
}
