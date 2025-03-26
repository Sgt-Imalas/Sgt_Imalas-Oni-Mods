using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class VentController_Patches
    {
        [HarmonyPatch(typeof(LiquidVentConfig), nameof(LiquidVentConfig.DoPostConfigureComplete))]
        public class LiquidVentConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<VentTintable>();
            }
        }
    }
}
