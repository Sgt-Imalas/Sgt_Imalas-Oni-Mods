using UnityEngine;

namespace BlueprintsV2.Visualizers
{
	public interface IVisual
	{
		GameObject Visualizer { get; }
		Vector2I Offset { get; }

		PlanScreen.RequirementsState RequirementsState { get; }

		bool IsPlaceable(int cellParam);
		void MoveVisualizer(int cellParam, bool forceRedraw);

		void ForceRedraw();
		void RefreshColor();

		bool TryUse(int cellParam);

		PermittedRotations GetAllowedRotations();
		void ApplyRotation(Orientation rotation, bool flippedX, bool flippedY);
	}
}
