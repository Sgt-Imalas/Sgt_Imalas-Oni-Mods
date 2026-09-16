using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SodaFountainConfig_Patches
	{

		[HarmonyPatch(typeof(SodaFountainConfig), nameof(SodaFountainConfig.ConfigureBuildingTemplate))]
		public class SodaFountainConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;

			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix(GameObject go)
			{
				CustomSodaFountain.ConfigureBuilding(go);

			}
		}
	}
}
