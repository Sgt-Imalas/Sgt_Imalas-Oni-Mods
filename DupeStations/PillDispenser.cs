using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DupeStations
{

    public class PillDispenser : StateMachineComponent<PillDispenser.StatesInstance>
    {
        public override void OnSpawn()
        {
            smi.StartSM();
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, PillDispenser, object>.GameInstance
        {
            public StatesInstance(PillDispenser master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, PillDispenser>
        {
            public class EnabledStates : State
            {
                public State notReady, ready;
            }

            public State disabled;
            public EnabledStates enabled;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = root;


            }
        }
    }
}
