using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainMod.Content.Scripts.PathSystem.Segmentation
{
	class RailSegment : ITrackSegment
	{
		public List<TrackPiece> Tracks;
		public TrackPiece StartSegment => Tracks != null ? Tracks.First() : null;
		public TrackPiece EndSegment => Tracks != null ? Tracks.Last() : null;

		public int GetSegmentCosts()
		{
			int costs = 0;
			foreach (TrackPiece track in Tracks)
				costs += track.PathCost;
			if (SegmentOccupied())
				costs *= 2;
			return costs;
		}
		bool SegmentOccupied() => false;
	}
}
