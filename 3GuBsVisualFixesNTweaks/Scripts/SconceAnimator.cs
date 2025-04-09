using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	public class SconceAnimator : StateMachineComponent<SconceAnimator.StatesInstance>
	{
		[MyCmpReq]
		Storage SconeStorage;
		[Serialize] float lastMassStored = -1;
		int cell;
	
		public override void OnSpawn()
		{
			cell = Grid.PosToCell(this);
			smi.StartSM();
		}
		public bool HadStorageChange()
		{
			float currentMassStored = SconeStorage.MassStored();
			bool noEmissionLastChange = Mathf.Approximately(currentMassStored,lastMassStored);
			lastMassStored = currentMassStored;
			return !noEmissionLastChange;				
		}
		

		public class StatesInstance : GameStateMachine<States, StatesInstance, SconceAnimator, object>.GameInstance
		{
			public StatesInstance(SconceAnimator master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, SconceAnimator>
		{
			public State off;
			public State on;
			public State on_pst_fin_anim;
			public State on_pst;

			private OxyrockSublimatesFX.Instance CreateFX(StatesInstance smi) => !smi.isMasterNull ? new OxyrockSublimatesFX.Instance(smi.master, new Vector3(0.0f, 0.5f, -0.1f)) : (OxyrockSublimatesFX.Instance)null;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = off;
				off
					.PlayAnim("off")
					.EventTransition(GameHashes.OnStorageChange, on);

				on
					.PlayAnim("on", KAnim.PlayMode.Loop)
					.ToggleFX(smi=> CreateFX(smi))
					.Update((smi, dt) =>
					{

						if (!smi.master.HadStorageChange())
						{
							smi.GoTo(on_pst);
						}
					}, UpdateRate.SIM_1000ms)				
					;
				on_pst
					.PlayAnim("on_pst")
					.OnAnimQueueComplete(off);
			}
		}
	}
}
