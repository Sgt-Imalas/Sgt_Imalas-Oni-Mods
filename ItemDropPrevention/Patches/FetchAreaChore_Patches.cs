using HarmonyLib;
using ItemDropPrevention.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ItemDropPrevention.Patches
{
	internal class FetchAreaChore_Patches
	{

        [HarmonyPatch(typeof(FetchAreaChore.StatesInstance), nameof(FetchAreaChore.StatesInstance.SetupDeliverables))]
        public class FetchAreaChore_StatesInstance_SetupDeliverables_Patch
		{
            public static void Postfix(FetchAreaChore.StatesInstance __instance)
            {
                if(!DroppablesHolder.TryGet(__instance.sm.fetcher.Get(__instance),out var droppablesHolder))
                    return;

                __instance.fetchables.RemoveAll(item => droppablesHolder.IsItemMarkedForDrop(item.gameObject));
            }
        }
	}
}
