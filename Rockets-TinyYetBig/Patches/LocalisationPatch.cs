using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig
{
    class LocalisationPatch
    {
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        //[HarmonyPatch(typeof(Localization))]
        //[HarmonyPatch(nameof(Localization.WriteStringsTemplate))]
        //internal class FIX_KLEI_TEMPLATE_BUG
        //{

        //    private static readonly MethodInfo ConverterMethod = AccessTools.Method(
        //       typeof(UnityEngine.Debug),
        //       nameof(UnityEngine.Debug.LogWarning),
        //       new[] {typeof(object)}
        //    );


        //    private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
        //            typeof(UnityEngine.Debug),
        //            nameof(UnityEngine.Debug.LogError),
        //            new[] { typeof(object) }
        //       );

        //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        //    {
        //        var code = instructions.ToList();
        //        var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


        //        if (insertionIndex != -1)
        //        {
        //            //Debug.LogWarning("FOOOOOUUUUUUUNNNNNNN;");
        //            code[insertionIndex] = new CodeInstruction(OpCodes.Call, ConverterMethod);
        //        }

        //        return code;
        //    }
        //}
    }
}
