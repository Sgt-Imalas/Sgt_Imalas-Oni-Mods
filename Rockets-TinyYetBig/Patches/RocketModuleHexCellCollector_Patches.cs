using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class RocketModuleHexCellCollector_Patches
	{

		[HarmonyPatch(typeof(RocketModuleHexCellCollector), nameof(RocketModuleHexCellCollector.InitializeStates))]
		public class RocketModuleHexCellCollector_InitializeStates_Patch
		{
			///TODO: restore this when Klei fixes the collector
			//[HarmonyPrepare]
			//public static bool Prepare() => Config.Instance.RebalancedCargoCapacity;
			public static void Postfix(RocketModuleHexCellCollector __instance)
			{
				if (Config.Instance.RebalancedCargoCapacity)
				{
					__instance.ground.Enter(smi =>
					{
						Clustercraft_Patches.Clustercraft_OnSpawn_Patch.RebalanceCollector(smi);
					});
				}
				///force refresh that trigger to check for roundtrip return, this can otherwise bug out
				///remove once klei fixes the order in which items are added to cargo bay and removed from hex storage (atm its the wrong way around, its adding first so the roundtrip check runs before the poi has been marked "empty")
				__instance.space.collecting.Exit(smi => smi.master.Trigger((int)GameHashes.OnStorageChange));
			}
		}

	}
}
