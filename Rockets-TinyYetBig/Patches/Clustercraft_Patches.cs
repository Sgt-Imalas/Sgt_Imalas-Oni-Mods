using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Engines;
using Rockets_TinyYetBig.Derelicts;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static Clustercraft;
using static Operational;
using static Rockets_TinyYetBig.Docking.DockingSpacecraftHandler;

namespace Rockets_TinyYetBig.Patches
{
	internal class Clustercraft_Patches
	{

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.OnClusterDestinationChanged))]
		public static class Clustercraft_OnClusterDestinationChanged_Patch
		{
			/// <summary>
			/// when two rockets are docked together, make them fly the same path
			/// if the docked target is a space station interior, undock instead
			/// </summary>
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
		public static class Clustercraft_OnClusterDestinationReached_Patch
		{
			/// <summary>
			/// when a rocket reaches its destination, if that destination is a station hex, auto dock to the station
			/// </summary>
			public static void Postfix(Clustercraft __instance)
			{
				if (__instance is SpaceStation || __instance is DerelictStation)
					return;

				var clusterDestinationSelector = __instance.m_moduleInterface.GetClusterDestinationSelector();

				if (__instance.status == CraftStatus.InFlight //In space on a station hex
					&& __instance.Location == clusterDestinationSelector.GetDestination() //at destination
					&& !LaunchPad.GetLaunchPadsForDestination(__instance.Location).Any(pad => pad.LandedRocket == null) //no pads to land on
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

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.SetCraftStatus))]
		public static class Clustercraft_SetCraftStatus_Patch
		{
			/// <summary>
			/// undock all docking ports on landing
			/// </summary>
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

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.CheckDesinationInRange))]
		public static class Clustercraft_CheckDesinationInRange_Patch
		{
			/// <summary>
			/// Add some rounding for designation in range check to fix vanilla bug that sometimes makes rockets think they are out of range by a tiny fraction
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="__result"></param>
			public static void Postfix(Clustercraft __instance, ref bool __result)
			{
				if (__result == true)
					return;

				if (__instance.m_clusterTraveler.CurrentPath == null)
				{
					return;
				}

				__result = Mathf.RoundToInt(__instance.Speed * __instance.m_clusterTraveler.TravelETA()) <= Mathf.RoundToInt(__instance.ModuleInterface.Range);
			}
		}

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.BurnFuelForTravel))]
		public static class BurnElectricityFuel_Patch
		{
			/// <summary>
			/// "burns" electricity from batteries for electric engines instead of fuel
			/// </summary>
			/// <param name="__instance"></param>
			public static void Postfix(Clustercraft __instance)
			{
				ElectricEngineCluster targetEngine = null;
				List<ModuleBattery> Batteries = new List<ModuleBattery>();

				foreach (var clusterModule in __instance.ModuleInterface.ClusterModules)
				{
					var md = clusterModule.Get();
					if (targetEngine == null && md.TryGetComponent<ElectricEngineCluster>(out var eng))
					{
						targetEngine = eng;
					}
					if (md.TryGetComponent<ModuleBattery>(out var battery))
					{
						Batteries.Add(battery);
					}
				}

				if (targetEngine == null || !targetEngine.TryGetComponent<RocketModuleCluster>(out var module))
				{
					return;
				}
				float joulesToBurn = targetEngine.Joules_Per_Hex;
				foreach (var battery in Batteries)
				{
					float joulesInBatteryToConsume = Mathf.Min(battery.JoulesAvailable, joulesToBurn);
					joulesToBurn -= joulesInBatteryToConsume;
					battery.ConsumeEnergy(joulesInBatteryToConsume, false);

					if (Mathf.Approximately(joulesToBurn, 0f))
						break;
				}

			}
		}

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.Sim4000ms))]
		public static class Clustercraft_Sim4000ms_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HabitatInteriorRadiation;
			/// <summary>
			/// Adjusts interior radiation of habitats dynamically by either copying exterior rads while landed or by distance to center of the starmap (higher distance == higher rads)
			/// </summary>
			public static void Postfix(Clustercraft __instance)
			{
				if (__instance is SpaceStation)
					return;
				KPrefabID prefab = null;
				ClustercraftExteriorDoor door = null;
				foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)__instance.m_moduleInterface.ClusterModules)
				{
					if (clusterModule.Get().TryGetComponent(out door))
					{
						door.TryGetComponent(out prefab);
						break;
					}
				}
				if (door == null)
					return;

				var world = door.GetMyWorld();
				var target = door.targetDoor;
				if (target == null)
					return;
				var interiorWorld = door.targetDoor.GetMyWorld();

				if (__instance.status != Clustercraft.CraftStatus.InFlight)
				{

					int cell = Grid.PosToCell(door);
					if (Grid.ExposedToSunlight[cell] > 0)
					{
						interiorWorld.sunlight = world.sunlight;
					}
					else
					{
						interiorWorld.sunlight = 0;
					}

					if (Grid.Radiation[cell] > 0)
					{
						interiorWorld.cosmicRadiation = (int)Grid.Radiation[cell];
					}
					else
					{
						interiorWorld.cosmicRadiation = 0;
					}
				}
				else
				{
					interiorWorld.sunlight = FIXEDTRAITS.SUNLIGHT.DEFAULT_VALUE;
					interiorWorld.cosmicRadiation = SpaceStationManager.SpaceRadiationRocket(__instance.Location);
				}
			}
		}

		#region Docked Rocket Movement Sync
		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.OnSpawn))]
		public static class Clustercraft_OnSpawn_Patch
		{
			/// <summary>
			/// attach a handler to pull stranded rockets along when a docked rocket travels
			/// </summary>
			/// <param name="__instance"></param>
			public static void Postfix(Clustercraft __instance)
			{
				__instance.m_clusterTraveler.onTravelCB += () =>
				{
					if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
					{
						foreach (var docked in manager.GetCurrentDocks())
						{
							var handler = docked.spacecraftHandler;
							if (handler != null
							&& (Mathf.RoundToInt(handler.clustercraft.ModuleInterface.Range) == 0 || handler.CraftType != DockableType.Rocket) // pull other rockets if they are empty or if __instance is not a rocket (space station or derelict - maybe flying derelicts later?)
							&& handler.clustercraft.Location != __instance.Location)
							{
								if (ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(__instance.Location, EntityLayer.Asteroid) == null)
								{
									SgtLogger.l("Pulled stranded rocket " + handler.clustercraft.Name + " to new tile with " + __instance.Name);
									handler.clustercraft.Location = __instance.Location;
								}
								else
								{
									SgtLogger.l("Disconnected " + handler.clustercraft.Name + " as stranded in orbit");
									handler.clustercraft.m_clusterTraveler.m_destinationSelector.SetDestination(handler.clustercraft.Location);
									//craft.m_clusterTraveler.m_destinationSelector.SetDestination(__instance.Location);
								}
							}
						}
					}
				};
			}
		}
		
		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.EnginePower), MethodType.Getter)]
		public static class EnginePower_Patch
		{
			/// <summary>
			/// Sums up engine power of all docked rockets
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="__result"></param>
			public static void Postfix(Clustercraft __instance, ref float __result)
			{
				if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
				{
					foreach (var docked in manager.GetConnectedRockets())
					{
						if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
						{
							var engine = craft.ModuleInterface.GetEngine();

							if (engine != null && engine.TryGetComponent<RocketModuleCluster>(out var engineModule) && Mathf.RoundToInt(craft.ModuleInterface.Range) > 0)
							{
								__result += engineModule.performanceStats.EnginePower;
							}
						}
					}
				}
				//SgtLogger.l("EnginePower " + __result);
			}
		}

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.TotalBurden), MethodType.Getter)]
		public static class TotalBurden_Patch
		{
			/// <summary>
			/// Sum up burden of all docked rockets
			/// skips original getter for docked rockets only
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="__result"></param>
			public static void Postfix(Clustercraft __instance, ref float __result)
			{
				if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
				{
					foreach (var docked in manager.GetConnectedRockets())
					{
						if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
						{
							foreach (var module in craft.ModuleInterface.ClusterModules)
							{
								__result += module.Get().performanceStats.Burden;
							}
						}
					}
				}
				//SgtLogger.l("TotalBurden " + __result);
			}
		}

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.Speed), MethodType.Getter)]
		public static class Speed_Patch
		{
			/// <summary>
			/// calculates the speed of docked rockets
			/// skips original getter for docked rockets only
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="__result"></param>
			/// <returns></returns>
			public static bool Prefix(Clustercraft __instance, ref float __result)
			{
				if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
				{
					if (manager.GetConnectedRockets().Count == 0)
						return true;

					float unmodifiedSpeed = __instance.EnginePower / __instance.TotalBurden;
					float totalAutoPilotMultiplier = __instance.AutoPilotMultiplier;
					float totalPilotSkillMultiplier = __instance.PilotSkillMultiplier;
					float totalControlStationBuffTimeRemaining = __instance.controlStationBuffTimeRemaining;

					RoboPilotModule robotPilotModule = __instance.ModuleInterface.GetRobotPilotModule();
					bool hasActiveRoboPilot = robotPilotModule != null && robotPilotModule.GetDataBanksStored() > 1f;
					bool hasPassengerModule = __instance.ModuleInterface.GetPassengerModule() != null;
					bool hasActiveHumanPilot = totalPilotSkillMultiplier > 0.5f;
					//float numberOfPilots = 1;


					foreach (var docked in manager.GetConnectedRockets())
					{
						if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
						{
							totalAutoPilotMultiplier = totalAutoPilotMultiplier < craft.AutoPilotMultiplier ? craft.AutoPilotMultiplier : totalAutoPilotMultiplier;
							totalPilotSkillMultiplier = totalPilotSkillMultiplier < craft.PilotSkillMultiplier ? craft.PilotSkillMultiplier : totalPilotSkillMultiplier;
							totalControlStationBuffTimeRemaining = totalControlStationBuffTimeRemaining < craft.controlStationBuffTimeRemaining ? craft.controlStationBuffTimeRemaining : totalControlStationBuffTimeRemaining;
							//++numberOfPilots;
							if(!hasActiveHumanPilot && craft.PilotSkillMultiplier > 0.5f)
							{
								hasActiveHumanPilot = true;
							}
							if (!hasActiveRoboPilot && craft.ModuleInterface.GetRobotPilotModule() != null && craft.ModuleInterface.GetRobotPilotModule().GetDataBanksStored() > 1f)
							{
								hasActiveRoboPilot = true;
							}
						}
					}
					//totalAutoPilotMultiplier /= numberOfPilots;
					//totalPilotSkillMultiplier /= numberOfPilots;
					//totalControlStationBuffTimeRemaining /= numberOfPilots;

					float finalValue = unmodifiedSpeed * totalAutoPilotMultiplier * totalPilotSkillMultiplier;

					if (hasActiveRoboPilot && hasActiveHumanPilot)
					{
						finalValue *= 1.5f;
					}
					else if (!hasActiveHumanPilot && hasPassengerModule)
					{
						finalValue *= 0.5f;
					}
					else if (!hasActiveRoboPilot && !hasPassengerModule)
					{
						finalValue = 0f;
					}
					if (totalControlStationBuffTimeRemaining > 0f && finalValue > 0)
					{
						finalValue += unmodifiedSpeed * 0.2f;
					}
					__result = finalValue;

					return false;
				}
				return true;
			}
		}
		#endregion
	}
}
