using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MassMoveTo.Content.Scripts
{
	internal class MultiFetch_MovePickupableChore : Chore<MultiFetch_MovePickupableChore.StatesInstance>
	{
		public class StatesInstance : GameStateMachine<States, StatesInstance, MultiFetch_MovePickupableChore, object>.GameInstance
		{
			public StatesInstance(MultiFetch_MovePickupableChore master)
				: base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, MultiFetch_MovePickupableChore>
		{
			public class ApproachStorage : State
			{
				public ApproachSubState<Storage> deliveryStorage;

				public ApproachSubState<Storage> unbagCritter;
			}

			public class DeliveryState : State
			{
				public State storing;

				public State deliverfail;
			}

			public class FetchState : State
			{
				public ApproachSubState<Pickupable> approach;

				public State pickup;

				public State approachCritter;

				public State wrangle;
			}

			public static CellOffset[] critterCellOffsets = new CellOffset[1]
			{
			new CellOffset(0, 0)
			};

			public static HashedString[] critterReleaseWorkAnims = new HashedString[2] { "place", "release" };

			public static KAnimFile[] critterReleaseAnim = new KAnimFile[1] { Assets.GetAnim("anim_restrain_creature_kanim") };

			public TargetParameter deliverer;

			public TargetParameter pickupablesource;

			public TargetParameter pickup;

			public TargetParameter deliverypoint;

			public FloatParameter requestedamount;

			public FloatParameter actualamount;

			public FetchState fetch;

			public ApproachStorage approachstorage;

			public State success;

			public DeliveryState delivering;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = fetch;
				Target(deliverypoint);
				fetch.Target(deliverer).DefaultState(fetch.approach).Enter(delegate (StatesInstance smi)
				{
					pickupablesource.Get<Pickupable>(smi).ClearReservations();
				})
					.ToggleReserve(deliverer, pickupablesource, requestedamount, actualamount)
					.EnterTransition(fetch.approachCritter, (StatesInstance smi) => IsCritter(smi))
					.OnTargetLost(pickupablesource, null);
				fetch.approachCritter.Enter(delegate (StatesInstance smi)
				{
					GameObject gameObject5 = pickupablesource.Get(smi);
					if (!gameObject5.HasTag(GameTags.Creatures.Bagged))
					{
						IdleStates.Instance sMI = gameObject5.GetSMI<IdleStates.Instance>();
						if (!sMI.IsNullOrStopped())
						{
							sMI.GoTo(sMI.sm.root);
						}

						FlopStates.Instance sMI2 = gameObject5.GetSMI<FlopStates.Instance>();
						if (!sMI2.IsNullOrStopped())
						{
							sMI2.GoTo(sMI2.sm.root);
						}

						gameObject5.GetComponent<Navigator>().Stop();
					}
				}).MoveTo<Capturable>(pickupablesource, fetch.wrangle, (Func<StatesInstance, NavTactic>)GetNavTactic, (State)null, (CellOffset[])null);
				fetch.wrangle.EnterTransition(fetch.approach, (StatesInstance smi) => pickupablesource.Get(smi).HasTag(GameTags.Creatures.Bagged)).ToggleWork<Capturable>(pickupablesource, fetch.approach, null, null);
				fetch.approach.MoveTo<IApproachable>(pickupablesource, fetch.pickup, (Func<StatesInstance, NavTactic>)GetNavTactic, (State)null, (CellOffset[])null);
				fetch.pickup.DoPickup(pickupablesource, pickup, actualamount, approachstorage, delivering.deliverfail).Exit(delegate (StatesInstance smi)
				{
					GameObject gameObject4 = pickup.Get(smi);
					Movable movable = ((gameObject4 != null) ? gameObject4.GetComponent<Movable>() : null);
					if (movable != null && movable.onPickupComplete != null)
					{
						movable.onPickupComplete(gameObject4);
					}
				});
				approachstorage.DefaultState(approachstorage.deliveryStorage);
				approachstorage.deliveryStorage.InitializeStates(GetNavTactic, deliverer, deliverypoint, delivering.storing, delivering.deliverfail).Target(deliverer).EventHandler(GameHashes.OnStorageChange, delegate (StatesInstance smi, object data)
				{
					GameObject gameObject2 = data as GameObject;
					if (gameObject2 != null)
					{
						GameObject gameObject3 = pickup.Get(smi);
						if (gameObject3 == null || gameObject2 == gameObject3)
						{
							smi.GoTo(delivering.deliverfail);
						}
					}
				});
				delivering.storing.Target(deliverer).DoDelivery(deliverer, deliverypoint, success, delivering.deliverfail);
				delivering.deliverfail.ReturnFailure();
				success.Enter(delegate (StatesInstance smi)
				{
					Storage component = deliverypoint.Get(smi).GetComponent<Storage>();
					Storage component2 = deliverer.Get(smi).GetComponent<Storage>();
					float num = actualamount.Get(smi);
					GameObject gameObject = pickup.Get(smi);
					num += gameObject.GetComponent<PrimaryElement>().Mass;
					actualamount.Set(num, smi);
					component2.Transfer(pickup.Get(smi), component);
					DropPickupable(component, gameObject);
					MultiFetch_CancellableMove component3 = component.GetComponent<MultiFetch_CancellableMove>();
					Movable component4 = gameObject.GetComponent<Movable>();
					component3.MultiChore_RemoveMovable(component4);
					component4.ClearMove();					
				}).ReturnSuccess();
			}

			public NavTactic GetNavTactic(StatesInstance smi)
			{
				WorkerBase component = deliverer.Get(smi).GetComponent<WorkerBase>();
				if (component != null && component.IsFetchDrone())
				{
					return NavigationTactics.FetchDronePickup;
				}

				return NavigationTactics.ReduceTravelDistance;
			}

			public void DropPickupable(Storage storage, GameObject delivered)
			{
				if (delivered.GetComponent<Capturable>() != null)
				{
					List<GameObject> items = storage.items;
					int count = items.Count;
					Vector3 position = Grid.CellToPosCBC(Grid.PosToCell(storage), Grid.SceneLayer.Creatures);
					for (int num = count - 1; num >= 0; num--)
					{
						GameObject gameObject = items[num];
						storage.Drop(gameObject);
						gameObject.transform.SetPosition(position);
						gameObject.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
					}
				}
				else
				{
					storage.DropAll();
				}

				Movable component = delivered.GetComponent<Movable>();
				if (component.onDeliveryComplete != null)
				{
					component.onDeliveryComplete(delivered);
				}
			}

			public bool IsCritter(StatesInstance smi)
			{
				GameObject gameObject = pickupablesource.Get(smi);
				if (gameObject != null)
				{
					return gameObject.GetComponent<Capturable>() != null;
				}

				return false;
			}
		}

		public int pickupableOnReachableChangedHandlerID;

		public int targetOnReachableChangedHandlerID;

		public GameObject movePlacer;

		public static Precondition CanReachCritter = new Precondition
		{
			id = "CanReachCritter",
			description = global::STRINGS.DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
			fn = delegate (ref Precondition.Context context, object data)
			{
				GameObject gameObject = (GameObject)data;
				return !(gameObject == null) && gameObject.HasTag(GameTags.Reachable);
			}
		};

		public MultiFetch_MovePickupableChore(IStateMachineTarget target, GameObject pickupable, Action<Chore> onEnd)
			: base((!Movable.IsCritterPickupable(pickupable)) ? Db.Get().ChoreTypes.Fetch : Db.Get().ChoreTypes.Ranch, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, onEnd, PriorityScreen.PriorityClass.basic, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
		{
			base.smi = new StatesInstance(this);
			Pickupable component = pickupable.GetComponent<Pickupable>();
			AddPrecondition(ChorePreconditions.instance.CanMoveTo, target.GetComponent<Storage>());
			AddPrecondition(ChorePreconditions.instance.IsNotARobot, "FetchDrone");
			AddPrecondition(ChorePreconditions.instance.IsNotTransferArm, this);
			if (Movable.IsCritterPickupable(pickupable))
			{
				AddPrecondition(CanReachCritter, pickupable);
				AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanWrangleCreatures);
				IApproachable approachable = pickupable.GetComponent<IApproachable>();
				AddPrecondition(ChorePreconditions.instance.CanMoveToDynamicCell, (Func<int>)(() => approachable.GetCell()));
			}
			else
			{
				AddPrecondition(ChorePreconditions.instance.CanPickup, component);
			}

			PrimaryElement primaryElement = component.PrimaryElement;
			base.smi.sm.requestedamount.Set(primaryElement.Mass, base.smi);
			base.smi.sm.pickupablesource.Set(pickupable.gameObject, base.smi);
			base.smi.sm.deliverypoint.Set(target.gameObject, base.smi);
			movePlacer = target.gameObject;
			OnReachableChanged(BoxedBools.Box(MinionGroupProber.Get().IsReachable(Grid.PosToCell(pickupable), OffsetGroups.Standard) && MinionGroupProber.Get().IsReachable(Grid.PosToCell(target.gameObject), OffsetGroups.Standard)));
			pickupableOnReachableChangedHandlerID = pickupable.Subscribe(-1432940121, OnReachableChanged);
			targetOnReachableChangedHandlerID = target.Subscribe(-1432940121, OnReachableChanged);
			Prioritizable component2 = target.GetComponent<Prioritizable>();
			if (!component2.IsPrioritizable())
			{
				component2.AddRef();
			}

			SetPrioritizable(target.GetComponent<Prioritizable>());
		}

		public override void Cleanup()
		{
			base.Cleanup();
			if (target != null)
			{
				target.Unsubscribe(targetOnReachableChangedHandlerID);
			}

			GameObject gameObject = base.smi.sm.pickupablesource.Get(base.smi);
			if (gameObject != null)
			{
				gameObject.Unsubscribe(pickupableOnReachableChangedHandlerID);
			}
		}

		public void OnReachableChanged(object data)
		{
			Color color = (((Boxed<bool>)data).value ? Color.white : new Color(0.91f, 0.21f, 0.2f));
			SetColor(movePlacer, color);
		}

		public void SetColor(GameObject visualizer, Color color)
		{
			if (visualizer != null)
			{
				visualizer.GetComponentInChildren<MeshRenderer>().material.color = color;
			}
		}

		public override void Begin(Precondition.Context context)
		{
			if (context.consumerState.consumer == null)
			{
				Debug.LogError("MovePickupable null context.consumer");
				return;
			}

			if (base.smi == null)
			{
				Debug.LogError("MovePickupable null smi");
				return;
			}

			if (base.smi.sm == null)
			{
				Debug.LogError("MovePickupable null smi.sm");
				return;
			}

			if (base.smi.sm.pickupablesource == null)
			{
				Debug.LogError("MovePickupable null smi.sm.pickupablesource");
				return;
			}

			base.smi.sm.deliverer.Set(context.consumerState.gameObject, base.smi);
			base.Begin(context);
		}
	}
}
