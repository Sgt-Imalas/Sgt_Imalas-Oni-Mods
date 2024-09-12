using UnityEngine;

namespace UtilLibs.YeetUtils
{
	/// <summary>
	/// Credit: Akis FUtility
	/// https://github.com/aki-art/ONI-Mods/
	/// </summary>
	internal class Rotator : KMonoBehaviour
	{
		[SerializeField]
		public float baseDegreesPerSec = 360;

		[SerializeField]
		private Vector2 direction;

		[SerializeField]
		private float scale;

		[SerializeField]
		public float minDistance;

		[SerializeField]
		public bool stopOnLand = true;

		[MyCmpGet] private KBatchedAnimController animController;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			if (stopOnLand)
			{
				Subscribe((int)GameHashes.Landed, _ => StopRotation());
			}
		}

		public void SetVec(Vector2 vec)
		{
			direction = vec;
			// Multiply speed by y component divided by minimum y
			// Rotate in direction of explosion (+ is CCW, - is CW)
			scale = -Mathf.Sign(direction.x) * (direction.y / minDistance);
			enabled = true;
		}

		private void Update()
		{
			if (animController != null)
			{
				// Time.DeltaTime scales with the timescale
				animController.Rotation += scale * baseDegreesPerSec * Time.deltaTime % 360;
			}
		}

		private void StopRotation()
		{
			if (animController != null)
			{
				animController.Rotation = 0f;
			}

			enabled = false;
		}
	}
}
