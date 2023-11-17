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
        [HarmonyPatch("CollectNeighbourToChain")]
        public static class HeadBuildingTagAdjustmentsInChainMethod_VerticalityAdditions
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



                if (
                    (!component.HasTag(__instance.def.linkBuildingTag)&& !component.HasTag(__instance.def.headBuildingTag))
                    || Grid.CellToXY(Grid.PosToCell(__instance)).y != Grid.CellToXY(Grid.PosToCell(go)).y
                    )
                    return false;

                //SgtLogger.l($"{Grid.CellToXY(Grid.PosToCell(__instance)).y} == {Grid.CellToXY(Grid.PosToCell(go)).y}");

                if (__instance.gameObject != null && __instance.gameObject.TryGetComponent<AttachableBuilding>(out var attachPoint))
                {
                    foreach (var part in AttachableBuilding.GetAttachedNetwork(attachPoint))
                    {
                        if (part.GetSMI<ChainedBuilding.StatesInstance>() != null && part.TryGetComponent<KPrefabID>(out var linkedPrefabId)
                            && (linkedPrefabId.HasTag(__instance.def.linkBuildingTag) || linkedPrefabId.HasTag(__instance.def.headBuildingTag))
                            )
                        {
                            part.GetSMI<ChainedBuilding.StatesInstance>()?.CollectToChain(ref chain, ref foundHead, ignoredLink);
                        }

                    }
                }

                go.GetSMI<ChainedBuilding.StatesInstance>()?.CollectToChain(ref chain, ref foundHead, ignoredLink);


                return false;
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
                go.GetComponent<KPrefabID>().AddTag(ModAssets.Tags.RocketPlatformTag);

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
