using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Segmentation;

namespace TrainMod.Content.Scripts.PathSystem.Dijkstar
{
	public class Node : IEqualityComparer<Node>
	{
		public TrackPiece Track;
		public bool ConnectionIsInputToOutput;

		public List<Edge> Edges = new List<Edge>();

		public Node(TrackPiece track, bool inToOut)
		{
			this.Track = track;
			this.ConnectionIsInputToOutput = inToOut;
		}
		public Node AddNewEdge(Node child, int weight)
		{
			var edge = new Edge()
			{
				Parent = this,
				Child = child,
				Weight = weight
			};

			if (!Edges.Contains(edge))
				Edges.Add(edge);
			///no bi-directional connections automatically!
			///we use 2 separate Graphs for that

			//if (!child.Edges.Exists(a => a.Parent == child && a.Child == current_node))
			//{
			//	child.AddEdge(current_node, weight);
			//}
			return this;
		}

		internal int RemoveAllEdgesTo(TrackPiece trackPiece)
		{
			return Edges.RemoveAll(edge => edge.Parent.Track == trackPiece || edge.Child.Track == trackPiece);
		}
		public override string ToString()
		{
			return Track.ToString() + ", in2out: " + ConnectionIsInputToOutput;
		}

		#region equality
		public bool Equals(Node other)
		{
			return other.Track == this.Track && other.ConnectionIsInputToOutput == this.ConnectionIsInputToOutput;
		}
		public override bool Equals(object obj) => obj is Node other && Equals(other);

		public static bool operator ==(Node a, Node b) => a?.Track == b?.Track && a?.ConnectionIsInputToOutput == b?.ConnectionIsInputToOutput;
		public static bool operator !=(Node a, Node b) => !(a == b);
		public override int GetHashCode()
		{
			return Track.Guid.GetHashCode() ^ ConnectionIsInputToOutput.GetHashCode();
		}

		public bool Equals(Node x, Node y) => x == y;

		public int GetHashCode(Node obj) => obj.GetHashCode();

		#endregion
	}
}
