using HarmonyLib;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class Pump_Patches
	{

        [HarmonyPatch(typeof(Pump), nameof(Pump.OnSpawn))]
        public class Pump_OnSpawn_Patch
        {
            public static void Postfix(Pump __instance)
			{
				__instance.controller.SetSymbolVisiblity("leak_ceiling_bloom", false);
			}
        }
	}
}
