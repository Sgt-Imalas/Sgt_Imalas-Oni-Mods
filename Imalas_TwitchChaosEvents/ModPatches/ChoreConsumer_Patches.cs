using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.ModPatches
{
    class ChoreConsumer_Patches
    {

        [HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.IsPermittedByUser))]
        public class ChoreConsumer_IsPermittedByUser_Patch
        {
            public static void Postfix(ChoreConsumer __instance, ref bool __result)
            {
                if(__instance.TryGetComponent<KSelectable>(out var selectable) && selectable.HasStatusItem(ModAssets.StatusItems.WorkerOnStrike))
                {
					__result = false;
				}
            }
        }
    }
}
