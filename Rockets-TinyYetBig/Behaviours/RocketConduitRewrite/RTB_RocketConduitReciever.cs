using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Behaviours.RocketConduitRewrite
{
    public class RTB_RocketConduitReciever : StateMachineComponent<RTB_RocketConduitReciever.StatesInstance>,  ISecondaryOutput
    {
        [Serialize]
        public ConduitPortInfo conduitPortInfo;
        bool ISecondaryOutput.HasSecondaryConduitType(ConduitType type) => type == this.conduitPortInfo.conduitType;
        CellOffset ISecondaryOutput.GetSecondaryConduitOffset(ConduitType type) => type == this.conduitPortInfo.conduitType ? this.conduitPortInfo.offset : CellOffset.none;

        public override void OnSpawn()
        {
            smi.StartSM();
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, RTB_RocketConduitReciever, object>.GameInstance
        {
            public StatesInstance(RTB_RocketConduitReciever master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, RTB_RocketConduitReciever>
        {
            public override void InitializeStates(out BaseState default_state)
            {
                default_state = root;
            }
        }
    }
}
