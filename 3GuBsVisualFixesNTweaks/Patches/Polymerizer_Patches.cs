using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class Polymerizer_Patches
    {

        [HarmonyPatch(typeof(Polymerizer), nameof(Polymerizer.OnStorageChanged))]
        public class Polymerizer_OnStorageChanged_Patch
        {
            public static void Postfix(Polymerizer __instance)
			{
				__instance.UpdateOilMeter();
			}
        }
    }
}
