using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatelites.Behaviours
{
    public static class ModAssets
    {
        public static Components.Cmps<SatelliteGridEntity> Satellites = new Components.Cmps<SatelliteGridEntity>();

        public static Graph AdjazenzMatrixHolder = new Graph();

        public static void RedoAdjacencyMatrix()
        {
            AdjazenzMatrixHolder.UpdateAdjacencyMatrix();
        }
        public static bool FindConnectionViaAdjacencyMatrix(AxialI a, AxialI b)
        {
            bool HasConnection = false;
            AdjazenzMatrixHolder.AddItemToGraph(a);
            AdjazenzMatrixHolder.AddItemToGraph(b);
            //Add ConnectionSearch Here!!!!
            AdjazenzMatrixHolder.RemoveItemTFromGraph(a);
            AdjazenzMatrixHolder.RemoveItemTFromGraph(b);
            return HasConnection;
        }

        public static string GetSatelliteNameRandom()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var stringChars = new char[3];

            string returnString = string.Empty;
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            int number = random.Next(0, 999);
            returnString = new string(stringChars) + "-" + number.ToString("D3");
            return returnString;
        }
        public static class Tags
        {
           public static Tag LS_Satellite = TagManager.Create("LS_Space_Satellite");
        }

        public class Graph
        {
            public List<Node> AllNodes = new List<Node>();
            public bool?[,] AdjacencyMatrix;

            public Node CreateNode(AxialI location)
            {
                var n = new Node(location);
                if (!AllNodes.Contains(n)) { 
                    AllNodes.Add(n);
                }
                return n;
            }
            public void AddNodePair(AxialI location1, AxialI location2)
            {
                var n = new Node(location1);
                if (!AllNodes.Contains(n))
                {
                    AllNodes.Add(n);
                }
                var m = new Node(location2);
                if (!AllNodes.Contains(m))
                {
                    AllNodes.Add(m);
                }
                n.AddLink(m);
            }


            public void UpdateAdjacencyMatrix()
            {
                var nodes = Satellites.Items.Select(c => c.Location).ToList();
                foreach(var node in nodes)
                {
                    foreach(var nodeTarget in nodes)
                    {
                        if(node != nodeTarget)
                        {
                            int length = AxialUtil.GetDistance(node, nodeTarget);
                            if (length >0 && length <= Config.Instance.SatelliteLogicRange)
                            {
                                AddNodePair(node, nodeTarget);
                            }
                        }
                    }
                }
                AdjacencyMatrix = CreateAdjMatrix();
            } 
            public void AddItemToGraph(AxialI item)
            {
                if(AllNodes.Find(f=> f.Satellite == item) != null)
                {
                    return;
                }
                var newNode = CreateNode(item);
                foreach (var node in AllNodes)
                {
                    if (node != newNode)
                    {
                        int length = AxialUtil.GetDistance(node.Satellite, item);
                        if (length > 0 && length <= Config.Instance.SatelliteLogicRange)
                        {
                            node.AddLink(newNode);
                        }
                    }
                }
            }
            public void RemoveItemTFromGraph(AxialI item)
            {
                var nodeToRemove = AllNodes.Find(f => f.Satellite == item);
                if (nodeToRemove == null)
                {
                    return;
                }
                foreach (var link in nodeToRemove.Links)
                {
                    link.Child.RemoveLink(nodeToRemove);
                }
                AllNodes.Remove(nodeToRemove);
            }


            public bool?[,] CreateAdjMatrix()
            {
                bool?[,] adj = new bool?[AllNodes.Count, AllNodes.Count];

                for (int i = 0; i < AllNodes.Count; i++)
                {
                    Node n1 = AllNodes[i];

                    for (int j = 0; j < AllNodes.Count; j++)
                    {
                        Node n2 = AllNodes[j];

                        var arc = n1.Links.FirstOrDefault(a => a.Child == n2);

                        if (arc != null)
                        {
                            adj[i, j] = true;
                        }
                    }
                }
                return adj;
            }
        }

        public class Node
        {
            public AxialI Satellite;
            public List<Link> Links = new List<Link>();
            public Node(AxialI sat)
            {
                Satellite = sat;
            }
            public void RemoveLink(Node ch)
            {
                var LinkToRemove = Links.Find(f => f.Child == ch);
                Links.Remove(LinkToRemove);

            }
            public Node AddLink(Node child)
            {
                if(!Links.Exists(a => a.Child == child && a.Parent == this)) {
                Links.Add(new Link
                {
                    Parent = this,
                    Child = child,
                });
                }
                if (!child.Links.Exists(a => a.Parent == child && a.Child == this))
                {
                    child.AddLink(this);
                }

                return this;
            }

        }
        public class Link
        {
            public Node Parent;
            public Node Child;
        }
    }
}
