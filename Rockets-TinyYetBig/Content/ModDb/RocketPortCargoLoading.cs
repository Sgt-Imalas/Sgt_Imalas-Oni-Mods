using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.RocketFueling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rockets_TinyYetBig.RocketFueling.FuelLoaderComponent;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class RocketPortCargoLoading
	{
		public static float TotalMassStoredOfItems(IEnumerable<GameObject> items)
		{
			float num = 0f;
			foreach (var item in items)
			{
				if (item != null && item.TryGetComponent<PrimaryElement>(out var element))
				{
					num += element.Mass;
				}
			}

			return Mathf.RoundToInt(num * 1000f) / 1000f;
		}


		/// <summary>
		/// This Method replaces the loading code for the landing platform to integrate fuel loaders, radbolt loaders and drillcone diamond storage loading
		/// </summary>
		/// <param name="craftInterface"></param>
		/// <param name="chain"></param>
		/// <param name="SetCompleteAction"></param>
		public static void ReplacedCargoLoadingMethod(CraftModuleInterface craftInterface, HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain, System.Action<bool> SetCompleteAction)
		{
			bool HasLoadingProcess = false;
			var CargoBaysPool = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, CraftModuleInterface>.PooledList, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, CraftModuleInterface>.Allocate();

			var FuelTanksPool = DictionaryPool<Element.State, ListPool<FuelTank, CraftModuleInterface>.PooledList, CraftModuleInterface>.Allocate();
			FuelTanksPool[Element.State.Solid] = ListPool<FuelTank, CraftModuleInterface>.Allocate();
			FuelTanksPool[Element.State.Liquid] = ListPool<FuelTank, CraftModuleInterface>.Allocate();
			FuelTanksPool[Element.State.Gas] = ListPool<FuelTank, CraftModuleInterface>.Allocate();

			var OxidizerTanks = ListPool<OxidizerTank, CraftModuleInterface>.Allocate();
			var HEPStorages = ListPool<HighEnergyParticleStorage, CraftModuleInterface>.Allocate();
			var DrillConeStorages = ListPool<Storage, CraftModuleInterface>.Allocate();


			Tag FuelTag = SimHashes.Void.CreateTag();

			bool hasOxidizer;

			foreach (Ref<RocketModuleCluster> clusterModuleRef in (IEnumerable<Ref<RocketModuleCluster>>)craftInterface.ClusterModules)
			{
				var clusterModule = clusterModuleRef.Get();

				if (clusterModule.TryGetComponent<RocketEngineCluster>(out var engine))
				{
					FuelTag = engine.fuelTag;
					hasOxidizer = engine.requireOxidizer;
				}

				if (clusterModule.TryGetComponent<FuelTank>(out var fueltank))
				{
					var ele = ElementLoader.GetElement(fueltank.FuelType);

					if (ele != null && !ele.IsVacuum)
					{
						//Mask out non-state related bits
						FuelTanksPool[ele.state & Element.State.Solid].Add(fueltank);
					}
					else if (clusterModule.TryGetComponent<ConduitConsumer>(out var conduitConsumer))
					{
						if (conduitConsumer.conduitType == ConduitType.Gas)
							FuelTanksPool[Element.State.Gas].Add(fueltank);
						else if (conduitConsumer.conduitType == ConduitType.Liquid)
							FuelTanksPool[Element.State.Liquid].Add(fueltank);
						else if (conduitConsumer.conduitType == ConduitType.Solid)
							FuelTanksPool[Element.State.Solid].Add(fueltank);
					}
					else if (clusterModule.TryGetComponent<Building>(out var building) && building.Def.InputConduitType != ConduitType.None)
					{
						var defType = building.Def.InputConduitType;
						if (defType == ConduitType.Gas)
							FuelTanksPool[Element.State.Gas].Add(fueltank);
						else if (defType == ConduitType.Liquid)
							FuelTanksPool[Element.State.Liquid].Add(fueltank);
						else if (defType == ConduitType.Solid)
							FuelTanksPool[Element.State.Solid].Add(fueltank);
					}
					else
						FuelTanksPool[Element.State.Solid].Add(fueltank);
				}

				if (clusterModule.TryGetComponent<HighEnergyParticleStorage>(out var hepStorage))
					HEPStorages.Add(hepStorage);

				if (clusterModule.TryGetComponent<OxidizerTank>(out var oxTank))
				{
					hasOxidizer = true;
					OxidizerTanks.Add(oxTank);
				}

				if (clusterModule.TryGetComponent<CargoBayCluster>(out var cargoBay) && cargoBay.storageType != CargoBay.CargoType.Entities && cargoBay.RemainingCapacity > 0f)
				{
					CargoBaysPool[cargoBay.storageType].Add(cargoBay);
				}

				//if (clusterModule.Get().GetSMI<ResourceHarvestModule.StatesInstance>() != null)
				//{
				//    if (clusterModule.Get().TryGetComponent<Storage>(out var DrillConeStorage))
				//        DrillConeStorages.Add(DrillConeStorage);
				//}
				//if (clusterModule.Get().TryGetComponent<DrillConeAssistentModule>(out var helperModule))
				//{
				//    DrillConeStorages.Add(helperModule.DiamondStorage);
				//}

				if (clusterModule.TryGetComponent<DrillConeModeHandler>(out var Handler))
				{
					if (Handler.LoadingAllowed)
						DrillConeStorages.Add(Handler.DiamondStorage);
				}
			}

			foreach (ChainedBuilding.StatesInstance smi1 in chain)
			{
				ModularConduitPortController.Instance modularConduitPortController = smi1.GetSMI<ModularConduitPortController.Instance>();
				FuelLoaderComponent fuelLoader = smi1.GetComponent<FuelLoaderComponent>();
				IConduitConsumer conduitConsumerComponent = smi1.GetComponent<IConduitConsumer>();
				var operational = smi1.GetComponent<Operational>();


				if (modularConduitPortController == null || operational == null)
					continue;

				bool isLoading = false;
				if (fuelLoader != null && operational.IsOperational && (modularConduitPortController.SelectedMode == ModularConduitPortController.Mode.Load || modularConduitPortController.SelectedMode == ModularConduitPortController.Mode.Both))
				{
					//shouldDoNormal = false;
					modularConduitPortController.SetRocket(true);
					if (fuelLoader.loaderType == LoaderType.Fuel)
					{
						GameObject[] AllItems = Concat(fuelLoader.solidStorage.items, fuelLoader.liquidStorage.items, fuelLoader.gasStorage.items).ToArray();
						for (int index = AllItems.Count() - 1; index >= 0; --index)
						{
							GameObject toTransferFuel = AllItems[index];
							if (!toTransferFuel.TryGetComponent<PrimaryElement>(out var primaryElement) || primaryElement.Element.IsVacuum)
								continue;
							//Mask out non-state related bits
							var elementState = primaryElement.Element.state & Element.State.Solid;

							foreach (FuelTank fueltank in FuelTanksPool[elementState])
							{
								float remainingCapacity = fueltank.Storage.RemainingCapacity();
								float loaderItemsMass = TotalMassStoredOfItems(AllItems);
								//SgtLogger.l($"{fueltank}; TankFuelType: {fueltank.fuelType} EngineTargetTag: {FuelTag}: {toTransferFuel.HasTag(FuelTag)} && has fueltank type: {toTransferFuel.HasTag(fueltank.fuelType)} ");
								if (remainingCapacity > 0 && loaderItemsMass > 0 && toTransferFuel.HasTag(FuelTag) && (fueltank.fuelType == null || toTransferFuel.HasTag(fueltank.fuelType)))
								{
									isLoading = true;
									HasLoadingProcess = true;
									Pickupable pickupable = toTransferFuel.GetComponent<Pickupable>().Take(remainingCapacity);
									if (pickupable != null)
									{
										fueltank.storage.Store(pickupable.gameObject, true);
										//float internalMassStored = remainingCapacity - pickupable.PrimaryElement.Mass;
									}
								}
							}
						}
					}
					else if (fuelLoader.loaderType == LoaderType.HEP)
					{
						foreach (HighEnergyParticleStorage hepTank in HEPStorages)
						{
							float remainingCapacity = hepTank.RemainingCapacity();
							float SourceAmount = fuelLoader.HEPStorage.Particles;
							if ((double)remainingCapacity > 0.0 && (double)SourceAmount > 0.0)
							{
								isLoading = true;
								HasLoadingProcess = true;
								float ParticlesTaken = fuelLoader.HEPStorage.ConsumeAndGet(remainingCapacity);
								if (ParticlesTaken > 0.0f)
								{
									hepTank.Store(ParticlesTaken);
								}
							}
						}
					}
					else if (fuelLoader.loaderType == LoaderType.Oxidizer)
					{
						GameObject[] AllOXItems = Concat(fuelLoader.solidStorage.items, fuelLoader.liquidStorage.items).ToArray();
						for (int index = AllOXItems.Count() - 1; index >= 0; --index)
						{
							GameObject storageItem = AllOXItems[index];
							foreach (OxidizerTank oxTank in OxidizerTanks)
							{
								float remainingCapacity = oxTank.storage.RemainingCapacity();
								float massStoredLoader = oxTank.supportsMultipleOxidizers ? fuelLoader.solidStorage.MassStored() : fuelLoader.liquidStorage.MassStored();
								bool tagAllowed = oxTank.supportsMultipleOxidizers
									? storageItem.GetComponent<KPrefabID>().HasAnyTags(oxTank.GetComponent<FlatTagFilterable>().selectedTags)
									: storageItem.HasTag(oxTank.GetComponent<ConduitConsumer>().capacityTag);
								if (remainingCapacity > 0 && massStoredLoader > 0.0 && tagAllowed)
								{
									isLoading = true;
									HasLoadingProcess = true;
									Pickupable pickupable = storageItem.GetComponent<Pickupable>().Take(remainingCapacity);
									if (pickupable != null)
									{
										oxTank.storage.Store(pickupable.gameObject, true);
										//float internalMassStored = remainingCapacity - pickupable.PrimaryElement.Mass;
									}
								}
							}
						}
					}
				}
				else if (operational.IsOperational && (modularConduitPortController.SelectedMode == ModularConduitPortController.Mode.Load || modularConduitPortController.SelectedMode == ModularConduitPortController.Mode.Both) && conduitConsumerComponent != null)
				{
					modularConduitPortController.SetRocket(true);
					for (int num = conduitConsumerComponent.Storage.items.Count - 1; num >= 0; num--)
					{
						GameObject gameObject = conduitConsumerComponent.Storage.items[num];
						foreach (var diamondStorage in DrillConeStorages)
						{
							float remainingCapacity = diamondStorage.RemainingCapacity();
							float loaderMassStored = conduitConsumerComponent.Storage.MassStored();
							bool filterable = diamondStorage.storageFilters != null && diamondStorage.storageFilters.Count > 0;
							if (remainingCapacity > 0f && loaderMassStored > 0f && (filterable ? diamondStorage.storageFilters.Contains(gameObject.PrefabID()) : true))
							{
								isLoading = true;
								HasLoadingProcess = true;
								Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(remainingCapacity);
								if (pickupable != null)
								{
									diamondStorage.Store(pickupable.gameObject);
									remainingCapacity -= pickupable.PrimaryElement.Mass;
								}
							}
						}
						if (gameObject == null)
							continue;

						foreach (CargoBayCluster cargoBayCluster in CargoBaysPool[CargoBayConduit.ElementToCargoMap[conduitConsumerComponent.ConduitType]])
						{
							float remainingCapacity = cargoBayCluster.RemainingCapacity;
							float loaderMassStored = conduitConsumerComponent.Storage.MassStored();

							if (remainingCapacity > 0f && loaderMassStored > 0f && cargoBayCluster.GetComponent<TreeFilterable>().AcceptedTags.Contains(gameObject.PrefabID()))
							{
								isLoading = true;
								HasLoadingProcess = true;
								Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(remainingCapacity);
								if (pickupable != null)
								{
									cargoBayCluster.storage.Store(pickupable.gameObject);
									remainingCapacity -= pickupable.PrimaryElement.Mass;
								}
							}
						}
					}
				}
				//SgtLogger.l(isLoading.ToString(), smi1.gameObject.GetProperName());
				modularConduitPortController?.SetLoading(isLoading);
			}

			chain.Recycle();
			CargoBaysPool[CargoBay.CargoType.Solids].Recycle();
			CargoBaysPool[CargoBay.CargoType.Liquids].Recycle();
			CargoBaysPool[CargoBay.CargoType.Gasses].Recycle();
			CargoBaysPool.Recycle();

			FuelTanksPool[Element.State.Solid].Recycle();
			FuelTanksPool[Element.State.Liquid].Recycle();
			FuelTanksPool[Element.State.Gas].Recycle();
			FuelTanksPool.Recycle();

			DrillConeStorages.Recycle();
			HEPStorages.Recycle();
			OxidizerTanks.Recycle();

			SetCompleteAction.Invoke(!HasLoadingProcess);
		}

		public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] arr)
		{
			foreach (IEnumerable col in arr)
				foreach (T item in col)
					yield return item;
		}
	}
}
