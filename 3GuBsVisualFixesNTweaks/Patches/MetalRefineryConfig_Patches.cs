using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class MetalRefineryConfig_Patches
	{
		[HarmonyPatch(typeof(MetalRefineryConfig), nameof(MetalRefineryConfig.DoPostConfigureComplete))]
		public class MetalRefineryConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<MetalRefineryTint>().ProductStorage = go.AddComponent<Storage>();

			}
		}
	}
}
