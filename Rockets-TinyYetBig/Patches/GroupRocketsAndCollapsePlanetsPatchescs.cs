using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class GroupRocketsAndCollapsePlanetsPatchescs
    {
        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch("SortRows")]
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
                   // Debug.Log("Replacing Default Method in ChainedBuilding");
#endif
                    //code[insertionIndex] = new CodeInstruction(OpCodes.Callvirt, TagCheckReplacer);
                }

                return code;
            }
        }
        [HarmonyPatch(typeof(WorldSelector))]
        [HarmonyPatch("OnSpawn")]
        public static class GibStructure
        { 
            public static void Postfix(WorldSelector __instance)
            {
                Debug.Log("WorldSelector Start");
                UIUtils.ListAllChildren(__instance.transform);
                Debug.Log("WorldSelector End");
            }
        }
    }
}
