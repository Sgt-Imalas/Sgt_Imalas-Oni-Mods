using HarmonyLib;
using Rockets_TinyYetBig.Derelicts;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Clustercraft;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.ClustercraftDockingPatches
{
	internal class Clustercraft_Docking_Patches
	{

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.OnClusterDestinationChanged))]
		public static class UndockFromStationsOnFlight_PullDockedRockets
		{
			public static void Postfix(Clustercraft __instance)
			{
				if (RocketryUtils.IsRocketTraveling(__instance) && !(__instance is SpaceStation))
				{
					if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
					{
						var myDestination = manager.clustercraft.ModuleInterface.GetClusterDestinationSelector().GetDestination();

						foreach (var docked in manager.WorldDockables)
						{
							if (!DockingManagerSingleton.Instance.TryGetDockableIfDocked(docked.Value.GUID, out var dockedDockable))
								continue;

							if (SpaceStationManager.WorldIsSpaceStationInterior(dockedDockable.WorldId))
							{
								DockingManagerSingleton.Instance.AddPendingUndock(docked.Value.GUID, dockedDockable.GUID);
							}
							else
							{
								var selector = dockedDockable.spacecraftHandler.clustercraft.ModuleInterface.GetClusterDestinationSelector();
								if (selector.GetDestination() != myDestination)
								{
									selector.SetDestination(myDestination);
								}
							}
						}
					}
				}
			}
		}
		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.OnClusterDestinationReached))]
		public static class AutoDockToStation
		{
			public static void Postfix(Clustercraft __instance)
			{
				if (__instance is SpaceStation || __instance is DerelictStation)
					return;

				var clusterDestinationSelector = __instance.m_moduleInterface.GetClusterDestinationSelector();

				if (__instance.status == CraftStatus.InFlight //In space on a station hex
					&& __instance.Location == clusterDestinationSelector.GetDestination()
					&& __instance.TryGetComponent<DockingSpacecraftHandler>(out var handler)
					&& SpaceStationManager.GetSpaceStationAtLocation(__instance.Location, out var TargetStation)
					&& TargetStation.TryGetComponent<DockingSpacecraftHandler>(out var stationHandler)
					&& !DockingManagerSingleton.Instance.HandlersConnected(handler, stationHandler, out _, out _))
				{
					SgtLogger.l("onDestinationReached dock: " + __instance.Name);
					DockingManagerSingleton.Instance.AddPendingToStationDock(handler.WorldId, stationHandler.WorldId);
					__instance.UpdateStatusItem();
				}
			}
		}

		/// <summary>
		/// undock all on landing
		/// </summary>
		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.SetCraftStatus))]
		public static class UndockOnLand
		{
			public static void Postfix(Clustercraft __instance, CraftStatus craft_status)
			{
				if (__instance != null
					&& __instance.gameObject != null
					&& __instance.TryGetComponent<DockingSpacecraftHandler>(out var manager)
					&& !craft_status.Equals(CraftStatus.InFlight))
				{
					manager.UndockAll();
				}
			}
		}
	}
}
