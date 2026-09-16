using HarmonyLib;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class ModularLaunchpadPortBridgeConfig_Patches
	{
		[HarmonyPatch(typeof(ModularLaunchpadPortBridgeConfig), nameof(ModularLaunchpadPortBridgeConfig.ConfigureBuildingTemplate))]
		public static class ModularLaunchpadPortBridgeConfig_ConfigureBuildingTemplate_Patch
		{
			/// <summary>
			/// replace the headBuildingTag with the rocket platform tag to allow alternative rocket platforms
			/// </summary>
			/// <param name="go"></param>
			public static void Postfix(GameObject go)
			{
				ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
				def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
			}
		}
	}
}
