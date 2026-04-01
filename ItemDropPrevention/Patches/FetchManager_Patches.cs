using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemDropPrevention.Patches
{
	internal class FetchManager_Patches
	{

        [HarmonyPatch(typeof(FetchManager), nameof(FetchManager.IsFetchablePickup))]
        public class FetchManager_IsFetchablePickup_Patch
        {
            public static bool Prefix(Pickupable pickup, ref bool __result)
            {
                if(pickup != null && pickup.KPrefabID.HasTag(ModAssets.BlockedFromDoingStuff))
                {
                    __result = false;
                    return false;
				}
                return true;
			}
        }
	}
}
