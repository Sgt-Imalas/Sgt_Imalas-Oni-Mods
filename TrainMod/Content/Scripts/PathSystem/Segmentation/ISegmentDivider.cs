using System.Collections.Generic;

namespace TrainMod.Content.Scripts.PathSystem.Segmentation
{
    public interface ISegmentDivider
    {
        List<ITrackSegment> AttachedSegments { get; }

    }
}
