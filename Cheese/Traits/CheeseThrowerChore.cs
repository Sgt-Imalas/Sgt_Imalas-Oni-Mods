using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Cheese.Traits
{
    /// <summary>
    /// Credit: Akis Beached, PlushieGifter
    /// </summary>
    internal class CheeseThrowerChore : Chore<CheeseThrowerChore.StatesInstance>, IWorkerPrioritizable
    {
        private Precondition HasMessTableCondition = new()
        {
            id = "Cheese_Precondition_HasMessTable",
            description = "Found a Table to slap a cheese on.",
            fn = HasTable
        };

        private static bool HasTable(ref Precondition.Context context, object data)
        {
            return data is Chore<StatesInstance> chore && chore.smi.HasTargetCell();
        }

        public CheeseThrowerChore(IStateMachineTarget target) : base(
            Db.Get().ChoreTypes.JoyReaction,
            target,
            target.GetComponent<ChoreProvider>(),
            false,
            master_priority_class: PriorityScreen.PriorityClass.high,
            report_type: ReportManager.ReportType.PersonalTime)
        {
            showAvailabilityInHoverText = false;
            smi = new StatesInstance(this, target.gameObject);

            AddPrecondition(HasMessTableCondition, this);
            AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
            AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
            AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
        }

        public bool GetWorkerPriority(Worker worker, out int priority)
        {
            priority = RELAXATION.PRIORITY.TIER1;
            return true;
        }

        public class States : GameStateMachine<States, StatesInstance, CheeseThrowerChore>
        {
            public TargetParameter artist;
            public State goToTable;
            public State idle;
            public State creatingPlushie;
            public State success;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = idle;
                Target(artist);

                root
                    .EventTransition(GameHashes.ScheduleBlocksChanged, idle, smi => !smi.IsRecTime());

                idle
                    .UpdateTransition(goToTable, FindTable, UpdateRate.SIM_1000ms);
                //.EventTransition(GameHashes.NewBuilding, smi => GameplayEventManager.Instance, goToTable, FindTable);

                goToTable
                    .UpdateTransition(idle, (smi, dt) => smi.targetTable == null)
                    .EventHandlerTransition(GameHashes.QueueDestroyObject, smi => smi.targetTable, idle, (smi, data) => true)
                    .MoveTo(smi => smi.GetTargetCell(), creatingPlushie);

                creatingPlushie
                    .PlayAnim("working_pre")
                    .QueueAnim("working_loop")
                    .QueueAnim("working_pst")
                    .OnAnimQueueComplete(success);

                success
                    .Enter(smi =>
                    {
                        smi.YeetCheese();
                        smi.StopSM("completed");
                    })
                    .ReturnSuccess();
            }

            private bool FindTable(StatesInstance smi, float dt)
            {
                foreach (CheeseTable cheesetable in ModAssets.CheeseTableTargets)
                {
                    if (IsTableEligible(cheesetable, smi))
                    {
                        smi.targetTable = cheesetable;
                        return true;
                    }
                }

                return false;
            }

            private bool IsTableEligible(CheeseTable table, StatesInstance smi)
            {
                return 
                       table.GetComponent<Operational>().IsOperational
                    && smi.navigator.GetNavigationCost(table.NaturalBuildingCell()) != -1;
            }
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, CheeseThrowerChore, object>.GameInstance
        {
            public CheeseTable targetTable;
            public Navigator navigator;
            private GameObject plushieGifter;

            public StatesInstance(CheeseThrowerChore master, GameObject plushieGifter) : base(master)
            {
                this.plushieGifter = plushieGifter;
                navigator = master.GetComponent<Navigator>();
                sm.artist.Set(plushieGifter, smi);
            }

            public bool HasTargetCell() => targetTable != null;

            public bool IsRecTime() => master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);

            public int GetTargetCell() => targetTable.NaturalBuildingCell();

            public void YeetCheese()
            {
                if (targetTable != null)
                    plushieGifter.GetSMI<CheeseThrower.Instance>().YeetCheese(GetTargetCell());

                targetTable = null;
            }
        }
    }
}
