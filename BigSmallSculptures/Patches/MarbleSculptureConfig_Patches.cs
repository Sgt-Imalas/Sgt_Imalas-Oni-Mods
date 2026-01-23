using BigSmallSculptures.Content.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BigSmallSculptures.Patches
{
	internal class MarbleSculptureConfig_Patches
	{

        [HarmonyPatch(typeof(MarbleSculptureConfig), nameof(MarbleSculptureConfig.DoPostConfigureComplete))]
        public class MarbleSculptureConfig_DoPostConfigureComplete_Patch
		{
            public static void Postfix(GameObject go)
			{
				go.AddOrGet<MarbleSculptureAnimScaler>();
			}
		}
	}
}
