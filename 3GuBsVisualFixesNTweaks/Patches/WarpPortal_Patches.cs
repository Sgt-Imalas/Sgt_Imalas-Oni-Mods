using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class WarpPortal_Patches
    {

        [HarmonyPatch(typeof(WarpPortal), nameof(WarpPortal.OnSpawn))]
        public class WarpPortal_OnSpawn_Patch
        {
            public static void Postfix(WarpPortal __instance)
            {
                __instance.gameObject.AddOrGet<WarpPortalMeterController>();
			}
        }

        [HarmonyPatch(typeof(WarpPortal.WarpPortalSM), nameof(WarpPortal.WarpPortalSM.InitializeStates))]
        public class WarpPortal_WarpPortalSM_InitializeStates_Patch
        {
            public static void Postfix(WarpPortal.WarpPortalSM __instance)
            {
                __instance.recharging
                    .PlayAnim("off")
                    .QueueAnim("recharge", true);

			}
        }
    }
}
