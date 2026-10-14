using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class Lamp_Patches
	{

        [HarmonyPatch(typeof(CeilingLightConfig), nameof(CeilingLightConfig.DoPostConfigureComplete))]
        public class CeilingLightConfig_DoPostConfigureComplete_Patch
        {
			public static void Postfix(GameObject go) => go.AddOrGet<LumaFlies>().FlyOffset = new Vector3(0,-0.5f);
		}
		[HarmonyPatch(typeof(FloorLampConfig), nameof(FloorLampConfig.DoPostConfigureComplete))]
		public class FloorLampConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go) => go.AddOrGet<LumaFlies>().FlyOffset = new Vector3(0.1f, 0.5f);
		}
	}
}
