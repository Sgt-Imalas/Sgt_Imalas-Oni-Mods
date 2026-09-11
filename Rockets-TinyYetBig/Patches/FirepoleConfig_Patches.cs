using HarmonyLib;
using Rockets_TinyYetBig.RocketFueling;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class FirepoleConfig_Patches
	{

        [HarmonyPatch(typeof(FirePoleConfig), nameof(FirePoleConfig.ConfigureBuildingTemplate))]
        public class FirePoleConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(GameObject go)
			{
				if(go.TryGetComponent<AnimTileable>(out var anim))
					anim.tags = [LoaderFirepoleAdapterConfig.ID, FirePoleConfig.ID];
			}
        }
	}
}
