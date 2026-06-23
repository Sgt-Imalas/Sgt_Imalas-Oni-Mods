using AquaticMinnowMinion.Content.ModDb;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace AquaticMinnowMinion.Content.Scripts
{
	internal class GillIrritationMonitor : GameStateMachine<GillIrritationMonitor, GillIrritationMonitor.Instance, IStateMachineTarget, GillIrritationMonitor.Def>
	{
		private const float amountToCough = 0.600f; //600g
		private const float decayRate = 0.05f;
		private const float coughInterval = 0.1f;
		public State idle;
		public State coughing;
		public BoolParameter shouldCough = new BoolParameter(false);

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			this.serializable = StateMachine.SerializeType.ParamsOnly;
			default_state = (StateMachine.BaseState)this.idle;
			this.idle
				.EventHandler(ModAssets.AqHashes.PoorBreathableLiquidQuality, this.OnBreatheDirtyWater);
			//	.ParamTransition(this.shouldCough, this.coughing, (smi, bShouldCough) => bShouldCough);
			//this.coughing
			//	.ToggleStatusItem(Db.Get().DuplicantStatusItems.Coughing)
			//	.ToggleReactable((smi => smi.GetReactable()))
			//	.ParamTransition(this.shouldCough, this.idle, (smi, bShouldCough) => !bShouldCough);
		}

		private void OnBreatheDirtyWater(GillIrritationMonitor.Instance smi, object data)
		{
			float timeInCycles = GameClock.Instance.GetTimeInCycles();
			if (timeInCycles > coughInterval && (double)timeInCycles -smi.lastIrritationTriggeredTime <= coughInterval)
				return;
			float consumedMass = ((Boxed<float>)data).value;
			float lastConsumeTimeDelta = smi.lastConsumeTime <= 0 ? 0 : timeInCycles - smi.lastConsumeTime;
			smi.lastConsumeTime = timeInCycles;
			smi.amountConsumed -= decayRate * lastConsumeTimeDelta;
			smi.amountConsumed = Mathf.Max(smi.amountConsumed, 0);
			smi.amountConsumed += consumedMass;
			if (smi.amountConsumed < 1f)
				return;
			//this.shouldCough.Set(true, smi);
			smi.lastConsumeTime = 0.0f;
			smi.amountConsumed = 0.0f;

			smi.AddGillIrritation(smi.master.gameObject);
		}
		public class Def : StateMachine.BaseDef
		{
		}
		public new class Instance : GameInstance
		{
			[Serialize]
			public float lastIrritationTriggeredTime;
			[Serialize]
			public float lastConsumeTime;
			[Serialize]
			public float amountConsumed;

			public Instance(IStateMachineTarget master, GillIrritationMonitor.Def def) : base(master, def)
			{
			}

			//public Reactable GetReactable()
			//{
			//	Emote coughSmall = Db.Get().Emotes.Minion.Cough_Small;
			//	SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(this.master.gameObject, (HashedString)"BadAirCough", Db.Get().ChoreTypes.Cough, localCooldown: 0.0f);
			//	selfEmoteReactable.SetEmote(coughSmall);
			//	selfEmoteReactable.preventChoreInterruption = true;
			//	return (Reactable)selfEmoteReactable.RegisterEmoteStepCallbacks((HashedString)"react_small", (System.Action<GameObject>)null, new System.Action<GameObject>(this.FinishedCoughing));
			//}

			public void AddGillIrritation(GameObject cougher)
			{
				cougher.GetComponent<Effects>().Add(Aq_Effects.ItchyGills, true);
				this.sm.shouldCough.Set(false, this.smi);
				this.smi.lastIrritationTriggeredTime = GameClock.Instance.GetTimeInCycles();
			}
		}
	}
}
