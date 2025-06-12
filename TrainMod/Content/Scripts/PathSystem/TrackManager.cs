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
			var trackPiecesToSort2 = (TrackPieces).ToHashSet();

			Forward = new Graph(); Backward = new Graph();

			while (trackPiecesToSort.Any())
			{
				var root = trackPiecesToSort.First();

				var rootNode = Forward.CreateNode(root);
				CollectNodesRecursively(root, rootNode, trackPiecesToSort, root.GetInputConnections(), Forward);

			}
			while (trackPiecesToSort2.Any())
			{
				var root2 = trackPiecesToSort2.First();
				var root2Node = Backward.CreateNode(root2);
				CollectNodesRecursively(root2, root2Node, trackPiecesToSort2, root2.GetOutputConnections(), Backward);
			}
		}
		static void CollectNodesRecursively(TrackPiece current, Node parentNode, HashSet<TrackPiece> remaining, List<TrackPiece> childPiecesToCheck, Graph graph)
		{
			SgtLogger.l(current.GetProperName() + " at " + Grid.PosToCell(current) + ": " + childPiecesToCheck.Count + " side nodes");
			remaining.Remove(current);

			var node = graph.CreateNode(current);
			if (parentNode != null)
				parentNode.AddEdge(node, current.PathCost);

			if (childPiecesToCheck == null)
				return;

			foreach (var childConnection in childPiecesToCheck)
			{
				//SgtLogger.l("child node " + global::STRINGS.UI.StripLinkFormatting(childConnection.GetProperName()) + " at cell: " + Grid.PosToCell(childConnection));
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
					ConnectTracks(newTrackPiece, existingTrackPiece, inputCell, inputConnectsToCell);
			}
			CellsWithTrackPieceConnectors[inputCell].Add(newTrackPiece);
			for (int i = 0; i < outputCells.Length; i++)
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
						ConnectTracks(newTrackPiece, existingTrackPiece, outputCell, inputConnectsToCell);
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
		public static void ConnectTracks(TrackPiece track1, TrackPiece track2, int cell, int connectionCell)
		{

			if (track1 == track2) return;
			SgtLogger.l("connecting " + track1 + " and " + track2);

			track1.AddConnection(track2, cell, connectionCell);
			track2.AddConnection(track1, cell, connectionCell);
		}
	}
}
