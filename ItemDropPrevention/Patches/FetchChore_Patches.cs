using HarmonyLib;
using ItemDropPrevention.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ItemDropPrevention.Patches
{
	internal class FetchChore_Patches
	{

		[HarmonyPatch(typeof(FetchChore), nameof(FetchChore.FindFetchTarget), [typeof(ChoreConsumerState)])]
		public class FetchChore_FindFetchTarget_Patch
		{
			public static bool Prefix(FetchChore __instance, ChoreConsumerState consumer_state, ref Pickupable __result)
			{
				if (consumer_state.hasSolidTransferArm || __instance.destination == null || !DroppablesHolder.TryGet(consumer_state.consumer.gameObject, out var holder))
					return true;

				__result = holder.FindFetchTargetFromDroppables(__instance, consumer_state);
				return __result == null;
			}
		}
	}
}
