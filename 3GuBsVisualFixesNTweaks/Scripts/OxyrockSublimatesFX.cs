using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
    class OxyrockSublimatesFX : GameStateMachine<OxyrockSublimatesFX, OxyrockSublimatesFX.Instance>
	{
		public StateMachine<OxyrockSublimatesFX, OxyrockSublimatesFX.Instance, IStateMachineTarget, object>.TargetParameter fx;

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = (StateMachine.BaseState)this.root;
			this.Target(this.fx);
			this.root
				.Enter(smi=>smi.sm.fx.Get(smi).GetComponent<KBatchedAnimController>()?.SetSymbolVisiblity("bubble", false))
				.PlayAnim("bubble",KAnim.PlayMode.Loop)
				.Exit("DestroyFX", (smi => smi.DestroyFX()));
		}

		public new class Instance : GameInstance
		{
			public Instance(IStateMachineTarget master, Vector3 offset)
			  : base(master)
			{
				var kbac = FXHelpers.CreateEffect("oxyrockfx_kanim", (smi.master.transform.GetPosition() + offset));
				this.sm.fx.Set(kbac.gameObject, this.smi, false);
				kbac.SetSymbolVisiblity("bubble", false); //only used for sound
			}

			public void DestroyFX() => Util.KDestroyGameObject(this.sm.fx.Get(this.smi));
		}
	}
}
