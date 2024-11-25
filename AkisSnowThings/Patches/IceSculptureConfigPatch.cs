using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisSnowThings.Patches
{
	internal class IceSculptureConfigPatch
	{

        [HarmonyPatch(typeof(IceSculptureConfig), nameof(IceSculptureConfig.ConfigureBuildingTemplate))]
        public class IceSculptureConfig_ConfigureBuildingTemplate_Patch
		{
            public static void Postfix(GameObject go)
			{
				go.AddOrGet<BuildingAttachPoint>().points = [new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), ModAssets.AttachmentTag, null)];
			}
        }
	}
}
