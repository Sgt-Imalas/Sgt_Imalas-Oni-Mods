
using BlueprintsV2.BlueprintData;
using UnityEngine;
using UtilLibs;
using static LogicGateVisualizer;
using static STRINGS.UI.SPACEARTIFACTS;

namespace BlueprintsV2.Visualizers
{

	public sealed class UtilityVisual : BuildingVisual
	{
		private IUtilityNetworkMgr _networkMgr;
		public UtilityVisual(BuildingConfig buildingConfig, int cell, ulong playerId) : base(buildingConfig, cell, playerId)
		{
			_networkMgr = buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>()?.GetNetworkManager();
			UpdateConnectionVis();
		}

		public override void ApplyRotation(Orientation rotation, bool flippedX, bool flippedY)
		{
			BlueprintRotationStateHolder = rotation;
			base.ApplyRotation(rotation, flippedX, flippedY);
			UpdateConnectionVis();
		}
		void UpdateConnectionVis(bool built = false)
		{
			if (hasKbac && _networkMgr != null && buildingConfig.GetConduitFlags(out var flags))
			{
				string animation = _networkMgr.GetVisualizerString((UtilityConnections)GetRotatedUtilityConnectionFlags(flags));
				if(!built)
					animation += "_place";
				
				if (kbac.HasAnimation(animation))
					kbac.Play(animation);
			}
		}
		public override void MoveVisualizer(int cellParam, bool forceRedraw)
		{
			if (cellParam != cell || forceRedraw)
			{
				Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, Grid.SceneLayer.Building));				
				cell = cellParam;
				ApplyColorIfChanged(cell);
			}
		}
		public override void RefreshColor()
		{
			ApplyColorIfChanged(cell);
		}
		public override bool AllowedForRotation(Orientation rotation, bool flippedX, bool flippedY) => true;
	}
}
