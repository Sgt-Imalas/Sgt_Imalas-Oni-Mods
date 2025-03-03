using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.RocketPlatformPatches
{
    internal class Launchpad_Patches
    {
        /// <summary>
        /// Allow evaluation of the ribbon input of the advanced rocket platform
        /// </summary>
        [HarmonyPatch(typeof(LaunchPad), nameof(LaunchPad.Sim1000ms))]
        public class AdjustForRibbonInput
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
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ConverterMethod));
                }
                else
                    Debug.LogError("Rocketry Expanded: Launchpad transpiler failed");

                return code;
            }
        }
    }
}
