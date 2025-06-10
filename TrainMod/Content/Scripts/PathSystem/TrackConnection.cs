using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainMod.Content.Scripts.PathSystem
{
    class TrackConnection
    {

        static int[,] ConnectionMatrix;
        public TrackStation Source, Destination;
        public int PathCost;
        public List<TrackPiece> Pieces;

        public static void BuildMatrix()
		{

			ConnectionMatrix = new int[TrackManager.TrackPieces.Count, TrackManager.TrackPieces.Count];

			for (int i = 0; i < TrackManager.TrackPieces.Count; i++)
			{
				var n1 = TrackManager.TrackPieces[i];

				for (int j = 0; j < TrackManager.TrackPieces.Count; j++)
				{
					var n2 = TrackManager.TrackPieces[j];

					var arc = n1.GetAllConnections().FirstOrDefault(a => a == n2);

					if (arc != null)
					{
						ConnectionMatrix[i, j] = arc.PathCost;
					}
				}
			}

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
		public static List<TrackConnection> WalkTrackPathsAStart(TrackStation source)
        {
			var result = new List<TrackConnection>();
            foreach(var output in source.GetAllConnections())
            {
            }

            return result;
        }
        static void WalkChildren(TrackPiece target, TrackPiece previous, HashSet<TrackPiece> walkedPieces, List<Tuple<TrackPiece,TrackPiece>> backtracking)
        {
            walkedPieces.Add(target);
            backtracking.Add()

            foreach(var connected in target.GetAllConnections()) 
                WalkChildren(connected, target, walkedPieces, backtracking);
        }


		internal static Dictionary<TrackStation, List<TrackConnection>> PopulateConnections()
		{
            var result = new Dictionary<TrackStation, List<TrackConnection>>();
            foreach(var station in TrackManager.TrackStations)
            {
                result[station]  = WalkTrackPathsAStart(station);
			}
            return result;

		}
	}
}
