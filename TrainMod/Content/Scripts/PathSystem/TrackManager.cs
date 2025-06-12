using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Dijkstar;
using TrainMod.Content.Scripts.PathSystem.Segmentation;
using UtilLibs;
using static STRINGS.INPUT_BINDINGS;

namespace TrainMod.Content.Scripts.PathSystem
{
	public static class TrackManager
	{
		static Dictionary<int, HashSet<TrackPiece>> CellsWithTrackPieceConnectors = new();
		public static List<TrackPiece> TrackPieces = new();
		public static HashSet<TrackStation> TrackStations = new();

		public static Graph ConnectionGraph = new();

		public static List<TrackPiece> Pathfind(TrackPiece origin, TrackPiece target, bool forwards)
		{
			var src = ConnectionGraph.AddOrGetNode(origin, forwards);
			var dest = ConnectionGraph.AddOrGetNode(target, forwards);
			var dest2 = ConnectionGraph.AddOrGetNode(target, !forwards);

			if (ConnectionGraph.TryFindPath(src, dest, out var path))
				return path;
			if (ConnectionGraph.TryFindPath(src, dest2, out var path2))
				return path2;
			return null;
		}

		public static void RecalculatePaths()
		{
			CreateSegmentations();
		}
		/// <summary>
		/// populate two directional graphs for both directions
		/// </summary>
		static void CreateSegmentations() ///This should function in theory...
		{
			SgtLogger.l("Current Nodes: ");
			foreach (Node node in ConnectionGraph.AllNodes)
			{
				SgtLogger.l(" ");
				SgtLogger.l(node.ToString());
				foreach (var edge in node.Edges)
				{
					SgtLogger.l("---> " + edge.Child.ToString());
				}
				if(!node.Edges.Any())
					SgtLogger.l("---> X");

			}

			string[] labels = ConnectionGraph.AllNodes.Select(s => s.Track.ToString()).ToArray();

			int?[,] adj = ConnectionGraph.CreateAdjMatrix();

			ConnectionGraph.PrintMatrix(ref adj, labels, ConnectionGraph.AllNodes.Count);

			//var trackPiecesToSort = (TrackStations).ToHashSet();
			//var trackPiecesToSort2 = (TrackStations).ToHashSet();

			//Forward = new Graph(); Backward = new Graph();

			//while (trackPiecesToSort.Any())
			//{
			//	SgtLogger.l("Path tracer going backwards");
			//	var root = trackPiecesToSort.First();
			//	CollectNodesRecursively(root, null, trackPiecesToSort, root.GetOutputConnections(), Backward);

			//}
			//while (trackPiecesToSort2.Any())
			//{
			//	SgtLogger.l("Path tracer going forwards");
			//	var root2 = trackPiecesToSort2.First();
			//	CollectNodesRecursively(root2, null, trackPiecesToSort2, root2.GetInputConnections(), Forward);
			//}
		}
		//static void CollectNodesRecursively(TrackPiece current, Node parentNode, HashSet<TrackStation> remaining, List<TrackPiece> childPiecesToCheck, Graph graph)
		//{
		//	SgtLogger.l(current.GetProperName() + " at " + Grid.PosToCell(current) + ": " + childPiecesToCheck.Count + " side nodes");
		//	if (current is TrackStation station)
		//		remaining.Remove(station);

		//	var node = graph.AddOrGetNode(current);
		//	if (parentNode != null)
		//		parentNode.AddEdge(node, current.PathCost);

		//	if (childPiecesToCheck == null || !remaining.Any())
		//		return;

		//	foreach (var childConnection in childPiecesToCheck)
		//	{
		//		//SgtLogger.l("child node " + global::STRINGS.UI.StripLinkFormatting(childConnection.GetProperName()) + " at cell: " + Grid.PosToCell(childConnection));
		//		if (childConnection.GetReachableConnectionsFrom(current, out var newChildren))
		//			CollectNodesRecursively(childConnection, node, remaining, newChildren, graph);
		//	}
		//}

		public static void RegisterTrack(TrackPiece newTrackPiece)
		{
			TrackPieces.Add(newTrackPiece);
			if (newTrackPiece is TrackStation station)
				TrackStations.Add(station);

			int inputCell = newTrackPiece.InputCell.first;
			int inputConnectsToCell = newTrackPiece.InputCell.second;

			var outputCells = newTrackPiece.OutputCells;

			if (!CellsWithTrackPieceConnectors.ContainsKey(inputCell))
				CellsWithTrackPieceConnectors[inputCell] = new HashSet<TrackPiece>();

			if (!CellsWithTrackPieceConnectors.ContainsKey(inputConnectsToCell))
				CellsWithTrackPieceConnectors[inputConnectsToCell] = new HashSet<TrackPiece>();

			foreach (var existingTrackPiece in CellsWithTrackPieceConnectors[inputConnectsToCell])
			{
				ConnectTracks(newTrackPiece, existingTrackPiece);
			}
			CellsWithTrackPieceConnectors[inputCell].Add(newTrackPiece);
			for (int i = 0; i < outputCells.Length; i++)
			{
				var outputCell = outputCells[i].first;
				var outputConnectionCell = outputCells[i].second;

				if (!CellsWithTrackPieceConnectors.ContainsKey(outputCell))
					CellsWithTrackPieceConnectors[outputCell] = new HashSet<TrackPiece>();
				if (!CellsWithTrackPieceConnectors.ContainsKey(outputConnectionCell))
					CellsWithTrackPieceConnectors[outputConnectionCell] = new HashSet<TrackPiece>();

				foreach (var existingTrackPiece in CellsWithTrackPieceConnectors[outputConnectionCell])
				{
					ConnectTracks(newTrackPiece, existingTrackPiece);
				}
				CellsWithTrackPieceConnectors[outputCell].Add(newTrackPiece);
			}
			RecalculatePaths();
		}

		public static void UnregisterTrack(TrackPiece trackPiece)
		{
			if (trackPiece is TrackStation station)
				TrackStations.Remove(station);
			TrackPieces.Remove(trackPiece);
			foreach (var cell in CellsWithTrackPieceConnectors.Values)
			{
				if (cell.Contains(trackPiece))
					cell.Remove(trackPiece);
			}
			trackPiece.RemoveAllConnections();
			RecalculatePaths();
		}
		public static void ConnectTracks(TrackPiece track1, TrackPiece track2)
		{

			if (track1 == track2) return;
			SgtLogger.l("connecting " + track1 + " and " + track2);

			track1.TryAddConnection(track2);
			track2.TryAddConnection(track1);

		}
	}
}
