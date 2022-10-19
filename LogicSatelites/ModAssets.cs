using LogicSatellites.Buildings;
using LogicSatellites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static LogicSatellites.STRINGS.ITEMS;

namespace LogicSatellites.Behaviours
{
    public static class ModAssets
    {
        public static Components.Cmps<SatelliteGridEntity> Satellites = new Components.Cmps<SatelliteGridEntity>();
        public static Components.Cmps<SolarReciever> SolarRecievers = new Components.Cmps<SolarReciever>();

        public static Graph AdjazenzMatrixHolder = new Graph();

        public static void RedoAdjacencyMatrix()
        {
            AdjazenzMatrixHolder.UpdateAdjacencyMatrix();
        }


        public static bool FindConnectionViaAdjacencyMatrix(AxialI a, AxialI b)
        {
            bool HasConnection = false;
            Node A = AdjazenzMatrixHolder.AddItemToGraph(a);
            Node B = AdjazenzMatrixHolder.AddItemToGraph(b);
            HasConnection = AdjazenzMatrixHolder.PathFinding(A,B);
            AdjazenzMatrixHolder.RemoveItemTFromGraph(A);
            AdjazenzMatrixHolder.RemoveItemTFromGraph(B);
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

        public enum SatType
        {
            Exploration = 0,
            SolarLens = 1,
            RadLens = 2,
            DysonComponent = 3
        }
        public static TechItem ExplorationSatellite;
        public static TechItem SolarSatellite;


        public static Dictionary<int, SatelliteConfiguration> SatelliteConfigurations = new Dictionary<int, SatelliteConfiguration>()
        {
            {(int)SatType.Exploration,
                new SatelliteConfiguration(
                    SATELLITE.SATELLITETYPES.EXPLORATION+SATELLITE.TITLE,
                    SATELLITE.DESC+SATELLITE.SATELLITETYPES.EXPLORATIONDESC,
                    SatelliteGridConfig.ID,
                    DeployLocation.anywhere,
                    GameStrings.Technology.Computers.SensitiveMicroimaging,
                    "LS_Exploration_Sat")
            },
            {(int)SatType.SolarLens,
                new SatelliteConfiguration(
                    SATELLITE.SATELLITETYPES.SOLAR+SATELLITE.TITLE,
                    SATELLITE.DESC+SATELLITE.SATELLITETYPES.SOLARDESC,
                    SatelliteGridSolarConfig.ID,
                    DeployLocation.orbital,
                    GameStrings.Technology.Power.ImprovedHydrocarbonPropulsion,
                    "LS_Solar_Sat")
            }
        };

        public struct SatelliteConfiguration
        {
            public string NAME;
            public string DESC;
            public string GridID;
            public DeployLocation AllowedLocation;
            public string TechId;
            public string TechItemId;

            public SatelliteConfiguration(string name, string desc, string id, DeployLocation loc, string tech, string itemId)
            {
                NAME = name;
                DESC = desc;
                GridID = id;
                AllowedLocation = loc;
                TechId = tech;
                TechItemId = itemId;
            }
        }
        public enum DeployLocation
        {
            anywhere = 0,
            orbital = 1,
            deepSpace = 2,
            temporalTear = 3
        }


        public class Graph
        {
            public List<Node> AllNodes = new List<Node>();
            public bool?[,] AdjacencyMatrix;

            public bool PathFinding(Node startNode, Node TargetNode)
            {
                var openList = new List<Node>();
                var closedList = new List<Node>();
                openList.Add(startNode);
                while (openList.Count > 0)
                {
                    var currenNode = openList.First();
                    

                    if(currenNode == TargetNode)
                    {
                        return true;
                    }
                    else
                    {
                        foreach(var link in currenNode.Links)
                        {
                            if (!closedList.Contains(link.Child)){
                                 openList.Add(link.Child);
                            }
                        }
                        openList.RemoveAt(0);
                        closedList.Add(currenNode);
                    }
                }
                return false;

            }

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
            public Node AddItemToGraph(AxialI item)
            {
                if(AllNodes.Find(f=> f.Satellite == item) != null)
                {
                    return AllNodes.Find(f => f.Satellite == item);
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
                return newNode;
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
            public void RemoveItemTFromGraph(Node item)
            {
                if (!AllNodes.Contains(item))
                {
                    return;
                }
                foreach (var link in item.Links)
                {
                    link.Child.RemoveLink(item);
                }
                AllNodes.Remove(item);
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
