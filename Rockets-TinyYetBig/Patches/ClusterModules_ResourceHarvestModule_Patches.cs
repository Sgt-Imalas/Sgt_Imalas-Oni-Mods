using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Nosecones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class ClusterModules_ResourceHarvestModule_Patches
	{

        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), MethodType.Constructor, [typeof(IStateMachineTarget),typeof(ResourceHarvestModule.Def)])]
		public class ResourceHarvestModule_Instance_Constructor_Patch
		{
            public static void Postfix(ResourceHarvestModule.StatesInstance __instance)
            {
                SgtLogger.l("Adding SpaceHarvestModule tag to ResourceHarvestModule");
				__instance.gameObject.AddTag(ModAssets.Tags.SpaceHarvestModule);
			}
        }

        [HarmonyPatch(typeof(RocketModuleHexCellCollector.Instance), MethodType.Constructor, [typeof(IStateMachineTarget), typeof(RocketModuleHexCellCollector.Def)])]
		public class RocketModuleHexCellCollector_Instance_Constructor_Patch
		{
			public static void Postfix(RocketModuleHexCellCollector.Instance __instance)
			{
				SgtLogger.l("Adding SpaceHarvestModule tag to RocketModuleHexCellCollector");
				__instance.gameObject.AddTag(ModAssets.Tags.SpaceHarvestModule);
			}
		}


        [HarmonyPatch(typeof(ArtifactHarvestModule.StatesInstance), MethodType.Constructor, [typeof(IStateMachineTarget), typeof(ArtifactHarvestModule.Def)])]
		public class ArtifactHarvestModule_Instance_Constructor_Patch
		{
            public static void Postfix(ArtifactHarvestModule.StatesInstance __instance)
			{
                SgtLogger.l("Adding SpaceHarvestModule tag to ArtifactHarvestModule");
				__instance.gameObject.AddTag(ModAssets.Tags.SpaceHarvestModule);
			}
        }
	}
}
