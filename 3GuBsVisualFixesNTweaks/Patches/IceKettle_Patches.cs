using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class IceKettle_Patches
    {

        [HarmonyPatch(typeof(IceKettle.Instance), nameof(IceKettle.Instance.StartSM))]
        public class IceKettle_Instance_StartSM_Patch
        {
            public static void Postfix(IceKettle.Instance __instance)
            {
                __instance.master.gameObject.AddOrGet<IceKettle_FuelMeterController>();
            }
        }
    }
}
