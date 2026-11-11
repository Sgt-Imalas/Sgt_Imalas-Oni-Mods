using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens;
using Rockets_TinyYetBig.Derelicts;
using Rockets_TinyYetBig.Elements;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.SpaceStationPatches
{

	class SpaceStationPatches
	{

		/// <summary>
		/// prevents a crash when a rocket starts from inside a station and its control station was selected previously
		/// </summary>
		[HarmonyPatch(typeof(RocketRestrictionSideScreen))]
		[HarmonyPatch(nameof(RocketRestrictionSideScreen.UpdateButtonStates))]
		public static class RocketRestrictionSideScreen_UpdateButtonStates
		{
			public static bool Prefix(RocketRestrictionSideScreen __instance, object data)
			{
				return __instance.controlStation != null;
			}
		}
		/// <summary>
		/// No Boosting
		/// </summary>
		[HarmonyPatch(typeof(MissionControlCluster.Instance))]
		[HarmonyPatch(nameof(MissionControlCluster.Instance.CanBeBoosted))]
		public static class DisableRocketBoost
		{
			public static bool Prefix(Clustercraft clustercraft, ref bool __result)
			{
				if (clustercraft is SpaceStation)
				{
					__result = false;
					return false;
				}
				return true;

			}
		}
		/// <summary>
		/// Removes Station from launchpad
		/// </summary>
		[HarmonyPatch(typeof(ClusterGrid))]
		[HarmonyPatch(nameof(ClusterGrid.GetEntitiesInRange))]
		public static class HideStationsFromLandingPads
		{
			public static void Postfix(ref List<ClusterGridEntity> __result)
			{
				__result.RemoveAll(entity => entity is SpaceStation);
			}
		}


		[HarmonyPatch(typeof(CameraController), nameof(CameraController.ConstrainToWorld))]
		public static class ConstrainToSmallerWorld_FixForModules
		{
			public static bool Prefix(CameraController __instance)
			{
				if (Game.Instance == null || Game.Instance.IsLoading() || __instance.FreeCameraEnabled)
					return false;

				WorldContainer activeWorld = ClusterManager.Instance.activeWorld;



				var bottomLeftBoundryBox = activeWorld.WorldOffset; // + (Vector2)activeWorld.WorldSize * 0.05f
				var topRightBoundryBox = activeWorld.WorldOffset + (Vector2)activeWorld.WorldSize;

				if (activeWorld.TryGetComponent<SpaceStation>(out var spaceStation))
				{
					bottomLeftBoundryBox = activeWorld.WorldOffset + spaceStation.bottomLeftCorner;
					topRightBoundryBox = activeWorld.WorldOffset + spaceStation.topRightCorner;
				}


				var Pos = __instance.baseCamera.transform.GetPosition();

				bool modify = false;
				if (Pos.x < bottomLeftBoundryBox.x)
				{
					modify = true;
					Pos.x = bottomLeftBoundryBox.x;
				}
				else if (Pos.x > topRightBoundryBox.x)
				{
					modify = true;
					Pos.x = topRightBoundryBox.x;
				}
				if (Pos.y < bottomLeftBoundryBox.y)
				{
					modify = true;
					Pos.y = bottomLeftBoundryBox.y;
				}
				else if (Pos.y > topRightBoundryBox.y)
				{
					modify = true;
					Pos.y = topRightBoundryBox.y;
				}

				if (modify)
				{
					Pos.z = -100;
					__instance.transform.SetPosition(Pos);
				}
				return false;
			}
		}


		/// <summary>
		/// Nameable Stations
		/// </summary>
		[HarmonyPatch(typeof(UserNameable), nameof(UserNameable.SetName))]
		public static class NameableStationsPatch
		{
			public static void Postfix(string name, UserNameable __instance)
			{
				if (__instance.TryGetComponent<SpaceStation>(out var station))
				{
					ClusterNameDisplayScreen.Instance.UpdateName(station);
					__instance.Trigger((int)GameHashes.NameChanged, name);
				}
			}
		}

		/// <summary>
		/// Removing that method from space station interiors (to prevent crash)
		/// </summary>
		[HarmonyPatch(typeof(ClusterManager), nameof(ClusterManager.UpdateWorldReverbSnapshot))]
		public static class DisableAudioReverbGetter
		{
			public static bool Prefix(int worldId, ClusterManager __instance)
			{
				WorldContainer world = __instance.GetWorld(worldId);

				if (world == null || world.TryGetComponent<SpaceStation>(out _))
					return false;
				return true;
			}
		}
		[HarmonyPatch(typeof(CraftModuleInterface), nameof(CraftModuleInterface.GetInteriorWorld))]
		public static class GetInteriorWorld_SpaceStation
		{
			public static bool Prefix(CraftModuleInterface __instance, ref WorldContainer __result)
			{
				if (__instance.TryGetComponent(out SpaceStation station))
				{
					__result = null;
					if (station.SpaceStationInteriorId != -1)
						__result = ClusterManager.Instance.GetWorld(station.SpaceStationInteriorId);
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(RadiationBalanceDisplayer), nameof(RadiationBalanceDisplayer.GetTooltip))]
		public static class Fixes_Crash_for_out_of_world_dupes
		{
			public static bool Prefix(Amount master, AmountInstance instance)
			{
				int targetCell = Grid.PosToCell(instance.gameObject);
				return Grid.IsValidCell(targetCell);
			}
		}


		
		static bool IsGridCellConsideredSolid(ref Grid.BuildFlagsSolidIndexer indexer, int cell)
		{
			// == Grid.Solid[cell] 
			if (Grid.Solid[cell] && Grid.Element[cell].id != ModElements.SpaceStationForceField.SimHash)
				return true;
			return false;
		}

		static IEnumerable<CodeInstruction> StaticCheckReplaceTranspiler(IEnumerable<CodeInstruction> orig, string info)
		{
			var solidGetter = AccessTools.Method(typeof(Grid.BuildFlagsSolidIndexer), "get_Item");
			var injection = AccessTools.Method(typeof(SpaceStationPatches), nameof(IsGridCellConsideredSolid));
			//List<CodeInstruction> newS = [];
			bool injectionFound = false;
			foreach (var ci in orig)
			{
				if (ci.Calls(solidGetter))
				{
					injectionFound = true;
					var ci2 = new CodeInstruction(OpCodes.Call, injection);
					ci2.labels = ci.ExtractLabels();
					yield return ci2;
					//newS.Add(ci2);
				}
				else
				{
					yield return ci;
					//newS.Add(ci);
				}
			}

			if (injectionFound == false)
			{
				TranspilerHelper.PrintError("Could not add forcefield check to "+info);
			}
			else
			{
				SgtLogger.l(info+" transpiler ran successfully");
			}
			//TranspilerHelper.PrintInstructions(newS);
		}

		[HarmonyPatch(typeof(RailGun), nameof(RailGun.Sim200ms))]
		public static class Railgun_SpaceStationFix
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig) => StaticCheckReplaceTranspiler(orig, "RailGun.Sim200ms");
		}
		[HarmonyPatch(typeof(LandingBeacon), nameof(LandingBeacon.UpdateLineOfSight))]
		public static class LandingBeacon_SpaceStationFix
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig) => StaticCheckReplaceTranspiler(orig, "LandingBeacon.UpdateLineOfSight");
		}



		[HarmonyPatch(typeof(WorldContainer), nameof(WorldContainer.maximumBounds), MethodType.Getter)]
		public static class DimensionLimiter_SpaceStation_max
		{
			public static void Postfix(WorldContainer __instance, ref Vector2 __result)
			{
				if (!__instance.IsNullOrDestroyed() && __instance.TryGetComponent<SpaceStation>(out var spaceStation))
				{
					__result = Vector2.Min(__result, __instance.WorldOffset + spaceStation.topRightCorner);
				}
			}
		}
		[HarmonyPatch(typeof(WorldContainer), nameof(WorldContainer.minimumBounds), MethodType.Getter)]
		public static class DimensionLimiter_SpaceStation_min
		{
			public static void Postfix(WorldContainer __instance, ref Vector2 __result)
			{
				if (!__instance.IsNullOrDestroyed() && __instance.TryGetComponent<SpaceStation>(out var spaceStation))
				{
					__result = Vector2.Max(__result, __instance.WorldOffset + spaceStation.bottomLeftCorner);
				}
			}
		}

		[HarmonyPatch(typeof(Grid), nameof(Grid.IsValidBuildingCell))]
		public static class SpaceStation_Modified_BuildRules
		{
			public static void Postfix(int cell, ref bool __result)
			{
				if (!Grid.IsWorldValidCell(cell))
					return;
				WorldContainer world = ClusterManager.Instance.GetWorld(Grid.WorldIdx[cell]);
				if (world == null)
					return;


				if (world.TryGetComponent<DerelictStation>(out _))
				{
					__result = false;
				}
				else if (world.TryGetComponent<SpaceStation>(out _) && !__result)
				{
					Vector2I xy = Grid.CellToXY(cell);
					__result = xy.x >= world.minimumBounds.x && xy.x <= world.maximumBounds.x && xy.y >= world.minimumBounds.y && xy.y <= world.maximumBounds.y;

				}
			}

		}


		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.Destination), MethodType.Getter)]
		public static class NoDestinationSelection
		{
			public static bool Prefix(Clustercraft __instance)
			{
				if (__instance is SpaceStation)
					return false;
				return true;
			}

		}
		[HarmonyPatch(typeof(ClusterMapVisualizer), nameof(ClusterMapVisualizer.OnSpawn))]
		public static class SwitchOutAnimator
		{
			public static bool Prefix(ClusterMapVisualizer __instance, ClusterGridEntity ___entity)
			{
				if (___entity is SpaceStation)
				{
					//new ClusterMapFXAnimator.StatesInstance(__instance, ___entity).StartSM();
					return false;

				}
				return true;
			}

		}

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.TotalBurden), MethodType.Getter)]
		public static class NoBurdenForSpaceStation
		{
			public static bool Prefix(Clustercraft __instance, ref float __result)
			{
				if (__instance is SpaceStation)
				{
					__result = 0f;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.CanLandAtAsteroid))]
		public static class NoLandingForSpaceStation
		{
			public static bool Prefix(Clustercraft __instance, ref bool __result)
			{
				if (__instance is SpaceStation)
				{
					__result = false;
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(RocketLaunchConditionVisualizerEffect))]
		[HarmonyPatch(nameof(RocketLaunchConditionVisualizerEffect.HasClearPathToSpace))]
		public static class LaunchVisualizerWorldBounds
		{
			public static void Postfix(int cell, Vector2I worldMax, ref bool __result)
			{
				if (!__result && ClusterManager.Instance != null && SpaceStationManager.ActiveWorldIsSpaceStationInterior())
				{
					if (!Grid.IsValidCell(cell))
					{
						return;
					}

					int cell2 = cell;
					while ((!Grid.IsSolidCell(cell2) || Grid.Element[cell2].id == ModElements.SpaceStationForceField.SimHash || Grid.Element[cell2].id == SimHashes.Unobtanium) && Grid.CellToXY(cell2).y < worldMax.y)
					{
						cell2 = Grid.CellAbove(cell2);
					}
					if ((!Grid.IsSolidCell(cell2) || Grid.Element[cell2].id == ModElements.SpaceStationForceField.SimHash || Grid.Element[cell2].id == SimHashes.Unobtanium) && Grid.CellToXY(cell2).y == worldMax.y)
					{
						__result = true;
					}

				}
			}
		}




		/// <summary>
		/// No status items
		/// </summary>
		[HarmonyPatch(typeof(Clustercraft))]
		[HarmonyPatch(nameof(Clustercraft.UpdateStatusItem))]
		public static class NoStatusItemsForSpaceStation
		{
			public static bool Prefix(Clustercraft __instance)
			{
				if (__instance is SpaceStation)
				{
					return false;
				}
				return true;
			}
		}
		//[HarmonyPatch(typeof(RocketSimpleInfoPanel))]
		//[HarmonyPatch(nameof(RocketSimpleInfoPanel.Refresh))]
		//public static class NoStatusItemsForSpaceStation2
		//{
		//    public static void Prefix(ref CollapsibleDetailContentPanel rocketStatusContainer,ref GameObject selectedTarget)
		//    {
		//        if (selectedTarget.TryGetComponent<SpaceStation>(out var Station))
		//        {
		//            selectedTarget = null;
		//            rocketStatusContainer.gameObject.SetActive(false);
		//            rocketStatusContainer.Commit();
		//        }
		//    }
		//}

		/// <summary>
		/// prob. not needed anymore. 
		/// removes ability to land for stations
		/// </summary>
		[HarmonyPatch(typeof(Clustercraft))]
		[HarmonyPatch(nameof(Clustercraft.CanLandAtPad))]
		public static class NoLandingForSpaceStation2
		{
			public static bool Prefix(Clustercraft __instance, ref Clustercraft.PadLandingStatus __result)
			{
				if (__instance is SpaceStation)
				{
					__result = Clustercraft.PadLandingStatus.CanNeverLand;
					return false;
				}
				return true;
			}
		}
		/// <summary>
		/// No Self Destruct button for derelicts
		/// </summary>
		[HarmonyPatch(typeof(SelfDestructButtonSideScreen))]
		[HarmonyPatch(nameof(SelfDestructButtonSideScreen.IsValidForTarget))]
		public static class NoSpaceStationSelfDestruct
		{
			public static void Postfix(GameObject target, ref bool __result)
			{
				if (target.TryGetComponent<DerelictStation>(out _))
				{
					__result = false;
				}
			}
		}
		[HarmonyPatch(typeof(CraftModuleInterface))]
		[HarmonyPatch(nameof(CraftModuleInterface.CompleteSelfDestruct))]
		public static class FixStationSelfDestruct
		{
			public static bool Prefix(CraftModuleInterface __instance)
			{
				if (!__instance.TryGetComponent<SpaceStation>(out var station))
					return true;
				//skip non existing module chain and blow up world
				station.SetExploding();
				SpaceStationManager.Instance.DestroySpaceStationInteriorWorld(station.SpaceStationInteriorId);
				__instance.Trigger((int)GameHashes.RocketSelfDestructRequested);
				ClusterMapScreen.Instance.RemoveDeletedEntities(__instance.gameObject);
				ClusterMapSelectTool.Instance.SelectNextFrame(null);

				return false;
			}
		}



		/// <summary>
		/// Bouncy Stations
		/// </summary>
		[HarmonyPatch(typeof(ClusterMapScreen))]
		[HarmonyPatch(nameof(ClusterMapScreen.FloatyAsteroidAnimation))]
		public static class Bouncing_Space_Stations
		{
			public static void Postfix(ClusterMapScreen __instance)
			{
				float num = 0.0f;
				foreach (var worldContainer in ClusterManager.Instance.WorldContainers)
				{
					if (worldContainer.TryGetComponent<SpaceStation>(out var component) && component.GetType() != typeof(DerelictStation))
					{
						if (__instance.m_gridEntityVis.ContainsKey(component))
							__instance.m_gridEntityVis[component].GetFirstAnimController().Offset = new Vector2(0.0f, __instance.floatCycleOffset + __instance.floatCycleScale * Mathf.Sin(__instance.floatCycleSpeed * (num + GameClock.Instance.GetTime())));
						++num;
					}
				}
			}
		}


		[HarmonyPatch(typeof(ClusterUtil))]
		[HarmonyPatch(nameof(ClusterUtil.GetAsteroidWorldIdAtLocation))]
		public static class WorldTargetForSpaceStation
		{
			public static void Postfix(AxialI location, ref int __result)
			{
				if (__result == -1)
				{
					foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.cellContents[location])
					{
						if (clusterGridEntity is SpaceStation station)
						{
							__result = station.SpaceStationInteriorId;
						}
					}
				}
			}
		}


		[HarmonyPatch(typeof(ClusterMapScreen))]
		[HarmonyPatch(nameof(ClusterMapScreen.GetSelectorGridEntity))]
		public static class FixInteriorRailgunsAndSensor
		{
			public static bool Prefix(ClusterDestinationSelector selector, ref ClusterGridEntity __result)
			{
				if (selector.TryGetComponent<LogicClusterLocationSensor>(out var logicSensor))
				{
					var world = logicSensor.GetMyWorld();
					if (world != null && world.TryGetComponent<ClusterGridEntity>(out var clusterGridEntity))
					{
						__result = clusterGridEntity;
						return false;
					}
				}

				ClusterGridEntity component = selector.GetComponent<ClusterGridEntity>();
				if (component != null && ClusterGrid.Instance.IsVisible(component))
					return component;
				ClusterGridEntity entityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(selector.GetMyWorldLocation(), EntityLayer.Asteroid);
				if (entityOfLayerAtCell == null && SpaceStationManager.GetSpaceStationAtLocation(selector.GetMyWorldLocation(), out var station))
				{
					entityOfLayerAtCell = station;
				}

				Debug.Assert(component != null || entityOfLayerAtCell != null, string.Format("{0} has no grid entity and isn't located at a visible asteroid at {1}", selector, selector.GetMyWorldLocation()));
				__result = (bool)entityOfLayerAtCell ? entityOfLayerAtCell : component;
				return false;
			}
		}

		/// <summary>
		/// Patch for Railguns inside spacestation
		/// </summary>
		[HarmonyPatch(typeof(ClusterGrid))]
		[HarmonyPatch(nameof(ClusterGrid.GetPath))]
		[HarmonyPatch(new Type[] { typeof(AxialI), typeof(AxialI), typeof(ClusterDestinationSelector), typeof(string), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
		public static class tarnspilerforPathSpaceStation
		{
			static ClusterGridEntity AllowSpaceStation(ClusterGridEntity original, ClusterDestinationSelector selector, AxialI target)
			{
				//SgtLogger.debuglog("All params: " + original + ", " + selector + ", " + target);
				if (original == null && selector.requireAsteroidDestination)
				{
					var station = ClusterGrid.Instance.GetEntitiesOnCell(target).OfType<SpaceStation>();
					if (station != null && station.Count() > 0)
					{
						return station.First();
					}

				}

				return original;
			}

			private static readonly MethodInfo AllowSpaceStationMethod = AccessTools.Method(
			   typeof(tarnspilerforPathSpaceStation),
			   nameof(AllowSpaceStation)
			);


			private static readonly MethodInfo MethodToFind = AccessTools.Method(
				typeof(ClusterGrid),
			   nameof(ClusterGrid.GetVisibleEntityOfLayerAtCell)
			);

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();
				var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Stloc_1);

				if (insertionIndex != -1)
				{
#if DEBUG
                            SgtLogger.debuglog("GetPathMethod found");
#endif
					code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_3));
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldarg_2));
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, AllowSpaceStationMethod));

					//foreach (var v in code)
					//Console.WriteLine("OPcode: " + v.opcode + ", operand: " + v.operand);
				}

				return code;
			}
		}


		[HarmonyPatch]
		public static class PatchRailgunPayloadTravel
		{
			[HarmonyPostfix]
			public static void Postfix(RailGunPayload.StatesInstance __instance, AxialI source, AxialI destination)
			{
				if (__instance.master.gameObject.TryGetComponent(out ClusterTraveler clusterTraveler) && SpaceStationManager.IsSpaceStationAt(destination) && clusterTraveler.quickTravelToAsteroidIfInOrbit == true)
				{
					clusterTraveler.quickTravelToAsteroidIfInOrbit = false;
					SgtLogger.l("Railgun projectile set for space station, deactivating orbit fast travel");

				}
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				yield return typeof(RailGunPayload.StatesInstance).GetMethod("Travel");
				yield return typeof(RailGunPayload.StatesInstance).GetMethod("Launch");
			}
		}
		//[HarmonyPatch(typeof(GridSettings))]
		//[HarmonyPatch(nameof(GridSettings.Reset))]
		//public static class Size_Experiment
		//{
		//    public static void Prefix(ref int width,ref int height)
		//    {
		//        width += 500;
		//        height += 500;
		//    }
		//}
	}
}
