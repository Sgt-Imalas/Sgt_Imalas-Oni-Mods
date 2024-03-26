using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations.Construction.ModuleBuildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI;

namespace Rockets_TinyYetBig.RocketFueling
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


                if ((!component.HasTag(__instance.def.linkBuildingTag) && !component.HasTag(__instance.def.headBuildingTag))
                    || Grid.CellToXY(Grid.PosToCell(__instance)).y != Grid.CellToXY(Grid.PosToCell(go)).y
                    )
                    return false;

                if (__instance.gameObject != null && __instance.gameObject.TryGetComponent<VerticalPortAttachment>(out var verticalPortAttachment))
                {
                    verticalPortAttachment.PropagateCollectionEvents(ref chain, ref foundHead, ignoredLink);
                    //foreach (var part in VerticalPortAttachment.GetNetwork(verticalPortAttachment))
                    //{
                    //    if (part.chainedBuilding != null
                    //        && part.TryGetComponent<KPrefabID>(out var linkedPrefabId)
                    //        && (linkedPrefabId.HasTag(__instance.def.linkBuildingTag) || linkedPrefabId.HasTag(__instance.def.headBuildingTag))
                    //        && !chain.Contains(part.chainedBuilding)
                    //        )
                    //    {
                    //        SgtLogger.l("colleccting....");
                    //        part.chainedBuilding?.CollectToChain(ref chain, ref foundHead, ignoredLink);
                    //    }
                    //}
                }


                go.GetSMI<ChainedBuilding.StatesInstance>()?.CollectToChain(ref chain, ref foundHead, ignoredLink);
                return false;
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
        [HarmonyPatch(typeof(ModularConduitPortController.Instance), nameof(ModularConduitPortController.Instance.SetLoading))]
        public static class LogicOutputLoaderBuildings_UpdateLogic_Loading
        {
            [HarmonyPrepare]
            static bool Prepare() => Config.Instance.EnableRocketLoaderLogicOutputs;
            
            public static void Postfix(ModularConduitPortController.Instance __instance, bool isLoading)
            {
               var logicPorts = __instance.GetComponent<LogicPorts>();

                logicPorts.SendSignal(ROCKETPORTLOADER_ACTIVE, isLoading ? 1 : 0);
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

                logicPorts.SendSignal(ROCKETPORTLOADER_ACTIVE, isUnloading ? 1 : 0);
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
                //go.GetComponent<KPrefabID>().AddTag(ModAssets.Tags.RocketPlatformTag);

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
