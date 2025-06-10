using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainMod.Content.Scripts.PathSystem
{
	public static class TrackManager
	{
		static Dictionary<int, HashSet<TrackPiece>> CellsWithTrackPieceConnectors = new();
		public static List<TrackPiece> TrackPieces = new();
		public static HashSet<TrackStation> TrackStations = new();
		static Dictionary<TrackStation, List<TrackConnection>> Connections = new();

		public static void RecalculatePaths()
		{
			Connections.Clear();
			Connections = TrackConnection.PopulateConnections();
		}

		public static void RegisterTrack(TrackPiece newTrackPiece)
		{
			TrackPieces.Add(newTrackPiece);

			if(newTrackPiece is TrackStation station)
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
