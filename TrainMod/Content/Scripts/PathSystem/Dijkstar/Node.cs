using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Segmentation;

namespace TrainMod.Content.Scripts.PathSystem.Dijkstar
{
	public class Node
	{
		public TrackPiece Track;
		public Node current_node;
		public List<Edge> Edges = new List<Edge>();
		
		

		public Node(TrackPiece track)
		{
			this.Track = track;
			current_node = this;
		}
		public Node AddEdge(Node child, int weight)
		{
			Edges.Add(new Edge()
			{
				Parent = current_node,
				Child = child,
				Weight = weight
			});
			///no bi-directional connections automatically!
			///we use 2 separate Graphs for that
			
			//if (!child.Edges.Exists(a => a.Parent == child && a.Child == current_node))
			//{
			//	child.AddEdge(current_node, weight);
			//}
			return current_node;
		}
	}
}
