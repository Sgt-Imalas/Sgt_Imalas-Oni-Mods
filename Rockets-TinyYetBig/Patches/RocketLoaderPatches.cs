using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class RocketLoaderPatches
    {
        [HarmonyPatch(typeof(ChainedBuilding.StatesInstance))]
        [HarmonyPatch("CollectNeighbourToChain")]
        public static class HeadBuildingTagAdjustmentsInChainMethod
        {
            private static readonly MethodInfo TagCheckReplacer = AccessTools.Method(
               typeof(KPrefabID),
               nameof(KPrefabID.HasTag)
            );


            private static readonly MethodInfo MethodToReplace = AccessTools.Method(
                typeof(KPrefabID),
               nameof(KPrefabID.IsPrefabID)
            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == MethodToReplace);

                if (insertionIndex != -1)
                {
#if DEBUG
                    SgtLogger.debuglog("Replacing Default Method in ChainedBuilding");
#endif
                    code[insertionIndex] = new CodeInstruction(OpCodes.Callvirt, TagCheckReplacer);
                }

                return code;
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

        [HarmonyPatch(typeof(BaseModularLaunchpadPortConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class PatchDefaultTagIntoLoaders
        {
            public static void Postfix(GameObject go)
            {
                ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
                def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            }
        }

    }
}
