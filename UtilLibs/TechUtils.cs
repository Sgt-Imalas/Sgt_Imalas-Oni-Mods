using System.Collections.Generic;
using System.Linq;

namespace UtilLibs
{
	public static class TechUtils
	{
		public static void AddNode(
			ResourceTreeLoader<ResourceTreeNode> tech_tree_nodes_instance,
			string newTechId,
			string previousNode,
			float xDiff,
			float yDiff
			)
		{
			AddNode(tech_tree_nodes_instance, newTechId, new string[] { previousNode }, xDiff, yDiff);
		}

		public static void AddNode(
			ResourceTreeLoader<ResourceTreeNode> tech_tree_nodes_instance,
			string newTechId,
			string[] previousNodes,
			float xDiff,
			float yDiff
			)

		{
			const float X_Step = 350;
			const float Y_Step = 250;
			xDiff *= X_Step;
			yDiff *= Y_Step;

			ResourceTreeNode originNode = null;
			var prevItems = new List<ResourceTreeNode>();


			foreach (var item in tech_tree_nodes_instance)
			{
				if (previousNodes.Count() > 0 && item.Id == previousNodes.First())
				{
					originNode = item;
				}
				if (previousNodes.Contains(item.Id))
				{
					prevItems.Add(item);
				}
			}
			if (originNode == null)
			{
				return;
			}

			var id = newTechId;
			var node = new ResourceTreeNode
			{
				height = originNode.height,
				width = originNode.width,
				nodeX = originNode.nodeX + xDiff,
				nodeY = originNode.nodeY + yDiff,
				edges = new List<ResourceTreeNode.Edge>(originNode.edges),
				references = new List<ResourceTreeNode>() { },
				Disabled = false,
				Id = id,
				Name = id

			};

			foreach (var prevNode in prevItems)
			{
				prevNode.references.Add(node);
			}

			tech_tree_nodes_instance.resources.Add(node);
		}



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
				if (previousNodes.Count() > 0 && item.Id == previousNodes.First())
				{
					tempModNode = item;
					Debug.Log("X: " + item.nodeX + ", Y: " + item.nodeY);
				}
				if (previousNodes.Contains(item.Id))
				{
					prevItems.Add(item);
				}
				if (item.Id == y_coord_ref_techNode)
				{
					y = item.nodeY;
					Debug.Log("X: " + item.nodeX + ", Y: " + item.nodeY);
				}
				else if (item.Id == x_coord_ref_techNode)
				{
					x = item.nodeX;
					Debug.Log("X: " + item.nodeX + ", Y: " + item.nodeY);
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
				references = prevItems,
				Disabled = false,
				Id = id,
				Name = id

			};

			//foreach (var prevNode in prevItems)
			//{
			//	prevNode.references.Add(node);
			//}

			tech_tree_nodes_instance.resources.Add(node);
		}
	}
}
