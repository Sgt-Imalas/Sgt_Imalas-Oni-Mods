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
		bool TryUse(int cellParam);

	}
}
