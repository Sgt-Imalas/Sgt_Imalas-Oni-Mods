using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class ClusterDestinationSelector_Patches
	{

        [HarmonyPatch(typeof(ClusterDestinationSelector), nameof(ClusterDestinationSelector.HasAsteroidDestination))]
        public class ClusterDestinationSelector_HasAsteroidDestination_Patch
        {
            public static void Postfix(ClusterDestinationSelector __instance, ref bool __result)
            {
                if (!__result)
                    return;
                if (SpaceStationManager.GetSpaceStationAtLocation(__instance.m_destination, out _))
                    __result = false;
            }
        }
	}
}
