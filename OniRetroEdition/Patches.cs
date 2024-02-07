using Database;
using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using OniRetroEdition.Behaviors;
using OniRetroEdition.BuildingDefModification;
using PeterHan.PLib.Actions;
using PeterHan.PLib.Core;
using ProcGenGame;
using ShockWormMob;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using TUNING;
using UnityEngine;
using UtilLibs;
using static OniRetroEdition.ModAssets;
using static SimDebugView;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.CREATURES.STATS;
using static STRINGS.MISC.STATUSITEMS;

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
                foreach (var config in BuildingModifications.Instance.LoadedBuildingOverrides)
                {


                    if (config.Value.buildMenuCategory != null && config.Value.buildMenuCategory.Length > 0)
                    {
                        string buildingId = config.Key;
                        string category = config.Value.buildMenuCategory;

                        string relativeBuildingId = null;

                        if (config.Value.placedBehindBuildingId != null && config.Value.placedBehindBuildingId.Length > 0)
                        {
                            relativeBuildingId = config.Value.placedBehindBuildingId;
                            if (relativeBuildingId == null || relativeBuildingId.Length == 0)
                                continue;

                            if (config.Value.placeBefore.HasValue)
                            {
                                bool before = config.Value.placeBefore.Value;
                                InjectionMethods.MoveExistingBuildingToNewCategory(category, buildingId, relativeBuildingId, string.Empty, before ? ModUtil.BuildingOrdering.Before : ModUtil.BuildingOrdering.After);
                                continue;
                            }

                            InjectionMethods.MoveExistingBuildingToNewCategory(category, buildingId, relativeBuildingId);
                            continue;
                        }
                        InjectionMethods.MoveExistingBuildingToNewCategory(category, buildingId);
                    }
                }
            }
        }// <summary>
        /// Register Buildings to existing Technologies (newly added techs are in "ResearchTreePatches" class
        /// </summary>
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.Employment, RoleStationConfig.ID);
                foreach (var config in BuildingModifications.Instance.LoadedBuildingOverrides)
                {
                    if (config.Value.techOverride != null && config.Value.techOverride.Length > 0)
                    {
                        var previousTech = __instance.Techs.TryGetTechForTechItem(config.Key);
                        if (previousTech != null)
                        {
                            previousTech.RemoveUnlockedItemIDs(config.Key);
                        }

                        InjectionMethods.AddBuildingToTechnology(config.Value.techOverride, config.Key);
                    }
                }
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

        [HarmonyPatch]
        ///Connects mesh+airflow and normal tiles
        public static class ConnectingTiles
        {
            [HarmonyPrepare]
            public static bool Prepare() => Config.Instance.TileTopsMerge;

            [HarmonyPostfix]
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = TileConfig.BlockTileConnectorID;
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.ConfigureBuildingTemplate);
                yield return typeof(MeshTileConfig).GetMethod(name);
                yield return typeof(GasPermeableMembraneConfig).GetMethod(name);
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

        #endregion

        [HarmonyPatch(typeof(LogicElementSensorGasConfig))]
        [HarmonyPatch(nameof(LogicElementSensorGasConfig.CreateBuildingDef))]
        public static class LogicElementSensorGasNeedsPower
        {
            public static bool Prepare() => Config.Instance.gassensorpower;

            public static void Postfix(ref BuildingDef __result)
            {
                __result.RequiresPowerInput = true;
                __result.AddLogicPowerPort = false;
                __result.EnergyConsumptionWhenActive = 25f;

            }
        }
        [HarmonyPatch(typeof(LogicElementSensorLiquidConfig))]
        [HarmonyPatch(nameof(LogicElementSensorLiquidConfig.CreateBuildingDef))]
        public static class LogicElementSensorliquidNeedsPower
        {
            public static bool Prepare() => Config.Instance.liquidsensorpower;

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

        [HarmonyPatch(typeof(MopTool))]
        [HarmonyPatch(nameof(MopTool.OnPrefabInit))]
        public static class Moppable_AlwaysMop
        {
            [HarmonyPrepare]
            public static bool Prepare() => Config.Instance.succmop;
            public static void Postfix(MopTool __instance)
            {
                MopTool.maxMopAmt = float.PositiveInfinity;
            }
        }


        [HarmonyPatch(typeof(MopTool), "OnDragTool")]
        public class MopTool_OnDragTool_Patch
        {
            [HarmonyPrepare]
            public static bool Prepare() => Config.Instance.succmop;
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var index = codes.FindIndex(ci => ci.opcode == OpCodes.Stloc_1);

                if (index == -1)
                {
                    SgtLogger.error("mop transpiler found no target!");
                    return codes;
                }

                var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(MopTool_OnDragTool_Patch), "InjectedMethod");

                // inject right after the found index
                codes.InsertRange(index, new[]
                {
                            new CodeInstruction(OpCodes.Call, m_InjectedMethod)
                        });

                TranspilerHelper.PrintInstructions(codes);
                return codes;
            }

            private static bool InjectedMethod(bool old)
            {
                return true;
            }
        }
        [HarmonyPatch(typeof(Moppable))]
        [HarmonyPatch(nameof(Moppable.OnSpawn))]
        public static class Moppable_Watergun
        {
            [HarmonyPrepare]
            public static bool Prepare() => Config.Instance.succmop;
            public static void Postfix(Moppable __instance)
            {
                __instance.overrideAnims = null;
                __instance.faceTargetWhenWorking = true;
                __instance.multitoolContext = "fetchliquid";
                __instance.multitoolHitEffectTag = WaterSuckEffect.ID;
                __instance.SetOffsetTable(OffsetGroups.InvertedStandardTable);
            }
        }
        /// <summary>
        /// Teleports "mopped" liquids to the dupe
        /// </summary>
        [HarmonyPatch(typeof(Moppable), "OnCellMopped")]
        public class Moppable_OnCellMopped_Patch
        {
            [HarmonyPrepare]
            public static bool Prepare() => Config.Instance.succmop;


            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var index = codes.FindIndex(ci => ci.Calls(AccessTools.Method(typeof(TransformExtensions), nameof(TransformExtensions.SetPosition))));

                if (index == -1)
                {
                    SgtLogger.error("mop transpiler found no target!");
                    TranspilerHelper.PrintInstructions(codes);
                    return codes;
                }

                var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(Moppable_OnCellMopped_Patch), "InjectedMethod");

                // inject right after the found index
                codes.InsertRange(index, new[]
                {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Call, m_InjectedMethod)
                        });


                return codes;
            }

            private static Vector3 InjectedMethod(Vector3 toConsume, Moppable instance)
            {
                if (instance == null || instance.worker == null)
                    return toConsume;

                return instance.worker.transform.GetPosition();

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

        [HarmonyPatch(typeof(Assets))]
        [HarmonyPatch(nameof(Assets.GetAnim))]
        public static class TryGetRetroAnim_GetAnim
        {

            public static void Prefix(Assets __instance, ref HashedString name)
            {
                string retroStringVariant = name.ToString().Replace("_kanim", "_retro_kanim");
                if (name.IsValid && Assets.AnimTable.ContainsKey(retroStringVariant))
                {
                    name = retroStringVariant;
                }
            }
        }


        [HarmonyPatch(typeof(ComplexFabricatorSM.States))]
        [HarmonyPatch(nameof(ComplexFabricatorSM.States.InitializeStates))]
        public static class Add_Working_pst_complete_anim_ComplexFabricator
        {

            public static void Postfix(ComplexFabricatorSM.States __instance)
            {
                __instance.operating.working_pst.transitions.Clear();
                __instance.operating.working_pst.OnAnimQueueComplete(__instance.operating.working_pst_complete);
            }
        }

        //[HarmonyPatch(typeof(PoweredActiveController))]
        //[HarmonyPatch(nameof(PoweredActiveController.InitializeStates))]
        //public static class ExtendedPoweredActiveController
        //{
        //    static GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State poweredAnimComplete;
        //    public static void Postfix(PoweredActiveController __instance)
        //    {
        //        //for(int i = __instance.states.Count- 1; i >= 0;i--)
        //        //{
        //        //    var state = __instance.states[i];
        //        //    if (state.name.Contains("stressed"))
        //        //    {
        //        //        __instance.states.RemoveAt(i);
        //        //    }
        //        //}

        //        poweredAnimComplete = new GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State();


        //        poweredAnimComplete
        //            //.PlayAnim("stop")
        //            .OnAnimQueueComplete(__instance.on);

        //        __instance.working.pst.transitions.Clear();
        //        __instance.working.pst.OnAnimQueueComplete(poweredAnimComplete);
        //    }
        //}

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
        //[HarmonyPatch(typeof(RailGunPayloadOpenerConfig))]
        //[HarmonyPatch(nameof(RailGunPayloadOpenerConfig.ConfigureBuildingTemplate))]
        //public static class ManualRailgunOpener
        //{

        //    public static bool Prepare() => Config.Instance.manualRailgunPayloadOpener;
        //    public static void Postfix(GameObject go)
        //    {
        //        var manualOperatable = go.AddComponent<GenericWorkableComponent>();
        //        manualOperatable.overrideAnims = new KAnimFile[1]
        //        {
        //            Assets.GetAnim((HashedString) "retro_anim_interact_railgun_opener_kanim")
        //        };
        //        RailGunPayloadOpener railGunPayloadOpener = go.GetComponent<RailGunPayloadOpener>();

        //        manualOperatable.workOffset = new CellOffset(0, 0);
        //        manualOperatable.WorkTime = (11f);
        //        manualOperatable.workLayer = Grid.SceneLayer.BuildingUse;
        //        manualOperatable.IsWorkable = () =>
        //        {
        //            return railGunPayloadOpener.payloadStorage.Count > 0;
        //        };
        //    }
        //}



        [HarmonyPatch(typeof(AlgaeDistilleryConfig))]
        [HarmonyPatch(nameof(AlgaeDistilleryConfig.ConfigureBuildingTemplate))]
        public static class ManualAlgaeDestillery
        {

            public static bool Prepare() => Config.Instance.manualSlimemachine;
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
        [HarmonyPatch(typeof(Turbine), nameof(Turbine.ResolveStrings))]
        public static class TurbineStringFix
        {
            public static void Postfix(ref string __result, object data)
            {
                Turbine turbine = (Turbine)data;
                __result = __result.Replace("{Src_Element}", ElementLoader.FindElementByHash(turbine.srcElem).name);
                __result = __result.Replace("{Active_Temperature}", GameUtil.GetFormattedTemperature(turbine.minActiveTemperature));
            }
        }
        [HarmonyPatch(typeof(Turbine), nameof(Turbine.InitializeStatusItems))]
        public static class TurbineStringFix2
        {
            public static void Postfix()
            {
                SgtLogger.l("patching turbine status");
                Turbine.insufficientMassStatusItem.resolveTooltipCallback = delegate (string str, object data)
                {
                    Turbine turbine = (Turbine)data;
                    str = str.Replace("{Min_Mass}", GameUtil.GetFormattedMass(turbine.requiredMassFlowDifferential));
                    str = str.Replace("{Src_Element}", ElementLoader.FindElementByHash(turbine.srcElem).name);
                    return str;
                };
                Turbine.insufficientMassStatusItem.resolveStringCallback = delegate (string str, object data)
                {
                    Turbine turbine = (Turbine)data;
                    str = str.Replace("{Min_Mass}", GameUtil.GetFormattedMass(turbine.requiredMassFlowDifferential));
                    str = str.Replace("{Src_Element}", ElementLoader.FindElementByHash(turbine.srcElem).name);
                    return str;
                };
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
        [HarmonyPatch(typeof(OverlayScreen), "RegisterModes")]
        public static class OverlayScreen_RegisterModes_Patch
        {
            public static void Postfix(OverlayScreen __instance)
            {
                __instance.RegisterMode(new OverlayModes.Sound());
            }
        }
        [HarmonyPatch(typeof(NoisePolluter), nameof(NoisePolluter.OnActiveChanged))]
        public static class NoisePolluter_RegisterModes_Patch
        {
            public static bool Prefix(NoisePolluter __instance, object data)
            {
                bool _isActive = false;
                if (data == null)
                {
                    return false;
                }
                if (data is bool b)
                {
                    _isActive = b;
                }
                else if (data is Operational operational
                    )
                {
                    _isActive = operational.IsActive;
                }
                else return false;


                bool isActive = _isActive;
                __instance.SetActive(isActive);
                __instance.Refresh();
                return false;
            }
        }
        [HarmonyPatch(typeof(AudioEventManager), nameof(AudioEventManager.AddSplat))]
        public static class CrashDetect
        {
            public static void Prefix(AudioEventManager __instance)
            {
                if (__instance.spatialSplats.cells == null)
                {
                    SgtLogger.l("Resetting noise splat grid");

                    __instance.spatialSplats.Reset(Grid.WidthInCells, Grid.HeightInCells, 16, 16);
                }
            }
        }
        [HarmonyPatch(typeof(AudioEventManager), nameof(AudioEventManager.OnSpawn))]
        public static class CrashDetect2
        {
            public static bool Prefix()
            {
                return false;
            }
        }
        //[HarmonyPatch(typeof(OverlayModes.Oxygen), nameof(OverlayModes.Oxygen.GetOxygenMapColour))]
        //public static class OverlayModes_Color
        //{
        //    public static void Postfix(SimDebugView instance, int cell, ref Color __result)
        //    {
        //        if (__result == instance.unbreathableColour && Grid.Element[cell].toxicity > 0)
        //        {
        //            float t = Mathf.Clamp((Grid.Pressure[cell] - instance.minPressureExpected) / (instance.maxPressureExpected - (instance.minPressureExpected)), 0.0f, 1f);
        //            __result = Color.Lerp(instance.toxicColour[0], instance.toxicColour[1], t);
        //        }
        //    }
        //}
        //[HarmonyPatch(typeof(SimDebugView), nameof(SimDebugView.GetOxygenMapColour))]
        //public static class GetToxicityColor
        //{
        //    public static void Postfix(SimDebugView instance, int cell, ref Color __result)
        //    {
        //        if(__result == instance.unbreathableColour && Grid.Element[cell].toxicity>0)
        //        {
        //            float t = Mathf.Clamp((Grid.Pressure[cell] - instance.minPressureExpected) / (instance.maxPressureExpected - (instance.minPressureExpected)), 0.0f, 1f);
        //            __result = Color.Lerp(instance.toxicColour[0], instance.toxicColour[1], t);
        //        }
        //    }
        //}
        [HarmonyPatch(typeof(OverlayModes.Sound), nameof(OverlayModes.Sound.OnSaveLoadRootUnregistered))]
        public static class CrashDetect3
        {
            public static bool Prefix(SaveLoadRoot item)
            {
                return item != null && item.gameObject != null && item.TryGetComponent<NoisePolluter>(out _);
            }
        }
        /// <summary>
        /// Sound Overlay
        /// </summary>
        [HarmonyPatch(typeof(SimDebugView), "OnPrefabInit")]
        public static class SimDebugView_OnPrefabInit_Patch
        {
            public static void Postfix(Dictionary<HashedString, Func<SimDebugView, int, Color>> ___getColourFuncs)
            {
                ___getColourFuncs.Add(OverlayModes.Sound.ID, GetCellColor);
            }

            static Color bad = new Color(0.75f, 0, 0);
            static Color good = new Color(0, 0.0f, 0);

            private static Color GetCellColor(SimDebugView instance, int cell)
            {
                var db = AudioEventManager.Get().GetDecibelsAtCell(cell);
                return Color.Lerp(good, bad, Mathf.Clamp(db, 0, 200f) / 200f);
            }
        }


        [HarmonyPatch(typeof(SelectToolHoverTextCard), "UpdateHoverElements")]
        public static class SelectToolHoverTextCard_UpdateHoverElements_Patch
        {
            private static readonly FieldInfo InfoId = AccessTools.Field(typeof(OverlayModes.Sound), nameof(OverlayModes.Sound.ID));

            private static readonly FieldInfo LogicId = AccessTools.Field(
                typeof(OverlayModes.Logic),
                nameof(OverlayModes.Logic.ID)
            );

            private static readonly MethodInfo HashEq = AccessTools.Method(
                typeof(HashedString),
                "op_Equality",
                new[] { typeof(HashedString), typeof(HashedString) }
            );

            private static readonly MethodInfo Helper = AccessTools.Method(
                typeof(SelectToolHoverTextCard_UpdateHoverElements_Patch),
                nameof(DrawerHelper)
            );

            public static IEnumerable<CodeInstruction> Transpiler(
                IEnumerable<CodeInstruction> orig,
                ILGenerator generator
            )
            {
                List<CodeInstruction> list = orig.ToList<CodeInstruction>();
                int index1 = list.FindIndex((Predicate<CodeInstruction>)(ci =>
                {
                    FieldInfo operand = ci.operand as FieldInfo;
                    return (object)operand != null && operand == SelectToolHoverTextCard_UpdateHoverElements_Patch.LogicId;
                }));
                System.Reflection.Emit.Label label = generator.DefineLabel();
                list[index1 + 2].operand = (object)label;
                int index2 = list.FindIndex(index1, (Predicate<CodeInstruction>)(ci => ci.opcode == OpCodes.Endfinally)) + 1;
                System.Reflection.Emit.Label operand1 = generator.DefineLabel();
                list[index2].labels.Add(operand1);
                int index3 = index2;
                int num1 = index3 + 1;
                list.Insert(index3, new CodeInstruction(OpCodes.Ldloc_2)
                {
                    labels = {
            label
          }
                });
                int index4 = num1;
                int num2 = index4 + 1;
                list.Insert(index4, new CodeInstruction(OpCodes.Ldsfld, (object)SelectToolHoverTextCard_UpdateHoverElements_Patch.InfoId));
                int index5 = num2;
                int num3 = index5 + 1;
                list.Insert(index5, new CodeInstruction(OpCodes.Call, (object)SelectToolHoverTextCard_UpdateHoverElements_Patch.HashEq));
                int index6 = num3;
                int num4 = index6 + 1;
                list.Insert(index6, new CodeInstruction(OpCodes.Brfalse, (object)operand1));
                int index7 = num4;
                int num5 = index7 + 1;
                list.Insert(index7, new CodeInstruction(OpCodes.Ldarg_0));
                int index8 = num5;
                int num6 = index8 + 1;
                list.Insert(index8, new CodeInstruction(OpCodes.Ldloc_0));
                int index9 = num6;
                int num7 = index9 + 1;
                list.Insert(index9, new CodeInstruction(OpCodes.Ldloc_1));
                int index10 = num7;
                int num8 = index10 + 1;
                list.Insert(index10, new CodeInstruction(OpCodes.Call, (object)SelectToolHoverTextCard_UpdateHoverElements_Patch.Helper));
                int index11 = num8;
                int num9 = index11 + 1;
                list.Insert(index11, new CodeInstruction(OpCodes.Br, (object)operand1));
                return (IEnumerable<CodeInstruction>)list;

            }

            private static void DrawerHelper(SelectToolHoverTextCard inst, int cell, HoverTextDrawer drawer)
            {
                if(AudioEventManager.Get() == null) { return; }

                // Cell position info
                drawer.BeginShadowBar();
                var db = AudioEventManager.Get().GetDecibelsAtCell(cell);
                drawer.DrawText("NOISE", inst.Styles_Title.Standard);
                drawer.NewLine();
                drawer.DrawText($"Total noise Level: {db} dB", inst.Styles_BodyText.Standard);
                if (db > 0)
                {
                    drawer.NewLine();
                    drawer.NewLine();
                    drawer.DrawText($"Noise Sources:", inst.Styles_BodyText.Standard);
                    foreach (AudioEventManager.PolluterDisplay source in AudioEventManager.Get().GetPollutersForCell(cell))
                    {
                        drawer.NewLine();
                        drawer.DrawText($" - {source.name}: {source.value} dB.", inst.Styles_BodyText.Standard);
                    }
                }
                drawer.EndShadowBar();



            }
        }




        /// <summary>
        /// Applied to OverlayMenu to add a button for our overlay.
        /// </summary>
        [HarmonyPatch(typeof(OverlayMenu), "InitializeToggles")]
        public static class OverlayMenu_InitializeToggles_Patch
        {
            private const BindingFlags INSTANCE_ALL = PPatchTools.BASE_FLAGS | BindingFlags.
                Instance;
            private static readonly Type OVERLAY_TYPE = typeof(OverlayMenu).GetNestedType(
                "OverlayToggleInfo", INSTANCE_ALL);

            /// <summary>
            /// Applied after InitializeToggles runs.
            /// </summary>
            internal static void Postfix(ICollection<KIconToggleMenu.ToggleInfo> ___overlayToggleInfos)
            {
                var action =
                    //(OpenOverlay == null) ? 
                    PAction.MaxAction
                    //: OpenOverlay.GetKAction()
                    ;
                var info = CreateOverlayInfo("Sound Overlay", "overlay_sound", OverlayModes.Sound.ID, action,
                    global::STRINGS.UI.TOOLTIPS.NOISE_POLLUTION_OVERLAY_STRING);
                if (info != null)
                    ___overlayToggleInfos?.Add(info);
            }


            private static KIconToggleMenu.ToggleInfo CreateOverlayInfo(string text,
                    string icon_name, HashedString sim_view, Action openKey,
                    string tooltip)
            {
                const int KNOWN_PARAMS = 7;
                KIconToggleMenu.ToggleInfo info = null;
                ConstructorInfo[] cs;
                if (OVERLAY_TYPE == null || (cs = OVERLAY_TYPE.GetConstructors(INSTANCE_ALL)).
                        Length != 1)
                    PUtil.LogWarning("Unable to add TileOfInterest - missing constructor");
                else
                {
                    var cons = cs[0];
                    var toggleParams = cons.GetParameters();
                    int paramCount = toggleParams.Length;
                    // Manually plug in the knowns
                    if (paramCount < KNOWN_PARAMS)
                        PUtil.LogWarning("Unable to add TileOfInterest - parameters missing");
                    else
                    {
                        object[] args = new object[paramCount];
                        args[0] = text;
                        args[1] = icon_name;
                        args[2] = sim_view;
                        args[3] = "";
                        args[4] = openKey;
                        args[5] = tooltip;
                        args[6] = text;
                        // 3 and further (if existing) get new optional values
                        for (int i = KNOWN_PARAMS; i < paramCount; i++)
                        {
                            var op = toggleParams[i];
                            if (op.IsOptional)
                                args[i] = op.DefaultValue;
                            else
                            {
                                PUtil.LogWarning("Unable to add TileOfInterest - new parameters");
                                args[i] = null;
                            }
                        }
                        info = cons.Invoke(args) as KIconToggleMenu.ToggleInfo;
                    }
                }
                return info;
            }
        }

        [HarmonyPatch(typeof(SolidConduitFlowVisualizer))]
        [HarmonyPatch(typeof(SolidConduitFlowVisualizer), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(SolidConduitFlow), typeof(Game.ConduitVisInfo), typeof(FMODUnity.EventReference), typeof(SolidConduitFlowVisualizer.Tuning) })]
        public class AddRetroConveyorBox
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
        [HarmonyPatch(typeof(Assets), "LoadAnims")]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
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
                        SgtLogger.logError("Failed at importing sprite: " + File.FullName + ",\nError: " + e);
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
                SgtElementUtil.SetTexture_ShineMask(aluminium, "alumiminX_retro_ShineMask");

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
                else
                    SgtElementUtil.SetTexture_ShineMask(ironORe, "hematite_(t)_retro_ShineMask.png");


                var bleachstone = ElementLoader.GetElement(SimHashes.BleachStone.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(bleachstone, "bleach_stone_retro");

                var radEle = ElementLoader.GetElement(SimHashes.Radium.CreateTag()).substance.material;
                SgtElementUtil.SetTexture_Main(radEle, "radium_retro");
            }

        }

    }
}
