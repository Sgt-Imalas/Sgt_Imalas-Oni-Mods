using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using Rockets_TinyYetBig.RocketFueling;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.RocketFueling.FuelLoaderComponent;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class RocketPortCargoLoading
	{
		readonly struct OxTankEntry
		{
			public readonly OxidizerTank Tank;
			public readonly FlatTagFilterable Filter;   // null for single-oxidizer tanks
			public readonly Tag CapacityTag;            // default for multi-oxidizer tanks

			public Storage Storage => Tank.storage;
			public OxTankEntry(OxidizerTank tank)
			{
				Tank = tank;
				if (tank.supportsMultipleOxidizers)
				{
					Filter = tank.GetComponent<FlatTagFilterable>();
					CapacityTag = Tag.Invalid;
				}
				else if (tank.TryGetComponent<ConduitConsumer>(out var consumer))
				{
					Filter = null;
					CapacityTag = consumer.capacityTag;
				}
				else
				{
					Filter = null;
					CapacityTag = Tag.Invalid;
					Report(tank);
				}
			}
			static void Report(OxidizerTank tank)
			{
				if (_malformedOxidizerTanks.Contains(tank.GetInstanceID()))
					return;
				_malformedOxidizerTanks.Add(tank.GetInstanceID());
				SgtLogger.error("Invalid oxidizer tank found: " + tank.PrefabID());
			}

			static readonly HashSet<int> _malformedOxidizerTanks = [];

			public bool OxidizerAllowed(KPrefabID prefabId)
			{
				if (Tank.supportsMultipleOxidizers && Filter != null)
					return prefabId.HasAnyTags(Filter.selectedTags);
				return prefabId.HasTag(CapacityTag);
			}
		}
		readonly struct CargobayEntry
		{
			public readonly CargoBayCluster Cargobay;
			public readonly TreeFilterable Filter;
			public Storage Storage => Cargobay.storage;
			public CargobayEntry(CargoBayCluster cargobay)
			{
				Cargobay = cargobay;
				Filter = cargobay.GetComponent<TreeFilterable>();
			}
			public bool ItemAccepted(Tag itemId)
			{
				return Filter.AcceptedTags.Contains(itemId);
			}
		}


		public static void ReplacedCargoUnloadingMethod(CraftModuleInterface craftInterface, HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain, System.Action<bool> SetCompleteAction)
		{
			bool hidePopups = Config.Instance.CargoTransferPopupSupression;
			var CargoBaysPool = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, CraftModuleInterface>.PooledList, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, CraftModuleInterface>.Allocate();
			//var HEPStorages = ListPool<HighEnergyParticleStorage, CraftModuleInterface>.Allocate();
			var multiMaterialCargoBays = ListPool<MultiMaterialCargoBay, CraftModuleInterface>.Allocate();

			//SgtLogger.l("Unloading on " + craftInterface);

			foreach (Ref<RocketModuleCluster> clusterModuleRef in craftInterface.ClusterModules)
			{
				//SgtLogger.l("checking module " + clusterModuleRef.Get());	
				var clusterModule = clusterModuleRef.Get();


				//if (clusterModule.TryGetComponent<HighEnergyParticleStorage>(out var hepStorage) && clusterModule.TryGetComponent<RadiationBatteryOutputHandler>(out _)) //only allow battery module to unload
				//	HEPStorages.Add(hepStorage);


				if (clusterModule.TryGetComponent<CargoBayCluster>(out var cargoBay) && cargoBay.storageType != CargoBay.CargoType.Entities && cargoBay.storage.Count > 0f)
				{
					CargoBaysPool[cargoBay.storageType].Add(cargoBay);
					//SgtLogger.l(" has cargo bay with " + cargoBay.storage.Count + " items", clusterModule.gameObject.GetProperName());
				}
				if (clusterModule.TryGetComponent<MultiMaterialCargoBay>(out var advCargoBay) && advCargoBay.Storage.items.Count > 0)
				{
					multiMaterialCargoBays.Add(advCargoBay);
					//SgtLogger.l(" has multi material cargo bay with " + advCargoBay.Storage.Count + " items", clusterModule.gameObject.GetProperName());
				}
			}
			bool hasUnloadingProcess = false;
			foreach (ChainedBuilding.StatesInstance receiver in (HashSet<ChainedBuilding.StatesInstance>)chain)
			{
				ModularConduitPortController.Instance portControllerSmi = receiver.GetSMI<ModularConduitPortController.Instance>();
				IConduitDispenser conduitDispenser = receiver.GetComponent<IConduitDispenser>();
				Operational component2 = receiver.GetComponent<Operational>();
				bool isUnloading = false;
				if (conduitDispenser != null && (portControllerSmi == null || portControllerSmi.SelectedMode == ModularConduitPortController.Mode.Unload || portControllerSmi.SelectedMode == ModularConduitPortController.Mode.Both) && (component2 == null || component2.IsOperational)
					&& receiver.gameObject.TryGetComponent<TreeFilterable>(out var treeFilterable)
					)
				{
					portControllerSmi?.SetRocket(true);
					float amount = conduitDispenser.Storage.RemainingCapacity();
					//SgtLogger.l("remaining capacity: " + amount, receiver.gameObject.GetProperName());
					foreach (var multiMaterialCargoBay in multiMaterialCargoBays)
					{
						if (amount <= 0f && isUnloading)
							break;

						if (multiMaterialCargoBay.Storage.Count > 0)
						{
							for (int index = multiMaterialCargoBay.Storage.items.Count - 1; index >= 0; --index)
							{
								GameObject go = multiMaterialCargoBay.Storage.items[index];
								if (treeFilterable.AcceptedTags.Contains(go.PrefabID()))
								{
									isUnloading = true;
									hasUnloadingProcess = true;
									if (amount <= 0f)
										break;
									Pickupable pickupable = go.GetComponent<Pickupable>().Take(amount);
									if (pickupable != null)
									{
										conduitDispenser.Storage.Store(pickupable.gameObject, hidePopups);
										amount -= pickupable.PrimaryElement.Mass;
									}
									if (amount <= 0f)
										break;
								}
							}
						}
					}

					foreach (CargoBayCluster cargoBayCluster in CargoBaysPool[CargoBayConduit.ElementToCargoMap[conduitDispenser.ConduitType]])
					{
						if (amount <= 0f && isUnloading)
							break;

						if (cargoBayCluster.storage.Count > 0)
						{
							for (int index = cargoBayCluster.storage.items.Count - 1; index >= 0; --index)
							{
								GameObject go = cargoBayCluster.storage.items[index];
								if (treeFilterable.AcceptedTags.Contains(go.PrefabID()))
								{
									isUnloading = true;
									hasUnloadingProcess = true;
									if (amount <= 0f)
										break;
									Pickupable pickupable = go.GetComponent<Pickupable>().Take(amount);
									if (pickupable != null)
									{
										conduitDispenser.Storage.Store(pickupable.gameObject, hidePopups);
										amount -= pickupable.PrimaryElement.Mass;
									}
									if (amount <= 0f)
										break;
								}
							}
						}
					}
				}
				portControllerSmi?.SetUnloading(isUnloading);
			}
			chain.Recycle();
			CargoBaysPool[CargoBay.CargoType.Solids].Recycle();
			CargoBaysPool[CargoBay.CargoType.Liquids].Recycle();
			CargoBaysPool[CargoBay.CargoType.Gasses].Recycle();
			CargoBaysPool.Recycle();
			multiMaterialCargoBays.Recycle();
			//HEPStorages.Recycle();

			SetCompleteAction.Invoke(!hasUnloadingProcess);
		}


		/// <summary>
		/// This Method replaces the loading code for the landing platform to integrate fuel loaders, radbolt loaders and drillcone diamond storage loading
		/// </summary>
		/// <param name="craftInterface"></param>
		/// <param name="chain"></param>
		/// <param name="SetCompleteAction"></param>
		static readonly List<GameObject> ItemsInStorage = new List<GameObject>(64);
		static void CacheFuelLoaderItems(FuelLoaderComponent fuelLoader)
		{
			ItemsInStorage.Clear();
			ItemsInStorage.AddRange(fuelLoader.solidStorage.items);
			ItemsInStorage.AddRange(fuelLoader.liquidStorage.items);
			if (fuelLoader.loaderType == LoaderType.Fuel)
				ItemsInStorage.AddRange(fuelLoader.gasStorage.items);
		}

		public static void ReplacedCargoLoadingMethod(CraftModuleInterface craftInterface, HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain, System.Action<bool> SetCompleteAction)
		{
			bool HasLoadingProcess = false;
			var CargoBaysPool = DictionaryPool<CargoBay.CargoType, ListPool<CargobayEntry, CraftModuleInterface>.PooledList, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Solids] = ListPool<CargobayEntry, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Liquids] = ListPool<CargobayEntry, CraftModuleInterface>.Allocate();
			CargoBaysPool[CargoBay.CargoType.Gasses] = ListPool<CargobayEntry, CraftModuleInterface>.Allocate();

			var FuelTanksPool = DictionaryPool<Element.State, ListPool<FuelTank, CraftModuleInterface>.PooledList, CraftModuleInterface>.Allocate();
			FuelTanksPool[Element.State.Solid] = ListPool<FuelTank, CraftModuleInterface>.Allocate();
			FuelTanksPool[Element.State.Liquid] = ListPool<FuelTank, CraftModuleInterface>.Allocate();
			FuelTanksPool[Element.State.Gas] = ListPool<FuelTank, CraftModuleInterface>.Allocate();

			var OxidizerTanks = ListPool<OxTankEntry, CraftModuleInterface>.Allocate();
			var HEPStorages = ListPool<HighEnergyParticleStorage, CraftModuleInterface>.Allocate();
			var DrillConeStorages = ListPool<Storage, CraftModuleInterface>.Allocate();
			var multiMaterialCargoBays = ListPool<MultiMaterialCargoBay, CraftModuleInterface>.Allocate();

			Tag FuelTag = Tag.Invalid;
			bool hidePopups = Config.Instance.CargoTransferPopupSupression;

			//bool hasOxidizer;

			foreach (Ref<RocketModuleCluster> clusterModuleRef in craftInterface.ClusterModules)
			{
				var clusterModule = clusterModuleRef.Get();

				bool isRocketEngine = false;
				if (clusterModule.TryGetComponent<RocketEngineCluster>(out var engine))
				{
					isRocketEngine = true;
					FuelTag = engine.fuelTag;
					//hasOxidizer = engine.requireOxidizer;
				}
				if (clusterModule.TryGetComponent<EngineFuelFilter>(out var fuelFilter))
				{
					FuelTag = fuelFilter.SelectedFuel;
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
					if (isRocketEngine)
						HEPStorages.Insert(0, hepStorage);
					else
						HEPStorages.Add(hepStorage);

				if (clusterModule.TryGetComponent<OxidizerTank>(out var oxTank))
				{
					//hasOxidizer = true;
					OxidizerTanks.Add(new(oxTank));
				}

				if (clusterModule.TryGetComponent<CargoBayCluster>(out var cargoBay) && cargoBay.storageType != CargoBay.CargoType.Entities && cargoBay.RemainingCapacity > 0f)
				{
					CargoBaysPool[cargoBay.storageType].Add(new(cargoBay));
				}
				else if (clusterModule.TryGetComponent<MultiMaterialCargoBay>(out var advCargoBay))
				{
					multiMaterialCargoBays.Add(advCargoBay);
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

				if (clusterModule.TryGetComponent<ModuleDeliveryModeHandler>(out var Handler))
				{
					if (Handler.LoadingAllowed)
						DrillConeStorages.Add(Handler.DiamondStorage);
				}
			}

			var solidFuelTanks = FuelTanksPool[Element.State.Solid];
			var liquidFuelTanks = FuelTanksPool[Element.State.Liquid];
			var gasFuelTanks = FuelTanksPool[Element.State.Gas];

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
					modularConduitPortController.SetRocket(true);
					if (fuelLoader.loaderType == LoaderType.Fuel && FuelTag != Tag.Invalid)
					{
						CacheFuelLoaderItems(fuelLoader);
						for (int index = ItemsInStorage.Count - 1; index >= 0; --index)
						{
							GameObject toTransferFuel = ItemsInStorage[index];
							if (!toTransferFuel.TryGetComponent<Pickupable>(out var fuelPickupable) || fuelPickupable.primaryElement.Element.IsVacuum || !fuelPickupable.TryGetComponent<KPrefabID>(out var fuelPrefabId))
								continue;

							//items that are not valid engine fuel can never be transferred
							if (!fuelPrefabId.HasTag(FuelTag))
								continue;

							var itemElement = fuelPickupable.primaryElement;
							float remainingMass = itemElement.Mass;

							if (remainingMass <= 0f)
								continue;

							//Mask out non-state related bits
							var elementState = itemElement.Element.state & Element.State.Solid;

							var fuelTanks = elementState switch
							{
								Element.State.Solid => solidFuelTanks,
								Element.State.Liquid => liquidFuelTanks,
								Element.State.Gas => gasFuelTanks,
								_ => null
							};
							if (fuelTanks == null)
								continue;

							foreach (FuelTank fueltank in fuelTanks)
							{
								//if the tank has a limiting fuel tag, also check for that
								if (fueltank.fuelType != null && !fuelPrefabId.HasTag(fueltank.fuelType))
									continue;

								float remainingCapacity = fueltank.Storage.RemainingCapacity();
								if (remainingCapacity <= 0)
									continue;

								isLoading = true;
								HasLoadingProcess = true;
								Pickupable pickupable = fuelPickupable.Take(remainingCapacity);
								if (pickupable != null)
								{
									fueltank.storage.Store(pickupable.gameObject, true);
									remainingMass -= pickupable.primaryElement.Mass;
									//float internalMassStored = remainingCapacity - pickupable.PrimaryElement.Mass;
								}
								if (remainingMass <= 0)
									break;
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
						CacheFuelLoaderItems(fuelLoader);
						for (int index = ItemsInStorage.Count - 1; index >= 0; --index)
						{
							GameObject storageItem = ItemsInStorage[index];

							if (!storageItem.TryGetComponent<Pickupable>(out var oxidizerPickupable) || oxidizerPickupable.primaryElement.Element.IsVacuum || !storageItem.TryGetComponent<KPrefabID>(out var prefabID))
								continue;
							var itemElement = oxidizerPickupable.primaryElement;
							float remainingMass = itemElement.Mass;

							if (remainingMass <= 0f)
								continue;

							foreach (OxTankEntry oxTank in OxidizerTanks)
							{
								if (!oxTank.OxidizerAllowed(prefabID))
									continue;

								float remainingCapacity = oxTank.Storage.RemainingCapacity();
								if (remainingCapacity <= 0)
									continue;

								isLoading = true;
								HasLoadingProcess = true;
								Pickupable pickupable = oxidizerPickupable.Take(remainingCapacity);
								if (pickupable != null)
								{
									oxTank.Storage.Store(pickupable.gameObject, true);
									remainingMass -= pickupable.primaryElement.Mass;
								}
								if (remainingMass <= 0)
									break;
							}
						}
					}
				}
				else if (operational.IsOperational && (modularConduitPortController.SelectedMode == ModularConduitPortController.Mode.Load || modularConduitPortController.SelectedMode == ModularConduitPortController.Mode.Both) && conduitConsumerComponent != null)
				{
					modularConduitPortController.SetRocket(true);
					ListPool<CargobayEntry, CraftModuleInterface>.PooledList cargoBayEntries = null;

					for (int num = conduitConsumerComponent.Storage.items.Count - 1; num >= 0; num--)
					{
						GameObject item = conduitConsumerComponent.Storage.items[num];
						if (!item.TryGetComponent<Pickupable>(out var itemPickupable))
							continue;

						float remainingMass = itemPickupable.primaryElement.Mass;

						if (remainingMass <= 0f)
							continue;

						Tag itemId = item.PrefabID();


						foreach (MultiMaterialCargoBay mmcb in multiMaterialCargoBays)
						{
							if (!mmcb.Filterable.AcceptedTags.Contains(itemId))
								continue;

							float remainingCapacity = mmcb.RemainingCapacityFor(itemPickupable.primaryElement);
							if (remainingCapacity <= 0f)
								continue;

							isLoading = true;
							HasLoadingProcess = true;
							Pickupable pickupable = itemPickupable.Take(remainingCapacity);
							if (pickupable != null)
							{
								mmcb.Storage.Store(pickupable.gameObject, hidePopups);
								remainingMass -= pickupable.primaryElement.Mass;
							}
							if (remainingMass <= 0)
								break;
						}

						if (remainingMass <= 0)
							continue;

						foreach (var diamondStorage in DrillConeStorages)
						{
							bool filterable = diamondStorage.storageFilters != null && diamondStorage.storageFilters.Count > 0;
							if (filterable && !diamondStorage.storageFilters.Contains(itemId))
								continue;

							float remainingCapacity = diamondStorage.RemainingCapacity();
							if (remainingCapacity <= 0f)
								continue;

							isLoading = true;
							HasLoadingProcess = true;
							Pickupable pickupable = itemPickupable.Take(remainingCapacity);
							if (pickupable != null)
							{
								diamondStorage.Store(pickupable.gameObject, hidePopups);
								remainingMass -= pickupable.primaryElement.Mass;
							}
							if (remainingMass <= 0)
								break;
						}
						if (remainingMass <= 0)
							continue;

						if (cargoBayEntries == null)
							cargoBayEntries = CargoBaysPool[CargoBayConduit.ElementToCargoMap[conduitConsumerComponent.ConduitType]];

						foreach (CargobayEntry entry in cargoBayEntries)
						{
							if (!entry.ItemAccepted(itemId))
								continue;

							float remainingCapacity = entry.Cargobay.RemainingCapacity;
							if (remainingCapacity <= 0f)
								continue;

							isLoading = true;
							HasLoadingProcess = true;
							Pickupable pickupable = itemPickupable.Take(remainingCapacity);
							if (pickupable != null)
							{
								entry.Storage.Store(pickupable.gameObject, hidePopups);
								remainingMass -= pickupable.primaryElement.Mass;
							}
							if (remainingMass <= 0)
								break;
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
			multiMaterialCargoBays.Recycle();
			ItemsInStorage.Clear();

			SetCompleteAction.Invoke(!HasLoadingProcess);
		}
	}
}
