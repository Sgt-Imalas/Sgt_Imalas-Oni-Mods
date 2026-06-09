using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
				PlayUtilityAnim(targetKbac);
			}
		}
	}
}
