using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace Rockets_TinyYetBig.SpaceStations.OrbitUpkeep
{
    internal class OrbitalUpkeepObject : StateMachineComponent<OrbitalUpkeepObject.StatesInstance>, ISaveLoadable
    {
        

        public class StatesInstance : GameStateMachine<States, StatesInstance, OrbitalUpkeepObject, StatesInstance.Def>.GameInstance
        {
            [Serialize] public float gracePeriodTimer = 600;
            public StatesInstance(OrbitalUpkeepObject master) : base(master)
            {
            }
            public class Def : StateMachine.BaseDef
            {
                public float orbitDecayRate;

                public Def(float decay)
                {
                    orbitDecayRate = decay;
                }
            }
        }

        public class States : GameStateMachine<States, StatesInstance, OrbitalUpkeepObject, StatesInstance.Def>
        {
            [Serialize] public FloatParameter OrbitStability;
            [Serialize] public FloatParameter DriftingSpeed;
            [Serialize] public BoolParameter GracePeriodFullfilled;
            

            public State Init;
            public State Crash;
            public ActiveStates activeStates;

            public class ActiveStates : State
            {
                public State StableOrbit;
                public State Drifting;
            }

            public override void InitializeStates(out BaseState defaultState)
            {

                defaultState = Init;

                Init
                    .Update((smi, dt) =>
                    {
                        if (smi.gracePeriodTimer < 0)
                        {
                            GracePeriodFullfilled.Set(true,smi);
                        }
                        smi.gracePeriodTimer-=dt;
                    },UpdateRate.SIM_4000ms)
                    .ParamTransition<bool>(this.GracePeriodFullfilled, this.activeStates.StableOrbit, IsTrue)
                    .Exit(smi =>
                    {
                        OrbitStability.Set(1600,smi);
                    })
                    ;
                activeStates
                    .Update((smi, dt) =>
                    {
                        var currentStab = OrbitStability.Get(smi);
                        currentStab -= (smi.def.orbitDecayRate*dt);
                        
                    }, UpdateRate.SIM_4000ms);
                activeStates.StableOrbit
                    //.ToggleStatusItem("Stable Orbit");
                    .ParamTransition<float>(this.OrbitStability, this.activeStates.Drifting, ((smi, stab) => stab < 0.0f));
                activeStates.Drifting
                    //.ToggleStatusItem("Drifting");
                    .ParamTransition<float>(this.OrbitStability, this.activeStates.StableOrbit, ((smi, stab) => stab > 0.0f));

            }
        }

    }
}
