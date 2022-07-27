using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryopod.Buildings
{
    class CryopodReusable : Workable, ISaveLoadable
	//, IGameObjectEffectDescriptor
	{
		[MyCmpReq]
		public Assignable assignable;
		[MyCmpReq] protected Operational operational;
		[MyCmpReq] private KSelectable selectable;
		[MyCmpReq] private MinionStorage DupeStorage;
		private Chore chore;
		private GameObject FreezeDupe;
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.assignable.OnAssign += new System.Action<IAssignableIdentity>(this.Assign);
		}
		private void Assign(IAssignableIdentity new_assignee)
		{
			this.CancelActivateChore();
			if (new_assignee == null)
				return;
			this.FreezeChore();
		}

		public Chore FreezeChore(object param = null)
		{
			if (this.chore != null)
				return chore;
			this.chore = (Chore)new WorkChore<CryopodFreezeWorkable>(Db.Get().ChoreTypes.Migrate, (IStateMachineTarget)this, on_complete: ((System.Action<Chore>)(o => this.CompleteActivateChore())), priority_class: PriorityScreen.PriorityClass.high);
			return chore;
		}

		public void CancelActivateChore(object param = null)
		{
			if (this.chore == null)
				return;
			this.chore.Cancel("User cancelled");
			this.chore = (Chore)null;
		}
		private void CompleteActivateChore()
		{
			this.FreezeDupe = this.chore.driver.gameObject;
			this.smi.GoTo((StateMachine.BaseState)this.smi.sm.HoldingDuplicant);
			this.chore = (Chore)null;
			this.DupeStorage.SerializeMinion(FreezeDupe);
			Game.Instance.userMenu.Refresh(this.gameObject);
		}

		#region Spawn&Cleanup
		protected override void OnSpawn()
		{
			base.OnSpawn();
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		#endregion

		
		#region StateMachine

		public class CryopodReusableSM: StateMachineComponent<CryopodReusableSM, CryopodReusableSM.StatesInstance, CryopodReusable> { 
		public class CryopodReusableSM : GameStateMachine<States, StatesInstance, CryopodReusable, object>.GameInstance
		{
			public StatesInstance(CryopodReusable master) : base(master)
			{

			}
		}

		public class States : GameStateMachine<States, StatesInstance, CryopodReusable>
		{

			public State Idle;
			public State HoldingDuplicant;
			public State AwaitingDuplicant;

			public override void InitializeStates(out BaseState defaultState)
			{

				defaultState = Idle;
				Idle
					.QueueAnim("open")
					.ToggleChore((Func<CryopodReusable.StatesInstance, Chore>)(smi => smi.master.FreezeChore()), this.AwaitingDuplicant);


				AwaitingDuplicant
					.QueueAnim("open")
					.EventTransition(GameHashes.AssigneeChanged, Idle);

				HoldingDuplicant
					.PlayAnim("Closing")
					.QueueAnim("holdingDupe");
				
			}
		}
		}
		#endregion
	}
}

