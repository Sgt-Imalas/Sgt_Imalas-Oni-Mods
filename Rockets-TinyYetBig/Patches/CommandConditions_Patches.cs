using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class CommandConditions_Patches
	{

        [HarmonyPatch(typeof(RocketCommandConditions), nameof(RocketCommandConditions.OnSpawn))]
        public class RocketCommandConditions_OnSpawn_Patch
        {
            public static void Postfix(RocketCommandConditions __instance)
            {
                var module = __instance.GetComponent<RocketModuleCluster>();
                module.RemoveModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, __instance.HasCargoBayForNoseconeHarvest);
			}
        }

        [HarmonyPatch(typeof(RobotCommandConditions), nameof(RobotCommandConditions.OnSpawn))]
        public class RobotCommandConditions_OnSpawn_Patch
        {
            public static void Postfix(RobotCommandConditions __instance)
			{
				var module = __instance.GetComponent<RocketModuleCluster>();
				module.RemoveModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, __instance.HasCargoBayForNoseconeHarvest);
			}
        }
	}
}
