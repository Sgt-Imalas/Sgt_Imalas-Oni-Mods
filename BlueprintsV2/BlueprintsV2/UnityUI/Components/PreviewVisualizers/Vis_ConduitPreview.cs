using BlueprintsV2.BlueprintData;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers
{
	internal class Vis_ConduitPreview : Vis_BuildingPreview
	{
		internal override void Init(BuildingConfig building)
		{
			base.Init(building);

			var netWorkManagerGetter = building.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
			if (netWorkManagerGetter != null && building.GetConduitFlags(out var flags))
			{
				kbac.defaultAnim = defaultAnim = netWorkManagerGetter.GetNetworkManager().GetVisualizerString((UtilityConnections)flags);
			}
		}
	}
}
