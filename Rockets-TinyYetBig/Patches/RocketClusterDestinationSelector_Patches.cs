using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Nosecones;
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

		[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.CanRocketHarvest))]
		public static class RocketClusterDestinationSelector_CanRocketHarvest_Patch
		{
			/// <summary>
			/// Adds Laser Nosecone to harvestCheck
			/// </summary>
			public static void Postfix(RocketClusterDestinationSelector __instance, ref bool __result)
			{
				if (!__result)
				{
					List<NoseConeHEPHarvest.StatesInstance> resourceHarvestModules = GetAllLaserNoseconeHarvestModules(__instance.GetComponent<Clustercraft>());
					if (resourceHarvestModules.Count > 0)
					{
						foreach (var statesInstance in resourceHarvestModules)
						{
							if (statesInstance.CheckIfCanHarvest())
							{
								__result = true;
							}
						}
						SgtLogger.l(__instance.name + " can harvest with laser drillcone?:" + __result);
					}
				}
			}
			public static List<NoseConeHEPHarvest.StatesInstance> GetAllLaserNoseconeHarvestModules(Clustercraft craft)
			{
				List<NoseConeHEPHarvest.StatesInstance> laserNosecones = new List<NoseConeHEPHarvest.StatesInstance>();
				foreach (Ref<RocketModuleCluster> clusterModule in craft.ModuleInterface.ClusterModules)
				{
					NoseConeHEPHarvest.StatesInstance smi = clusterModule.Get().GetSMI<NoseConeHEPHarvest.StatesInstance>();
					if (smi != null)
						laserNosecones.Add(smi);
				}
				return laserNosecones;
			}
		}

		[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.WaitForPOIHarvest))]
		public static class RocketClusterDestinationSelector_WaitForPOIHarvest_Patch
		{
			/// <summary>
			/// Sub all hep storage change handlers on start mining
			/// </summary>
			public static void Postfix(RocketClusterDestinationSelector __instance)
			{
				foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)__instance.GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
				{
					if ((bool)clusterModule.Get().GetComponent<HighEnergyParticleStorage>())
					{
						__instance.Subscribe(clusterModule.Get().gameObject, (int)GameHashes.OnParticleStorageChanged, __instance.OnStorageChange);
					}
				}
			}
		}

		[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.OnStorageChange))]
		public static class RocketClusterDestinationSelector_OnStorageChange_Patch
		{
			/// <summary>
			/// Unsub all hep storage change handlers on returntrip
			/// </summary>
			public static void Postfix(RocketClusterDestinationSelector __instance)
			{
				if (__instance.CanRocketHarvest())
					return;

				foreach (Ref<RocketModuleCluster> clusterModule in __instance.GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
				{

					if (clusterModule.Get().GetComponent<HighEnergyParticleStorage>())
					{
						//SgtLogger.debuglog("HEP FOUND; UNSUBSCRIBING");
						__instance.Unsubscribe(clusterModule.Get().gameObject, (int)GameHashes.OnParticleStorageChanged, __instance.OnStorageChange);
					}
				}
			}
		}
		#endregion
	}
}
