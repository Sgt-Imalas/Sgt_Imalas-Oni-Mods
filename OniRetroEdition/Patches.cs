using Database;
using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using OniRetroEdition.Behaviors;
using ProcGenGame;
using ShockWormMob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static OniRetroEdition.ModAssets;
using static STRINGS.BUILDINGS.PREFABS;
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
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Base, MouldingTileConfig.ID, CarpetTileConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Rocketry, CrewCapsuleConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Food, AtmoicGardenConfig.ID, FarmTileConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Refinement, GenericFabricatorConfig.ID, RockCrusherConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Stations, MachineShopConfig.ID, PowerControlStationConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Medicine, AdvancedApothecaryConfig.ID, ApothecaryConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Stations, OxygenMaskStationConfig.ID, OxygenMaskMarkerConfig.ID, string.Empty, ModUtil.BuildingOrdering.Before);


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

        #region reenableDeprecateds
        [HarmonyPatch(typeof(AirborneCreatureLureConfig))]
        [HarmonyPatch(nameof(AirborneCreatureLureConfig.CreateBuildingDef))]
        public static class Revive_AirborneCreatureLureConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(PressureSwitchGasConfig))]
        [HarmonyPatch(nameof(PressureSwitchGasConfig.CreateBuildingDef))]
        public static class Revive_PressureSwitchGasConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(AtmoicGardenConfig))]
        [HarmonyPatch(nameof(AtmoicGardenConfig.CreateBuildingDef))]
        public static class Revive_AtmoicGardenConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(CrewCapsuleConfig))]
        [HarmonyPatch(nameof(CrewCapsuleConfig.CreateBuildingDef))]
        public static class Revive_CrewCapsuleConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(GasConduitOverflowConfig))]
        [HarmonyPatch(nameof(GasConduitOverflowConfig.CreateBuildingDef))]
        public static class Revive_GasConduitOverflowConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(GasConduitPreferentialFlowConfig))]
        [HarmonyPatch(nameof(GasConduitPreferentialFlowConfig.CreateBuildingDef))]
        public static class Revive_GasConduitPreferentialFlowConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(GenericFabricatorConfig))]
        [HarmonyPatch(nameof(GenericFabricatorConfig.CreateBuildingDef))]
        public static class Revive_GenericFabricatorConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(LiquidCooledFanConfig))]
        [HarmonyPatch(nameof(LiquidCooledFanConfig.CreateBuildingDef))]
        public static class Revive_LiquidCooledFanConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(PressureSwitchLiquidConfig))]
        [HarmonyPatch(nameof(PressureSwitchLiquidConfig.CreateBuildingDef))]
        public static class Revive_PressureSwitchLiquidConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(LiquidConduitOverflowConfig))]
        [HarmonyPatch(nameof(LiquidConduitOverflowConfig.CreateBuildingDef))]
        public static class Revive_LiquidConduitOverflowConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(LiquidConduitPreferentialFlowConfig))]
        [HarmonyPatch(nameof(LiquidConduitPreferentialFlowConfig.CreateBuildingDef))]
        public static class Revive_LiquidConduitPreferentialFlowConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(MachineShopConfig))]
        [HarmonyPatch(nameof(MachineShopConfig.CreateBuildingDef))]
        public static class Revive_MachineShopConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(AdvancedApothecaryConfig))]
        [HarmonyPatch(nameof(AdvancedApothecaryConfig.CreateBuildingDef))]
        public static class Revive_AdvancedApothecaryConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(OxygenMaskStationConfig))]
        [HarmonyPatch(nameof(OxygenMaskStationConfig.CreateBuildingDef))]
        public static class Revive_OxygenMaskStationConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(AstronautTrainingCenterConfig))]
        [HarmonyPatch(nameof(AstronautTrainingCenterConfig.CreateBuildingDef))]
        public static class Revive_AstronautTrainingCenterConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(SteamTurbineConfig))]
        [HarmonyPatch(nameof(SteamTurbineConfig.CreateBuildingDef))]
        public static class Revive_SteamTurbineConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(TemperatureControlledSwitchConfig))]
        [HarmonyPatch(nameof(TemperatureControlledSwitchConfig.CreateBuildingDef))]
        public static class Revive_TemperatureControlledSwitchConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(MouldingTileConfig))]
        [HarmonyPatch(nameof(MouldingTileConfig.CreateBuildingDef))]
        public static class Revive_MouldingTileConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.Deprecated = false;
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
#endregion


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
        [HarmonyPatch(typeof(OuthouseConfig))]
        [HarmonyPatch(nameof(OuthouseConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_Outhouse
        {

            public static void Postfix(ref BuildingDef __result)
            {
                __result.HeightInCells = 2;
                __result.WidthInCells = 2;
                __result.GenerateOffsets();
            }
        }
        [HarmonyPatch(typeof(BottleEmptierConfig))]
        [HarmonyPatch(nameof(BottleEmptierConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_BottleEmptier
        {

            public static void Postfix(ref BuildingDef __result)
            {
                __result.HeightInCells = 3;
                __result.WidthInCells = 2;
                __result.GenerateOffsets();
            }
        }
        [HarmonyPatch(typeof(GasBottlerConfig))]
        [HarmonyPatch(nameof(GasBottlerConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_GasBottler
        {

            public static void Postfix(ref BuildingDef __result)
            {

                __result.UtilityInputOffset = new CellOffset(1, 2);
                __result.HeightInCells = 3;
                __result.WidthInCells = 2;
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

        [HarmonyPatch(typeof(SolidConduitFlowVisualizer))]
        [HarmonyPatch(typeof(SolidConduitFlowVisualizer), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(SolidConduitFlow), typeof(Game.ConduitVisInfo), typeof(FMODUnity.EventReference), typeof(SolidConduitFlowVisualizer.Tuning) })]
        public class ChangeGame
        {
            public static void Prefix(
                SolidConduitFlowVisualizer.Tuning tuning)
            {
                SgtLogger.l("GamePrefabInit");

                var path = Path.Combine(UtilMethods.ModPath, "assets");
                var texture = AssetUtils.LoadTexture("conveyor_box_retro", path);


                SgtLogger.Assert("TextureNotNull", texture);
                if(texture != null)
                {
                    tuning.foregroundTexture = texture;
                }

            }
        }


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
                if(Config.Instance.IronOreTexture == Config.EarlierVersion.Alpha) 
                    SgtElementUtil.SetTexture_ShineMask(aluminium, "hematite_(t)_retro_ShineMask");

                var bleachstone = ElementLoader.GetElement(SimHashes.BleachStone.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(bleachstone, "bleach_stone_retro");

                var radEle = ElementLoader.GetElement(SimHashes.Radium.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(radEle, "radium_retro");
            }

        }

    }
}
