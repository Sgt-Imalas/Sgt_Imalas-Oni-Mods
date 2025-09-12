using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.DUPLICANTS.ATTRIBUTES;
using static STRINGS.UI.CLUSTERMAP;

namespace ForceFieldWallTile.Content.Scripts.MeshGen
{
	internal class ShieldGrid
	{
		public static Dictionary<int, Node> ShieldNodes = [];

		public static GameObject RendererGO;
		public static MeshFilter Filter;
		public static MeshRenderer Renderer;


		private float scale = -1;
		private Vector2 gridscale = new(1f, 1f);

		public static void AddNode(int cell, Node node)
		{
			if (!ShieldNodes.ContainsKey(cell))
			{
				ShieldNodes.Add(cell, node);
				GenerateMesh();
			}
		}
		public static void RemoveNode(int cell, Node node)
		{
			if (ShieldNodes.ContainsKey(cell))
			{
				ShieldNodes.Remove(cell);
				GenerateMesh();
			}
		}

		static List<Color> Colors = [];

		public static void GenerateMesh()
		{
			var vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> uvs = new List<Vector2>();
			Colors.Clear();
			foreach (var cell in ShieldNodes)
			{
				AddNodeVertices(vertices, triangles, uvs, cell.Key, cell.Value);
			}

			var mesh = new Mesh();
			mesh.SetVertices(vertices);
			mesh.SetTriangles(triangles, 0);
			mesh.SetUVs(0, uvs);
			if (RendererGO != null)
			{
				GameObject.Destroy(RendererGO);
			}
			RendererGO = new GameObject("ShieldMesh", typeof(MeshFilter), typeof(MeshRenderer));
			RendererGO.SetActive(true);
			Filter = RendererGO.GetComponent<MeshFilter>();
			Renderer = RendererGO.GetComponent<MeshRenderer>();
			Renderer.transform.position = new(0, 0, Grid.GetLayerZ(Grid.SceneLayer.Ground));

			Colors = new List<Color>(vertices.Count);
			for (int i = 0; i < vertices.Count; i++)
			{
				var color = UIUtils.HSVShift(Color.red, UnityEngine.Random.value * 100); // random tint
				Colors.Add(color);
			}

			foreach (var nodecell in ShieldNodes.Keys)
			{
				RedrawColors(nodecell, false);
			}

			mesh.SetColors(Colors);



			Filter.mesh = mesh;

			//Renderer.material = new Material(Shader.Find("Sprites/Default"))
			Renderer.material = new(ModAssets.ForceFieldMaterial)
			{
				renderQueue = RenderQueues.Liquid + 1
			};


			//Renderer.material.color = new Color(0.2f, 0.6f, 1f, 0.4f);
		}

		public static void AddNodeVertices(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, int cell, Node node)
		{
			var bottomLeft = Grid.CellToPos(cell);
			var topRight = Grid.CellToPos(Grid.OffsetCell(cell, new(1, 1)));
			var topLeft = Grid.CellToPos(Grid.OffsetCell(cell, new(0, 1)));
			var bottomRight = Grid.CellToPos(Grid.OffsetCell(cell, new(1, 0)));


			uvs.Add(new Vector2(0, 0));
			vertices.Add(bottomLeft);
			int bottomLeftIndex = vertices.Count - 1;


			uvs.Add(new Vector2(1, 1));
			vertices.Add(topRight);
			int topRightIndex = vertices.Count - 1;


			uvs.Add(new Vector2(0, 1));
			vertices.Add(topLeft);
			int topLeftIndex = vertices.Count - 1;

			uvs.Add(new Vector2(1, 0));
			vertices.Add(bottomRight);
			int bottomRightIndex = vertices.Count - 1;

			// Add two triangles for the square
			triangles.Add(topLeftIndex);
			triangles.Add(topRightIndex);
			triangles.Add(bottomLeftIndex);

			triangles.Add(bottomLeftIndex);
			triangles.Add(topRightIndex);
			triangles.Add(bottomRightIndex);


			node.SetVertexInfo(bottomLeft, topRight, topLeft, bottomRight, bottomLeftIndex, topRightIndex, topLeftIndex, bottomRightIndex);
		}


		public static UtilityConnections GetCellConnections(int cell)
		{
			UtilityConnections con = (UtilityConnections)0;
			if (!ShieldNodes.ContainsKey(cell))
			{
				return con;
			}
			if (ShieldNodes.ContainsKey(Grid.CellLeft(cell)))
				con |= UtilityConnections.Left;
			if (ShieldNodes.ContainsKey(Grid.CellRight(cell)))
				con |= UtilityConnections.Right;
			if (ShieldNodes.ContainsKey(Grid.CellAbove(cell)))
				con |= UtilityConnections.Up;
			if (ShieldNodes.ContainsKey(Grid.CellBelow(cell)))
				con |= UtilityConnections.Down;
			return con;
		}

