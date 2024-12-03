using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Content.Scripts.Entities
{

	public class TreeSapProducer : StateMachineComponent<TreeSapProducer.StatesInstance>
	{
		public Storage SapStorage;
		public Growing growth;
		public override void OnSpawn()
		{
			base.OnSpawn();
			growth = this.GetSMI<Growing>();
			smi.StartSM();
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, TreeSapProducer, object>.GameInstance
		{
			public StatesInstance(TreeSapProducer master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, TreeSapProducer>
		{
			public class OffStates : State
			{
				public State noTap;
				public State growing;
			}



			public OffStates off;
			public State producing;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = root;
			}
		}
	}
}
