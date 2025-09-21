using HarmonyLib;
using ItemDropPrevention.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemDropPrevention.Patches
{
	internal class Storage_Patches
	{

        [HarmonyPatch(typeof(Storage), nameof(Storage.Deserialize))]
        public class Storage_Deserialize_Patch
		{
            public static void Prefix(Storage __instance, ref DroppablesHolder __state)
            {
                if(__instance.dropOnLoad && __instance.TryGetComponent<DroppablesHolder>(out __state))
                {
                    __instance.dropOnLoad = false;
				}
            }
            public static void Postfix(Storage __instance, DroppablesHolder __state)
            {
                if (__state == null)
                    return;

                __state.MarkAllItemsForDrop();
                __instance.dropOnLoad = true;
            }
        }
	}
}
