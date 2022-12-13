using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoveDupeHere
{
    internal class MoveDupeHereSM : StateMachineComponent<MoveDupeHereSM.StatesInstance>, ISaveLoadable
    //, IGameObjectEffectDescriptor
    {
        public static readonly HashedString PORT_ID = (HashedString)nameof(MoveDupeHereSM);
        [MyCmpReq]
        public Assignable assignable;
        [MyCmpGet]
        private Operational operational;
        public CellOffset targetCellOffset = CellOffset.none;
        public int TargetCellInt = -1;

        public bool HasDupeAssigned()
        {
            return assignable.IsAssigned();
        }
        public Navigator GetTargetNavigator()
        {
            if (!assignable.IsAssigned())
                return null;
            //Debug.Log("Ass: " + assignable.assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
            return assignable.assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<Navigator>();
        }

        protected override void OnSpawn()
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
            if (logicValueChanged.portID != MoveDupeHereSM.PORT_ID)
                return;
            bool logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
            operational.SetActive(logic_on);
        }

        void RedoAssignment(IAssignableIdentity target)
        {
            smi.GoTo(smi.sm.Idle);
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        #region StateMachine

        public class StatesInstance : GameStateMachine<States, StatesInstance, MoveDupeHereSM>.GameInstance
        {
            public Navigator nav;
            public MoveToLocationMonitor.Instance targetLocationMonitor;
            public StatesInstance(MoveDupeHereSM master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<MoveDupeHereSM.States, MoveDupeHereSM.StatesInstance, MoveDupeHereSM, object>
        {
            public class DupeAssignedStates : State
            {
                public State RedSignal;
                public State GreenSignal;
            }

            public State Init;
            public State Idle;
            public DupeAssignedStates dupeAssignedStates;

            public override void InitializeStates(out BaseState defaultState)
            {

                defaultState = Init;

                Init.Enter((smi) =>
                {
                    //Debug.Log("enter init");

                    if (smi.master.HasDupeAssigned() && smi.GetComponent<Operational>().IsOperational)
                    {
                        smi.GoTo(dupeAssignedStates);
                    }
                    else
                    {
                        smi.GoTo(Idle);
                    }
                });


                Idle

                    //.Enter(smi => Debug.Log("enter idle"))
                    .PlayAnim("no_power")
                    .Update((smi, dt) =>
                    {
                        if (smi.master.HasDupeAssigned()&& smi.GetComponent<Operational>().IsOperational)
                        {
                            smi.GoTo(dupeAssignedStates);
                        }
                    }, UpdateRate.RENDER_1000ms);


                dupeAssignedStates.defaultState = dupeAssignedStates.RedSignal;

                dupeAssignedStates
                    .Enter((smi) =>
                    {
                        //Debug.Log("enter assigned");
                        smi.nav = smi.master.GetTargetNavigator();
                        if (smi.nav != null)
                            smi.targetLocationMonitor = smi.nav.GetSMI<MoveToLocationMonitor.Instance>();
                    })
                    .Update((smi, dt) =>
                    {
                        //Debug.Log("Operational: " + smi.GetComponent<Operational>().IsOperational + ", isActive: " + smi.GetComponent<Operational>().IsActive);
                        if (!smi.master.HasDupeAssigned())
                        {
                            smi.GoTo(Idle);
                        }
                    }, UpdateRate.RENDER_1000ms)
                    .EventTransition(GameHashes.OperationalChanged, Idle, smi => !smi.GetComponent<Operational>().IsOperational)
                    .Exit((smi) =>
                    {
                        smi.nav = null;
                        smi.targetLocationMonitor = null;
                    });

                dupeAssignedStates.RedSignal
                    .PlayAnim("off")
                    //.Enter(smi => Debug.Log("enter red sig"))
                    .EventTransition(GameHashes.ActiveChanged, dupeAssignedStates.GreenSignal, smi => smi.GetComponent<Operational>().IsActive)
                    ;

                dupeAssignedStates.GreenSignal
                    .Enter((smi) =>
                    {
                        Debug.Log("enter green sig"); 
                        smi.nav = smi.master.GetTargetNavigator();
                        if (smi.nav != null)
                            smi.targetLocationMonitor = smi.nav.GetSMI<MoveToLocationMonitor.Instance>();
                    }).PlayAnim("on")
                    .Update((smi, dt) =>
                    {
                        //Debug.Log("nav: " + smi.nav + ", monitor: " + smi.targetLocationMonitor);
                        if(smi.nav != null && smi.targetLocationMonitor != null)
                        {
                            if (smi.nav.CanReach(smi.master.TargetCellInt)&&smi.targetLocationMonitor.IsInsideState(smi.targetLocationMonitor.sm.satisfied))
                            {
                                smi.targetLocationMonitor.MoveToLocation(smi.master.TargetCellInt);
                            }
                        }
                        
                    }, UpdateRate.RENDER_1000ms)
                    .EventTransition(GameHashes.ActiveChanged, dupeAssignedStates.RedSignal, smi => !smi.GetComponent<Operational>().IsActive)
                    ;
            }
        }


        #endregion
    }
}


