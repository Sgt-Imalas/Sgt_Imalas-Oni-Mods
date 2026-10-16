using HarmonyLib;
using SatisfyingPowerShards.Components;
using UnityEngine;

namespace SatisfyingPowerShards.Patches
{
	internal class StaterpillarConfig_Patch
	{
		[HarmonyPatch(typeof(StaterpillarConfig), nameof(StaterpillarConfig.CreatePrefab))]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix(GameObject __result)
			{
				__result.AddOrGet<PowerShardGrowthMonitor>();
			}
		}
	}
}
