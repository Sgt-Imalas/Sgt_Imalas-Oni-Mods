using HarmonyLib;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.ClustercraftDockingPatches
{
	internal class RocketClusterDestinationSelector_Patches
	{
		[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.OnClusterLocationChanged))]
		public static class AutoDockToSpaceStation
		{
			public static bool Prefix(RocketClusterDestinationSelector __instance, object data)
			{
				if (__instance.Repeat && __instance.TryGetComponent<DockingSpacecraftHandler>(out var mng))
				{
					if (mng.clustercraft.status != Clustercraft.CraftStatus.InFlight)
					{
						mng.UndockAll();
						return true;
					}

					ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent)data;
					if (locationChangedEvent.newLocation != __instance.m_destination)
						return true;


					//Skips immediate RoundTrip-return if there is a station at the target location
					if (SpaceStationManager.GetSpaceStationAtLocation(locationChangedEvent.newLocation, out var station))
					{
						mng.clustercraft.ModuleInterface.TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationReached, null);
						return false;
					}
				}
				return true;
			}
		}
	}
}
