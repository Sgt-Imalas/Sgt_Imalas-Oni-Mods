using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class SolidConduitDispenser_Patches
    {

        [HarmonyPatch(typeof(SolidConduitDispenser), nameof(SolidConduitDispenser.ConduitUpdate))]
        public class SolidConduitDispenser_ConduitUpdate_Patch
		{
            public static ConfigurableSolidConduitDispenser dispenserInstance;
            public static void Prefix(SolidConduitDispenser __instance)
            {
                if (__instance is ConfigurableSolidConduitDispenser dispenser)
                {
					dispenserInstance = dispenser;
                }
                else
                    dispenserInstance = null;
			}

            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();
				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(SolidConduitDispenser_ConduitUpdate_Patch), "ReplaceCapacityConditionally");

				for (int i = codes.Count - 1; i >= 0; i--)
				{
					var current = codes[i];
                    double nr = 20;

					if (current.LoadsConstant(nr))
                    {
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, m_InjectedMethod));
					}
				}
                TranspilerHelper.PrintInstructions(codes);
				return codes;
            }

            private static double ReplaceCapacityConditionally(double input)
            {
                if(dispenserInstance != null)
				{
					return dispenserInstance.massDispensed;
				}
				return input;
			}
        }
    }
}
