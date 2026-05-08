using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class StarmapHexCellInventory_Patches
	{

        [HarmonyPatch(typeof(StarmapHexCellInventory), nameof(StarmapHexCellInventory.ExtractAndSpawnItemMass))]
        public class StarmapHexCellInventory_ExtractAndSpawnItemMass_Patch
        {
            public static void Postfix(Tag ID, float mass) => DiscoverItemIfNotFound(ID, mass);
		}

        [HarmonyPatch(typeof(StarmapHexCellInventory), nameof(StarmapHexCellInventory.ExtractAndStoreItemMass))]
        public class StarmapHexCellInventory_ExtractAndStoreItemMass_Patch
        {
            public static void Postfix(Tag ID, float mass) => DiscoverItemIfNotFound(ID, mass);

		}
        static void DiscoverItemIfNotFound(Tag ID, float mass)
        {
            if (mass <= 0 || DiscoveredResources.Instance.IsDiscovered(ID))
				return;
			DiscoveredResources.Instance.Discover(ID);
		} 
	}
}
