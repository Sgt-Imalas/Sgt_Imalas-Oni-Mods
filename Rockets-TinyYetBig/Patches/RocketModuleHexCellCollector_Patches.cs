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
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.RebalancedCargoCapacity;

			public static void Postfix(RocketModuleHexCellCollector __instance)
			{
				__instance.ground.Enter(smi =>
				{
					Clustercraft_Patches.Clustercraft_OnSpawn_Patch.RebalanceCollector(smi);
				});
				///force refresh that trigger to check for roundtrip return, this can otherwise bug out
				__instance.space.collecting.Exit(smi => smi.master.Trigger((int)GameHashes.OnStorageChange));
			}
		}

	}
}
