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
	public class ElementConversionBuilding : StateMachineComponent<ElementConversionBuilding.StatesInstance>
	{
		public class StatesInstance : GameStateMachine<States, StatesInstance, ElementConversionBuilding, object>.GameInstance
		{
			public StatesInstance(ElementConversionBuilding smi)
				: base(smi)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, ElementConversionBuilding>
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
					.DefaultState(on.waiting);
				on.waiting
					.EventTransition(GameHashes.OnStorageChange, on.working_pre, (StatesInstance smi) => smi.master.HasEnoughMassToStartConverting());
				on.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(on.working);
				on.working.Enter(delegate (StatesInstance smi)
				{
					smi.master.operational.SetActive(value: true);
				}).QueueAnim("working_loop", loop: true)
				.EventTransition(GameHashes.OnStorageChange, on.working_pst, (StatesInstance smi) => !smi.master.CanConvertAtAll())
				.Exit(delegate (StatesInstance smi)
				{
					smi.master.operational.SetActive(value: false);
				});
				on.working_pst.PlayAnim("working_pst")
					.OnAnimQueueComplete(on.waiting);
			}
		}

		[MyCmpGet]
		public Operational operational;
		[MyCmpGet]
		public ConduitConsumer consumer;

		public ElementConverter[] converters;

		public ManualDeliveryKG[] deliveryComponents;
		[SerializeField]
		/// when a building has secondary converters that should not run without the primary one, only check for the primary converter state to determine operational
		public bool UsePrimaryConverterOnly = false;


		public static readonly EventSystem.IntraObjectHandler<ElementConversionBuilding> OnConduitConnectionChangedDelegate = new EventSystem.IntraObjectHandler<ElementConversionBuilding>(delegate (ElementConversionBuilding component, object data)
		{
			component.OnConduitConnectionChanged(data);
		});

		public override void OnSpawn()
		{
			base.OnSpawn();
			converters = GetComponents<ElementConverter>();	

			Debug.Assert(converters.Length > 0, "ElementConversionBuilding must have at least one ElementConverter component.");


			if (UsePrimaryConverterOnly)
				converters = [converters.First()];

			deliveryComponents = GetComponents<ManualDeliveryKG>();
			if(consumer != null)
				OnConduitConnectionChanged(consumer.IsConnected);
			Subscribe(-2094018600, OnConduitConnectionChangedDelegate);
			base.smi.StartSM();
		}
		public bool HasEnoughMassToStartConverting()
		{
			foreach(var conv in converters)
			{
				if (conv.HasEnoughMassToStartConverting())
					return true;
			}
			return false;
		}
		public bool CanConvertAtAll()
		{
			foreach (var conv in converters)
			{
				if (conv.CanConvertAtAll())
					return true;
			}
			return false;
		}


		public void OnConduitConnectionChanged(object data)
		{
			bool pause = (bool)data;
			ManualDeliveryKG[] array = deliveryComponents;
			foreach (ManualDeliveryKG manualDeliveryKG in array)
			{
				Element element = ElementLoader.GetElement(manualDeliveryKG.RequestedItemTag);
				if (element != null && element.IsLiquid)
				{
					manualDeliveryKG.Pause(pause, "pipe connected");
				}
			}
		}
	}
}
