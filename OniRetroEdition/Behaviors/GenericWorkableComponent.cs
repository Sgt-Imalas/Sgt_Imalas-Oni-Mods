using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace OniRetroEdition.Behaviors
{
    internal class GenericWorkableComponent : Workable
    {
        [MyCmpReq]
        Operational operational;

        [MyCmpReq]
        ElementConverter converter;

        private WorkChore<GenericWorkableComponent> OperateWorkable;
        private ConverterWorkableSM.Instance smi;

        public string choreTypeID;
        public float WorkTime = 30.0f;
        public CellOffset workOffset = new CellOffset(0, 0);

        public override void OnSpawn()
        {
            base.OnSpawn();
            operational.IsOperational = false;
            this.smi = new ConverterWorkableSM.Instance(this);
            this.smi.StartSM();
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
            this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
            this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
            this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
            this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            this.SetWorkTime(WorkTime);
            this.SetOffsets(new[] { workOffset });
        }

        public WorkChore<GenericWorkableComponent> CreateWorkChore()
        {
            this.OperateWorkable = new WorkChore<GenericWorkableComponent>(Db.Get().ChoreTypes.Fabricate, (IStateMachineTarget)this);
            return this.OperateWorkable;
        }


        public override void OnStartWork(Worker worker)
        {
            base.OnStartWork(worker);
            operational.IsOperational = true;
        }
        public override void OnCompleteWork(Worker worker)
        {
            base.OnCompleteWork(worker);
            operational.IsOperational = false;
        }
        public class ConverterWorkableSM :
    GameStateMachine<ConverterWorkableSM, ConverterWorkableSM.Instance, GenericWorkableComponent>
        {
            public State idle;
            public State requiresWork;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = (BaseState)this.idle;
                this.idle
                    .EventTransition(GameHashes.OnStorageChange, this.requiresWork, (smi => smi.master.converter.HasEnoughMassToStartConverting()));
                this.requiresWork
                    .ToggleChore((smi => smi.master.CreateWorkChore()), this.idle);
            }

            public new class Instance :
              GameInstance
            {
                public Instance(GenericWorkableComponent master)
                  : base(master)
                {
                }
            }
        }
    }
}
