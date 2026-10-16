using HarmonyLib;

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
