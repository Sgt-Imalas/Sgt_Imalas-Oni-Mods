using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Nosecones;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System.Collections.Generic;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.ClustercraftDockingPatches
{
	internal static class RocketClusterDestinationSelector_Patches
	{
		//public static string Log(this AxialI pos) => "("+pos.Q + "," + pos.R + ")";


		//[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.SetUpReturnTrip))]
		//public class RocketClusterDestinationSelector_SetUpReturnTrip_Patch
		//{
		//	public static void Prefix(RocketClusterDestinationSelector __instance)
		//	{
		//		SgtLogger.l("RocketClusterDestinationSelector Setting Up ReturnTrip");
		//	}
		//}

		//[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.WaitForPOIHarvest))]
		//public class TargetType_TargetMethod_Patch
		//{
		//	public static void Prefix(RocketClusterDestinationSelector __instance)
		//	{
		//		SgtLogger.l("RocketClusterDestinationSelector wait for POI harvest");
		//	}
		//}
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
				//ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent)data;
				//SgtLogger.l("OnClusterLocationChanged] target: " + __instance.m_destination.Log() + ", current " + locationChangedEvent.newLocation.Log());
				ClusterGrid_Patches.SkipStations(true);
				if (__instance.Repeat && __instance.TryGetComponent<DockingSpacecraftHandler>(out var mng))
				{
					//SgtLogger.l("craft status: " + mng.clustercraft.Status);

					if (mng.clustercraft.status != Clustercraft.CraftStatus.InFlight)
					{
						//SgtLogger.l(__instance.name + "RocketClusterDestinationSelector not in flight anymore, undocking all");
						mng.UndockAll();
						return true;
					}

					ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent)data;
					if (locationChangedEvent.newLocation != __instance.m_destination)
						return true;


					//bool entityAtLayer = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(locationChangedEvent.newLocation, EntityLayer.POI) != (UnityEngine.Object)null;
					//bool canDrill = __instance.CanRocketDrill();
					//bool canCollecc = __instance.CanCollectFromHexCellInventory();
					//SgtLogger.l($"RocketClusterDestinationSelector at targetLocation - rocket state: {mng.clustercraft.Status} EntityAtLAyer: {entityAtLayer}, canDrill: {canDrill}, canCollecc: {canCollecc}");


					//Skips immediate RoundTrip-return if there is a station at the target location and the rocket cannot land there
					if (SpaceStationManager.GetSpaceStationAtLocation(locationChangedEvent.newLocation, out var station) && SpaceStationManager.CanDockAtSpacestation(mng.clustercraft, locationChangedEvent.newLocation))
					{
						mng.clustercraft.ModuleInterface.TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationReached, null);
						return false;
					}
				}
				return true;
			}
			public static void Postfix() => ClusterGrid_Patches.SkipStations(false);
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
								//SgtLogger.l(__instance.name + " can harvest with laser drillcone");
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
