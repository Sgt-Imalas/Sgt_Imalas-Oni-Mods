using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.BuildingDefPatches
{
	internal class KeroseneEngineClusterSmallConfig_Patches
	{

        [HarmonyPatch(typeof(KeroseneEngineClusterSmallConfig), nameof(KeroseneEngineClusterSmallConfig.DoPostConfigureComplete))]
        public class KeroseneEngineClusterSmallConfig_DoPostConfigureComplete_Patch
		{
			/// <summary>
			/// replace fuel tag with all combustibles if the config option is enabled
			/// </summary>
			/// <param name="go"></param>
			public static void Postfix(GameObject go)
			{
				FuelTank tank = go.GetComponent<FuelTank>();
				RocketEngineCluster rocketEngineCluster = go.GetComponent<RocketEngineCluster>();
				if (Config.Instance.EthanolEngines)
				{
					rocketEngineCluster.fuelTag = GameTags.CombustibleLiquid;
					tank.FuelType = GameTags.CombustibleLiquid;
				}
				ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
				conduitConsumer.conduitType = ConduitType.Liquid;
				conduitConsumer.consumptionRate = 10f;
				conduitConsumer.capacityTag = rocketEngineCluster.fuelTag;
				conduitConsumer.capacityKG = tank.storage.capacityKg;
				conduitConsumer.forceAlwaysSatisfied = true;
				conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			}
		}
	}
}
