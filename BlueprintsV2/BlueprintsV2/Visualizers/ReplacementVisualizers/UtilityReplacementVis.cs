using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers
{
	internal class UtilityReplacementVis : ReplacementVis
	{
		protected override void ApplyExtraDataToBuilt(GameObject built)
		{
			base.ApplyExtraDataToBuilt(built);
			if(built.TryGetComponent<KBatchedAnimController>(out var targetKbac))
			{
				PlayUtilityAnim(targetKbac, true);
			}
			if (built.TryGetComponent<KAnimGraphTileVisualizer>(out var vis))
			{
				var newConnections = (UtilityConnections)conduitFlags;
				if (vis.Connections != newConnections)
				{
					UtilityConnections neew = vis.Connections | newConnections;

					vis.UpdateConnections(neew);
					vis.Refresh();
				}
			}
		}
	}
}
