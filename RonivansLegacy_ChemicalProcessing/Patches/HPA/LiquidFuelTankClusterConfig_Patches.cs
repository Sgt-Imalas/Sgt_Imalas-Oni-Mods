using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	internal class LiquidFuelTankClusterConfig_Patches
	{
		[HarmonyPatch(typeof(LiquidFuelTankClusterConfig), nameof(LiquidFuelTankClusterConfig.DoPostConfigureComplete))]
		public static class LiquidFuelTankClusterConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				if (go.TryGetComponent<ConduitConsumer>(out var consumer))
					consumer.consumptionRate = HighPressureConduitRegistration.GetMaxConduitCapacity(ConduitType.Liquid, true);
			}
		}
	}
}
