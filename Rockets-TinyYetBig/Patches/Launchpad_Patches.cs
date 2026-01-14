using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.Content.Scripts.Buildings;

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

        [HarmonyPatch(typeof(LaunchPad.LaunchPadTower), nameof(LaunchPad.LaunchPadTower.AddTowerRow))]
        public class LaunchPad_TargetMethod_Patch
        {
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var index = codes.FindIndex(ci => ci.LoadsConstant("rocket_launchpad_tower_kanim"));

                if (index == -1)
                {
                    SgtLogger.error("LAUNCHPAD TRANSPILER FAILED!!");
                    return codes;
                }

                var m_ReplaceTowerAnim = AccessTools.DeclaredMethod(typeof(LaunchPad_TargetMethod_Patch), "ReplaceTowerAnim");

                // inject right after the found index
                codes.InsertRange(index + 1, new[]
                {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Call, m_ReplaceTowerAnim)
                        });

				return codes;
            }

            private static string ReplaceTowerAnim(string animName,LaunchPad.LaunchPadTower instance)
            {
                if (instance.pad.TryGetComponent<AdvancedRocketStatusProvider>(out _))
                    return "rocket_advanced_launchpad_tower_kanim";
				if (instance.pad.TryGetComponent<BunkerLaunchpadDigger>(out _))
					return "rocket_bunker_launchpad_tower_kanim";
				return animName;


			}
        }
    }
}
