using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    internal class ExtendedClusterModuleAnimator : StateMachineComponent<ExtendedClusterModuleAnimator.StatesInstance>
    {
        [MyCmpReq]
        public KBatchedAnimController animController;
        public Light2D flameLight;

        public bool UseLight = false;

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.smi.StartSM();
        }

        public void ConfigureFlameLight()
        {
            this.flameLight = this.gameObject.AddOrGet<Light2D>();
            this.flameLight.Color = Color.white;
            this.flameLight.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
            this.flameLight.Range = 10f;
            this.flameLight.Angle = 0.0f;
            this.flameLight.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
            this.flameLight.Offset = LIGHT2D.LIGHTBUG_OFFSET;
            this.flameLight.shape = LightShape.Circle;
            this.flameLight.drawOverlay = true;
            this.flameLight.Lux = 80000;
            this.flameLight.emitter.RemoveFromGrid();
            this.gameObject.AddOrGet<LightSymbolTracker>().targetSymbol = this.GetComponent<KBatchedAnimController>().CurrentAnim.rootSymbol;
            this.flameLight.enabled = false;
        }

        public void UpdateFlameLight(int cell)
        {
            int num = (int)this.smi.master.flameLight.RefreshShapeAndPosition();
            if (Grid.IsValidCell(cell))
            {
                if (this.smi.master.flameLight.enabled || (double)this.smi.timeinstate <= 3.0)
                    return;
                this.smi.master.flameLight.enabled = true;
            }
            else
                this.smi.master.flameLight.enabled = false;
        }

        public override void OnCleanUp() => base.OnCleanUp();

        public class StatesInstance :
          GameStateMachine<ExtendedClusterModuleAnimator.States, ExtendedClusterModuleAnimator.StatesInstance, ExtendedClusterModuleAnimator, object>.GameInstance
        {
            public Vector3 radiationEmissionBaseOffset;
            public int pad_cell;

            public StatesInstance(ExtendedClusterModuleAnimator smi)
              : base(smi)
            {
            }

            public void BeginBurn()
            {

            }

            public void DoBurn(float dt)
            {

            }

            public void EndBurn()
            {

            }
        }

        public class States :
          GameStateMachine<ExtendedClusterModuleAnimator.States, ExtendedClusterModuleAnimator.StatesInstance, ExtendedClusterModuleAnimator>
        {
            public InitializingStates initializing;
            public IdleStates idle;
            public State burning_pre;
            public State burning;
            public State burnComplete;
            public State space;

            public override void InitializeStates(out StateMachine.BaseState default_state)
            {
                default_state = initializing.load;
                this.initializing.load
                    .ScheduleGoTo(0.0f, initializing.decide);
                this.initializing.decide
                    .Transition(this.space, new Transition.ConditionCallback(this.IsRocketInSpace))
                    .Transition(this.burning, new Transition.ConditionCallback(this.IsRocketAirborne))
                    .Transition((State)this.idle, new Transition.ConditionCallback(this.IsRocketGrounded));
                this.idle.DefaultState(this.idle.grounded).EventTransition(GameHashes.RocketLaunched, this.burning_pre);
                this.idle.grounded.EventTransition(GameHashes.LaunchConditionChanged, this.idle.ready, new Transition.ConditionCallback(this.IsReadyToLaunch)).QueueAnim("grounded", true);
                this.idle.ready
                    .EventTransition(GameHashes.LaunchConditionChanged, this.idle.grounded, Not(new Transition.ConditionCallback(this.IsReadyToLaunch)))
                    .PlayAnim("pre_ready_to_launch", KAnim.PlayMode.Once)
                    .QueueAnim("ready_to_launch", true)
                    .Exit(smi =>
                    {
                        KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
                        if (component == null)
                            return;
                        component.Play((HashedString)"pst_ready_to_launch");
                    });
                this.burning_pre
                    .PlayAnim("launch_pre")
                    .OnAnimQueueComplete(this.burning);
                this.burning
                    .EventTransition(GameHashes.RocketLanded, this.burnComplete)
                    .PlayAnim("launch", KAnim.PlayMode.Loop)
                    .Enter(smi => smi.BeginBurn())
                    .Update((smi, dt) => smi.DoBurn(dt))
                    .Exit((smi => smi.EndBurn()))
                    .TagTransition(GameTags.RocketInSpace, this.space);
                this.space
                    .EventTransition(GameHashes.DoReturnRocket, this.burning);
                this.burnComplete
                    .PlayAnim("launch_pst", KAnim.PlayMode.Loop).GoTo((State)this.idle);
            }

            public bool IsReadyToLaunch(StatesInstance smi) => smi.GetComponent<RocketModuleCluster>().CraftInterface.CheckPreppedForLaunch();

            public bool IsRocketAirborne(StatesInstance smi) => smi.master.HasTag(GameTags.RocketNotOnGround) && !smi.master.HasTag(GameTags.RocketInSpace);

            public bool IsRocketGrounded(StatesInstance smi) => smi.master.HasTag(GameTags.RocketOnGround);

            public bool IsRocketInSpace(StatesInstance smi) => smi.master.HasTag(GameTags.RocketInSpace);

            public class InitializingStates :
              State
            {
                public State load;
                public State decide;
            }

            public class IdleStates :
              State
            {
                public State grounded;
                public State ready;
            }
        }
    }

}
