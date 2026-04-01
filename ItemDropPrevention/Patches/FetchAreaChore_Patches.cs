using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemDropPrevention.Patches
{
	internal class FetchAreaChore_Patches
	{

        [HarmonyPatch(typeof(FetchAreaChore.StatesInstance), nameof(FetchAreaChore.StatesInstance.SetupDeliverables))]
        public class FetchAreaChore_StatesInstance_SetupDeliverables_Patch
        {
            public static void Postfix(FetchAreaChore.StatesInstance __instance)
            {
                for(int i = __instance.deliverables.Count - 1; i >= 0; i--)
                {
                    var deliverable = __instance.deliverables[i];
					if (deliverable != null && deliverable.KPrefabID.HasTag(ModAssets.BlockedFromDoingStuff))
                    {
                        __instance.deliverables.RemoveAt(i);
                    }
				}
			}
        }

        [HarmonyPatch(typeof(FetchAreaChore.StatesInstance), nameof(FetchAreaChore.StatesInstance.ReservePickupables))]
        public class FetchAreaChore_StatesInstance_ReservePickupables_Patch
        {
            public static void Prefix(FetchAreaChore.StatesInstance __instance)
            {
				for (int i = __instance.fetchables.Count - 1; i >= 0; i--)
				{
					var fetchable = __instance.fetchables[i];
					if (fetchable != null && fetchable.KPrefabID.HasTag(ModAssets.BlockedFromDoingStuff))
					{
						__instance.fetchables.RemoveAt(i);
					}
				}
			}
        }
	}
}
