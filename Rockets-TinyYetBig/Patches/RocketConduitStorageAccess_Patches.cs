using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static STRINGS.UI.DEVELOPMENTBUILDS.ALPHA;

namespace Rockets_TinyYetBig.Patches.RocketLoadingPatches
{
	public class RocketConduitStorageAccess_Patches
	{
		[HarmonyPatch(typeof(RocketConduitStorageAccess), nameof(RocketConduitStorageAccess.Sim200ms))]
		public static class RocketConduitStorageAccess_Sim200ms_Patch
		{
			const float MinTransfer = 0.00001f;
			/// <summary>
			/// This throws out food that is not allowed in cargo bays if there is no freezer module
			/// Also allows feeding the drillcone support module from inside the rocket
			/// </summary>
			public static bool CouldStorageAllowThisTag(Tag tagAllowed, Storage storage)
			{
				var filters = storage.storageFilters;
				if (filters == null)
					return false;

				var discovered = DiscoveredResources.Instance;
				for (int i = 0; i < filters.Count; i++)
				{
					if (discovered.GetDiscoveredResourcesFromTag(filters[i]).Contains(tagAllowed))
						return true;
				}
				return false;
			}

			/// <summary>
			/// returns true of the pickupable was not completely transfered and there is still mass on it left
			/// </summary>
			/// <returns>if the item was not yet fully transferred and has mass left</returns>
			static bool PushItemToRocketStorage(Pickupable item, Storage target, float remainingCargoCapacity, ref float remainingItemMass)
			{
				if (remainingCargoCapacity <= 0.00001f)
					return true;

				var part = item.Take(remainingCargoCapacity);
				if (part == null)
					return true;

				float transferredMass = part.primaryElement.Mass;
				target.Store(part.gameObject, true);
				remainingItemMass -= transferredMass;
				return remainingItemMass > 0f;
			}
			/// <summary>
			/// puts item in storage and returns true if the remaining storagecapacity is > 0
			/// </summary>
			/// <returns>true if remainingStorageCapacity > 0</returns>
			static bool PullItemFromRocketStorage(Pickupable item, Storage target, ref float remainingStorageCapacity)
			{
				if (remainingStorageCapacity <= 0.00001f)
					return false;

				var part = item.Take(remainingStorageCapacity);
				if (part == null)
					return true;

				remainingStorageCapacity -= part.primaryElement.Mass;
				target.Store(part.gameObject, true);
				return remainingStorageCapacity > 0f;
			}

