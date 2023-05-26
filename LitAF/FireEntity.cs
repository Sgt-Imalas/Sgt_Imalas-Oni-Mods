using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitAF
{
    public class FireEntity : StateMachineComponent<FireEntity.StatesInstance>, ISlicedSim1000ms
    {
        private int cell;
        public override void OnSpawn()
        {
            smi.StartSM();
            SlicedUpdaterSim1000ms<FireEntity>.instance.RegisterUpdate1000ms(this);
            this.cell = Grid.PosToCell(this);
        }

        public void SlicedSim1000ms(float dt)
        {
            this.Burn(dt);
        }

        void Burn(float dt)
        {

        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, FireEntity, object>.GameInstance
        {
            public StatesInstance(FireEntity master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, FireEntity>
        {
            public override void InitializeStates(out BaseState default_state)
            {
                default_state = root;
            }
        }
    }
}
