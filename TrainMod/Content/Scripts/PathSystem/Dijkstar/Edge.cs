using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Segmentation;

namespace TrainMod.Content.Scripts.PathSystem.Dijkstar
{
	public class Edge
	{
		public int Weight;
		public ITrackSegment Segment;
		public Node? Parent;
		public Node? Child;
	}
}
