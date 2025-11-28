using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.RocketModulePatches
{
    public class CargobayCluster_Patches
    {
		[HarmonyPatch(typeof(CargoBayCluster), nameof(CargoBayCluster.OnSpawn))]
		public static class CargoBayRebalance
		{
			/// <summary>
			/// Adds (or removes, depending on config setting) Insulation to cargo bays
			/// </summary>
			public static void Prefix(CargoBayCluster __instance)
			{
				if (Config.Instance.InsulatedCargoBays)
				{
					if (__instance.storage.defaultStoredItemModifers.SequenceEqual(Storage.StandardSealedStorage))
					{
						__instance.storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
						ApplyModifiedModifiers(__instance.storage);
						SgtLogger.l(__instance.name, "Adding Insulation to");
					}
				}
				else
				{
					if (__instance.storage.defaultStoredItemModifers.SequenceEqual(Storage.StandardInsulatedStorage))
					{
						__instance.storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
						ApplyModifiedModifiers(__instance.storage);
						SgtLogger.l(__instance.name, "Removing Insulation from");
					}
				}

			}
			static void ApplyModifiedModifiers(Storage storage)
			{
				foreach (GameObject item in storage.items)
				{
					storage.ApplyStoredItemModifiers(item, is_stored: true, is_initializing: true);
					if (storage.sendOnStoreOnSpawn)
					{
						item.Trigger(856640610, storage);
					}
				}
			}
			/// <summary>
			/// Applies Cargo Capacity Settings
			/// </summary>
			[HarmonyPriority(Priority.VeryLow)]
			public static void Postfix(CargoBayCluster __instance)
			{
				float targetCapacity = 0f;

				if (__instance.storageType == CargoBay.CargoType.Entities) return;

				if (__instance.TryGetComponent<KPrefabID>(out var def))
				{

					// SgtLogger.l(def.PrefabID().ToString());
					CustomCargoBayDB.TryGetCargoBayCapacity(def.PrefabID().ToString(), out targetCapacity);

					//SgtLogger.l(targetCapacity.ToString(), def.PrefabID().ToString());
					//SgtLogger.l(__instance.storage.capacityKg.ToString(), def.PrefabID().ToString());

					if (targetCapacity == 0) return;

					if (__instance.storage.capacityKg != targetCapacity)
					{
						SgtLogger.l(__instance.storage.capacityKg + " old -> new " + targetCapacity);
						if (__instance.userMaxCapacity == __instance.storage.capacityKg)
							__instance.userMaxCapacity = targetCapacity;

						__instance.storage.capacityKg = targetCapacity;
					}

					if (!Config.Instance.RebalancedCargoCapacity)
					{
						if (__instance.userMaxCapacity > targetCapacity)
							__instance.userMaxCapacity = targetCapacity;
					}
				}
			}
		}
    }
}
