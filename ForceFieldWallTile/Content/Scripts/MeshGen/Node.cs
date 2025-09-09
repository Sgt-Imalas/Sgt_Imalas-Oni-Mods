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
		static Gradient ColorGradient;
		static Node()
		{
			ColorGradient = new Gradient();
			GradientColorKey[] colors = [
					new GradientColorKey(Color.red, 0.0f),
					new GradientColorKey(Color.yellow, 0.5f),
					new GradientColorKey(UIUtils.rgb(61, 142, 255), 1.0f)];

			// Blend alpha from opaque at 0% to transparent at 100%
			GradientAlphaKey[] alphas = [
					new GradientAlphaKey(1.0f, 0.0f),
					new GradientAlphaKey(1.0f, 0.5f),
					new GradientAlphaKey(1.0f, 1.0f)
				];
			ColorGradient.SetKeys(colors, alphas);		
		}

		public Node(int cell)
		{
			Cell = cell;
			Strenght = 1f;
		}

		public int Cell;

		public float Strenght;
		public float MaxStrenght;

		public int bl_idx, br_idx, tl_idx, tr_idx;

		public Color GetCurrentColor()
		{
			float colorval = Mathf.Clamp01(Strenght / MaxStrenght);
			return ColorGradient.Evaluate(colorval);
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
