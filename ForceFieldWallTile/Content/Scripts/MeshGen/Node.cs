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

		public int bl_idx, br_idx, tl_idx, tr_idx;

		int debugCol = 0;
		Color DebugColor => debug[debugCol];

		Color[] debug = [Color.red, Color.yellow, Color.blue];
 		internal void Cycle()
		{
			if (debugCol >= debug.Length -1)
				return;
			debugCol ++;
			
			debugCol %= debug.Length;

			SgtLogger.l("Node Cycle " + Cell + " to " + DebugColor);
			ShieldGrid.RedrawColors(Cell);
		}
		public Color GetCurrentColor()
		{
			return DebugColor;
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
