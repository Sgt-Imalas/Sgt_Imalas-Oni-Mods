using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatelites.Behaviours
{
    class SatelliteCarrierModule : StateMachineComponent<SatelliteCarrierModule.SMInstance>, ISaveLoadable
    {
        [MyCmpReq] private KSelectable selectable;
        [MyCmpGet] private ComplexFabricator fabricator;

		protected override void OnSpawn()
		{
			base.OnSpawn();
			this.smi.StartSM();
		}


		#region StateMachine
		public class SMInstance : GameStateMachine<States, SMInstance, SatelliteCarrierModule, object>.GameInstance
		{
			public SMInstance(SatelliteCarrierModule master) : base(master)
			{
			}

		}

		public class States : GameStateMachine<States, SMInstance, SatelliteCarrierModule>
		{
			public State InitState;
			public State InSpace;
			public State Landed;
			public State HoldingSatelliteLanded;
			public State NoSatelliteLoadedGround;
			public State HoldingSatelliteInSpace;
			public State NoSatelliteLoadedInSpace;
			public override void InitializeStates(out BaseState defaultState)
			{

				defaultState = InitState;

				NotRadiating
					.QueueAnim("on")
					.Update((smi, dt) => smi.master.AmIInSpace())
					.EventTransition(GameHashes.OperationalChanged, Retracting, smi => !smi.IsOperational)
					.ParamTransition(this.IsInTrueSpace, Radiating, IsTrue);


				Radiating
					.Update("Radiating", (smi, dt) =>
					{
						smi.master.RadiateIntoSpace();
						smi.master.AmIInSpace();

					}, UpdateRate.SIM_200ms)
					.QueueAnim("on_rad", true)
					.Exit(smi => smi.master.UpdateRadiation(false))
					.EventTransition(GameHashes.OperationalChanged, Retracting, smi => !smi.IsOperational)
					.ParamTransition(this.IsInTrueSpace, NotRadiating, IsFalse);

				Retracting
					.PlayAnim("on_pst")
					.OnAnimQueueComplete(Protecting);

				Protecting
					.Enter(smi => smi.master.SetBunkerState(true))
					.Exit(smi => smi.master.SetBunkerState(false))
					.QueueAnim("off", true)
					.EventTransition(GameHashes.OperationalChanged, Extending, smi => smi.IsOperational);

				Extending
					.PlayAnim("on_pre")
					.OnAnimQueueComplete(NotRadiating);
			}
		}
		#endregion
	}
}
