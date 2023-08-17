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
using TemplateClasses;
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

                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Food, FlyingCreatureBaitConfig.ID, EggCrackerConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Food, AirborneCreatureLureConfig.ID, EggCrackerConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Food, FishTrapConfig.ID, EggCrackerConfig.ID);
                InjectionMethods.MoveExistingBuildingToNewCategory(GameStrings.PlanMenuCategory.Food, CreatureTrapConfig.ID, EggCrackerConfig.ID);

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

            public static void Prefix(ManagementMenu __instance, ManagementMenu.ManagementMenuToggleInfo toggleInfo, ref string disabledTooltip)
            {
                if (toggleInfo == __instance.skillsInfo)
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
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }

        [HarmonyPatch(typeof(LureSideScreen))]
        [HarmonyPatch(nameof(LureSideScreen.SetTarget))]
        public static class Revive_AirborneCreatureLureScreenExtension
        {
            public static void Prefix(LureSideScreen __instance)
            {

                __instance.baitAttractionStrings = new Dictionary<Tag, string>
                {
                    { SimHashes.SlimeMold.CreateTag(), global::STRINGS.CREATURES.SPECIES.PUFT.NAME},
                    { SimHashes.Phosphorus.CreateTag(),global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME},
                    { SimHashes.Phosphorite.CreateTag(), global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME},
                    { SimHashes.BleachStone.CreateTag(),global::STRINGS.CREATURES.SPECIES.PUFT.NAME},
                    { SimHashes.Diamond.CreateTag(),global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME},
                    { SimHashes.OxyRock.CreateTag(),global::STRINGS.CREATURES.SPECIES.PUFT.NAME},
                };
            }
        }

        [HarmonyPatch(typeof(AirborneCreatureLureConfig))]
        [HarmonyPatch(nameof(AirborneCreatureLureConfig.ConfigureBuildingTemplate))]
        public static class Revive_AirborneCreatureLureConfig2
        {
            public static void Postfix(GameObject prefab)
            {

                CreatureLure creatureLure = prefab.AddOrGet<CreatureLure>();
                creatureLure.baitTypes = new List<Tag>
                {
                    SimHashes.SlimeMold.CreateTag(),
                    SimHashes.Phosphorus.CreateTag(),
                    SimHashes.Phosphorite.CreateTag(),
                    SimHashes.BleachStone.CreateTag(),
                    SimHashes.Diamond.CreateTag(),
                    SimHashes.OxyRock.CreateTag(),
                };
                creatureLure.baitStorage.storageFilters = creatureLure.baitTypes;
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
                __result.ShowInBuildMenu = true;
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
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(GasConduitPreferentialFlowConfig))]
        [HarmonyPatch(nameof(GasConduitPreferentialFlowConfig.CreateBuildingDef))]
        public static class Revive_GasConduitPreferentialFlowConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(GenericFabricatorConfig))]
        [HarmonyPatch(nameof(GenericFabricatorConfig.CreateBuildingDef))]
        public static class Revive_GenericFabricatorConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(LiquidCooledFanConfig))]
        [HarmonyPatch(nameof(LiquidCooledFanConfig.CreateBuildingDef))]
        public static class Revive_LiquidCooledFanConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(PressureSwitchLiquidConfig))]
        [HarmonyPatch(nameof(PressureSwitchLiquidConfig.CreateBuildingDef))]
        public static class Revive_PressureSwitchLiquidConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
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
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(MachineShopConfig))]
        [HarmonyPatch(nameof(MachineShopConfig.CreateBuildingDef))]
        public static class Revive_MachineShopConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(AdvancedApothecaryConfig))]
        [HarmonyPatch(nameof(AdvancedApothecaryConfig.CreateBuildingDef))]
        public static class Revive_AdvancedApothecaryConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(OxygenMaskStationConfig))]
        [HarmonyPatch(nameof(OxygenMaskStationConfig.CreateBuildingDef))]
        public static class Revive_OxygenMaskStationConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(AstronautTrainingCenterConfig))]
        [HarmonyPatch(nameof(AstronautTrainingCenterConfig.CreateBuildingDef))]
        public static class Revive_AstronautTrainingCenterConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(SteamTurbineConfig))]
        [HarmonyPatch(nameof(SteamTurbineConfig.CreateBuildingDef))]
        public static class Revive_SteamTurbineConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(TemperatureControlledSwitchConfig))]
        [HarmonyPatch(nameof(TemperatureControlledSwitchConfig.CreateBuildingDef))]
        public static class Revive_TemperatureControlledSwitchConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(MouldingTileConfig))]
        [HarmonyPatch(nameof(MouldingTileConfig.CreateBuildingDef))]
        public static class Revive_MouldingTileConfig
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
                BuildingTemplates.CreateFoundationTileDef(__result);
            }
        }
        [HarmonyPatch(typeof(MouldingTileConfig))]
        [HarmonyPatch(nameof(MouldingTileConfig.DoPostConfigureComplete))]
        public static class Revive_MouldingTileConfig2
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(GameTags.FloorTiles);
            }
        }
        [HarmonyPatch(typeof(GeyserGenericConfig))]
        [HarmonyPatch(nameof(GeyserGenericConfig.CreateGeyser))]
        public static class GeyserResize
        {
            public static void Prefix(string id, ref int width, ref int height)
            {
                if (id.Contains("steam") || id.Contains("hot_steam") || id.Contains("methane"))
                {
                    width = 3;
                    height = 3;
                }

            }
        }

        [HarmonyPatch(typeof(RoleStationConfig))]
        [HarmonyPatch(nameof(RoleStationConfig.CreateBuildingDef))]
        public static class ReviveOldJobStation2
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
                __result.Overheatable = false;

            }
        }

        [HarmonyPatch(typeof(CreatureTrapConfig))]
        [HarmonyPatch(nameof(CreatureTrapConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_CreatureTrap
        {

            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }
        [HarmonyPatch(typeof(FishTrapConfig))]
        [HarmonyPatch(nameof(FishTrapConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_FishTrap
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
            }
        }

        #endregion

        [HarmonyPatch(typeof(LogicElementSensorGasConfig))]
        [HarmonyPatch(nameof(LogicElementSensorGasConfig.CreateBuildingDef))]
        public static class LogicElementSensorGasNeedsPower
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result.RequiresPowerInput = true;
                __result.AddLogicPowerPort = false;
                __result.EnergyConsumptionWhenActive = 25f;

            }
        }

        [HarmonyPatch(typeof(ExobaseHeadquartersConfig))]
        [HarmonyPatch(nameof(ExobaseHeadquartersConfig.ConfigureBuildingTemplate))]
        public static class RemoveSkillUpFromPrinterMini
        {
            public static void Postfix(GameObject go)
            {
                if (go.TryGetComponent<RoleStation>(out var station))
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
                __result.ShowInBuildMenu = true;
                __result.Deprecated = false;
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
        [HarmonyPatch(typeof(BottleEmptierGasConfig))]
        [HarmonyPatch(nameof(BottleEmptierGasConfig.CreateBuildingDef))]
        public static class AdjustSizePatch_BottleEmptierGas
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
        [HarmonyPatch(typeof(Assets))]
        [HarmonyPatch(nameof(Assets.GetAnim))]
        public static class TryGetRetroAnim_GetAnim
        {

            public static void Prefix(Assets __instance,ref HashedString name)
            {
                string retroStringVariant = name.ToString().Replace("_kanim", "_retro_kanim");
                if(name.IsValid && Assets.AnimTable.ContainsKey(retroStringVariant))
                {
                    name = retroStringVariant;
                }
            }
        }
        [HarmonyPatch(typeof(Assets))]
        [HarmonyPatch(nameof(Assets.TryGetAnim))]
        public static class TryGetRetroAnim_TryGetAnim
        {

            public static void Prefix(ref HashedString name)
            {
                string retroStringVariant = name.ToString().Replace("_kanim", "_retro_kanim");
                if (name.IsValid && Assets.AnimTable.ContainsKey(retroStringVariant))
                {
                    name = retroStringVariant;
                }
            }
        }



        [HarmonyPatch(typeof(AlgaeDistilleryConfig))]
        [HarmonyPatch(nameof(AlgaeDistilleryConfig.ConfigureBuildingTemplate))]
        public static class ManualAlgaeDestillery
        {

            public static void Postfix(GameObject go)
            {
                go.TryGetComponent<ManualDeliveryKG>(out var manualDeliveryKG);
                manualDeliveryKG.refillMass = 300f;
                manualDeliveryKG.capacity = 1000f;

                go.TryGetComponent<AlgaeDistillery>(out var distillery);
                UnityEngine.Object.Destroy(distillery);

                ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
                elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(SimHashes.SlimeMold.CreateTag(), 1.8f)
                };
                elementConverter.outputElements = new ElementConverter.OutputElement[2]
                {
                    new ElementConverter.OutputElement(0.6f, SimHashes.Algae, 303.15f, useEntityTemperature: false, storeOutput: false, 1f, 1f),
                    new ElementConverter.OutputElement(1.2f, SimHashes.DirtyWater, 303.15f, useEntityTemperature: false, storeOutput: true)
                };

                elementConverter.OperationalRequirement = Operational.State.Operational;

                var manualOperatable = go.AddComponent<GenericWorkableComponent>();
                manualOperatable.overrideAnims = new KAnimFile[1]
                {
                    Assets.GetAnim((HashedString) "anim_interacts_algae_distillery_kanim")
                };
                manualOperatable.workOffset = new CellOffset(-1, 0);
                manualOperatable.WorkTime = (30f);
                manualOperatable.workLayer = Grid.SceneLayer.BuildingUse;
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
                if (texture != null)
                {
                    tuning.foregroundTexture = texture;
                }

            }
        }
        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Postfix(Assets __instance)
            {
                var path = Path.Combine(Path.Combine(UtilMethods.ModPath, "assets"), "ReplacementSprites");

                SgtLogger.l(path, "PATH for imports");
                var files = new DirectoryInfo(path).GetFiles();

                SgtLogger.l(files.Count().ToString(), "Files to import and override");

                for (int i = 0; i < files.Count(); i++)
                {
                    var File = files[i];
                    try
                    {
                        AssetUtils.OverrideSpriteTextures(__instance, File);
                    }
                    catch (Exception e)
                    {
                        SgtLogger.logError("Failed importing sprite: " + File.FullName + ",\nError: " + e);
                    }
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
                if (Config.Instance.IronOreTexture == Config.EarlierVersion.Alpha)
                    SgtElementUtil.SetTexture_ShineMask(ironORe, "hematite_(alpha)_retro_ShineMask");

                var bleachstone = ElementLoader.GetElement(SimHashes.BleachStone.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(bleachstone, "bleach_stone_retro");

                var radEle = ElementLoader.GetElement(SimHashes.Radium.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(radEle, "radium_retro");
            }

        }

    }
}
