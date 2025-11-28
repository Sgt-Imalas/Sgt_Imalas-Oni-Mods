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


		[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.CheckAndReturnRocketIfHarvestingEnded))]
		public class RocketClusterDestinationSelector_CheckAndReturnRocketIfHarvestingEnded_Patch2
		{
			public static void Prefix(RocketClusterDestinationSelector __instance)
			{
				SgtLogger.l("CheckAndReturnIfHarvestingEnded Called. CanRocketDrill(): " + __instance.CanRocketDrill() + " || CanCollectFromHexCellInventory()" + __instance.CanCollectFromHexCellInventory());
			}
		}


		[HarmonyPatch(typeof(RocketModuleHexCellCollector), nameof(RocketModuleHexCellCollector.CanCollect), [typeof(RocketModuleHexCellCollector.Instance)])]
		public class RocketModuleHexCellCollector_CanCollect_Patch
		{
			public static bool Prefix(RocketModuleHexCellCollector.Instance smi, ref bool __result)
			{

				__result = false;
				if ((double)smi.storage.RemainingCapacity() <= 0.0)
				{
					SgtLogger.l("cargobay full");
					__result = false; return false;
				}
				StarmapHexCellInventory hexCellInventory = ClusterGrid.Instance.AddOrGetHexCellInventory(smi.StarmapLocation);
				bool flag = (double)hexCellInventory.TotalMass > 0.0;
				if (smi.IsSpaceshipMoving || !flag)
				{
					SgtLogger.l("ship moving");
					__result = false; return false;
				}
				foreach (StarmapHexCellInventory.SerializedItem serializedItem in hexCellInventory.Items)
				{
					if (RocketModuleHexCellCollector.CanHexCellItemBeStored(serializedItem, smi, out bool _, out float _))
					{
						SgtLogger.l(serializedItem.ID + " can be successfully stored in " + smi.master.name);
						__result = true; return false;
					}
				}
				__result = false; return false;

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
						var module = clusterModule.Get();

						if (module.TryGetComponent<ResourceHarvestModuleHEPInjector>(out _))
						{
							__instance.Subscribe(module.gameObject, (int)GameHashes.OnParticleStorageChanged, __instance.OnStorageChange);
						}

						if (!module.HasTag(ModAssets.Tags.SpaceHarvestModule))
						{
							SgtLogger.warning("Module " + module.name + " is not a SpaceHarvestModule and should not trigger OnStorageChanged events for the RocketClusterDestinationSelector! Removing subscription to prevent crash..");
							module.Unsubscribe(module.gameObject, -1697596308, __instance.OnStorageChange);
						}
					}
				}
			}

			[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.CheckAndReturnRocketIfHarvestingEnded))]
			public static class RocketClusterDestinationSelector_CheckAndReturnRocketIfHarvestingEnded_Patch
			{
				/// <summary>
				/// Unsub all hep storage change handlers on returntrip
				/// </summary>
				public static void Postfix(RocketClusterDestinationSelector __instance)
				{
					if (__instance.CanRocketDrill() || __instance.CanCollectFromHexCellInventory())
						return;

					foreach (Ref<RocketModuleCluster> clusterModule in __instance.GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
					{

						if (clusterModule.Get().GetComponent<ResourceHarvestModuleHEPInjector>())
						{
							//SgtLogger.debuglog("HEP FOUND; UNSUBSCRIBING");
							__instance.Unsubscribe(clusterModule.Get().gameObject, (int)GameHashes.OnParticleStorageChanged, __instance.OnStorageChange);
						}
					}
				}
			}


			[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.CanCollectFromHexCellInventory))]
			public class RocketClusterDestinationSelector_CanCollectFromHexCellInventory_Patch
			{
				public static void Postfix(RocketClusterDestinationSelector __instance)
				{
					foreach (RocketModuleHexCellCollector.Instance allHexCellCollectorModule in __instance.GetComponent<Clustercraft>().GetAllHexCellCollectorModules())
					{
						bool canCollect = RocketModuleHexCellCollector.CanCollect(allHexCellCollectorModule);
						SgtLogger.l(allHexCellCollectorModule.GetProperName() + " can collect? " + canCollect);
					}
				}
			}
		}

		#endregion
	}
}
