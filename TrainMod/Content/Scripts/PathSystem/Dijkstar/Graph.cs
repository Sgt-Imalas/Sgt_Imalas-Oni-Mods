using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem.Segmentation;

namespace TrainMod.Content.Scripts.PathSystem.Dijkstar
{
	class Graph
	{
		public List<Node> AllNodes = new List<Node>();


		public Node CreateNode(TrackPiece track)
		{
			var n = new Node(track);
			AllNodes.Add(n);
			return n;
		}

		public int?[,] CreateAdjMatrix()
		{
			int?[,] adj = new int?[AllNodes.Count, AllNodes.Count];
			for (int i = 0; i < AllNodes.Count; i++)
			{
				Node node1 = AllNodes[i];
				for (int j = 0; j < AllNodes.Count; j++)
				{
					Node node2 = AllNodes[j];
					var edge = node1.Edges.FirstOrDefault(a => a.Child == node2);
					if (edge != null)
					{
						adj[i, j] = edge.Weight;
					}
					else
					{
						adj[i, j] = 0;
					}
				}
			}
			return adj;
		}
		public int miniDist(int[] distance, bool[] tset)
		{
			int minimum = int.MaxValue;
			int index = 0;
			for (int k = 0; k < distance.Length; k++)
			{
				if (!tset[k] && distance[k] <= minimum)
				{
					minimum = distance[k];
					index = k;
				}
			}
			return index;
		}
		public List<int> Dijkstar(int?[,] graph, int src, int dest)
		{
			int length = graph.GetLength(0);
			int[] distance = new int[length];
			bool[] used = new bool[length];
			int[] prev = new int[length];

			for (int i = 0; i < length; i++)
			{
				distance[i] = int.MaxValue;
				used[i] = false;
				prev[i] = -1;
			}
			distance[src] = 0;

			for (int k = 0; k < length - 1; k++)
			{
				int minNode = miniDist(distance, used);
				used[minNode] = true;
				for (int i = 0; i < length; i++)
				{
					if (graph[minNode, i] > 0)
					{
						int shortestToMinNode = distance[minNode];
						int? distanceToNextNode = (int?)graph[minNode, i];
						int? totalDistance = shortestToMinNode + distanceToNextNode;
						if (totalDistance < distance[i])
						{
							distance[i] = (int)totalDistance;
							prev[i] = minNode;
						}
					}
				}
			}
			if (distance[dest] == int.MaxValue)
			{
				return new List<int>();
			}
			var path = new LinkedList<int>();
			int currentNode = dest;
			while (currentNode != -1)
			{
				path.AddFirst(currentNode);
				currentNode = prev[currentNode];
			}
			return path.ToList();
		}
		//public void PrintMatrix(ref int?[,] matrix, string[] labels, int count)
		//{
		//	Console.Write("       ");
		//	for (int i = 0; i < count; i++)
		//	{
		//		Console.Write($" {labels[i]} ");
		//	}
		//	Console.WriteLine();

		//	for (int i = 0; i < count; i++)
		//	{
		//		Console.Write($" {labels[i]} | [ ");

		//		for (int j = 0; j < count; j++)
		//		{
		//			if (matrix[i, j] == null)
		//			{
		//				Console.Write(" ,");
		//			}
		//			else
		//			{
		//				Console.Write($" {matrix[i, j]},");
		//			}

		//		}
		//		Console.Write(" ]\r\n");
		//	}
		//	Console.Write("\r\n");
		//}
		//public bool TryFindPath(ref int?[,] graph, List<TrackPiece> tracks, TrackPiece src, TrackPiece dest, out List<TrackPiece> path)
		public bool TryFindPath( List<TrackPiece> tracks, TrackPiece src, TrackPiece dest, out List<TrackPiece> path)
		{
			var graph = CreateAdjMatrix();
			int source = tracks.IndexOf( src);
			int destination = tracks.IndexOf(dest);

			//Console.Write($" Shortest Path of [{src} -> {dest}] is : ");
			var paths = Dijkstar(graph, source, destination);

			if (paths.Count > 0)
			{
				int? path_length = 0;
				path = new();
				for (int i = 0; i < paths.Count - 1; i++)
				{
					int? length = (int?)graph[paths[i], paths[i + 1]];
					path_length += length;
					Console.Write($"{Grid.CellToXY(Grid.PosToCell(tracks[paths[i]]))} [{length}] -> ");
					path.Add(tracks[paths[i]]);
				}
				Console.WriteLine($"{tracks[destination]} (Distance {path_length})");
				return true;
			}
			else
			{
				path = null;
				Console.WriteLine("No Path");
				return false;
			}
		}
	}
}
