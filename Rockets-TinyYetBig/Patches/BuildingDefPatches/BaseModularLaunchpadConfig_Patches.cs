using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class BaseModularLaunchpadConfig_Patches
	{
		[HarmonyPatch(typeof(BaseModularLaunchpadPortConfig), nameof(BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort))]
		public static class BaseModularLaunchpadPortConfig_CreateBaseLaunchpadPort_Patch
		{
			[HarmonyPrepare]
			static bool Prepare() => Config.Instance.EnableRocketLoaderLogicOutputs;

			/// <summary>
			/// attach a logic port output to all loader buildings that outputs a green signal while loading/unloading
			/// </summary>
			/// <param name="__result"></param>
			public static void Postfix(BuildingDef __result)
			{
				__result.LogicOutputPorts = new List<LogicPorts.Port>(){
				LogicPorts.Port.OutputPort(ModAssets.LOGICPORT_ROCKETPORTLOADER_ACTIVE, new CellOffset(0, 1),
				STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER,
				STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER_ACTIVE,
				STRINGS.BUILDINGS.PREFABS.RTB_UNIVERSALFUELLOADER.LOGIC_PORT_ROCKETLOADER_INACTIVE)
				};
			}
		}

		[HarmonyPatch(typeof(BaseModularLaunchpadPortConfig), nameof(BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate))]
		public static class BaseModularLaunchpadPortConfig_ConfigureBuildingTemplate_Patch
		{
			/// <summary>
			/// replace header tag to allow alternative rocket platforms.
			/// also add a workable to drop all contents
			/// </summary>
			/// <param name="go"></param>
			public static void Postfix(GameObject go)
			{
				ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
				def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;


				DropAllWorkable dropAllWorkable = go.AddOrGet<DropAllWorkable>();
				dropAllWorkable.dropWorkTime = 15f;
			}
		}
	}
}
