using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorShield
{

    public class MeteorShield : StateMachineComponent<MeteorShield.StatesInstance>
    {
        [MyCmpGet]
        Operational operational;

        float MaxShieldStrength = 100;
        float ShieldRebuildingSpeedPerSec = 8;

        public override void OnSpawn()
        {
            smi.StartSM();
        }

        /// <summary>
        /// when a particle collides with a shield slice, it deals damage to the shield and gets destroyed
        /// </summary>
        /// <param name="particle"></param>
        public void RadboltImpact(HighEnergyParticle particle)
        {

        }
        /// <summary>
        /// when a comet collides with a shield slice, it deals damage to the shield and gets destroyed
        /// </summary>
        /// <param name="comet"></param>
        public void CometImpact(Comet comet)
        {

        }

        /// <summary>
        /// refresh sightlines for all slices, disabling those that are blocked by solid tiles
        /// </summary>
        private void RefreshShieldSlices()
        {

        }

        /// <summary>
        /// when fully charged, maintaining the shield takes less power -> fridge like implementation
        /// </summary>
        /// <param name="maintainanceActive"></param>
        private void ToggleMaintainanceConsumption(bool maintainanceActive)
        {
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, MeteorShield, object>.GameInstance
        {
            public StatesInstance(MeteorShield master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, MeteorShield>
        {
            public FloatParameter ShieldStrength = new FloatParameter(0);
            public class DefendingState : State
            {
                public State BubbleBuilding;
                public State BubbleFullyMaintained;
            }
            public class OnState : State
            {
                public State ShieldCollapse;
                public State ShieldRebuildCooldown;
                public DefendingState ShieldActive;
                public State Impact;
            }
            public State Off;
            public OnState On;
            

            public override void InitializeStates(out BaseState default_state)
            {
                this.serializable = StateMachine.SerializeType.ParamsOnly;
                default_state = root;
                Off
                    .PlayAnim("off")
                    .EventTransition(GameHashes.OperationalChanged, On, smi => smi.master.operational.IsOperational);
                On.DefaultState(On.ShieldActive)
                    .Update((smi,dt) => smi.master.RefreshShieldSlices())
                    .Enter(smi=>smi.master.operational.SetActive(true))
                    .Exit(smi=>smi.master.operational.SetActive(false))
                    .EventTransition(GameHashes.OperationalChanged, Off, smi => !smi.master.operational.IsOperational);
                On.ShieldActive
                    .ParamTransition(ShieldStrength,On.ShieldCollapse, (smi,strenght) => strenght >= 0f)
                    .Update((smi,dt)=>smi.master.DefendAgainstMeteors(smi,dt))
                    ;
                On.ShieldActive.BubbleBuilding
                   .ParamTransition(ShieldStrength, On.ShieldActive.BubbleBuilding, (smi, strength) => strength >= smi.master.MaxShieldStrength)
                   ;

                On.ShieldActive.BubbleFullyMaintained
                   .ParamTransition(ShieldStrength, On.ShieldActive.BubbleBuilding, (smi, strength) => strength < smi.master.MaxShieldStrength)
                   .Enter(smi => smi.master.ToggleMaintainanceConsumption(true))
                   .Exit(smi => smi.master.ToggleMaintainanceConsumption(false))
                   ;

                On.ShieldCollapse
                    .PlayAnim("collapse")
                    .OnAnimQueueComplete(On.ShieldRebuildCooldown);
                On.ShieldRebuildCooldown
                    .PlayAnim("rebooting")
                    .OnAnimQueueComplete(On.ShieldActive);


            }
        }

    }
}