		internal static void RedrawColors(int cell, bool apply = true)
		{
			if (!ShieldNodes.TryGetValue(cell, out var node))
			{
				return;
			}

			//var nodeColor = node.GetCurrentColor();

			int left = Grid.CellLeft(cell);
			int bottomLeft = Grid.CellDownLeft(cell);
			int topLeft = Grid.CellUpLeft(cell);
			int right = Grid.CellRight(cell);
			int bottomRight = Grid.CellDownRight(cell);
			int topRight = Grid.CellUpRight(cell);
			int top = Grid.CellAbove(cell);
			int bottom = Grid.CellBelow(cell);


			Node
				lowerStrength_bottomLeft = node,
				lowerStrength_left = node,
				lowerStrength_topLeft = node,
				lowerStrength_top = node,
				lowerStrength_topRight = node,
				lowerStrength_right = node,
				lowerStrength_bottomRight = node,
				lowerStrength_bottom = node;


			//Color
			//	bottomLeftCol = nodeColor,
			//	leftCol = nodeColor,
			//	topLeftCol = nodeColor,
			//	topCol = nodeColor,
			//	topRightCol = nodeColor,
			//	rightCol = nodeColor,
			//	bottomRightCol = nodeColor,
			//	bottomCol = nodeColor;

			Node GetLowerStrenghtNode(Node other)
			{
				if (other == null || other.CurrentStrenght > node.CurrentStrenght)
					return node;
				return other;
			}
			Node GetLowerStrenghtNodeMult(Node first, Node second, Node third)
			{
				if (first == null && second == null && third == null)
					return node;

				var ret = node;

				float firstVal = first == null ? float.MaxValue : first.CurrentStrenght;
				float secondVal = second == null ? float.MaxValue : second.CurrentStrenght;
				float thirdVal = third == null ? float.MaxValue : third.CurrentStrenght;

				if (firstVal < node.CurrentStrenght)
					ret = first;
				if (secondVal < firstVal)
					ret = second;
				if(thirdVal < secondVal)
					ret = third;
				return ret;
			}



			if (ShieldNodes.TryGetValue(left, out var leftNode))
			{
				lowerStrength_left = GetLowerStrenghtNode(leftNode);
				//leftCol = GetLowerValueColor(leftNode);
			}
			if (ShieldNodes.TryGetValue(right, out var rightNode))
			{
				lowerStrength_right = GetLowerStrenghtNode(rightNode);
				//rightCol = GetLowerValueColor(rightNode);
			}
			if (ShieldNodes.TryGetValue(top, out var topNode))
			{
				lowerStrength_top = GetLowerStrenghtNode(topNode);
				//topCol = GetLowerValueColor(topNode);
			}
			if (ShieldNodes.TryGetValue(bottom, out var bottomNode))
			{
				lowerStrength_bottom = GetLowerStrenghtNode(bottomNode);
				//bottomCol = GetLowerValueColor(bottomNode);
			}
			if (ShieldNodes.TryGetValue(bottomLeft, out var bottomLeftNode))
			{
				lowerStrength_bottomLeft = GetLowerStrenghtNode(bottomLeftNode);
				//bottomLeftCol = GetLowerValueColor(bottomLeftNode);
			}
			if (ShieldNodes.TryGetValue(topLeft, out var topLeftNode))
			{
				lowerStrength_topLeft = GetLowerStrenghtNode(topLeftNode);
				//topLeftCol = GetLowerValueColor(topLeftNode);
			}
			if (ShieldNodes.TryGetValue(topRight, out var topRightNode))
			{
				lowerStrength_topRight = GetLowerStrenghtNode(topRightNode);
				//topRightCol = GetLowerValueColor(topRightNode);
			}
			if (ShieldNodes.TryGetValue(bottomRight, out var bottomRightNode))
			{
				lowerStrength_bottomRight = GetLowerStrenghtNode(bottomRightNode);
				//bottomRightCol = GetLowerValueColor(bottomRightNode);
			}

			var bl_col = GetLowerStrenghtNodeMult(lowerStrength_bottom, lowerStrength_bottomLeft, lowerStrength_left).GetCurrentColor();// Color.Lerp(Color.Lerp(bottomCol, leftCol, 0.5f), Color.Lerp(bottomLeftCol, nodeColor, 0.5f), 0.5f);
			var tl_col = GetLowerStrenghtNodeMult(lowerStrength_top, lowerStrength_topLeft, lowerStrength_left).GetCurrentColor(); //Color.Lerp(Color.Lerp(topCol, leftCol, 0.5f), Color.Lerp(topLeftCol, nodeColor, 0.5f), 0.5f);
			var tr_col = GetLowerStrenghtNodeMult(lowerStrength_top, lowerStrength_topRight, lowerStrength_right).GetCurrentColor(); //Color.Lerp(Color.Lerp(topCol, rightCol, 0.5f), Color.Lerp(topRightCol, nodeColor, 0.5f), 0.5f);
			var br_col = GetLowerStrenghtNodeMult(lowerStrength_bottom, lowerStrength_bottomRight, lowerStrength_right).GetCurrentColor(); //Color.Lerp(Color.Lerp(bottomCol, rightCol, 0.5f), Color.Lerp(bottomRightCol, nodeColor, 0.5f), 0.5f);

			Colors[node.bl_idx] = bl_col;
			Colors[node.tl_idx] = tl_col;
			Colors[node.tr_idx] = tr_col;
			Colors[node.br_idx] = br_col;

			if (leftNode != null)
			{
				Colors[leftNode.br_idx] = bl_col;
				Colors[leftNode.tr_idx] = tl_col;
			}
			if (rightNode != null)
			{
				Colors[rightNode.bl_idx] = br_col;
				Colors[rightNode.tl_idx] = tr_col;
			}
			if (topNode != null)
			{
				Colors[topNode.bl_idx] = tl_col;
				Colors[topNode.br_idx] = tr_col;
			}
			if (bottomNode != null)
			{
				Colors[bottomNode.tl_idx] = bl_col;
				Colors[bottomNode.tr_idx] = br_col;
			}
			if (bottomLeftNode != null)
			{
				Colors[bottomLeftNode.tr_idx] = bl_col;
			}
			if (topLeftNode != null)
			{
				Colors[topLeftNode.br_idx] = tl_col;
			}
			if (topRightNode != null)
			{
				Colors[topRightNode.bl_idx] = tr_col;
			}
			if (bottomRightNode != null)
			{
				Colors[bottomRightNode.tl_idx] = br_col;
			}
			if (apply)
				Filter.mesh.SetColors(Colors);

		}
	}
}
