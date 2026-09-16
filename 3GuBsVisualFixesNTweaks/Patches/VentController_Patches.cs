using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
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
