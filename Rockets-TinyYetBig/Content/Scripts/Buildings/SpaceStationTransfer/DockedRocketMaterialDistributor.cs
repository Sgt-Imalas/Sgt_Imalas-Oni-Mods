using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets_TinyYetBig
{
	class DockedRocketMaterialDistributor :
  GameStateMachine<DockedRocketMaterialDistributor, DockedRocketMaterialDistributor.Instance, IStateMachineTarget, DockedRocketMaterialDistributor.Def>
	{
		public State inoperational;
		public DockedRocketMaterialDistributor.OperationalStates operational;
		private TargetParameter attachedRocket;
		private BoolParameter emptyComplete;
		private BoolParameter fillComplete;

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inoperational;
			this.serializable = StateMachine.SerializeType.ParamsOnly;

			this.inoperational
				.EventTransition(GameHashes.OperationalChanged, (State)this.operational, (smi => smi.GetComponent<Operational>().IsOperational));

			this.operational
				.DefaultState(this.operational.noRocket)
				.EventTransition(GameHashes.OperationalChanged, this.inoperational, (smi => !smi.GetComponent<Operational>().IsOperational))
				//.EventHandler(GameHashes.ChainedNetworkChanged, ((smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi)))
				.EventHandler(ModAssets.Hashes.DockingConnectionConnected, ((smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi)));

			this.operational
				.noRocket
					.Update(((smi, dt) => this.SetAttachedRocket(smi.GetDockedRocket(), smi)))
					.EventHandler(ModAssets.Hashes.DockingConnectionConnected, (smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi))
					//.EventHandler(GameHashes.RocketCreated, (smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi))
					.ParamTransition<GameObject>(this.attachedRocket, this.operational.hasRocket, ((smi, p) => p != null));

			//this.operational
			//    .rocketLanding
			//        .EventTransition(GameHashes.RocketLaunched, this.operational.rocketLost)
			//        .OnTargetLost(this.attachedRocket, this.operational.rocketLost)
			//        .Target(this.attachedRocket)
			//        .TagTransition(GameTags.RocketOnGround, (State)this.operational.hasRocket)
			//        .Target(this.masterTarget);

			this.operational
				.hasRocket
					.DefaultState((State)this.operational.hasRocket.transferring)
					.Update(((smi, dt) => smi.EmptyRocket(dt)), UpdateRate.SIM_1000ms)
					.Update(((smi, dt) => smi.FillRocket(dt)), UpdateRate.SIM_1000ms)
					.EventTransition(ModAssets.Hashes.DockingConnectionDisconnected, this.operational.rocketLost)
					.OnTargetLost(this.attachedRocket, this.operational.rocketLost)
					.Target(this.attachedRocket)
					.Target(this.masterTarget)
					.Enter((smi) => smi.SetConnectedRocketStatusLoading(true))
					.UpdateTransition(operational.rocketLost, (smi, dt) => { return smi.GetDockedRocket() == null; })
			;

			this.operational
				.hasRocket
					.transferring
					.DefaultState(this.operational.hasRocket.transferring.actual)
					.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoEmptying)
					.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFilling);

			this.operational
				.hasRocket
					.transferring
						.actual
						.ParamTransition<bool>(this.emptyComplete, this.operational.hasRocket.transferring.delay, ((smi, p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi)))
						.ParamTransition<bool>(this.fillComplete, this.operational.hasRocket.transferring.delay, ((smi, p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi)));

			this.operational
				.hasRocket
					.transferring
						.delay
						.ParamTransition<bool>(this.fillComplete, this.operational.hasRocket.transferring.actual, IsFalse)
						.ParamTransition<bool>(this.emptyComplete, this.operational.hasRocket.transferring.actual, IsFalse)
						.ScheduleGoTo(4f, this.operational.hasRocket.transferComplete);

			this.operational
				.hasRocket
					.transferComplete
					.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFull)
					.ToggleTag(GameTags.TransferringCargoComplete)
					.ParamTransition<bool>(this.fillComplete, (State)this.operational.hasRocket.transferring, IsFalse)
					.ParamTransition<bool>(this.emptyComplete, (State)this.operational.hasRocket.transferring, IsFalse)
					.EventTransition(ModAssets.Hashes.DockingConnectionDisconnected, this.operational.rocketLost)
				.Enter((smi) =>
				{
					smi.SetConnectedRocketStatusLoading(false);
					// this.SetAttachedRocket(null, smi);
				});

			this.operational
				.rocketLost
				.Enter((smi =>
				{
					this.emptyComplete.Set(false, smi);
					this.fillComplete.Set(false, smi);
					this.SetAttachedRocket(null, smi);
				}))
				.GoTo(this.operational.noRocket);
		}

		private void SetAttachedRocket(
		  CraftModuleInterface attached,
		  DockedRocketMaterialDistributor.Instance smi)
		{
			if (attached == this.attachedRocket.Get(smi))
				return;

			HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
			smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
			foreach (StateMachine.Instance smi1 in (HashSet<ChainedBuilding.StatesInstance>)chain)
				smi1.GetSMI<ModularConduitPortController.Instance>()?.SetRocket((UnityEngine.Object)attached != (UnityEngine.Object)null);
			this.attachedRocket.Set((KMonoBehaviour)attached, smi);
			chain.Recycle();

		}

		public class Def : StateMachine.BaseDef
		{
		}

		public class HasRocketStates :
		  State
		{
			public DockedRocketMaterialDistributor.HasRocketStates.TransferringStates transferring;
			public State transferComplete;

			public class TransferringStates :
			  State
			{
				public State actual;
				public State delay;
			}
		}

		public class OperationalStates :
		  State
		{
			public State noRocket;
			public State rocketLost;
			public HasRocketStates hasRocket;
		}

		public new class Instance :
		  GameStateMachine<DockedRocketMaterialDistributor, DockedRocketMaterialDistributor.Instance, IStateMachineTarget, DockedRocketMaterialDistributor.Def>.GameInstance
		{
			public Instance(IStateMachineTarget master, DockedRocketMaterialDistributor.Def def)
			  : base(master, def)
			{
			}

			public CraftModuleInterface GetDockedRocket()
			{

				if (this.gameObject.TryGetComponent<IDockable>(out var door)
					&& DockingManagerSingleton.Instance.TryGetDockableIfDocked(door.GUID, out var targetDock))
				{
					return targetDock.spacecraftHandler.Interface;
				}
				return null;
			}

			public void SetConnectedRocketStatusLoading(bool isLoadingOrUnloading)
			{
				gameObject.TryGetComponent<IDockable>(out var dockable);

				if (DockingManagerSingleton.Instance.TryGetDockableIfDocked(dockable.GUID, out var connected))
				{
					connected.spacecraftHandler.SetCurrentlyLoadingStuff(isLoadingOrUnloading);
				}
			}

			public void EmptyRocket(float dt)
			{
				CraftModuleInterface craftInterface = this.sm.attachedRocket.Get<CraftModuleInterface>(this.smi);
				DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.PooledList, DockedRocketMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.PooledList, DockedRocketMaterialDistributor>.Allocate();
				pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
				pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
				pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
				foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)craftInterface.ClusterModules)
				{
					CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.storageType != CargoBay.CargoType.Entities && (double)component.storage.MassStored() > 0.0)
						pooledDictionary[component.storageType].Add(component);
				}
				bool flag = false;
				HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
				this.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
				foreach (ChainedBuilding.StatesInstance smi1 in (HashSet<ChainedBuilding.StatesInstance>)chain)
				{
					ModularConduitPortController.Instance smi2 = smi1.GetSMI<ModularConduitPortController.Instance>();
					IConduitDispenser component1 = smi1.GetComponent<IConduitDispenser>();
					Operational component2 = smi1.GetComponent<Operational>();
					bool isUnloading = false;
					if (component1 != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Unload || smi2.SelectedMode == ModularConduitPortController.Mode.Both) && ((UnityEngine.Object)component2 == (UnityEngine.Object)null || component2.IsOperational))
					{
						smi2.SetRocket(true);
						TreeFilterable component3 = smi1.GetComponent<TreeFilterable>();
						float amount = component1.Storage.RemainingCapacity();
						foreach (CargoBayCluster cargoBayCluster in (List<CargoBayCluster>)pooledDictionary[CargoBayConduit.ElementToCargoMap[component1.ConduitType]])
						{
							if (cargoBayCluster.storage.Count != 0)
							{
								for (int index = cargoBayCluster.storage.items.Count - 1; index >= 0; --index)
								{
									GameObject go = cargoBayCluster.storage.items[index];
									if (component3.AcceptedTags.Contains(go.PrefabID()))
									{
										isUnloading = true;
										flag = true;
										if ((double)amount > 0.0)
										{
											Pickupable pickupable = go.GetComponent<Pickupable>().Take(amount);
											if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
											{
												component1.Storage.Store(pickupable.gameObject);
												amount -= pickupable.PrimaryElement.Mass;
											}
										}
										else
											break;
									}
								}
							}
						}
					}
					smi2?.SetUnloading(isUnloading);
				}
				chain.Recycle();
				pooledDictionary[CargoBay.CargoType.Solids].Recycle();
				pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
				pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
				pooledDictionary.Recycle();
				this.sm.emptyComplete.Set(!flag, this);
			}

			public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] arr)
			{
				foreach (IEnumerable col in arr)
					foreach (T item in col)
						yield return item;
			}

			public void FillRocket(float dt)
			{
				CraftModuleInterface craftInterface = this.sm.attachedRocket.Get<CraftModuleInterface>(this.smi);

				HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
				this.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);

				System.Action<bool> fillCompleteAction = new Action<bool>((isLoading) => this.sm.fillComplete.Set(isLoading, this.smi));

				RocketPortCargoLoading.ReplacedCargoLoadingMethod(craftInterface, chain, fillCompleteAction);
			}
		}
	}

}
