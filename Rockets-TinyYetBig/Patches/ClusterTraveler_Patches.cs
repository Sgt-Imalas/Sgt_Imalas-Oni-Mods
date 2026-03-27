using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class ClusterTraveler_Patches
	{
		/// <summary>
		/// Correct travel time estimates for Spacestation world targets
		/// </summary>
        [HarmonyPatch(typeof(ClusterTraveler), nameof(ClusterTraveler.RemainingTravelDistance))]
        public class ClusterTraveler_RemainingTravelDistance_Patch
        {
            public static void Postfix(ClusterTraveler __instance, ref float __result)
			{
				int destinationWorldId = __instance.GetDestinationWorldID();
				if (destinationWorldId >= 0 && SpaceStationManager.WorldIsSpaceStationInterior(destinationWorldId))
				{
					__result = __instance.RemainingTravelNodes() * 600f - __instance.m_movePotential;
				}
			}
        }
	}
}
