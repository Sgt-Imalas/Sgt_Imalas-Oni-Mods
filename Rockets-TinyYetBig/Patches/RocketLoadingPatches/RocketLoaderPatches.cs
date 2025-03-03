using HarmonyLib;
using Rockets_TinyYetBig.RocketFueling;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.RocketLoadingPatches
{
    /// <summary>
    /// Replace Launchpad tag with universal RocketPlatform tag to allow connecting to other launchpads and station doors
    /// </summary>
    class RocketLoaderPatches
    {
        //        [HarmonyPatch(typeof(ChainedBuilding.StatesInstance))]
        //        [HarmonyPatch("CollectNeighbourToChain")]
        //        public static class HeadBuildingTagAdjustmentsInChainMethod
        //        {
        //            private static readonly MethodInfo TagCheckReplacer = AccessTools.Method(
        //               typeof(KPrefabID),
        //               nameof(KPrefabID.HasTag)
        //            );


        //            private static readonly MethodInfo MethodToReplace = AccessTools.Method(
        //                typeof(KPrefabID),
        //               nameof(KPrefabID.IsPrefabID)
        //            );

        //            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        //            {
        //                var code = instructions.ToList();
        //                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == MethodToReplace);

        //                if (insertionIndex != -1)
        //                {
        //#if DEBUG
        //                    SgtLogger.debuglog("Replacing Default Method in ChainedBuilding");
        //#endif
        //                    code[insertionIndex] = new CodeInstruction(OpCodes.Callvirt, TagCheckReplacer);
        //                }

        //                return code;
        //            }
        //        }

        [HarmonyPatch(typeof(ChainedBuilding.StatesInstance))]
        [HarmonyPatch(nameof(ChainedBuilding.StatesInstance.CollectNeighbourToChain))]
        public static class HeadBuildingTagAdjustmentsInChainMethod
        {
            public static bool Prefix(
                ChainedBuilding.StatesInstance __instance,
                int cell,
                ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain,
                ref bool foundHead,
                ChainedBuilding.StatesInstance ignoredLink = null)
            {
                GameObject go = Grid.Objects[cell, (int)__instance.def.objectLayer];
                if (go == null)
                    return false;

                go.TryGetComponent<KPrefabID>(out var component);


                if (__instance.def.headBuildingTag != ModAssets.Tags.RocketPlatformTag
                    || !component.HasTag(__instance.def.linkBuildingTag) && !component.HasTag(__instance.def.headBuildingTag)
                    || Grid.CellToXY(Grid.PosToCell(__instance)).Y != Grid.CellToXY(Grid.PosToCell(go)).Y && Grid.CellToXY(Grid.PosToCell(__instance)).X != Grid.CellToXY(Grid.PosToCell(go)).X
                    )
                    return false;

                go.GetSMI<ChainedBuilding.StatesInstance>()?.CollectToChain(ref chain, ref foundHead, ignoredLink);
                return false;
            }
        }

        [HarmonyPatch(typeof(ChainedBuilding.StatesInstance), nameof(ChainedBuilding.StatesInstance.StartSM))]
        public class ChainedBuilding_StatesInstance_StartSM_Patch //constructor not directly patchable (?)
        {
            public static void Prefix(ChainedBuilding.StatesInstance __instance)
            {
                if (__instance.gameObject.TryGetComponent<VerticalPortAttachment>(out var portAttachment))
                {
                    if (portAttachment.CrossPiece)
                    {
                        if (!__instance.neighbourCheckCells.Contains(portAttachment.TopCell))
                            __instance.neighbourCheckCells.Add(portAttachment.TopCell);
                        if (!__instance.neighbourCheckCells.Contains(portAttachment.BottomCell))
                            __instance.neighbourCheckCells.Add(portAttachment.BottomCell);
                        //SgtLogger.l($"added crosspiece vertical cells to chained building");
                        //foreach (var item in __instance.neighbourCheckCells)
                        //{
                        //	SgtLogger.l("Cell in list: " + item);
                        //}
                    }
                    else
                    {
                        __instance.neighbourCheckCells = [portAttachment.TopCell, portAttachment.BottomCell];
                        //SgtLogger.l($"replaced neigbor cells with vertical cells in chained building");
                        //foreach (var item in __instance.neighbourCheckCells)
                        //{
                        //	SgtLogger.l("Cell in list: " + item);
                        //}
                    }
                }
            }
        }


        //[HarmonyPatch(typeof(ChainedBuilding.StatesInstance))]
        //[HarmonyPatch(nameof(ChainedBuilding.StatesInstance.CollectToChain))]
        //public static class VerticalConnectionForRocketPorts
        //{
        //    public static void Postfix(
        //        ChainedBuilding.StatesInstance __instance,
        //        ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain,
        //        ref bool foundHead,
        //        ChainedBuilding.StatesInstance ignoredLink = null)
        //    {
        //        if ((ignoredLink != null && ignoredLink == __instance) || chain.Contains(__instance))
        //            return;


        //    }
        //}
        public static readonly HashedString ROCKETPORTLOADER_ACTIVE = "ROCKETPORTLOADER_ACTIVE";
        [HarmonyPatch]
        public static class LogicOutputLoaderBuildings_AddLogicPort
        {
            [HarmonyPrepare]
            static bool Prepare() => Config.Instance.EnableRocketLoaderLogicOutputs;

            [HarmonyPostfix]
            public static void Postfix(BuildingDef __result)
            {
                __result.LogicOutputPorts = new List<LogicPorts.Port>(){
                LogicPorts.Port.OutputPort(ROCKETPORTLOADER_ACTIVE, new CellOffset(0, 1),
                STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER,
                STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER_ACTIVE,
                STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER_INACTIVE)
                };
            }

            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                yield return typeof(BaseModularLaunchpadPortConfig).GetMethod(nameof(BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort));
            }
        }
        /// <summary>
        /// Add a 1 second delay to turnoff signal to prevent flickering
        /// </summary>
        static Dictionary<ModularConduitPortController.Instance, SchedulerHandle> ScheduledTurnOffs = new();
        static void OnSignalChanged(ModularConduitPortController.Instance instance, LogicPorts logicPorts, bool greenSignal)
        {
            if (greenSignal)
            {
                if (ScheduledTurnOffs.TryGetValue(instance, out var scheduledTurnoff))
                {
                    GameScheduler.Instance?.scheduler?.Clear(scheduledTurnoff);
                    ScheduledTurnOffs.Remove(instance);
                    logicPorts?.SendSignal(ROCKETPORTLOADER_ACTIVE, 1);
                }
            }
            else
            {
                if (!ScheduledTurnOffs.ContainsKey(instance))
                {
                    ScheduledTurnOffs[instance] = GameScheduler.Instance.Schedule("turn off loader logic port", 1, (_) =>
                    {
                        logicPorts?.SendSignal(ROCKETPORTLOADER_ACTIVE, 0);
                        ScheduledTurnOffs.Remove(instance);
                    });
                }
            }
        }

        [HarmonyPatch(typeof(ModularConduitPortController.Instance), nameof(ModularConduitPortController.Instance.SetLoading))]
        public static class LogicOutputLoaderBuildings_UpdateLogic_Loading
        {
            [HarmonyPrepare]
            static bool Prepare() => Config.Instance.EnableRocketLoaderLogicOutputs;

            public static void Postfix(ModularConduitPortController.Instance __instance, bool isLoading)
            {
                var logicPorts = __instance.GetComponent<LogicPorts>();
                OnSignalChanged(__instance, logicPorts, isLoading);
            }
        }
        [HarmonyPatch(typeof(ModularConduitPortController.Instance), nameof(ModularConduitPortController.Instance.SetUnloading))]
        public static class LogicOutputLoaderBuildings_UpdateLogic_Unloading
        {
            [HarmonyPrepare]
            static bool Prepare() => Config.Instance.EnableRocketLoaderLogicOutputs;

            public static void Postfix(ModularConduitPortController.Instance __instance, bool isUnloading)
            {
                var logicPorts = __instance.GetComponent<LogicPorts>();
                OnSignalChanged(__instance, logicPorts, isUnloading);
            }
        }


        [HarmonyPatch(typeof(LaunchPadConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class PatchDefaultTagIntoLaunchpad
        {
            public static void Postfix(GameObject go)
            {
                go.GetComponent<KPrefabID>().AddTag(ModAssets.Tags.RocketPlatformTag);

                ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
                def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            }
        }

        [HarmonyPatch(typeof(ModularLaunchpadPortBridgeConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class PatchDefaultTagIntoVanillaAdapter
        {
            public static void Postfix(GameObject go)
            {
                ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
                def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            }
        }

        [HarmonyPatch(typeof(BaseModularLaunchpadPortConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class PatchDefaultTagIntoLoaders
        {
            public static void Postfix(GameObject go)
            {
                ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
                def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;


                DropAllWorkable dropAllWorkable = go.AddOrGet<DropAllWorkable>();
                dropAllWorkable.dropWorkTime = 15f;
            }
        }
        //[HarmonyPatch(typeof(BaseModularLaunchpadPortConfig))]
        //[HarmonyPatch("DoPostConfigureComplete")]
        //public static class AddDropAllComponent
        //{
        //    [HarmonyPriority(Priority.LowerThanNormal)]
        //    public static void Postfix(GameObject go)
        //    {
        //    }
        //}

    }
}
