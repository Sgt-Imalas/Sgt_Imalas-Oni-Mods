using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class AIO_DecompressionValve : StateMachineComponent<AIO_DecompressionValve.StatesInstance>
	{
		[MyCmpReq] Storage storage;
		[MyCmpReq] Operational operational;
		public override void OnSpawn()
		{
			smi.StartSM();
		}
		float lastTimeEmpty = 0f;

		public bool ShouldTurnOff()
		{
			if (storage.MassStored() > 0f)
			{
				lastTimeEmpty = Time.unscaledDeltaTime;
			}
			
			if (Time.unscaledDeltaTime - lastTimeEmpty > 1.5f)
			{
				return true;
			}
			return false;
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, AIO_DecompressionValve, object>.GameInstance
		{
			public StatesInstance(AIO_DecompressionValve master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, AIO_DecompressionValve>
		{
			public State TransportingItems;
			public State TransportingItems_pst;
			public State Idle;


			public override void InitializeStates(out BaseState default_state)
			{
				default_state = Idle;
				Idle.EventTransition(GameHashes.OnStorageChange, TransportingItems, (smi) => smi.master.storage.MassStored() > 0)
					.PlayAnim("off", KAnim.PlayMode.Once);
				TransportingItems
					.PlayAnim("on_flow", KAnim.PlayMode.Loop)
					.Enter(smi => smi.master.operational.SetActive(true))
					.Exit(smi => smi.master.operational.SetActive(false))
					.UpdateTransition(TransportingItems_pst, (smi, dt) => smi.master.ShouldTurnOff(), UpdateRate.SIM_1000ms)
					;
				TransportingItems_pst
					.PlayAnim("off_flow", KAnim.PlayMode.Once)
					.OnAnimQueueComplete(Idle);
			}
		}
	}
}
