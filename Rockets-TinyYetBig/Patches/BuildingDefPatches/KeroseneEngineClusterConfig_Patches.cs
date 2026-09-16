using HarmonyLib;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.BuildingDefPatches
{
	internal class KeroseneEngineClusterConfig_Patches
	{
		[HarmonyPatch(typeof(KeroseneEngineClusterConfig), nameof(KeroseneEngineClusterConfig.DoPostConfigureComplete))]
		public static class KeroseneEngineClusterConfig_DoPostConfigureComplete_Patch
		{
			/// <summary>
			/// replace fuel tag with all combustibles if the config option is enabled
			/// </summary>
			/// <param name="go"></param>
			public static void Postfix(GameObject go)
			{
				if (Config.Instance.EthanolEngines)
				{
					RocketEngineCluster rocketEngineCluster = go.GetComponent<RocketEngineCluster>();
					rocketEngineCluster.fuelTag = GameTags.CombustibleLiquid;
				}
			}
		}
	}
}
