using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class RoboPilotModuleConfig_Patches
	{

        [HarmonyPatch(typeof(RoboPilotModuleConfig), nameof(RoboPilotModuleConfig.DoPostConfigureComplete))]
        public class RoboPilotModuleConfig_DoPostConfigureComplete_Patch
        {
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<ModuleDeliveryModeHandler>();
				if (go.TryGetComponent<Storage>(out var databankStorage))
				{
					if (databankStorage.storageFilters == null)
						databankStorage.storageFilters = new List<Tag>() { DatabankHelper.TAG };
					else if (!databankStorage.storageFilters.Contains(DatabankHelper.TAG))
						databankStorage.storageFilters.Add(DatabankHelper.TAG);
				}
			}
		}
	}
}
