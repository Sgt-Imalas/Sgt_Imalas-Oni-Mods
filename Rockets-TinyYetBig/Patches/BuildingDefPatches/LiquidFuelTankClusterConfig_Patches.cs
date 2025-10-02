using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.BuildingDefPatches
{
	internal class LiquidFuelTankClusterConfig_Patches
	{
		[HarmonyPatch(typeof(LiquidFuelTankClusterConfig), nameof(LiquidFuelTankClusterConfig.DoPostConfigureComplete))]
		public static class LiquidFuelTankClusterConfig_DoPostConfigureComplete_Patch
		{
			/// <summary>
			/// replace consumer fuel tag of tank to only accept rocket fuels
			/// </summary>
			/// <param name="go"></param>
			public static void Postfix(GameObject go)
			{
				var consumer = go.GetComponent<ConduitConsumer>();

				consumer.capacityTag = ModAssets.Tags.RocketFuelTag;
				UnityEngine.Object.Destroy(go.GetComponent<ManualDeliveryKG>());

				consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
				go.AddOrGet<DropAllWorkable>().dropWorkTime = 60f;
			}
		}
	}
}
