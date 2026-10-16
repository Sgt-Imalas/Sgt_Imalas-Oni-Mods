using KSerialization;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace AkisDecorPackB.Content.Scripts.BigFossil
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class BigFossilCablesRenderer : KMonoBehaviour
	{
		[SerializeField] public Color baseColor;

		private static readonly HashedString cableOriginMarker = "cableorigin_marker";

		[MyCmpReq] private KBatchedAnimController kbac;
		[MyCmpReq] private Building building;
		[MyCmpReq] private AnchorMonitor monitor;

		private readonly float zOffset = Grid.GetLayerZ(Grid.SceneLayer.Front);

		public List<SingleCableRenderer> cables;
		private List<Vector3> startPoints;
		private bool dirtyCables;

		private bool showCables;

		public override void OnSpawn()
		{
			cables = [];
			Subscribe((int)ModAssets.Hashes.GoundingChanged, _ => UpdateAllCableLengths());
			SetColor(baseColor);
			kbac.OnTintChanged += RefreshColors;
			dirtyCables = true;
		}

		public void ToggleCables(bool on)
		{
			showCables = on;
			if (on)
			{
				ReCreateCables();
			}
			else
			{
				RemoveCables();
			}
		}

		public void UpdateAllCableLengths()
		{
			if (!showCables)
				return;

			for (int i = 0; i < monitor.ceilingHeights.Length; i++)
				UpdateCableLengths(i, monitor.ceilingHeights[i]);
		}

		private void Update()
		{
			if (dirtyCables)
			{
				if (showCables)
				{
					RemoveCables();
					CollectMarkers();
					CreateCables();
					RefreshColors(kbac.TintColour);
					UpdateAllCableLengths();
				}

				dirtyCables = false;
			}
		}

		public void ReCreateCables()
		{
			dirtyCables = true;
		}

		public void UpdateCableLengths(int column, float length)
		{
			SgtLogger.l($"updating cable lents {column} {length}");

			foreach (var cable in cables)
			{
				if (cable.column == column)
					cable.SetLength(length);
			}
		}

		public void SetColor(Color color)
		{
			baseColor = color;

			if (cables != null)
				RefreshColors(kbac.TintColour);
		}

		private void RefreshColors(Color color)
		{
			foreach (var cable in cables)
				cable.SetColor(baseColor * color);
		}

		private void RemoveCables()
		{
			foreach (var cable in cables)
				Destroy(cable.gameObject);

			cables.Clear();
		}

		public void CreateCables()
		{
			cables ??= [];

			if (startPoints == null || startPoints.Count == 0)
				return;

			foreach (var point in startPoints)
			{
				var go = new GameObject("cable");
				go.transform.parent = transform;

				var renderer = go.AddComponent<SingleCableRenderer>();

				renderer.InitLineRenderer();
				renderer.column = (int)point.x + (building.Def.WidthInCells / 2);
				renderer.SetAttachmentPosition(point);
				go.SetActive(true);

				cables.Add(renderer);
			}
		}

		// Collects the positions of marker symbols from the kbac
		public void CollectMarkers()
		{
			startPoints = [];

			var batch = kbac.GetBatch();

			if (batch != null)
			{
				var frame = batch.group.data.GetAnimFrames()[0];
				for (var frameElementIdx = 0; frameElementIdx < frame.numElements; ++frameElementIdx)
				{
					var offsetIdx = frame.firstElementIdx + frameElementIdx;
					if (offsetIdx < batch.group.data.frameElements.Count)
					{
						var frameElement = batch.group.data.frameElements[offsetIdx];
						if (frameElement.symbol == cableOriginMarker)
						{
							var vector3 = (kbac.GetTransformMatrix() * frameElement.transform)
								.MultiplyPoint(Vector3.zero) - transform.GetPosition();

							startPoints.Add(vector3 with { z = zOffset });
						}
					}
				}
			}
		}
	}
}
