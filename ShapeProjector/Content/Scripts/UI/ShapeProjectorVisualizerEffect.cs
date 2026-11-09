using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UtilLibs;

namespace ShapeProjector.Content.Scripts.UI
{
	internal class ShapeProjectorVisualizerEffect : MonoBehaviour
	{
		private Material material;
		private Camera myCamera;
		public Color highlightColor => _master.IsNullOrDestroyed() ? Color.green : _master.HighlightColor;
		private Texture2D OcclusionTex;

		ShapeProjectorLogic _master = null;
		public void SetMaster(ShapeProjectorLogic master)
		{
			_master = (master);
		}
		private void Start() => material = new Material(Shader.Find("Klei/PostFX/Range"));
		private void OnPostRender()
		{
			if (_master == null)
				return;

			if (_master.WorldId != ClusterManager.Instance.activeWorldId)
				return;

			HashSet<int> activeCells = _master.GetActiveCells();
			if (activeCells == null || activeCells.Count == 0)
				return;

			if (myCamera == null)
			{
				myCamera = GetComponent<Camera>();
				if (myCamera == null)
					return;
			}

			// 1. bounding box of the set
			int minX = int.MaxValue, minY = int.MaxValue;
			int maxX = int.MinValue, maxY = int.MinValue;
			foreach (int cell in activeCells)
			{
				Grid.CellToXY(cell, out int cx, out int cy);
				if (cx < minX) minX = cx;
				if (cy < minY) minY = cy;
				if (cx > maxX) maxX = cx;
				if (cy > maxY) maxY = cy;
			}

			int boxWidth = maxX - minX + 1;
			int boxHeight = maxY - minY + 1;

			if (OcclusionTex == null || OcclusionTex.width < boxWidth || OcclusionTex.height < boxHeight)
			{
				int w = OcclusionTex == null ? boxWidth : Mathf.Max(OcclusionTex.width, boxWidth);
				int h = OcclusionTex == null ? boxHeight : Mathf.Max(OcclusionTex.height, boxHeight);
				if (OcclusionTex != null)
					Destroy(OcclusionTex);
				OcclusionTex = new Texture2D(w, h, TextureFormat.Alpha8, false);
				OcclusionTex.filterMode = FilterMode.Point;
				OcclusionTex.wrapMode = TextureWrapMode.Clamp;
			}

			int texWidth = OcclusionTex.width;
			NativeArray<byte> pixelData = OcclusionTex.GetPixelData<byte>(0);
			for (int i = 0; i < pixelData.Length; i++)
				pixelData[i] = 0;

			foreach (int cell in activeCells)
			{
				Grid.CellToXY(cell, out int cx, out int cy);
				pixelData[(cy - minY) * texWidth + (cx - minX)] = byte.MaxValue;
			}
			OcclusionTex.Apply(false, false);

			Ray ray = myCamera.ViewportPointToRay(Vector3.zero);
			float distance1 = Mathf.Abs(ray.origin.z / ray.direction.z);
			Vector3 point1 = ray.GetPoint(distance1);
			Vector4 uvOffsetScale;
			uvOffsetScale.x = point1.x;
			uvOffsetScale.y = point1.y;
			ray = myCamera.ViewportPointToRay(Vector3.one);
			float distance2 = Mathf.Abs(ray.origin.z / ray.direction.z);
			Vector3 point2 = ray.GetPoint(distance2);
			uvOffsetScale.z = point2.x - uvOffsetScale.x;
			uvOffsetScale.w = point2.y - uvOffsetScale.y;
			material.SetVector("_UVOffsetScale", uvOffsetScale);

			material.SetVector("_RangeParams", new Vector4(minX, minY, maxX + 1, maxY + 1));
			material.SetColor("_HighlightColor", highlightColor);
			material.SetVector("_OcclusionParams", new Vector4(1f / OcclusionTex.width, 1f / OcclusionTex.height, 0f, 0f));
			material.SetTexture("_OcclusionTex", OcclusionTex);
			material.SetVector("_WorldParams", new Vector4(
				Grid.WidthInCells, Grid.HeightInCells,
				1f / Grid.WidthInCells, 1f / Grid.HeightInCells));

			// 5. fullscreen quad
			GL.PushMatrix();
			material.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(5);
			GL.Color(Color.white);
			GL.Vertex3(0f, 0f, 0f);
			GL.Vertex3(0f, 1f, 0f);
			GL.Vertex3(1f, 0f, 0f);
			GL.Vertex3(1f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
		private void OnDestroy()
		{
			if (material != null)
			{
				Destroy(material);
				material = null;
			}
			if (OcclusionTex != null)
			{
				Destroy(OcclusionTex);
				OcclusionTex = null;
			}
		}

		private void FindWorldBounds(out Vector2I world_min, out Vector2I world_max)
		{
			if (ClusterManager.Instance != null)
			{
				WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
				world_min = activeWorld.WorldOffset;
				world_max = activeWorld.WorldOffset + activeWorld.WorldSize;
			}
			else
			{
				world_min.x = 0;
				world_min.y = 0;
				world_max.x = Grid.WidthInCells;
				world_max.y = Grid.HeightInCells;
			}
		}
	}
}
