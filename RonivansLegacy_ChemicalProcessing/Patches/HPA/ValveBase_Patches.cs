using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
    class ValveBase_Patches
    {

        [HarmonyPatch(typeof(ValveBase), nameof(ValveBase.ConduitUpdate))]
        public class ValveBase_ConduitUpdate_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var conduitFlow_GetContents = AccessTools.Method(typeof(ConduitFlow), nameof(ConduitFlow.GetContents));
				MethodInfo overPressurePatch = AccessTools.Method(typeof(ValveBase_ConduitUpdate_Patch), nameof(OperationalValveOverPressure));
				foreach (var code in orig) 
                {
                    if (code.Calls(conduitFlow_GetContents))
					{
						//Integrate patch when the following line is called:
						//  ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager)
						//Utilize the contents value to determine if overpressure damage is necessary
						yield return code; //ConduitFlow.ConduitContents contents
						yield return new CodeInstruction(OpCodes.Ldarg_0); //this (ValveBase)
						yield return new CodeInstruction(OpCodes.Ldloc_0); //ConduitFlow flowManager
						yield return new CodeInstruction(OpCodes.Call, overPressurePatch);
					}
					else
						yield return code;
				}
			}
			private static ConduitFlow.ConduitContents OperationalValveOverPressure(ConduitFlow.ConduitContents contents, ValveBase valveBase, ConduitFlow flowManager)
			{
				if (valveBase is OperationalValve op)
				{
					if (op.CurrentFlow > 0f)
					{
						float receiverMax = HighPressureConduitComponent.GetMaxCapacityAt(valveBase.outputCell, valveBase.conduitType, out var receiver);
						float inputMass = contents.mass;
						//If there is greater than 200% of the outputs capacity inside the shutoff valves input pipe, deal overpressure damage 33% of the time.
						HighPressureConduitComponent.PressureDamageHandling(receiver, inputMass, receiverMax);
					}
				}
				//since this patch consumed the contents variable on the stack, return the contents back to prevent issues with the next code statement in IL
				return contents;
			}
		}
    }
}
