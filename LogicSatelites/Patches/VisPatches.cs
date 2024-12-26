using HarmonyLib;
using LogicSatellites.Behaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace LogicSatellites.Patches
{
	internal class VisPatches
	{
		[HarmonyPatch(typeof(MissionControlClusterWorkable))]
		[HarmonyPatch(nameof(MissionControlClusterWorkable.IsRocketInRange))]
		public static class MissionControlClusterWorkableRangePatch
		{
			public static void Postfix(ref bool __result, AxialI worldLocation, AxialI rocketLocation)
			{
				if (__result)
					return;

				__result = ModAssets.FindConnectionViaAdjacencyMatrix(worldLocation, rocketLocation, out var Connection);
				//PathUpdating.AddOrUpdateConnection(receiver, Connection);
			}
		}

		[HarmonyPatch(typeof(LogicBroadcastReceiver))]
		[HarmonyPatch(nameof(LogicBroadcastReceiver.CheckRange))]
		public static class BroadcastRecieverRangePatch
		{
			public static bool Prefix(ref bool __result, GameObject broadcaster, GameObject receiver)
			{
				AxialI a, b;
				a = broadcaster.GetMyWorldLocation();
				b = receiver.GetMyWorldLocation();
				bool returnValue = AxialUtil.GetDistance(a, b) <= LogicBroadcaster.RANGE;
				if (returnValue)
				{
					__result = true;
					//PathUpdating.AddOrUpdateConnection(receiver, new List<AxialI>() { a, b });
					return false;
				}
				__result = ModAssets.FindConnectionViaAdjacencyMatrix(a, b, out var Connection);
				//PathUpdating.AddOrUpdateConnection(receiver, Connection);
				return false;
			}
		}

		[HarmonyPatch(typeof(ClusterMapScreen))]
		[HarmonyPatch(nameof(ClusterMapScreen.UpdatePaths))]
		public static class PathUpdating
		{
			static Color satelliteConnectionColor = UIUtils.rgba(255, 255, 255, 0.15f);
			static Color activeConnectionColor = UIUtils.rgba(90,255, 90, 0.3f);
			public static void AddOrUpdateConnection(GameObject item, List<AxialI> connections)
			{
				if (item == null)
				{
					SgtLogger.l("BroadcastReciever was null!");
					return;
				}

				if (ActiveConnections.ContainsKey(item))
				{
					Util.KDestroyGameObject(ActiveConnections[item]);
					ActiveConnections.Remove(item);
				}
				if (connections != null && connections.Count > 0)
				{
					var path = ClusterMapScreen.Instance.pathDrawer.AddPath();
					path.SetPoints(connections.Select(note => note.ToWorld2D()).ToList());
					path.SetColor(satelliteConnectionColor);
					ActiveConnections.Add(item, path);
				}
			}

			public static Dictionary<GameObject, ClusterMapPath> ActiveConnections = new Dictionary<GameObject, ClusterMapPath>();
			static Dictionary<ModAssets.Node, List<ClusterMapPath>> Paths = new Dictionary<ModAssets.Node, List<ClusterMapPath>>();
			public static void Postfix(ClusterMapScreen __instance)
			{
				foreach (var path in Paths)
				{
					for (int i = path.Value.Count - 1; i >= 0; i--)
					{
						var item = path.Value[i];
						if (!item.IsNullOrDestroyed() && !item.gameObject.IsNullOrDestroyed())
							Util.KDestroyGameObject(item);
					}
				}
				Paths.Clear();

				if (__instance.m_selectedEntity is SatelliteGridEntity satellite)
				{
					if (ModAssets.AdjazenzMatrixHolder.TryGetNodeAt(satellite.Location, out var activeNode))
					{
						var values = new List<ClusterMapPath>();
						foreach (var link in activeNode.Links)
						{
							var path = __instance.pathDrawer.AddPath();
							path.SetPoints(new List<UnityEngine.Vector2>() { link.Parent.SatelliteLocation.ToWorld2D(), link.Child.SatelliteLocation.ToWorld2D() });
							path.SetColor(satelliteConnectionColor);
							values.Add(path);
						}
						Paths[activeNode] = values;
						//activePaths.ForEach(path => path.SetColor(activeConnectionColor));
					}
				}
				//foreach (var node in ModAssets.AdjazenzMatrixHolder.LogicConnectionNodes)
				//{
				//	var values = new List<ClusterMapPath>();
				//	foreach (var link in node.Links)
				//	{
				//		var path = __instance.pathDrawer.AddPath();
				//		path.SetPoints(new List<UnityEngine.Vector2>() { link.Parent.SatelliteLocation.ToWorld2D(), link.Child.SatelliteLocation.ToWorld2D() });
				//		path.SetColor(satelliteConnectionColor);
				//		values.Add(path);
				//	}
				//	Paths[node] = values;
				//}
				//if(__instance.m_selectedEntity is SatelliteGridEntity satellite)
				//{
				//	if(ModAssets.AdjazenzMatrixHolder.TryGetNodeAt(satellite.Location, out var activeNode)
				//		&& Paths.TryGetValue(activeNode, out var activePaths))
				//	{
				//		activePaths.ForEach(path =>path.SetColor(activeConnectionColor));
				//	}
				//}
			}
		}
	}
}
