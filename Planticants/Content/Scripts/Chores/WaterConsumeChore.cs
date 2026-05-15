using FoodRehydrator;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Planticants.Content.Scripts.Chores
{
	internal class WaterConsumeChore : Chore<WaterConsumeChore.StatesInstance>
	{
		public static readonly Chore.Precondition EdibleIsNotNull = new Chore.Precondition()
		{
			id = nameof(EdibleIsNotNull),
			description = (string)global::STRINGS.DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
			fn = ((ref Chore.Precondition.Context context, object data) => null != context.consumerState.consumer.GetSMI<RationMonitor.Instance>().GetEdible())
		};

		public static IDiningSeat ResolveDiningSeat(GameObject messStation)
		{
			if (messStation == null)
			{
				Debug.LogWarning((object)"messStation GameObject is null");
				return (IDiningSeat)null;
			}
			IDiningSeat component;
			if (messStation.TryGetComponent<IDiningSeat>(out component))
				return component;
			Debug.LogWarning((object)"messStation GameObject has no IDiningSeat component");
			return (IDiningSeat)null;
		}

		private static KAnimFile ResolveEatAnim(IDiningSeat diningSeat, bool dinerIsBionic)
		{
			HashedString name = diningSeat != null ? (dinerIsBionic ? diningSeat.ReloadElectrobankAnim : diningSeat.EatAnim) : MessStation.eatAnim;
			KAnimFile anim = Assets.GetAnim(name);
			if (!(anim == null))
				return anim;
			Debug.LogError((object)$"Animation asset [{name}] does not exist");
			return (KAnimFile)null;
		}

		private static KAnimFile ResolveEatAnim(GameObject messStation, bool dinerIsBionic)
		{
			return WaterConsumeChore.ResolveEatAnim(WaterConsumeChore.ResolveDiningSeat(messStation), dinerIsBionic);
		}

		public WaterConsumeChore(IStateMachineTarget master)
		  : base(Db.Get().ChoreTypes.Eat, master, master.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.personalNeeds, report_type: ReportManager.ReportType.PersonalTime)
		{
			this.smi = new WaterConsumeChore.StatesInstance(this);
			this.showAvailabilityInHoverText = false;
			this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, (object)null);
			this.AddPrecondition(WaterConsumeChore.EdibleIsNotNull, (object)null);
		}

		public override void Begin(Chore.Precondition.Context context)
		{
			if (context.consumerState.consumer == null)
			{
				Debug.LogError((object)"WaterConsumeChore null context.consumer");
			}
			else
			{
				RationMonitor.Instance smi = context.consumerState.consumer.GetSMI<RationMonitor.Instance>();
				if (smi == null)
				{
					Debug.LogError((object)"WaterConsumeChore null RationMonitor.Instance");
				}
				else
				{
					Edible edible = smi.GetEdible();
					if (edible.gameObject == null)
						Debug.LogError((object)"WaterConsumeChore null edible.gameObject");
					else if (this.smi == null)
						Debug.LogError((object)"WaterConsumeChore null smi");
					else if (this.smi.sm == null)
						Debug.LogError((object)"WaterConsumeChore null smi.sm");
					else if (this.smi.sm.ediblesource == null)
					{
						Debug.LogError((object)"WaterConsumeChore null smi.sm.ediblesource");
					}
					else
					{
						this.smi.sm.ediblesource.Set(edible.gameObject, this.smi, false);
						KCrashReporter.Assert((double)edible.FoodInfo.CaloriesPerUnit > 0.0, edible.GetProperName() + " has invalid calories per unit. Will result in NaNs");
						AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(this.gameObject);
						float num1 = (amountInstance.GetMax() - amountInstance.value) / edible.FoodInfo.CaloriesPerUnit;
						KCrashReporter.Assert((double)num1 > 0.0, "WaterConsumeChore is requesting an invalid amount of food");
						double num2 = (double)this.smi.sm.requestedfoodunits.Set(num1, this.smi);
						this.smi.sm.eater.Set(context.consumerState.gameObject, this.smi, false);
						base.Begin(context);
					}
				}
			}
		}

		public static bool IsMessStationNonOperational(GameObject messStation)
		{
			if (messStation == null)
				return true;
			IDiningSeat diningSeat = WaterConsumeChore.ResolveDiningSeat(messStation);
			if (diningSeat == null)
				return true;
			Operational operational = diningSeat.FindOperational();
			return operational == null || !operational.IsOperational;
		}

		private static bool IsMessStationNonOperational(WaterConsumeChore.StatesInstance _, GameObject messStation)
		{
			return WaterConsumeChore.IsMessStationNonOperational(messStation);
		}

		public class StatesInstance(WaterConsumeChore master) :
		  GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.GameInstance(master)
		{
			private int locatorCell;
			public KAnimFile eatAnim;

			private static Assignable GetPreferredMessStation(GameObject diner)
			{
				Ownables soleOwner = diner.GetComponent<MinionIdentity>().GetSoleOwner();
				Navigator component;
				diner.TryGetComponent<Navigator>(out component);
				foreach (Assignable preferredAssignable in Game.Instance.assignmentManager.GetPreferredAssignables((Assignables)soleOwner, component, Db.Get().AssignableSlots.MessStation))
				{
					IDiningSeat diningSeat = WaterConsumeChore.ResolveDiningSeat(preferredAssignable.gameObject);
					if (diningSeat != null)
					{
						Operational operational = diningSeat.FindOperational();
						if ((!(operational != null) || operational.IsOperational) && preferredAssignable.GetComponent<Reservable>().IsReservableBy(diner))
							return preferredAssignable;
					}
				}
				return (Assignable)null;
			}

			public static Assignable ReserveMessStation(GameObject messStation, GameObject diner)
			{
				if (messStation != null)
					messStation.GetComponent<Reservable>().ClearReservation();
				Assignable preferredMessStation = WaterConsumeChore.StatesInstance.GetPreferredMessStation(diner);
				if (preferredMessStation != null)
				{
					Reservable component = preferredMessStation.GetComponent<Reservable>();
					if (!component.Reserve(diner))
					{
						if (component.IsReservableBy(diner))
							Debug.Log((object)"Failed to reserve dining seat. We have already reserved it.");
						else
							Debug.LogWarning((object)"Failed to reserve dining seat. Someone else has already reserved it!");
					}
				}
				return preferredMessStation;
			}

			public void UpdateMessStation()
			{
				this.sm.messstation.Set((KMonoBehaviour)WaterConsumeChore.StatesInstance.ReserveMessStation(this.sm.messstation.Get(this.smi), this.sm.eater.Get(this.smi)), this.smi);
			}

			public void ClearMessStation()
			{
				GameObject gameObject = this.smi.sm.messstation.Get(this.smi);
				if (gameObject != null)
					gameObject.GetComponent<Reservable>().ClearReservation();
				this.sm.messstation.Set((KMonoBehaviour)null, this.smi);
			}

			public static bool UseSalt(GameObject messStation)
			{
				if (messStation == null)
					return false;
				IDiningSeat diningSeat = WaterConsumeChore.ResolveDiningSeat(messStation);
				return diningSeat != null && diningSeat.HasSalt;
			}

			public bool UseSalt()
			{
				return this.smi.sm.messstation != null && WaterConsumeChore.StatesInstance.UseSalt(this.sm.messstation.Get(this.smi));
			}

			public static (GameObject, int) CreateLocator(
			  Sensors sensors,
			  Transform transform,
			  string locatorName)
			{
				int num = sensors.GetSensor<SafeCellSensor>().GetCellQuery();
				if (num == Grid.InvalidCell)
					num = Grid.PosToCell(transform.GetPosition());
				Vector3 posCbc = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
				Grid.Reserved[num] = true;
				return (ChoreHelpers.CreateLocator(locatorName, posCbc), num);
			}

			public void CreateLocator()
			{
				GameObject gameObject;
				(gameObject, this.locatorCell) = WaterConsumeChore.StatesInstance.CreateLocator(this.sm.eater.Get<Sensors>(this.smi), this.sm.eater.Get<Transform>(this.smi), "EatLocator");
				this.sm.locator.Set(gameObject, this, false);
			}

			public void DestroyLocator()
			{
				Grid.Reserved[this.locatorCell] = false;
				ChoreHelpers.DestroyLocator(this.sm.locator.Get(this));
				this.sm.locator.Set((KMonoBehaviour)null, this);
			}

			public static KAnimFile OnEnterMessStation(
			  GameObject messStation,
			  GameObject diner,
			  GameObject food,
			  bool dinerIsBionic,
			  float? effectDurationOverride = null)
			{
				IDiningSeat diningSeat = WaterConsumeChore.ResolveDiningSeat(messStation);
				if (diningSeat == null)
					return (KAnimFile)null;
				KAnimControllerBase component1 = diner.GetComponent<KAnimControllerBase>();
				KAnimFile kanimFile = WaterConsumeChore.ResolveEatAnim(diningSeat, dinerIsBionic);
				KAnimFile kanim_file = kanimFile;
				component1.AddAnimOverrides(kanim_file);
				Edible component2;
				if (food != null && food.TryGetComponent<Edible>(out component2))
					component2.workLayer = Grid.SceneLayer.BuildingFront;
				EffectInstance effectInstance1 = (EffectInstance)null;
				Effects component3 = diner.GetComponent<Effects>();
				Storage storage = diningSeat.FindStorage();
				if (storage != null && storage.Has(TableSaltConfig.TAG))
				{
					storage.ConsumeIgnoringDisease(TableSaltConfig.TAG, TableSaltTuning.CONSUMABLE_RATE);
					effectInstance1 = component3.Add("MessTableSalt", true);
				}
				diningSeat.Diner = diner.GetComponent<KPrefabID>();
				messStation.Trigger(1356255274);
				Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(messStation);
				KPrefabID component4 = messStation.GetComponent<KPrefabID>();
				if (effectDurationOverride.HasValue)
				{
					List<EffectInstance> result = (List<EffectInstance>)null;
					roomOfGameObject?.roomType.TriggerRoomEffects(component4, component3, out result);
					if (effectInstance1 != null)
					{
						if (result == null)
							result = new List<EffectInstance>();
						result.Add(effectInstance1);
					}
					if (result != null)
					{
						foreach (EffectInstance effectInstance2 in result)
							effectInstance2.timeRemaining = effectDurationOverride.Value;
					}
				}
				else
					roomOfGameObject?.roomType.TriggerRoomEffects(component4, component3);
				return kanimFile;
			}

			public static void OnExitMessStation(
			  GameObject messStation,
			  GameObject diner,
			  KAnimFile eatAnim)
			{
				diner.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(eatAnim);
				IDiningSeat diningSeat = WaterConsumeChore.ResolveDiningSeat(messStation);
				if (diningSeat == null)
					return;
				diningSeat.Diner = (KPrefabID)null;
			}
		}

		public class States : GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore>
		{
			public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.TargetParameter eater;
			public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.TargetParameter ediblesource;
			public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.TargetParameter ediblechunk;
			public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.TargetParameter messstation;
			public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.FloatParameter requestedfoodunits;
			public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.FloatParameter actualfoodunits;
			public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.TargetParameter locator;
			public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State chooseaction;
			public WaterConsumeChore.States.RehydrateSubState rehydrate;
			public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.FetchSubState fetch;
			public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State choosewheretoeat;
			public WaterConsumeChore.States.EatOnFloorState eatonfloorstate;
			public WaterConsumeChore.States.EatAtMessStationState eatatmessstation;

			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = (StateMachine.BaseState)this.chooseaction;
				this.Target(this.eater);
				this.root.Enter("SetMessStation", (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi => smi.UpdateMessStation())).EventHandler(GameHashes.AssignablesChanged, (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi => smi.UpdateMessStation())).Exit((StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi => smi.ClearMessStation()));
				this.chooseaction.EnterTransition((GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.rehydrate, (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.Transition.ConditionCallback)(smi => this.ediblesource.Get(smi).HasTag(GameTags.Dehydrated))).EnterTransition((GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.fetch, (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.Transition.ConditionCallback)(smi => true));
				this.rehydrate.Enter((StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi =>
				{
					DehydratedFoodPackage component = this.ediblesource.Get(smi).GetComponent<Pickupable>().storage.gameObject.GetComponent<DehydratedFoodPackage>();
					this.rehydrate.foodpackage.Set((KMonoBehaviour)component, smi);
					this.rehydrate.rehydrator.Set(component.Rehydrator != null ? component.Rehydrator.GetComponent<AccessabilityManager>() : (AccessabilityManager)null, smi);
					AccessabilityManager accessabilityManager = this.rehydrate.rehydrator.Get(smi);
					if (accessabilityManager != null)
					{
						GameObject worker = this.eater.Get(smi);
						if (accessabilityManager.CanAccess(worker))
							accessabilityManager.Reserve(this.eater.Get(smi));
						else
							smi.GoTo((StateMachine.BaseState)null);
					}
					else
						smi.GoTo((StateMachine.BaseState)null);
				})).Exit((StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi =>
				{
					AccessabilityManager accessabilityManager = this.rehydrate.rehydrator.Get(smi);
					if (!(accessabilityManager != null))
						return;
					accessabilityManager.Unreserve();
				})).DefaultState((GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.rehydrate.approach);
				this.rehydrate.approach.InitializeStates(this.eater, this.rehydrate.foodpackage, this.rehydrate.work, tactic: NavigationTactics.ReduceTravelDistance).OnTargetLost(this.ediblesource, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)null);
				this.rehydrate.work.ToggleWork("Rehydrate", (Action<WaterConsumeChore.StatesInstance>)(smi => this.eater.Get<WorkerBase>(smi).StartWork((WorkerBase.StartWorkInfo)new DehydratedFoodPackage.RehydrateStartWorkItem(this.rehydrate.foodpackage.Get<DehydratedFoodPackage>(smi), (Action<GameObject>)(result => this.ediblechunk.Set(result, smi, false))))), (Func<WaterConsumeChore.StatesInstance, bool>)(smi =>
				{
					AccessabilityManager accessabilityManager = this.rehydrate.rehydrator.Get(smi);
					return !(accessabilityManager == null) && accessabilityManager.CanAccess(this.eater.Get<WorkerBase>(smi).gameObject);
				}), (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.eatatmessstation, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)null);
				this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, this.choosewheretoeat);
				this.choosewheretoeat.ParamTransition<GameObject>((StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.Parameter<GameObject>)this.messstation, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.eatonfloorstate, (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.Parameter<GameObject>.Callback)((smi, p) => p == null || WaterConsumeChore.IsMessStationNonOperational(p))).GoTo((GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.eatatmessstation);
				this.eatatmessstation.DefaultState((GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.eatatmessstation.moveto).ParamTransition<GameObject>((StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.Parameter<GameObject>)this.messstation, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)null, (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.Parameter<GameObject>.Callback)((smi, p) => p == null || WaterConsumeChore.IsMessStationNonOperational(p)));
				this.eatatmessstation.moveto.InitializeStates(this.eater, this.messstation, this.eatatmessstation.eat);
				this.eatatmessstation.eat.Enter("OnEnterMessStation", (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi => smi.eatAnim = WaterConsumeChore.StatesInstance.OnEnterMessStation(this.messstation.Get(smi), this.eater.Get(smi), this.ediblechunk.Get(smi), false))).Transition((GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.eatonfloorstate, (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.Transition.ConditionCallback)(smi => smi.eatAnim == null)).DoEat(this.ediblechunk, this.actualfoodunits, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)null, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)null).Exit((StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi => WaterConsumeChore.StatesInstance.OnExitMessStation(this.messstation.Get(smi), this.eater.Get(smi), smi.eatAnim)));
				this.eatonfloorstate.DefaultState((GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)this.eatonfloorstate.moveto).Enter("CreateLocator", (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi => smi.CreateLocator())).Exit("DestroyLocator", (StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State.Callback)(smi => smi.DestroyLocator()));
				this.eatonfloorstate.moveto.InitializeStates(this.eater, this.locator, this.eatonfloorstate.eat, this.eatonfloorstate.eat);
				this.eatonfloorstate.eat.ToggleAnims("anim_eat_floor_kanim").DoEat(this.ediblechunk, this.actualfoodunits, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)null, (GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State)null);
			}

			public class EatOnFloorState :
			  GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State
			{
				public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.ApproachSubState<IApproachable> moveto;
				public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State eat;
			}

			public class EatAtMessStationState :
			  GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State
			{
				public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.ApproachSubState<IApproachable> moveto;
				public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State eat;
			}

			public class RehydrateSubState :
			  GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State
			{
				public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.TargetParameter foodpackage;
				public StateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.ObjectParameter<AccessabilityManager> rehydrator;
				public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.ApproachSubState<DehydratedFoodPackage> approach;
				public GameStateMachine<WaterConsumeChore.States, WaterConsumeChore.StatesInstance, WaterConsumeChore, object>.State work;
			}
		}
	}

}
