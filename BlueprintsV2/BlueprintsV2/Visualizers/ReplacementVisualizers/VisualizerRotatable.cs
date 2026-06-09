using System;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers
{
	/// <summary>
	/// Cloned from regular rotatable to work without Building Component
	/// </summary>
	public class VisualizerRotatable : KMonoBehaviour
	{
		public BuildingDef def;
		[MyCmpGet] KBatchedAnimController kbac;
		[MyCmpGet] KBoxCollider2D collider;

		public void UpdateRotation()
		{
			if (kbac != null)
			{
				kbac.Pivot = GetVisualizerPivot();
				kbac.Rotation = GetVisualizerRotation();
				kbac.Offset = GetVisualizerOffset();
				kbac.FlipX = GetVisualizerFlipX();
				kbac.FlipY = GetVisualizerFlipY();
			}
			if (collider != null)
			{
				OrientCollider();
			}
		}

		void OrientCollider()
		{
			float x = 0.5f * (float)((this.width + 1) % 2);
			float num1 = 0.0f;
			switch (CurrentOrientation)
			{
				case Orientation.R90:
					num1 = -90f;
					break;
				case Orientation.R180:
					num1 = -180f;
					break;
				case Orientation.R270:
					num1 = -270f;
					break;
				case Orientation.FlipH:
					collider.offset = new Vector2((float)((double)x + (double)(this.width % 2) - 1.0), 0.5f * (float)this.height);
					collider.size = new Vector2((float)this.width, (float)this.height);
					break;
				case Orientation.FlipV:
					collider.offset = new Vector2(x, -0.5f * (float)(this.height - 2));
					collider.size = new Vector2((float)this.width, (float)this.height);
					break;
				default:
					collider.offset = new Vector2(x, 0.5f * (float)this.height);
					collider.size = new Vector2((float)this.width, (float)this.height);
					break;
			}
			if ((double)num1 == 0.0)
				return;
			Matrix2x3 matrix2x3_1 = Matrix2x3.Translate(-this.pivot);
			Matrix2x3 matrix2x3_2 = Matrix2x3.Rotate(num1 * ((float)Math.PI / 180f));
			Matrix2x3 matrix2x3_3 = Matrix2x3.Translate((Vector2)(this.pivot + new Vector3(x, 0.0f, 0.0f))) * matrix2x3_2 * matrix2x3_1;
			Vector2 v1 = new Vector2(-0.5f * (float)this.width, 0.0f);
			Vector2 v2 = new Vector2(0.5f * (float)this.width, (float)this.height);
			Vector2 v3 = new Vector2(0.0f, 0.5f * (float)this.height);
			Vector2 vector2_1 = (Vector2)matrix2x3_3.MultiplyPoint((Vector3)v1);
			v2 = (Vector2)matrix2x3_3.MultiplyPoint((Vector3)v2);
			Vector2 vector2_2 = (Vector2)matrix2x3_3.MultiplyPoint((Vector3)v3);
			float num2 = Mathf.Min(vector2_1.x, v2.x);
			float num3 = Mathf.Max(vector2_1.x, v2.x);
			float num4 = Mathf.Min(vector2_1.y, v2.y);
			float num5 = Mathf.Max(vector2_1.y, v2.y);
			collider.offset = vector2_2;
			collider.size = new Vector2(num3 - num2, num5 - num4);
		}


		private Orientation CurrentOrientation = Orientation.Neutral;
		int width, height;
		private Vector3 pivot = Vector3.zero;
		private Vector3 visualizerOffset = Vector3.zero;

		public bool GetVisualizerFlipX() => CurrentOrientation == Orientation.FlipH;

		public bool GetVisualizerFlipY() => CurrentOrientation == Orientation.FlipV;
		public float GetVisualizerRotation()
		{
			switch (def.PermittedRotations)
			{
				case PermittedRotations.R90:
				case PermittedRotations.R360:
					return -90f * (float)CurrentOrientation;
				default:
					return 0.0f;
			}
		}
		public Vector3 GetVisualizerPivot()
		{
			Vector3 pivot = this.pivot;
			switch (CurrentOrientation)
			{
				case Orientation.FlipH:
					pivot.x = -this.pivot.x;
					break;
			}
			return pivot;
		}
		private Vector3 GetVisualizerOffset()
		{
			Vector3 visualizerOffset;
			switch (CurrentOrientation)
			{
				case Orientation.FlipH:
					visualizerOffset = new Vector3(-this.visualizerOffset.x, this.visualizerOffset.y, this.visualizerOffset.z);
					break;
				case Orientation.FlipV:
					visualizerOffset = new Vector3(this.visualizerOffset.x, 1f, this.visualizerOffset.z);
					break;
				default:
					visualizerOffset = this.visualizerOffset;
					break;
			}
			return visualizerOffset;
		}

		public void Init(BuildingDef def, Orientation orientation)
		{
			this.def = def;
			CurrentOrientation = orientation;
			SetSize(def.WidthInCells, def.HeightInCells);
		}

		public void SetSize(int width, int height)
		{
			this.width = width;
			this.height = height;
			if (width % 2 == 0)
			{
				pivot = new Vector3(-0.5f, 0.5f, 0.0f);
				visualizerOffset = new Vector3(0.5f, 0.0f, 0.0f);
			}
			else
			{
				pivot = new Vector3(0.0f, 0.5f, 0.0f);
				visualizerOffset = Vector3.zero;
			}
		}
	}
}
