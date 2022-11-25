using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public static class TechUtils
    {
        public static void AddNode(
            ResourceTreeLoader<ResourceTreeNode> tech_tree_nodes_instance,
            string newTechId,
            string previousNode,
            string x_coord_ref_techNode,
            string y_coord_ref_techNode
            )
        {
            AddNode(tech_tree_nodes_instance, newTechId, new string[] { previousNode }, x_coord_ref_techNode, y_coord_ref_techNode);
        }


        public static void AddNode(
            ResourceTreeLoader<ResourceTreeNode> tech_tree_nodes_instance, 
            string newTechId,
            string[] previousNodes,
            string x_coord_ref_techNode, 
            string y_coord_ref_techNode
            )
        
        {
            ResourceTreeNode tempModNode = null;
            var x = 0f;
            var y = 0f;
            var prevItems = new List<ResourceTreeNode>();


            foreach (var item in tech_tree_nodes_instance)
            {
                if (previousNodes.Count()>0&&item.Id == previousNodes.First())
                {
                    tempModNode = item;
                }
                if (previousNodes.Contains(item.Id)){
                    prevItems.Add(item);
                }
                if (item.Id == y_coord_ref_techNode)
                {
                    y = item.nodeY;
                }
                else if (item.Id == x_coord_ref_techNode)
                {
                    x = item.nodeX;
                }
            }
            if (tempModNode == null)
            {
                return;
            }

            var id = newTechId;
            var node = new ResourceTreeNode
            {
                height = tempModNode.height,
                width = tempModNode.width,
                nodeX = x,
                nodeY = y,
                edges = new List<ResourceTreeNode.Edge>(tempModNode.edges),
                references = new List<ResourceTreeNode>() { },
                Disabled = false,
                Id = id,
                Name = id

            };

            foreach(var prevNode in prevItems)
            {
                prevNode.references.Add(node);
            }

            tech_tree_nodes_instance.resources.Add(node);
        }
    }
}
