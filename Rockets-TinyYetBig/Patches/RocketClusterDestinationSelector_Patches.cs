using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Nosecones;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.ClustercraftDockingPatches
{
	internal class RocketClusterDestinationSelector_Patches
	{
		[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.OnClusterLocationChanged))]
		public static class RocketClusterDestinationSelector_OnClusterLocationChanged_Patch
		{
			/// <summary>
			/// for roundtrip rockets with docking capability: if the destination has a station and the rocket is not landed there (so it can only dock), dock instead of immediately turning around
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="data"></param>
			/// <returns></returns>
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
		#region HEP-Drillcone

		[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.CanRocketDrill))]
		public static class RocketClusterDestinationSelector_CanRocketHarvest_Patch
		{
			/// <summary>
			/// Adds Laser Nosecone to harvestCheck
			/// </summary>
			
			public static void Postfix(RocketClusterDestinationSelector __instance, ref bool __result)
			{
				if (!__result)
				{
					List<ResourceHarvestModuleHEP.StatesInstance> resourceHarvestModules = GetAllLaserNoseconeHarvestModules(__instance.GetComponent<Clustercraft>());
					if (resourceHarvestModules.Count > 0)
					{
						foreach (var statesInstance in resourceHarvestModules)
						{
							if (statesInstance.CheckIfCanDrill())
							{
								SgtLogger.l(__instance.name + " can harvest with laser drillcone");
								__result = true;
								return;
							}
						}
					}
				}
			}
			public static List<ResourceHarvestModuleHEP.StatesInstance> GetAllLaserNoseconeHarvestModules(Clustercraft craft)
			{
				List<ResourceHarvestModuleHEP.StatesInstance> laserNosecones = new List<ResourceHarvestModuleHEP.StatesInstance>();
				foreach (Ref<RocketModuleCluster> clusterModule in craft.ModuleInterface.ClusterModules)
				{
					ResourceHarvestModuleHEP.StatesInstance smi = clusterModule.Get().GetSMI<ResourceHarvestModuleHEP.StatesInstance>();
					if (smi != null)
						laserNosecones.Add(smi);
				}
				return laserNosecones;
			}
		}
		#endregion
	}
}
