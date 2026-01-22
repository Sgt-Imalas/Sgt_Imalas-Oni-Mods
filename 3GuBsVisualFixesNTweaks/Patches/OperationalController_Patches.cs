using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class OperationalController_Patches
	{

        [HarmonyPatch(typeof(OperationalController), nameof(OperationalController.InitializeStates))]
        public class OperationalController_InitializeStates_Patch
        {
            /// <summary>
            /// Remove the "go to off" transition from root, so the working_pst state is not skipped on operational change
            /// </summary>
            /// <param name="__instance"></param>
            public static void Postfix(OperationalController __instance)
            {
                __instance.root.ClearTransitions();
            }
        }
	}
}
