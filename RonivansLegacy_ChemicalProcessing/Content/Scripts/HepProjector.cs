using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HepProjector : StateMachineComponent<HepProjector.StatesInstance>
	{
		[MyCmpGet]
		private RadiationEmitter radEmitter;
		[MyCmpReq]
		private Operational operational;
		[MyCmpReq]
		private ElementConverter elementConverter;

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.smi.StartSM();
		}

		private void SetEmitRads(float rads)
		{
			smi.master.radEmitter.emitRads = rads;
			smi.master.radEmitter.Refresh();
			smi.master.radEmitter.SetEmitting(rads > 0);
		}

		public class States :
		  GameStateMachine<States, StatesInstance, HepProjector>
		{
			public State disabled;
			public State idle;
			public State active;

			private string AwaitingFuelResolveString(string str, object obj)
			{
				ElementConverter elementConverter = ((StatesInstance)obj).master.elementConverter;
				string str1 = elementConverter.consumedElements[0].Tag.ProperName();
				string formattedMass = GameUtil.GetFormattedMass(elementConverter.consumedElements[0].MassConsumptionRate, GameUtil.TimeSlice.PerSecond);
				str = string.Format(str, str1, formattedMass);
				return str;
			}

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = disabled;
				this.disabled
					.EventTransition(GameHashes.OperationalChanged, this.idle, smi => smi.master.operational.IsOperational);
				this.idle
					.EventTransition(GameHashes.OnStorageChange, this.active, smi => smi.master.elementConverter.HasEnoughMassToStartConverting())
					.EventTransition(GameHashes.OperationalChanged, this.disabled, smi => !smi.master.operational.IsOperational)
					.ToggleStatusItem(global::STRINGS.BUILDING.STATUSITEMS.AWAITINGFUEL.NAME, global::STRINGS.BUILDING.STATUSITEMS.AWAITINGFUEL.TOOLTIP, icon_type: StatusItem.IconType.Exclamation, notification_type: NotificationType.BadMinor, resolve_string_callback: AwaitingFuelResolveString);
				this.active
					.EventTransition(GameHashes.OperationalChanged, this.disabled, smi => !smi.master.operational.IsOperational)
					.EventTransition(GameHashes.OnStorageChange, this.idle, smi => !smi.master.elementConverter.HasEnoughMassToStartConverting())
					.Enter(smi =>
				{
					smi.master.operational.SetActive(true);
					smi.master.SetEmitRads(900f);
				}).Exit(smi =>
				{
					smi.master.operational.SetActive(false);
					smi.master.SetEmitRads(0);
				});
			}
		}

		public class StatesInstance :
		  GameStateMachine<States, StatesInstance, HepProjector, object>.GameInstance
		{
			public StatesInstance(HepProjector master)
			  : base(master)
			{
			}
		}
	}
}
