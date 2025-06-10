using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainMod.Content.Scripts.PathSystem.Segmentation
{
	public class SegmentDivider : ISegmentDivider
	{
		public List<ITrackSegment> AttachedSegments => _attachedSegments;
		private List<ITrackSegment> _attachedSegments = new();
		public void RemoveSegment(ITrackSegment segment)
		{
			_attachedSegments.Remove(segment);
		}
		public void AddSegment(ITrackSegment segment)
		{
			if (_attachedSegments.Contains(segment)) return;
			_attachedSegments.Add(segment);
		}
	}
}
