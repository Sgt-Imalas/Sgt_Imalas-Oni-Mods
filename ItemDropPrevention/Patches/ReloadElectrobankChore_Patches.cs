using HarmonyLib;
using ItemDropPrevention.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemDropPrevention.Patches
{
	internal class ReloadElectrobankChore_Patches
	{

		[HarmonyPatch(typeof(ReloadElectrobankChore), nameof(ReloadElectrobankChore.InstallElectrobank))]
		public class ReloadElectrobankChore_InstallElectrobank_Patch
		{
			public static void Prefix(ReloadElectrobankChore.Instance smi)
			{
				foreach (var storage in smi.Storages)
				{
					if (storage == smi.batteryMonitor.storage)
						continue;
					var chargedBatteryInStorage = storage.FindFirst(GameTags.ChargedPortableBattery);
					if (chargedBatteryInStorage != null)
					{
						storage.Transfer(chargedBatteryInStorage, smi.batteryMonitor.storage);
						break;
					}
				}
			}
		}

		[HarmonyPatch(typeof(ReloadElectrobankChore.States), nameof(ReloadElectrobankChore.States.InitializeStates))]
		public class ReloadElectrobankChore_States_InitializeStates_Patch
		{
			public static void Postfix(ReloadElectrobankChore.States __instance)
			{
				__instance.installAtMessStation.Enter(ForceDrop);
				__instance.installAtSafeLocation.Enter(ForceDrop);
			}
			static void ForceDrop(ReloadElectrobankChore.Instance smi)
			{
				var dupe = smi.sm.dupe.Get(smi);
				if (dupe == null)
					return;
				if (dupe.TryGetComponent<DroppablesHolder>(out var drop))
					drop.ForceDropCheck();
			}
		}
	}
}
