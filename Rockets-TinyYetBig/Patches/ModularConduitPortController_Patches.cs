using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
	internal class ModularConduitPortController_Patches
	{
		/// <summary>	 
		/// Add a 1 second delay to turnoff signal to prevent flickering
		/// </summary>
		static readonly Dictionary<ModularConduitPortController.Instance, SchedulerHandle> ScheduledTurnOffs = [];
		static void OnSignalChanged(ModularConduitPortController.Instance instance, LogicPorts logicPorts, bool greenSignal)
		{
			if (greenSignal)
			{
				if (ScheduledTurnOffs.TryGetValue(instance, out var scheduledTurnoff))
				{
					GameScheduler.Instance?.scheduler?.Clear(scheduledTurnoff);
					ScheduledTurnOffs.Remove(instance);
				}
				logicPorts?.SendSignal(ModAssets.LOGICPORT_ROCKETPORTLOADER_ACTIVE, 1);
			}
			else
			{
				if (!ScheduledTurnOffs.ContainsKey(instance))
				{
					ScheduledTurnOffs[instance] = GameScheduler.Instance.Schedule("turn off loader logic port", 1, (_) =>
					{
						logicPorts?.SendSignal(ModAssets.LOGICPORT_ROCKETPORTLOADER_ACTIVE, 0);
						ScheduledTurnOffs.Remove(instance);
					});
				}
			}
		}

		[HarmonyPatch(typeof(ModularConduitPortController.Instance), nameof(ModularConduitPortController.Instance.SetLoading))]
		public static class LogicOutputLoaderBuildings_UpdateLogic_Loading
		{
			[HarmonyPrepare]
			static bool Prepare() => Config.Instance.EnableRocketLoaderLogicOutputs;

			public static void Postfix(ModularConduitPortController.Instance __instance, bool isLoading)
			{
				var logicPorts = __instance.GetComponent<LogicPorts>();
				OnSignalChanged(__instance, logicPorts, isLoading);
			}
		}
		[HarmonyPatch(typeof(ModularConduitPortController.Instance), nameof(ModularConduitPortController.Instance.SetUnloading))]
		public static class LogicOutputLoaderBuildings_UpdateLogic_Unloading
		{
			[HarmonyPrepare]
			static bool Prepare() => Config.Instance.EnableRocketLoaderLogicOutputs;

			public static void Postfix(ModularConduitPortController.Instance __instance, bool isUnloading)
			{
				var logicPorts = __instance.GetComponent<LogicPorts>();
				OnSignalChanged(__instance, logicPorts, isUnloading);
			}
		}
	}
}
