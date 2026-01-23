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
	internal class SmallSculptureConfig_Patches
	{

        [HarmonyPatch(typeof(SmallSculptureConfig), nameof(SmallSculptureConfig.DoPostConfigureComplete))]
        public class SmallSculptureConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<SmallSculptureAnimScaler>();
			}
		}
	}
}
