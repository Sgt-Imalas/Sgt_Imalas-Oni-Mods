using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ElementUtilNamespace;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.RocketPlatformPatches
{
    internal class CraftModuleInterface_Patches
    {
        /// <summary>
        /// Allow evaluation of the ribbon input of the advanced rocket platform
        /// </summary>
        [HarmonyPatch(typeof(CraftModuleInterface), nameof(CraftModuleInterface.EvaluateConditionSet))]
        public class AdvancedRocketPlatformAutolaunchPatch
        {
            public static ProcessCondition.Status ConvertWarningIfNeeded(ProcessCondition condition, CraftModuleInterface craftModuleInterface, ProcessCondition.ProcessConditionType conditionType)
            {

                var current = craftModuleInterface.CurrentPad;
                if (current != null && current.TryGetComponent(out AdvancedRocketStatusProvider evaluator))
                {
                    ///Only convert if it should launch or bit 3 is set (aka apply condition overrides permanently
                    if (evaluator.GetBitMaskValAtIndex(0) || evaluator.GetBitMaskValAtIndex(3))
                    {
                        if (evaluator.GetBitMaskValAtIndex(1))
                        {
                            if (condition.GetType() == typeof(ConditionProperlyFueled) && condition.EvaluateCondition() != ProcessCondition.Status.Failure
                            || condition.GetType() == typeof(ConditionSufficientOxidizer) && condition.EvaluateCondition() != ProcessCondition.Status.Failure)
                                return ProcessCondition.Status.Ready;
                        }
                        if (evaluator.GetBitMaskValAtIndex(2)
                            && (conditionType == ProcessCondition.ProcessConditionType.RocketStorage //regular storage conditions
                            || condition.GetType() == typeof(ConditionHasRadbolts)) && condition.EvaluateCondition() != ProcessCondition.Status.Failure) //radbolt storage
                        {
                            return ProcessCondition.Status.Ready;
                        }
                    }
                }
                return condition.EvaluateCondition();
            }

            private static readonly MethodInfo ConverterMethod = AccessTools.Method(
              typeof(AdvancedRocketPlatformAutolaunchPatch),
              nameof(ConvertWarningIfNeeded)
           );


            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(ProcessCondition),
                    nameof(ProcessCondition.EvaluateCondition)
               );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


                //InjectionMethods.PrintInstructions(code);
                if (insertionIndex != -1)
                {
                    //int evaluatedConditionIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndex, false);
                    code[insertionIndex] = new CodeInstruction(OpCodes.Call, ConverterMethod);
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_1));
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_0));
                    //code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S,evaluatedConditionIndex));
                    //code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_0));                    
                }
                else SgtLogger.error("TRANSPILER FAILED: AdvancedRocketPlatformAutolaunchPatch");

                return code;
            }

        }
    }
}
