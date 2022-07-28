using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryopod.Buildings
{
    class CryopodReusable : StateMachineComponent<CryopodReusable.StatesInstance>, ISaveLoadable, ISidescreenButtonControl
	//, IGameObjectEffectDescriptor
	{
		[MyCmpReq]
		public Assignable assignable;
		[MyCmpReq] protected Operational operational;
		[MyCmpReq] private KSelectable selectable;
		[MyCmpReq] private MinionStorage DupeStorage;
		private Chore chore;
		private Chore activationChore;
		[Serialize]
		public float InternalTemperatureKelvin;
		public float InternalTemperatureKelvinUpperLimit = 293.15f;
		public float InternalTemperatureKelvinLowerLimit = 77.15f;
		public float TimeForProcess = 60f;

      
        protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.assignable.OnAssign += new System.Action<IAssignableIdentity>(this.Assign);
		}
		private void Assign(IAssignableIdentity new_assignee)
		{
			this.CancelFreezeChore();
			if (new_assignee == null)
				return;
			this.FreezeChore();
		}

		public Chore FreezeChore(object param = null)
		{
			if (this.chore != null)
				return chore;
			this.chore = (Chore)new WorkChore<CryopodFreezeWorkable>(Db.Get().ChoreTypes.Migrate, (IStateMachineTarget)this, on_complete: ((System.Action<Chore>)(o => this.CompleteFreezeChore())), priority_class: PriorityScreen.PriorityClass.high);
			return chore;
		}

        internal string GetDupeName()
        {
            if (HoldingDupe())
            {
				return DupeStorage.GetStoredMinionInfo().First().name;
			}
			return "No duplicant stored.";
        }

        public void CancelFreezeChore(object param = null)
		{
			if (this.chore == null)
				return;
			this.chore.Cancel("User cancelled");
			this.chore = (Chore)null;
		}
		private void CompleteFreezeChore()
		{
			this.smi.GoTo((StateMachine.BaseState)this.smi.sm.HoldingDuplicant);
			this.chore = (Chore)null;
			Game.Instance.userMenu.Refresh(this.gameObject);
		}
		public void RefreshSideScreen()
		{
			if (!this.GetComponent<KSelectable>().IsSelected)
				return;
			DetailsScreen.Instance.Refresh(this.gameObject);
		}
		public void SetAssignable(bool set_it)
		{
			this.assignable.SetCanBeAssigned(set_it);
			this.RefreshSideScreen();
		}
		public bool HoldingDupe()
        {
			return DupeStorage.GetStoredMinionInfo().Count>0;
		}

		#region Spawn&Cleanup
		protected override void OnSpawn()
		{
			base.OnSpawn();
			this.smi.StartSM();
			ModAssets.CryoPods.Add(this);
		}

		protected override void OnCleanUp()
		{
			ModAssets.CryoPods.Remove(this); 
			if (this.HoldingDupe())
			{
				ThrowOutDupe(true);
			}
			base.OnCleanUp();
		}
        #endregion

        #region SideScreen
        public string SidescreenButtonText => global::STRINGS.BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTON;

		public string SidescreenButtonTooltip => global::STRINGS.BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTONTOOLTIP;

		public bool SidescreenEnabled() => true;

		public bool SidescreenButtonInteractable() => this.HoldingDupe();

        public void OnSidescreenButtonPressed()
        {
			StartManualThawing();
		}
		public int ButtonSideScreenSortOrder() => 20;

		#endregion


		public void StartManualThawing()
        {
			//this.GetComponent<Operational>().enabled = !this.GetComponent<Operational>().enabled;
			this.smi.GoTo(smi.sm.HoldingDuplicant.Thawing);
		}

        #region Freezing&Thawing
		
		
		public void ThrowOutDupe(bool applyCryoSickness = true, Vector3 spawn_pos = new Vector3())
		{
			assignable.Unassign();
			var newDupe = DupeStorage.GetStoredMinionInfo().First();
			var spawn_position = spawn_pos == new Vector3() ? this.transform.position : (Vector3)spawn_pos;

			var NewDupeDeserialized = DupeStorage.DeserializeMinion(newDupe.id, spawn_position);
			Debug.Log(spawn_position + " spawned here");
			if (applyCryoSickness)
            {
				Debug.Log(NewDupeDeserialized + " should have CryoSickness");
            }
			SetAssignable(true);
			DupeStorage.GetStoredMinionInfo().Clear();
		}

        private void StartCoolingProcess()
		{
			SetAssignable(false);
			this.InternalTemperatureKelvin = InternalTemperatureKelvinUpperLimit; 
		}
		private void ChangeInternalTemperature(float dt, bool cooling)
        {
			float temperatureStep = ((InternalTemperatureKelvinUpperLimit - InternalTemperatureKelvinLowerLimit) / TimeForProcess) * dt;
			
			InternalTemperatureKelvin = cooling? InternalTemperatureKelvin - temperatureStep: InternalTemperatureKelvin + temperatureStep;
			if(cooling && InternalTemperatureKelvin <= InternalTemperatureKelvinLowerLimit || !cooling && InternalTemperatureKelvin >= InternalTemperatureKelvinUpperLimit)
            {
				smi.sm.HasReachedTargetTemp.Set(true, smi);
				InternalTemperatureKelvin = cooling ? InternalTemperatureKelvinLowerLimit : InternalTemperatureKelvinUpperLimit;

			}
		}

        #endregion
        #region StateMachine

        public class StatesInstance : GameStateMachine<States, StatesInstance, CryopodReusable, object>.GameInstance
		{
			public StatesInstance(CryopodReusable master) : base(master)
			{

			}
		}

		public class States : GameStateMachine<States, StatesInstance, CryopodReusable>
		{
			[Serialize] public BoolParameter HasReachedTargetTemp;
			[Serialize] public BoolParameter HoldsDuplicant;
			public class HoldingDuplicantStates : State
			{
				public State Cooling;
				public State OnTemperature;
				public State Thawing;
				public State ThrowDupeOut;
			}

			public State Idle;
			public HoldingDuplicantStates HoldingDuplicant;
			public State AwaitingDuplicant;

			public override void InitializeStates(out BaseState defaultState)
			{

				defaultState = Idle;

				Idle
					.QueueAnim("off")
					.ToggleChore((smi => smi.master.FreezeChore()), this.AwaitingDuplicant)
					.Update((smi, dt) => smi.master.CheckForDupe())
					.ParamTransition<bool>(this.HoldsDuplicant, this.HoldingDuplicant, IsTrue);


				AwaitingDuplicant
					.QueueAnim("off")
					.EventTransition(GameHashes.AssigneeChanged, Idle)
					.Exit(smi => smi.master.StartCoolingProcess());

				HoldingDuplicant.defaultState = HoldingDuplicant.Cooling;
				HoldingDuplicant
					.ToggleStatusItem(ModAssets.StatusItems.DupeName, smi => smi.master)
					.ToggleStatusItem(ModAssets.StatusItems.CurrentDupeTemperature, smi => smi.master);

				HoldingDuplicant.Cooling
					.PlayAnim("working_pre")
					.QueueAnim("working_loop")
					.Update("Freezing", (smi, dt) =>
					{
						smi.master.ChangeInternalTemperature(dt,true);

					}, UpdateRate.SIM_200ms)
					.ParamTransition<bool>(this.HasReachedTargetTemp, this.HoldingDuplicant.OnTemperature, IsTrue)
					.EventTransition(GameHashes.OperationalChanged, HoldingDuplicant.Thawing, smi => !smi.GetComponent<Operational>().IsOperational)
					;

				HoldingDuplicant.OnTemperature
					.Enter((smi) => HasReachedTargetTemp.Set(false, smi))
					.EventTransition(GameHashes.OperationalChanged, HoldingDuplicant.Thawing, smi => !smi.GetComponent<Operational>().IsOperational)
					;

				HoldingDuplicant.Thawing
					.Update("Thawing", (smi, dt) =>
					{
						smi.master.ChangeInternalTemperature(dt, false);

					}, UpdateRate.SIM_200ms)
					.ParamTransition<bool>(this.HasReachedTargetTemp, this.HoldingDuplicant.ThrowDupeOut, IsTrue)
					//.EventTransition(GameHashes.OperationalChanged, HoldingDuplicant.Cooling, smi => smi.GetComponent<Operational>().IsOperational)
					;

				HoldingDuplicant.ThrowDupeOut
					.PlayAnim("defrost")
					.QueueAnim("defrost_exit")
					.Enter(smi => 
					{
						HasReachedTargetTemp.Set(false, smi);
						smi.master.ThrowOutDupe(false); 
						HoldsDuplicant.Set(false, smi); 
					})
					.GoTo(Idle);
			}
		}

        private void CheckForDupe()
        {
            if (HoldingDupe())
			{
				this.smi.GoTo(smi.sm.HoldingDuplicant);
			}
        }

        #endregion
    }
}

