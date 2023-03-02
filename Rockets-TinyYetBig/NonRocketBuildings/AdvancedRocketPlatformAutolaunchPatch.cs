using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Rockets_TinyYetBig.OldPatches;
using UtilLibs;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    [HarmonyPatch(typeof(CraftModuleInterface))]
    [HarmonyPatch(nameof(CraftModuleInterface.EvaluateConditionSet))]
    internal class AdvancedRocketPlatformAutolaunchPatch
    {
        public static void Postfix(CraftModuleInterface __instance, ProcessCondition.ProcessConditionType conditionType, ref ProcessCondition.Status __result)
        {
            var current = __instance.CurrentPad;
            if(current != null && current.TryGetComponent<AdvancedRocketStatusProvider>(out AdvancedRocketStatusProvider evaluator))
            {
                evaluator.ConvertWarnings(conditionType,ref __result);
                SgtLogger.l(conditionType.ToString() + ": " + __result.ToString(), "FINAL");
            }
        }
    }
    [HarmonyPatch(typeof(LaunchPad))]
    [HarmonyPatch(nameof(LaunchPad.Sim1000ms))]
    internal class AdjustForRibbonInput
    {
        public static int ConvertInputValue(int original)
        {
            //SgtLogger.l(original.ToString(),"INPUT VAL");
            //SgtLogger.l((original%2).ToString(),"OUTPUT VAL");
            return original % 2;
        }

        private static readonly MethodInfo ConverterMethod = AccessTools.Method(
           typeof(AdjustForRibbonInput),
           nameof(ConvertInputValue)
        );


        private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                typeof(LogicPorts),
                nameof(LogicPorts.GetInputValue)
           );

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


            //InjectionMethods.PrintInstructions(code);
            if (insertionIndex != -1)
            {
                //SgtLogger.debuglog("FOUNDDDDDDDDDDD");
                code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ConverterMethod));
            }

            return code;
        }
    }
}
