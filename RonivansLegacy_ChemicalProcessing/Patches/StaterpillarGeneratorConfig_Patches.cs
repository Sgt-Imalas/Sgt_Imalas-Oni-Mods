using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class StaterpillarGeneratorConfig_Patches
	{

		[HarmonyPatch(typeof(StaterpillarGeneratorConfig), nameof(StaterpillarGeneratorConfig.CreateBuildingDef))]
		public class StaterpillarGeneratorConfig_CreateBuildingDef_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.Drywall_Hides_Pipes;
			public static void Postfix(BuildingDef __result)
			{
				__result.SceneLayer = ModAssets.AboveDrywallLayer;
			}
		}
	}
}
