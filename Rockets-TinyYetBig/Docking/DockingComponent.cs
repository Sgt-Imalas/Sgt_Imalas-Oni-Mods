//using KSerialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Rockets_TinyYetBig.Behaviours
//{
//    class DockingComponent : GameStateMachine<DockingComponent, DockingComponent.Instance, IStateMachineTarget, DockingComponent.Def>
//    {
//        public State docked;
//        public State docking;
//        public State undocking;
//        public State notDocked;

//        [Serialize]
//        Dictionary<DockingDoor, DockingComponent> DockingConnections = new Dictionary<DockingDoor, DockingComponent>();
//        [Serialize]
//        List<DockingDoor> DoorsReady = new List<DockingDoor>();

//        public class Def : BaseDef
//        {
//            public int ParallelDockingProcesses = 0;
//        }

//        public override void InitializeStates(out BaseState default_state)
//        {
//            default_state = (StateMachine.BaseState)this.notDocked;
//        }
//        public new class Instance : GameInstance
//        {
//            public override void StartSM()
//            {
//                base.StartSM();

//            }
//            protected override void OnCleanUp()
//            {
//                base.OnCleanUp();
//            }

//            public Instance(IStateMachineTarget master, Def def) : base(master, def)
//            {
//            }
//        }
//    }
//}
