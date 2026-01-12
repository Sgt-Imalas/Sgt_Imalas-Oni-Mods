using HarmonyLib;
using RoboRockets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoboRockets.Patches
{
	internal class RoboPilotModuleConfig_Patches
	{

        [HarmonyPatch(typeof(RoboPilotModuleConfig), nameof(RoboPilotModuleConfig.DoPostConfigureComplete))]
        public class RoboPilotModuleConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
			{
				go.GetComponent<ReorderableBuilding>().buildConditions.Add(new LimitOneAiCommandModule());
			}
        }
	}
}
