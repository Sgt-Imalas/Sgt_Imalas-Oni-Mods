using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
	internal class ReorderableBuilding_Patches
	{
		[HarmonyPatch(typeof(ReorderableBuilding), nameof(ReorderableBuilding.ApplyAnimOffset))]
		public static class ReorderableBuilding_ApplyAnimOffset_Patch
		{
			/// <summary>
			/// forcerefresh the logic ports of radbolt storage modules when the module list changes
			/// </summary>
			/// <param name="__instance"></param>
			public static void Postfix(ReorderableBuilding __instance)
			{
				RocketModuleCluster component = __instance.GetComponent<RocketModuleCluster>();
				if (component != null)
				{
					var Modules = component.CraftInterface.ClusterModules;
					foreach (var Module in Modules)
					{
						if (Module.Get().TryGetComponent<HighEnergyParticleStorage>(out var storage))
						{
							storage.UpdateLogicPorts();
						}
					}
				}
			}
		}
	}
}