			static void TransferToRocketStorages(RocketConduitStorageAccess instance)
			{
				bool RefillDiamonds = instance.cargoType == CargoBay.CargoType.Solids && Config.Instance.RefillDrillSupport;

				var cargobays = ListPool<CargoBayCluster, RocketConduitStorageAccess>.Allocate();
				var multiMaterialCargoBays = ListPool<MultiMaterialCargoBay, RocketConduitStorageAccess>.Allocate();
				var drillconeSupportModules = RefillDiamonds ? ListPool<DrillConeAssistentModule, RocketConduitStorageAccess>.Allocate() : null;
				var cargobayType = instance.cargoType;

				foreach (var moduleRef in instance.craftModuleInterface.ClusterModules)
				{
					var module = moduleRef.Get();
					if (module == null)
						continue;
					if (RefillDiamonds && module.TryGetComponent<DrillConeAssistentModule>(out var assistant))
						drillconeSupportModules.Add(assistant);
					else if (module.TryGetComponent<MultiMaterialCargoBay>(out var mmb) && mmb.HasAnyCapacityFor(cargobayType))
						multiMaterialCargoBays.Add(mmb);
					else if (module.TryGetComponent<CargoBayCluster>(out var cbc) && cbc.storageType == cargobayType)
						cargobays.Add(cbc);
				}

				for (int i = instance.storage.items.Count - 1; i >= 0; i--)
				{
					var itemInStorage = instance.storage.items[i];
					if (!itemInStorage.TryGetComponent<Pickupable>(out var itemPickupable) || itemPickupable.primaryElement.Element.IsVacuum || !itemInStorage.TryGetComponent<KPrefabID>(out var kprefabId))
						continue;

					var itemElement = itemPickupable.primaryElement;
					float remainingItemMass = itemElement.Mass;

					if (remainingItemMass <= 0f)
						continue;

					Tag itemId = kprefabId.PrefabTag;

					if (RefillDiamonds)
					{
						foreach (DrillConeAssistentModule supportModule in drillconeSupportModules)
						{
							if (!supportModule.TagAllowedByStorageFilter(itemId))
								continue;
							var diamondStorage = supportModule.DiamondStorage;
							if (!PushItemToRocketStorage(itemPickupable, diamondStorage, diamondStorage.RemainingCapacity(), ref remainingItemMass))
								break;
						}
					}
					if (remainingItemMass <= 0)
						continue;

					foreach (MultiMaterialCargoBay mmcb in multiMaterialCargoBays)
					{
						if (!mmcb.Filterable.AcceptedTags.Contains(itemId))
							continue;

						if (!PushItemToRocketStorage(itemPickupable, mmcb.Storage, mmcb.RemainingCapacityFor(itemPickupable.primaryElement), ref remainingItemMass))
							break;
					}

					if (remainingItemMass <= 0)
						continue;

					foreach (var cargoBay in cargobays)
					{
						if (!CouldStorageAllowThisTag(itemId, cargoBay.storage))
							continue;

						if (!PushItemToRocketStorage(itemPickupable, cargoBay.storage, cargoBay.RemainingCapacity, ref remainingItemMass))
							break;
					}
				}

				cargobays.Recycle();
				multiMaterialCargoBays.Recycle();
				if (RefillDiamonds)
					drillconeSupportModules.Recycle();
			}
			static void TransferFromRocketStorages(RocketConduitStorageAccess instance, float remainingCapacity)
			{
				var filter = instance.filterable;
				if (filter == null)
					return;

				var cargobays = ListPool<CargoBayCluster, RocketConduitStorageAccess>.Allocate();
				var multiMaterialCargoBays = ListPool<MultiMaterialCargoBay, RocketConduitStorageAccess>.Allocate();
				var cargobayType = instance.cargoType;

				foreach (var moduleRef in instance.craftModuleInterface.ClusterModules)
				{
					var module = moduleRef.Get();
					if (module == null)
						continue;
					else if (module.TryGetComponent<MultiMaterialCargoBay>(out var mmb) && mmb.HasAnyCapacityFor(cargobayType))
						multiMaterialCargoBays.Add(mmb);
					else if (module.TryGetComponent<CargoBayCluster>(out var cbc) && cbc.storageType == cargobayType)
						cargobays.Add(cbc);
				}

				var filterTag = filter.selectedTag;
				bool acceptAny = filterTag == GameTags.Void;
				var allowedElementState = filter.filterElementState
					switch
				{
					Filterable.ElementState.Solid => Element.State.Solid,
					Filterable.ElementState.Liquid => Element.State.Liquid,
					Filterable.ElementState.Gas => Element.State.Gas,
					_ => Element.State.Vacuum
				};
				var targetStorage = instance.storage;

				foreach (var mmb in multiMaterialCargoBays)
				{
					for (int i = mmb.Storage.items.Count - 1; i >= 0; i--)
					{
						var itemInStorage = mmb.Storage.items[i];
						if (!itemInStorage.TryGetComponent<Pickupable>(out var itemPickupable) || itemPickupable.primaryElement.Element.IsVacuum || !itemInStorage.TryGetComponent<KPrefabID>(out var kprefabId))
							continue;
						var itemElement = itemPickupable.primaryElement;
						if (!itemElement.Element.IsState(allowedElementState))
							continue;

						if ((acceptAny || filterTag == kprefabId.PrefabTag) && !PullItemFromRocketStorage(itemPickupable, targetStorage, ref remainingCapacity))
							break;
					}
					if (remainingCapacity <= 0)
						break;
				}
				if (remainingCapacity > 0)
				{
					foreach (var cb in cargobays)
					{
						for (int i = cb.storage.items.Count - 1; i >= 0; i--)
						{
							var itemInStorage = cb.storage.items[i];
							if (!itemInStorage.TryGetComponent<Pickupable>(out var itemPickupable) || itemPickupable.primaryElement.Element.IsVacuum || !itemInStorage.TryGetComponent<KPrefabID>(out var kprefabId))
								continue;
							if ((acceptAny || filterTag == kprefabId.PrefabTag) && !PullItemFromRocketStorage(itemPickupable, targetStorage, ref remainingCapacity))
								break;
						}
						if (remainingCapacity <= 0)
							break;
					}
				}
				cargobays.Recycle();
				multiMaterialCargoBays.Recycle();
			}
			public static bool Prefix(float dt,
				RocketConduitStorageAccess __instance)
			{
				var operational = __instance.operational;

				if (operational != null && !operational.IsOperational)
					return false;
				float currentMassStored = __instance.storage.MassStored();

				if (currentMassStored >= __instance.targetLevel - 0.01f && currentMassStored <= __instance.targetLevel + 0.01f)
					return false;

				operational?.SetActive(true);

				//targetLevel: positive values = pulls into storage, negative values = puts stuff into cargobay
				float amoundToTransfer = __instance.targetLevel - currentMassStored;

				//either no liquid to put into cargo bays or clogged up by full pipe
				if (amoundToTransfer == 0)
					return false;

				if (__instance.targetLevel > 0)
					TransferFromRocketStorages(__instance, amoundToTransfer);
				else
					TransferToRocketStorages(__instance);
				return false;
			}

		}

		/// <summary>
		/// Adds consumables and medicine to Loader to allow the loading of the freezer module
		/// </summary>
		[HarmonyPatch(typeof(BaseModularLaunchpadPortConfig))]
		[HarmonyPatch(nameof(BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate))]
		public static class FoodLoading
		{
			public static void Postfix(GameObject go, ConduitType conduitType)
			{
				if (conduitType == ConduitType.Solid)
				{
					if (go.TryGetComponent<Storage>(out var storage))
					{
						storage.storageFilters = [.. STORAGEFILTERS.NOT_EDIBLE_SOLIDS, .. STORAGEFILTERS.FOOD];
					}
				}
			}
		}

	}
}
