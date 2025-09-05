using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	/// <summary>
	/// cloned and tweaked WaterPurifier
	/// </summary>
	[SerializationConfig(MemberSerialization.OptIn)]
	public class ElementCompressorBuilding : StateMachineComponent<ElementCompressorBuilding.StatesInstance>
	{
		public class StatesInstance : GameStateMachine<States, StatesInstance, ElementCompressorBuilding, object>.GameInstance
		{
			public StatesInstance(ElementCompressorBuilding smi)
				: base(smi)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, ElementCompressorBuilding>
		{
			public class OnStates : State
			{
				public State waiting;

				public State working_pre;

				public State working;

				public State working_pst;
			}

			public State off;

			public OnStates on;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = off;
				off.PlayAnim("off")
					.EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.master.operational.IsOperational);
				on.PlayAnim("on")
					.EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.master.operational.IsOperational)
					.Enter(smi => smi.master.operational.SetActive(true))
					.Exit(smi => smi.master.operational.SetActive(false))
					.DefaultState(on.waiting);
				on.waiting
					.PlayAnim("on")
					.EventTransition(GameHashes.OnStorageChange, on.working_pre, (StatesInstance smi) => smi.master.ShouldPlayWorkingAnim());
				on.working_pre
					.PlayAnim("working_pre")
					.OnAnimQueueComplete(on.working);
				on.working
					.QueueAnim("working_loop", loop: true)
					.Transition(on.working_pst, (StatesInstance smi) => smi.master.ShouldStopPlayWorkingAnim());
				on.working_pst
					.PlayAnim("working_pst")
					.OnAnimQueueComplete(on.waiting);
			}
		}

		[MyCmpGet]
		public KSelectable selectable;
		[MyCmpGet]
		public Operational operational;
		[MyCmpGet]
		public ConduitConsumer consumer;
		[MyCmpGet]
		public ElementConverter converter;

		RefrigeratorController.StatesInstance FridgeController;

		public override void OnSpawn()
		{
			base.OnSpawn();
			FridgeController = gameObject.GetSMI<RefrigeratorController.StatesInstance>();
			converter = GetComponent<ElementConverter>();
			base.smi.StartSM();
		}
		public bool ShouldPlayWorkingAnim()
		{
			bool elementConverterHasMass = converter.HasEnoughMassToStartConverting();
			bool fridgeCooling = FridgeController.IsInsideState(FridgeController.sm.operational.cooling);

			return fridgeCooling || elementConverterHasMass;
		}
		public bool ShouldStopPlayWorkingAnim()
		{
			bool elementConverterCanConvert = converter.CanConvertAtAll();
			bool fridgeCooling = FridgeController.IsInsideState(FridgeController.sm.operational.cooling);

			return !fridgeCooling && !elementConverterCanConvert;
		}
	}
}
