using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainMod.Content.Scripts.PathSystem
{
	class TrackConnection
	{

		static bool?[,] ConnectionMatrix;

		public static bool PathFinding(TrackPiece startNode, TrackPiece TargetNode, out List<TrackPiece> validPath)
		{
			validPath = null;
			var openList = new List<TrackPiece>();
			var closedList = new List<TrackPiece>();
			var nodesWithParents = new List<Tuple<TrackPiece, TrackPiece>>();


			openList.Add(startNode);
			while (openList.Count > 0)
			{
				var currenNode = openList.First();


				if (currenNode == TargetNode)
				{
					validPath = [currenNode];
					var backtrackingNode = currenNode;
					while (nodesWithParents.Any(nod => nod.first == backtrackingNode))
					{
						backtrackingNode = nodesWithParents.First(nod => nod.first == backtrackingNode).second;
						validPath.Add(backtrackingNode);
					}

					return true;
				}
				else
				{
					foreach (var link in currenNode.GetAllConnections())
					{
						if (!closedList.Contains(link))
						{
							openList.Add(link);
							nodesWithParents.Add(new Tuple<TrackPiece, TrackPiece>(link, currenNode));
						}
					}
					openList.RemoveAt(0);
					closedList.Add(currenNode);
				}
			}
			return false;

		}
		public static void BuildMatrix()
		{

			ConnectionMatrix = new bool?[TrackManager.TrackPieces.Count, TrackManager.TrackPieces.Count];

			for (int i = 0; i < TrackManager.TrackPieces.Count; i++)
			{
				var n1 = TrackManager.TrackPieces[i];

				for (int j = 0; j < TrackManager.TrackPieces.Count; j++)
				{
					var n2 = TrackManager.TrackPieces[j];

					var arc = n1.GetAllConnections().FirstOrDefault(a => a == n2);

					if (arc != null)
					{
						ConnectionMatrix[i, j] = true;
					}
				}
			}

		}
	}
}
