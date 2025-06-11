using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Dijkstar;
using TrainMod.Content.Scripts.PathSystem.Segmentation;
using UtilLibs;

namespace TrainMod.Content.Scripts.PathSystem
{
	public static class TrackManager
	{
		static Dictionary<int, HashSet<TrackPiece>> CellsWithTrackPieceConnectors = new();
		public static List<TrackPiece> TrackPieces = new();
		public static HashSet<TrackStation> TrackStations = new();

		static Graph Forward, Backward;

		public static List<TrackPiece> Pathfind(TrackPiece origin, TrackPiece target, bool forwards)
		{
			var graph = forwards ? Forward : Backward;
			if (graph.TryFindPath(TrackPieces, origin, target, out var path))
				return path;
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
			SgtLogger.l("AAAAAAAAAAA");
			remaining.Remove(current);

			SgtLogger.l("BBBBBBB");
			var node = graph.CreateNode(current);
			SgtLogger.l(message: "CCCCCC");
			if (parentNode != null)
				parentNode.AddEdge(node, current.PathCost);
			SgtLogger.l(message: "DDDDD");

			if (childPiecesToCheck == null)
				return;

			foreach (var childConnection in childPiecesToCheck)
			{
				SgtLogger.l(message: "EEEEEE");
				if (childConnection.GetReachableConnectionsFrom(current, out var newChildren))
					CollectNodesRecursively(childConnection, node, remaining, newChildren, graph);
			}
		}

		public static void RegisterTrack(TrackPiece newTrackPiece)
		{
			TrackPieces.Add(newTrackPiece);

			if (newTrackPiece is TrackStation station)
				TrackStations.Add(station);

			int inputConnectsToCell = newTrackPiece.InputConnectionCell;
			int inputCell = newTrackPiece.InputCell;

			var outputCells = newTrackPiece.OutputCells;
			var outputConnectionCells = newTrackPiece.OutputConnectionCells;

			if (!CellsWithTrackPieceConnectors.ContainsKey(inputCell))
				CellsWithTrackPieceConnectors[inputCell] = new HashSet<TrackPiece>();

			if (!CellsWithTrackPieceConnectors.ContainsKey(inputConnectsToCell))
				CellsWithTrackPieceConnectors[inputConnectsToCell] = new HashSet<TrackPiece>();

			foreach (var existingTrackPiece in CellsWithTrackPieceConnectors[inputConnectsToCell])
			{
				if (existingTrackPiece.ConnectsFromTo(inputConnectsToCell, inputCell))
					ConnectTracks(newTrackPiece, existingTrackPiece, inputCell);
			}
			CellsWithTrackPieceConnectors[newTrackPiece.InputCell].Add(newTrackPiece);
			for(int i = 0; i< outputCells.Length; i++)
			{
				var outputCell = outputCells[i];
				var outputConnectionCell = outputConnectionCells[i];

				if (!CellsWithTrackPieceConnectors.ContainsKey(outputCell))
					CellsWithTrackPieceConnectors[outputCell] = new HashSet<TrackPiece>();
				if (!CellsWithTrackPieceConnectors.ContainsKey(outputConnectionCell))
					CellsWithTrackPieceConnectors[outputConnectionCell] = new HashSet<TrackPiece>();

				foreach (var existingTrackPiece in CellsWithTrackPieceConnectors[outputConnectionCell])
				{
					if (existingTrackPiece.ConnectsFromTo(inputConnectsToCell, inputCell))
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
