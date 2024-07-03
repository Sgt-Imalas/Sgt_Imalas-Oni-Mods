using HarmonyLib;
using OniRetroEdition.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
    internal class RetroMaskPatch
    {

        [HarmonyPatch(typeof(MaskStation.OxygenMaskReactable), "Run")]
        public class MaskStation_OxygenMaskReactable_Run_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var index = codes.FindIndex(ci => ci.opcode==OpCodes.Ldstr && ci.operand is string loaded && loaded == "Oxygen_Mask");

                if (index == -1)
                {
                    SgtLogger.error("RETRO MASK TRANSPILER FAILED");
                    return codes;
                }
                codes[index].operand = OxygenMaskRetroConfig.ID;

                return codes;
            }
        }
    }
}
