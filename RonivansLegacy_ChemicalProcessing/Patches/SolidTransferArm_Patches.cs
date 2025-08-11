using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SolidTransferArm_Patches
	{

        [HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.Sim1000ms))]
        public class SolidTransferArm_Sim1000ms_Patch
        {
            public static void Postfix(SolidTransferArm __instance)
            {
                if (__instance.rotation_complete && __instance.rotateSoundPlaying)
                    __instance.StopRotateSound();
            }
        }
	}
}
