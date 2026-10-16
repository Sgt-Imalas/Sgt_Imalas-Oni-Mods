using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace OniRetroEdition.Behaviors
{
	public class RetroOni_LightController : GameStateMachine<RetroOni_LightController, RetroOni_LightController.Instance>
	{
		public class Def : BaseDef
		{
		}

		public new class Instance : GameInstance
		{
			public Instance(IStateMachineTarget master, Def def)
				: base(master, (object)def)
			{
			}
		}

		public static bool HasAnim(Instance smi, string anim)
		{
			return smi.GetComponent<KBatchedAnimController>().HasAnimation(anim);
		}
		public static bool IsOperational(Instance smi)
		{
			return smi.GetComponent<Operational>().IsOperational;
		}

		public State off;

		public State on_pre;
		public State on;
		public State on_pst;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			off
				.PlayAnim("off")
				.EventTransition(GameHashes.OperationalChanged, on_pre, (Instance smi) => IsOperational(smi) && HasAnim(smi,"on_pre"))
				.EventTransition(GameHashes.OperationalChanged, on, (Instance smi) => IsOperational(smi) && !HasAnim(smi, "on_pre"));
			on_pre
				.PlayAnim("on_pre")
				.OnAnimQueueComplete(on);
			on.PlayAnim("on")
				.Enter("SetActive", delegate (Instance smi)
				{
					smi.GetComponent<Operational>().SetActive(value: true);
				})
				.EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !IsOperational(smi) & !HasAnim(smi, "on_pst"))
				.EventTransition(GameHashes.OperationalChanged, on_pst, (Instance smi) => !IsOperational(smi) & HasAnim(smi, "on_pst"))
				.ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight)
				;
			on_pst
				.PlayAnim("on_pst")
				.OnAnimQueueComplete(off);
		}
	}
}

