using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Dijkstar;
using TrainMod.Content.Scripts.PathSystem.Segmentation;

namespace TrainMod.Content.Scripts.PathSystem
{
	public static class TrackManager
	{
		static Dictionary<int, HashSet<TrackPiece>> CellsWithTrackPieceConnectors = new();
		public static List<TrackPiece> TrackPieces = new();
		public static HashSet<TrackStation> TrackStations = new();

		static Graph Forward, Backward;

		public static void RecalculatePaths()
		{
			CreateSegmentations();
		}
		/// <summary>
		/// populate two directional graphs for both directions
		/// </summary>
		static void CreateSegmentations() ///This should function in theory...
		{
			var trackPiecesToSort = (TrackPieces).ToHashSet();

			Forward = new Graph(); Backward = new Graph();

			while (trackPiecesToSort.Any())
			{
				var root = trackPiecesToSort.First();
				CollectNodesRecursively(root, null, trackPiecesToSort, root.GetInputConnections(), Forward);
				CollectNodesRecursively(root, null, trackPiecesToSort, root.GetOutputConnections(), Backward);
			}
		}
		static void CollectNodesRecursively(TrackPiece current, Node parentNode, HashSet<TrackPiece> remaining, List<TrackPiece> childPiecesToCheck, Graph graph)
		{
			remaining.Remove(current);

			var node = graph.CreateNode(current);
			if (parentNode != null)
				parentNode.AddEdge(node, current.PathCost);

			foreach (var childConnection in childPiecesToCheck)
			{
				if (childConnection.GetReachableConnectionsFrom(current, out var newChildren))
					CollectNodesRecursively(childConnection, node, remaining, newChildren, graph);
			}
		}

		public static void RegisterTrack(TrackPiece newTrackPiece)
		{
			TrackPieces.Add(newTrackPiece);

			if (newTrackPiece is TrackStation station)
				TrackStations.Add(station);

			int inputCell = newTrackPiece.InputCell;
			var outputCells = newTrackPiece.OutputCells;
			if (!CellsWithTrackPieceConnectors.ContainsKey(inputCell))
				CellsWithTrackPieceConnectors[inputCell] = new HashSet<TrackPiece>();

			foreach (var existingTrackPiece in CellsWithTrackPieceConnectors[inputCell])
			{
				ConnectTracks(newTrackPiece, existingTrackPiece, inputCell);
			}
			CellsWithTrackPieceConnectors[inputCell].Add(newTrackPiece);

			foreach (var outputCell in outputCells)
			{
				if (!CellsWithTrackPieceConnectors.ContainsKey(outputCell))
					CellsWithTrackPieceConnectors[outputCell] = new HashSet<TrackPiece>();

				foreach (var existingTrackPiece in CellsWithTrackPieceConnectors[outputCell])
				{
					ConnectTracks(newTrackPiece, existingTrackPiece, outputCell);
				}
				CellsWithTrackPieceConnectors[outputCell].Add(newTrackPiece);
			}
			if (newTrackPiece is TrackStation)
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
			if (trackPiece is TrackStation)
				RecalculatePaths();
		}
		public static void ConnectTracks(TrackPiece track1, TrackPiece track2, int cell)
		{

			if (track1 == track2) return;

			track1.AddConnection(track2, cell);
			track2.AddConnection(track1, cell);
		}
	}
}
