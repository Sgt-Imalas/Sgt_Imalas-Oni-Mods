using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ForceFieldWallTile.Content.Scripts.MeshGen
{
	internal class Node
	{

		public Node(int cell)
		{
			Cell = cell;
			Strenght = 1f;
		}

		public int Cell;

		public float Strenght;
		public float MaxStrenght;

		public int bl_idx, br_idx, tl_idx, tr_idx;

		public float CurrentStrenght => Mathf.Clamp01(Strenght / MaxStrenght);

		public Color GetCurrentColor()
		{
			return ModAssets.ColorGradientForcefield.Evaluate(CurrentStrenght);
		}


		internal void SetVertexInfo(Vector3 bottomLeft, Vector3 topRight, Vector3 topLeft, Vector3 bottomRight, int bottomLeftIndex, int topRightIndex, int topLeftIndex, int bottomRightIndex)
		{
			bl_idx = bottomLeftIndex;
			br_idx = bottomRightIndex;
			tl_idx = topLeftIndex;
			tr_idx = topRightIndex;
		}
	}
}
