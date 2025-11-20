using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
					var master = smi.master.gameObject;
					SgtLogger.l("HexCellCollector.Ground OnEnter");

					if (master.TryGetComponent<CargoBayCluster>(out var cargoBay) 
					&& cargoBay.storageType != CargoBay.CargoType.Entities
					&& master.TryGetComponent<KPrefabID>(out var prefabID)
					&& CustomCargoBayDB.TryGetCargobayCollectionSpeed(prefabID.PrefabID().ToString(), out float adjustedSpeed))
					{
						SgtLogger.l("Adjusting Cargobay collection speed of " + prefabID.PrefabID() + ", old kg per cycle: " + smi.def.collectSpeed * 600 + ", new kg per cycle: " + adjustedSpeed * 600);
						smi.def.collectSpeed = adjustedSpeed;
					}
				});
			}
		}
	}
}
