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

				
			}
		}
		#endregion
	}
}
