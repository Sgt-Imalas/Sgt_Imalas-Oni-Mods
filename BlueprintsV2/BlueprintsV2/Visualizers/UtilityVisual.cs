
using BlueprintsV2.BlueprintData;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.SPACEARTIFACTS;

namespace BlueprintsV2.Visualizers
{

	public sealed class UtilityVisual : BuildingVisual
	{

		public UtilityVisual(BuildingConfig buildingConfig, int cell) : base(buildingConfig, cell)
		{
			if (Visualizer.TryGetComponent<KBatchedAnimController>(out var batchedAnimController))
			{
				IUtilityNetworkMgr utilityNetworkManager = buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager();

				if (utilityNetworkManager != null && buildingConfig.GetConduitFlags(out int flags))
				{
					string animation = utilityNetworkManager.GetVisualizerString((UtilityConnections)flags) + "_place";

					if (batchedAnimController.HasAnimation(animation))
					{
						batchedAnimController.Play(animation);
					}
				}

				batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
				batchedAnimController.isMovable = true;
				batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset();
				batchedAnimController.TintColour = GetVisualizerColor(cell);

				batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
			}
			VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
		}

		public override void ApplyBuildingData(GameObject building)
		{
			base.ApplyBuildingData(building);
		}
		public override void ApplyRotation(Orientation rotation, bool flippedX, bool flippedY)
		{
			BlueprintRotationStateHolder = rotation;
			base.ApplyRotation(rotation, flippedX, flippedY);
			UpdateConnectionVis(Visualizer);
		}
		void UpdateConnectionVis(GameObject go)
		{
			var mng = buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager();
			if (mng != null && buildingConfig.GetConduitFlags(out var flags) && go.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				string animation = mng.GetVisualizerString((UtilityConnections)GetRotatedUtilityConnectionFlags(flags)) + "_place";

				if (kbac.HasAnimation(animation))
					kbac.Play(animation);
			}
			//else
			//	SgtLogger.l("no connections to update on " + go.name);
		}
		public override void MoveVisualizer(int cellParam, bool forceRedraw)
		{
			if (cellParam != cell || forceRedraw)
			{
				Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, Grid.SceneLayer.Building));
				VisualsUtilities.SetVisualizerColor(cellParam, GetVisualizerColor(cellParam), Visualizer, buildingConfig);

				cell = cellParam;
			}
		}
		public override void RefreshColor()
		{
			VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
		}
	}
}
