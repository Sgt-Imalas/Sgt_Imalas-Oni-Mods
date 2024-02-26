using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace OniRetroEdition
{

    public class NoiseRecieverSMI : GameStateMachine<NoiseRecieverSMI, NoiseRecieverSMI.Instance, IStateMachineTarget, NoiseRecieverSMI.Def>
    {
        private State AllQuiet;
        private State NormalNoise;
        private State LoudNoise;
        private State VeryLoudNoise;

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = NormalNoise;

            AllQuiet
                .Transition(NormalNoise, smi => smi.GetLoudness() >= 20)
                .ToggleEffect("NoisePeaceful");

            NormalNoise
                .Transition(AllQuiet, smi => smi.GetLoudness() < 20)
                .Transition(LoudNoise, smi => smi.GetLoudness() >= 70);
            LoudNoise
                .Transition(NormalNoise, smi=>smi.GetLoudness() < 70 )
                .Transition(VeryLoudNoise, smi=>smi.GetLoudness() >= 120)
                .ToggleEffect("NoiseMinor");
            VeryLoudNoise
                .Transition(LoudNoise, smi => smi.GetLoudness() < 120)
                .ToggleEffect("NoiseMajor");
        }
        public class Def : BaseDef
        {
        }

        public new class Instance : GameInstance
        {
            public Instance(IStateMachineTarget master, Def def) : base(master, def)
            {
            }
            public float GetLoudness()
            {
                int cell = Grid.PosToCell(this);
                if (AudioEventManager.Get() == null) { return 0; }

                return AudioEventManager.Get().GetDecibelsAtCell(cell);
            }
        }
    }
}
