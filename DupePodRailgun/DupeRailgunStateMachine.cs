using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DupePodRailgun
{
    internal class DupeRailgunStateMachine : StateMachineComponent<DupeRailgunStateMachine.StatesInstance>, ISaveLoadable
    //, IGameObjectEffectDescriptor
    {
        public static readonly HashedString PORT_ID = (HashedString)nameof(DupeRailgunStateMachine);
        [MyCmpReq]
        public Assignable assignable;
        [MyCmpGet]
        private Operational operational;
        public CellOffset targetCellOffset = CellOffset.none;
        public int TargetCellInt = -1;

        public bool HasDupeAssigned()
        {
            if (assignable != null)
                return assignable.IsAssigned();
            return true;
        }
        public Navigator GetTargetNavigator()
        {
            if (!assignable.IsAssigned())
                return null;
            //Debug.Log("Ass: " + assignable.assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
            return assignable.assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<Navigator>();
        }

        public override void OnSpawn()
        {
            assignable.OnAssign += new System.Action<IAssignableIdentity>(this.RedoAssignment);
            base.OnSpawn();
            TargetCellInt = Grid.OffsetCell(Grid.PosToCell(this), targetCellOffset);
            this.smi.StartSM();
            this.Subscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));

        }

        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (logicValueChanged.portID != DupeRailgunStateMachine.PORT_ID)
                return;
            bool logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
            operational.SetActive(logic_on);
        }

        void RedoAssignment(IAssignableIdentity target)
        {
            //smi.GoTo(smi.sm.Idle);
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        #region StateMachine

        public class StatesInstance : GameStateMachine<States, StatesInstance, DupeRailgunStateMachine>.GameInstance
        {
            public StatesInstance(DupeRailgunStateMachine master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<DupeRailgunStateMachine.States, DupeRailgunStateMachine.StatesInstance, DupeRailgunStateMachine, object>
        {
            public FloatParameter FireCooldown { get; set; }
            //replace that with railgun code @hom
            public class NonFunctioningStates : State
            {
                public State Cooldown;
                public State NotEnoughRailPieces;
                public State NotEnoughPower;
                public State NotEnoughRadbolts;
                public State NoTargetSelected;
            }
            public class FunctioningStates
            {
                public State Idle;
                public State NoOrdananceTypeSelected;
                public State NoOrdananceAvailable;

                public ReadyToShoot ReadyToShoot;
                public Firing Firing;
            }
            public class ReadyToShoot : State
            {
                public State NoneSelected;
                public State DupeAssigned;
                public State NukeAssigned;
            }
            public class Firing : State
            {
                public State LoadingProjectile;
                public State PoweringRails;
                public State Fire;
                public State PoweringDown;
            }

            public NonFunctioningStates NonFunctioning;
            public FunctioningStates Functioning;

            public override void InitializeStates(out BaseState defaultState)
            {

                defaultState = NonFunctioning;

                NonFunctioning.GoTo(NonFunctioning.Cooldown);

                NonFunctioning.Cooldown
                    .Update((smi, dt) => FireCooldown.Set((FireCooldown.Get(smi) - dt), smi))
                    .ParamTransition(FireCooldown, NonFunctioning.NotEnoughRailPieces, (smi, fcd) => fcd <= 0f);
                NonFunctioning.NotEnoughRailPieces
                    .Update((smi, dt) =>
                    {
                        if (smi.master.ConnectedRailPieceCount() >= 2)
                        {
                            smi.GoTo(NonFunctioning.NotEnoughPower);
                        }
                    }, UpdateRate.RENDER_1000ms);
                NonFunctioning.NotEnoughPower
                    .Update((smi, dt) =>
                    {
                        if (smi.master.RadboltsCharged())
                        {
                            smi.GoTo(NonFunctioning.NotEnoughRadbolts);
                        }
                    }, UpdateRate.RENDER_1000ms);
                NonFunctioning.NotEnoughRadbolts
                    .Update((smi, dt) =>
                    {
                        if (smi.master.RadboltsCharged())
                        {
                            smi.GoTo(NonFunctioning.NoTargetSelected);
                        }
                    }, UpdateRate.RENDER_1000ms);
                NonFunctioning.NoTargetSelected
                    .Update((smi, dt) =>
                    {
                        if (smi.master.assignable.assignee!=null)
                        {
                            smi.GoTo(NonFunctioning.NoTargetSelected);
                        }
                    }, UpdateRate.RENDER_1000ms);


                dupeAssignedStates.defaultState = dupeAssignedStates.RedSignal;

                dupeAssignedStates
                    .Enter((smi) =>
                    {
                        //Debug.Log("enter assigned");
                        smi.nav = smi.master.GetTargetNavigator();
                    })
                    .Update((smi, dt) =>
                    {
                        //Debug.Log("Operational: " + smi.GetComponent<Operational>().IsOperational + ", isActive: " + smi.GetComponent<Operational>().IsActive);
                        if (!smi.master.HasDupeAssigned())
                        {
                            smi.GoTo(Idle);
                        }
                    }, UpdateRate.SIM_200ms)
                    .EventTransition(GameHashes.OperationalChanged, Idle, smi => !smi.GetComponent<Operational>().IsOperational)
                    .Exit((smi) =>
                    {
                        smi.nav = null;
                    });

                dupeAssignedStates.RedSignal
                    .PlayAnim("off")
                    //.Enter(smi => Debug.Log("enter red sig"))
                    .EventTransition(GameHashes.ActiveChanged, dupeAssignedStates.GreenSignal, smi => smi.GetComponent<Operational>().IsActive)
                    ;
                dupeAssignedStates.GreenSignal.defaultState = dupeAssignedStates.GreenSignal.MovingDupe;

                dupeAssignedStates.GreenSignal
                    .EventTransition(GameHashes.ActiveChanged, dupeAssignedStates.RedSignal, smi => !smi.GetComponent<Operational>().IsActive)
                    .Enter((smi) =>
                    {
                        smi.nav = smi.master.GetTargetNavigator();
                    }).PlayAnim("on");
                dupeAssignedStates.GreenSignal.MovingDupe
                    .ToggleChore(CreateChore, dupeAssignedStates.GreenSignal.DupeArrived);
                dupeAssignedStates.GreenSignal.DupeArrived
                    .GoTo(dupeAssignedStates.GreenSignal.MovingDupe);
                
            }
        }

        private bool RadboltsCharged()
        {
            throw new NotImplementedException();
        }

        private int ConnectedRailPieceCount()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}

