using UnityEngine;

namespace AkisDecorPackB.Content.Scripts.BigFossil
{
	public class SingleCableRenderer : KMonoBehaviour
	{
		private LineRenderer lineRenderer;
		public int column;

		public override void OnSpawn()
		{
			base.OnSpawn();
			InitLineRenderer();
		}

		public void InitLineRenderer()
		{
			if (lineRenderer != null)
				return;

			lineRenderer = Instantiate(ModAssets.Prefabs.cablePrefab).GetComponent<LineRenderer>();

			lineRenderer.transform.position = transform.position;
			lineRenderer.transform.parent = transform;
			lineRenderer.useWorldSpace = false;
			lineRenderer.name = $"cable renderer";
			lineRenderer.positionCount = 2;

			lineRenderer.SetPosition(0, Vector3.zero);
			lineRenderer.SetPosition(1, Vector3.up);

			lineRenderer.gameObject.SetActive(true);
		}

		public void SetColor(Color color)
		{
			lineRenderer.startColor = lineRenderer.endColor = color;
		}

		public void SetAttachmentPosition(Vector3 localPosition)
		{
			//column = (int)(localPosition.x / 100f);
			//column = Mathf.Clamp(column, 0, 6);

			transform.SetLocalPosition(localPosition);
		}

		public void SetLength(float length)
		{
			lineRenderer.SetPosition(1, new Vector3(0, length - transform.localPosition.y, 0));
		}
	}
}
