using BionicBoostersPlus.Content.ModDb;
using UnityEngine;

namespace BionicBoostersPlus.Content.Scripts
{
	/// <summary>
	/// SMI that sits on the booster itself and stores the dreamdata progress
	/// mirrored from unused explorer booster
	/// </summary>
	internal class BionicUpgrade_DreamerBooster : GameStateMachine<BionicUpgrade_DreamerBooster, BionicUpgrade_DreamerBooster.Instance, IStateMachineTarget, BionicUpgrade_DreamerBooster.Def>
	{
		private FloatParameter Progress;
		public State not_ready;
		public State ready;

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			this.serializable = StateMachine.SerializeType.ParamsOnly;
			default_state = not_ready;
			this.not_ready
				.ParamTransition(Progress, this.ready, IsGTEOne)
				.ToggleStatusItem(BB_StatusItems.DreamBoosterJournalStorage);
			this.ready
				.ParamTransition(Progress, this.not_ready, IsLTOne)
				.ToggleStatusItem(BB_StatusItems.DreamBoosterJournalStorageFull);
		}

		public class Def : BaseDef
		{
		}

		public new class Instance(IStateMachineTarget master, Def def) : GameInstance(master, def)
		{
			private BionicUpgrade_DreamerBoosterMonitor.Instance monitor;

			public bool IsBeingMonitored => this.monitor != null;

			public bool IsReady => this.Progress >= 1;

			public float Progress => this.sm.Progress.Get(this);

			public void SetMonitor(BionicUpgrade_DreamerBoosterMonitor.Instance monitor)
			{
				this.monitor = monitor;
			}

			public void AddData(float dataProgressDelta)
			{
				this.SetDataProgress(Mathf.Clamp(this.Progress + dataProgressDelta, 0.0f, 1f));
			}

			public void SetDataProgress(float dataProgress)
			{
				dataProgress = Mathf.Clamp(dataProgress, 0.0f, 1f);
				this.sm.Progress.Set(dataProgress, this);
			}
		}
	}
}
