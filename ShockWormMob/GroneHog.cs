// GroneHog
using KSerialization;
using System.Runtime.Serialization;
using UnityEngine;

namespace ShockWormMob
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class GroneHog : StateMachineComponent<GroneHog.StatesInstance>, ISaveLoadable
    {
        public class StatesInstance : GameStateMachine<States, StatesInstance, GroneHog>.GameInstance
        {
            public StatesInstance(GroneHog master)
                : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, GroneHog>
        {
            public class GroundedState : State
            {
                public State idle;

                public State idle_alt;

                public State move;

                public State move_flee;

                public State startle;

                public State jump;

                public State harvest;

                public State death;
            }

            public GroundedState grounded = new GroundedState();

            public State fall = new State();

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = grounded.idle;
                grounded
                    .Update((smi, dt) =>
                    {
                        int num = Grid.PosToCell(smi.transform.position + Vector3.down);
                        if (Grid.IsValidCell(num) && !Grid.Solid[num])
                        {
                            smi.GoTo(fall);
                        }
                    })
                    .EventHandler(GameHashes.Attacked, (smi) =>
                    {
                        smi.master.FightOrFlight();
                    })
                    .EventTransition(GameHashes.Harvest, grounded.death);

                grounded.idle
                    .Enter((smi) =>
                    {
                        smi.Play("idle", KAnim.PlayMode.Loop);
                        if (!smi.master.fleeing && Random.Range(0f, 100f) > 80f)
                        {
                            smi.master.ReverseDirection();
                        }
                        if (!smi.master.fleeing && smi.master.mMound != null && CreatureHelpers.CrewNearby(smi.transform))
                        {
                            smi.GoTo(grounded.startle);
                        }
                        else if (smi.master.GetMoveTarget() != smi.transform.position)
                        {
                            if (!smi.master.fleeing)
                            {
                                smi.ScheduleGoTo(smi.master.moveSpeed, grounded.move);
                            }
                            else if (smi.master.AtMound() && smi.master.mMound.HogEnter())
                            {
                                smi.master.EnterMound();
                            }
                            else
                            {
                                smi.ScheduleGoTo(0f, grounded.move_flee);
                            }
                        }
                        else
                        {
                            smi.ScheduleGoTo(smi.master.moveSpeed, grounded.idle_alt);
                        }
                    });

                grounded.idle_alt
                    .Enter((smi) =>
                    {
                        if (!smi.master.fleeing && smi.master.mMound != null && CreatureHelpers.CrewNearby(smi.transform))
                        {
                            smi.GoTo(grounded.startle);
                        }
                        else
                        {
                            smi.ScheduleGoTo(smi.master.moveSpeed, grounded.idle);
                        }
                    });

                grounded.move
                    .Enter((smi) =>
                    {
                        if (!smi.master.fleeing && smi.master.mMound != null && CreatureHelpers.CrewNearby(smi.master.transform))
                        {
                            smi.GoTo(grounded.startle);
                        }
                        else
                        {
                            Vector3 moveTarget4 = smi.master.GetMoveTarget();
                            float y5 = moveTarget4.y;
                            Vector3 position6 = smi.transform.position;
                            if (y5 != position6.y)
                            {
                                smi.GoTo(grounded.jump);
                            }
                            else
                            {
                                float x2 = moveTarget4.x;
                                Vector3 position7 = smi.transform.position;
                                if (Mathf.Abs(x2 - position7.x) < 2f)
                                {
                                    smi.Play("walk", KAnim.PlayMode.Loop);
                                    //smi.master.mover.MoveToTarget(smi.master.GetMoveTarget(), smi.master.moveSpeed);
                                }
                                else
                                {
                                    smi.GoTo(grounded.jump);
                                }
                            }
                            CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
                            smi.ScheduleGoTo(smi.master.moveSpeed, grounded.idle);
                        }
                    });

                grounded.move_flee
                    .Enter((smi) =>
                    {
                        smi.master.SetDirectionToMound();
                        Vector3 moveTarget3 = smi.master.GetMoveTarget(reverseDirectionIfNecessary: false);
                        float y4 = moveTarget3.y;
                        Vector3 position4 = smi.transform.position;
                        if (y4 != position4.y)
                        {
                            smi.GoTo(grounded.jump);
                        }
                        else
                        {
                            float x = moveTarget3.x;
                            Vector3 position5 = smi.transform.position;
                            if (Mathf.Abs(x - position5.x) < 2f)
                            {
                                smi.Play("run");
                                //smi.master.mover.MoveToTarget(smi.master.GetMoveTarget(), smi.master.fleeMoveSpeed);
                            }
                            else
                            {
                                smi.GoTo(grounded.jump);
                            }
                        }
                        CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
                        smi.ScheduleGoTo(smi.master.fleeMoveSpeed, grounded.idle);
                    });
                grounded.jump
                    .Enter((smi) =>
                    {
                        Vector3 moveTarget2 = smi.master.GetMoveTarget();
                        CreatureHelpers.FlipAnim(smi.master.anim, smi.master.Heading);
                        float y = moveTarget2.y;
                        Vector3 position = smi.transform.position;
                        if (y - position.y >= 2f)
                        {
                            smi.Play("jump_2");
                        }
                        else
                        {
                            float y2 = moveTarget2.y;
                            Vector3 position2 = smi.transform.position;
                            if (y2 - position2.y >= 1f)
                            {
                                smi.Play("jump");
                            }
                            else
                            {
                                float y3 = moveTarget2.y;
                                Vector3 position3 = smi.transform.position;
                                if (y3 - position3.y < 0f)
                                {
                                    smi.Play("jump_down");
                                }
                                else
                                {
                                    smi.Play("jump_cross");
                                }
                            }
                        }
                        smi.ScheduleGoTo(smi.master.jumpDuration, grounded.idle);
                    }).Exit(delegate (StatesInstance smi)
                    {
                        Vector3 moveTarget = smi.master.GetMoveTarget();
                        //smi.master.mover.TeleportToTarget(moveTarget, 0f);
                    });

                grounded.startle
                    .Enter((smi) =>
                    {
                        smi.master.fleeing = true;
                        //smi.master.mover.StopMovement();
                        //smi.master.harvestable.ForceCancelHarvest();
                        smi.Play("startle");
                        smi.ScheduleGoTo(smi.master.moveSpeed, grounded.idle);
                    });

                grounded.harvest
                    .Enter((smi) =>
                    {
                        //smi.master.mover.StopMovement();
                        smi.master.harvestPosition = smi.transform.position;
                        smi.Play("harvest", KAnim.PlayMode.Loop);
                    }).EventTransition(GameHashes.StoppedAttacking, grounded.idle);

                grounded.harvest.Update((smi, dt) =>
                {
                    smi.transform.SetPosition(smi.master.harvestPosition);
                });

                grounded.death
                    .Enter(delegate (StatesInstance smi)
                    {
                        smi.Play("death");
                        smi.master.death();
                    })
                    .EventHandler(GameHashes.Butcher, (smi)=>
                    {
                        smi.Schedule(0.1f, delegate
                        {
                            Util.KDestroyGameObject(smi.gameObject);
                        });
                    });

                fall.ToggleGravity(grounded.idle);
            }
        }

        [MyCmpAdd]
        private KBatchedAnimController anim;

      //  [MyCmpAdd]
      //  private SimpleMover mover;

        //[MyCmpAdd]
        //private ElementEmitter emitter;

       // [MyCmpAdd]
       // private BoxCollider2D mCollider;

        //[MyCmpAdd]
        //private Harvestable harvestable;

        private Vector2 Heading;

        private float moveSpeed = 0.75f;

        private float fleeMoveSpeed = 0.5f;

        private float jumpDuration = 1f;

        [Serialize]
        private Ref<GroneHogMound> moundRef = new Ref<GroneHogMound>();

        private bool fleeing;

        private Vector3 harvestPosition;

        public GroneHogMound mMound
        {
            get
            {
                return moundRef.Get();
            }
            set
            {
                moundRef.Set(value);
            }
        }

        public void SetMound(GroneHogMound mound)
        {
            mMound = mound;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            base.smi.GoTo(base.smi.sm.grounded.idle);
        }

        private void OnEnable()
        {
            fleeing = false;
            if (Random.Range(0f, 100f) > 50f)
            {
                Heading = Vector2.right;
            }
            else
            {
                Heading = Vector2.left;
            }
            base.smi.GoTo(base.smi.sm.grounded.idle_alt);
        }

        public bool AtMound()
        {
            if (mMound != null && Grid.PosToCell(transform.position) == Grid.PosToCell(mMound.transform.position))
            {
                return true;
            }
            return false;
        }

        private void EnterMound()
        {
            if (SelectTool.Instance.selected != null && SelectTool.Instance.selected.gameObject == base.gameObject)
            {
                SelectTool.Instance.Select(null);
            }
            base.gameObject.SetActive(value: false);
        }

        private Vector3 GetMoveTarget(bool reverseDirectionIfNecessary = true)
        {
            Vector3 walkMoveTarget = CreatureHelpers.GetWalkMoveTarget(transform, Heading);
            if (reverseDirectionIfNecessary && walkMoveTarget == transform.position)
            {
                ReverseDirection();
            }
            return walkMoveTarget;
        }

        private void FightOrFlight()
        {
            if (mMound != null && GetMoveTarget(reverseDirectionIfNecessary: false) != transform.position)
            {
                fleeing = true;
                SetDirectionToMound();
                base.smi.GoTo(base.smi.sm.grounded.startle);
            }
            else
            {
                base.smi.GoTo(base.smi.sm.grounded.harvest);
            }
        }

        private void SetDirectionToMound()
        {
            if (!(mMound != null))
            {
                return;
            }
            Vector3 position = mMound.transform.position;
            float x = position.x;
            Vector3 position2 = transform.position;
            if (x < position2.x)
            {
                Heading = Vector3.left;
                return;
            }
            Vector3 position3 = mMound.transform.position;
            float x2 = position3.x;
            Vector3 position4 = transform.position;
            if (x2 > position4.x)
            {
                Heading = Vector3.right;
            }
        }

        private void death()
        {
            if (mMound != null)
            {
                mMound.gameObject.Trigger(1623392196, this);
            }
        }

        private void ReverseDirection()
        {
            Heading.x *= -1f;
        }

        [OnDeserialized]
        private void OnDeserialized()
        {
            if (moundRef != null)
            {
                moundRef.Get<GroneHogMound>().RestoreHog(base.gameObject);
            }
        }
    }
}


