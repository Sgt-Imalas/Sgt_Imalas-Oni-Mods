using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Patches
{
	internal class LuxuryBedConfig_Patches
	{

		[HarmonyPatch(typeof(LuxuryBedConfig), nameof(LuxuryBedConfig.CreateBuildingDef))]
		public class LuxuryBedConfig_CreateBuildingDef_Patch
		{
			public static void Postfix(BuildingDef __result) => __result.Floodable = false;
		}
	}
}
