using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Segmentation;

namespace TrainMod.Content.Scripts.PathSystem.Dijkstar
{
	public class Edge : IEqualityComparer<Edge>
	{
		public int Weight;
		//public ITrackSegment Segment;
		public Node? Parent;
		public Node? Child;


		#region equality
		public bool Equals(Edge other)
		{
			return other.Parent == this.Parent && other.Child == this.Child;
		}
		public override bool Equals(object obj) => obj is Node other && Equals(other);

		public static bool operator ==(Edge a, Edge b) => a?.Parent == b?.Parent && a?.Child == b?.Child;
		public static bool operator !=(Edge a, Edge b) => !(a == b);
		public override int GetHashCode()
		{
			return Parent.GetHashCode() ^ Child.GetHashCode();
		}

		public bool Equals(Edge x, Edge y) => x == y;

		public int GetHashCode(Edge obj) => obj.GetHashCode();
		#endregion
	}
}
