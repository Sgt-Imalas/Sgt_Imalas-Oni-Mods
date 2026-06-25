using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace AquaticMinnowMinion.Patches
{
	internal class MinnowImperativePOIStates_Patches
	{

        [HarmonyPatch(typeof(MinnowImperativePOIStates.Instance), nameof(MinnowImperativePOIStates.Instance.SpawnMinnow))]
        public class MinnowImperativePOIStates_Instance_SpawnMinnow_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                foreach (var ci in orig)
                {
                    if (ci.LoadsConstant("MINNOW"))
                        ci.operand = Aq_Personalities.AQUATIC_MINNOW;
                    yield return ci;
                }
            }
        }
	}
}
