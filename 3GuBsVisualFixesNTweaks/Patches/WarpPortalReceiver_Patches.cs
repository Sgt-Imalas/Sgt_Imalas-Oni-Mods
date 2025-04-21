using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class WarpPortalReceiver_Patches
    {

        [HarmonyPatch(typeof(WarpReceiver.WarpReceiverSM), nameof(WarpReceiver.WarpReceiverSM.InitializeStates))]
        public class WarpReceiver_WarpReceiverSM_InitializeStates_Patch
        {
            public static void Postfix(WarpReceiver.WarpReceiverSM __instance)

            {
                __instance.idle.PlayAnim("idle", KAnim.PlayMode.Loop);
			}
            
        }

        [HarmonyPatch(typeof(WarpReceiver), nameof(WarpReceiver.CompleteChore))]
        public class WarpReceiver_CompleteChore_Patch
        {
            public static void Postfix(WarpReceiver __instance)
            {
                __instance.GetComponent<KBatchedAnimController>().Play("idle", KAnim.PlayMode.Loop);
			}
        }
    }
}
