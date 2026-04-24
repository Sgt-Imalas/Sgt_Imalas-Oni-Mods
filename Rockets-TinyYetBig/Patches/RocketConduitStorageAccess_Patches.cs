using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace Rockets_TinyYetBig.Patches.RocketLoadingPatches
{
    public class RocketConduitStorageAccess_Patches
    {
        [HarmonyPatch(typeof(RocketConduitStorageAccess), nameof(RocketConduitStorageAccess.Sim200ms))]
        public static class RocketConduitStorageAccess_Sim200ms_Patch
		{
			/// <summary>
			/// This throws out food that is not allowed in cargo bays if there is no freezer module
			/// Also allows feeding the drillcone support module from inside the rocket
			/// </summary>
			public static bool CouldStorageAllowThisTag(Tag tagAllowed, Storage storage)
            {
                bool flag = false;
                foreach (Tag storageFilter in storage.storageFilters)
                {

                    if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(storageFilter).Contains(tagAllowed))
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
            public static bool Prefix(float dt,
                RocketConduitStorageAccess __instance,
                Operational ___operational,
                CraftModuleInterface ___craftModuleInterface,
                Filterable ___filterable)
            {
                if (___operational != null && !___operational.IsOperational)
                    return false;
                float currentMassStored = __instance.storage.MassStored();
                if ((double)currentMassStored >= (double)__instance.targetLevel - 0.01f && (double)currentMassStored <= (double)__instance.targetLevel + 0.01f)
                    return false;
                if (___operational != null)
                    ___operational.SetActive(true);
                float amount = __instance.targetLevel - currentMassStored;

				var storages = ListPool<Storage, RocketConduitStorageAccess>.Allocate();
				var remainingStorageCapacities = ListPool<float, RocketConduitStorageAccess>.Allocate();
				var storedMass = ListPool<float, RocketConduitStorageAccess>.Allocate();

				///Refilling drillcone support module with diamond
				if (Config.Instance.RefillDrillSupport && __instance.cargoType == CargoBay.CargoType.Solids)
                {
                    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)___craftModuleInterface.ClusterModules)
                    {

                        if (clusterModule.Get().TryGetComponent<DrillConeAssistentModule>(out var drillConeAssistentModule)
                            && amount < 0.0
                            && drillConeAssistentModule.DiamondStorage.RemainingCapacity() > 0.0)
                        {
                            double transferAmount = (double)Mathf.Min(-amount, drillConeAssistentModule.DiamondStorage.RemainingCapacity());

                            for (int index = __instance.storage.items.Count - 1; index >= 0; --index)
                            {
                                if (
                                    __instance.storage.items[index] == null ||
                                    __instance.storage.items[index].PrefabID() != SimHashes.Diamond.CreateTag())
                                {
                                    continue;
                                }
                                Pickupable pickupable = __instance.storage.items[index].GetComponent<Pickupable>().Take(-amount);

                                if (pickupable != null)
                                {
                                    amount += pickupable.PrimaryElement.Mass;
                                    drillConeAssistentModule.DiamondStorage.Store(pickupable.gameObject, true);
                                }
                                if ((double)amount >= 0.0)
                                    break;
                            }
                            if ((double)amount <= 0.0)
                                break;

                        }
                    }
                }


                foreach (Ref<RocketModuleCluster> clusterModule in ___craftModuleInterface.ClusterModules)
                {
                    var module = clusterModule.Get();

                    if (module.TryGetComponent<CargoBayCluster>(out var cargoBay) && cargoBay.storageType == __instance.cargoType)
                    {
                        storages.Add(cargoBay.storage);
						remainingStorageCapacities.Add(cargoBay.storage.RemainingCapacity());
						storedMass.Add(cargoBay.storage.MassStored());
					}
                    else if(module.TryGetComponent<MultiMaterialCargoBay> (out var multiMaterialCargoBay))
					{
						storages.Insert(0,multiMaterialCargoBay.Storage);
						remainingStorageCapacities.Insert(0, multiMaterialCargoBay.RemainingCapacityFor(__instance.cargoType));
                        storedMass.Insert(0, multiMaterialCargoBay.GetMassFor(__instance.cargoType));
					}
                }


                for(int i = 0; i < storages.Count; i++)
                {
                    var storage = storages[i];
                    var remainingCapacity = remainingStorageCapacities[i];
                    var massStored = storedMass[i];

					if (amount > 0.0f && massStored > 0.0f)
					{
						for (int index = storage.items.Count - 1; index >= 0; --index)
						{
							GameObject go = storage.items[index];
							if (!(___filterable != null) || !(___filterable.SelectedTag != GameTags.Void) || !(go.PrefabID() != ___filterable.SelectedTag)
								)
							{
								Pickupable pickupable = go.GetComponent<Pickupable>().Take(amount);
								if (pickupable != null)
								{
									amount -= pickupable.PrimaryElement.Mass;
									__instance.storage.Store(pickupable.gameObject, true);
								}
								if (amount <= 0.0)
									break;
							}
						}
						if (amount <= 0.0)
							break;
					}


					if (amount < 0.0f && remainingCapacity > 0.0f)
					{
						for (int index = __instance.storage.items.Count - 1; index >= 0; --index)
						{
							if (__instance.storage.items[index] != null &&
								__instance.cargoType == CargoBay.CargoType.Solids &&
								!CouldStorageAllowThisTag(__instance.storage.items[index].PrefabID(), storage))
							{
								continue;
							}
							Pickupable pickupable = __instance.storage.items[index].GetComponent<Pickupable>().Take(-amount);

							if (pickupable != null)
							{
								amount += pickupable.PrimaryElement.Mass;
								storage.Store(pickupable.gameObject, true);
							}
							if (amount >= 0.0)
								break;
						}
						if (amount >= 0.0)
							break;
					}
				}
				storages.Recycle();
				remainingStorageCapacities.Recycle();
                storedMass.Recycle();
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
                        List<Tag> tagList = new List<Tag>();
                        tagList.AddRange(STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
                        tagList.AddRange(STORAGEFILTERS.FOOD);
                        storage.storageFilters = tagList;
                    }
                }
            }
        }

    }
}
