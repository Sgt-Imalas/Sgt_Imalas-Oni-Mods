using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BawoonFwiend
{
    internal class Bawoongiver :
  StateMachineComponent<Bawoongiver.StatesInstance>,
  IGameObjectEffectDescriptor

    {

        static Type VaricolouredBalloonsHelperType = Type.GetType("VaricolouredBalloons.VaricolouredBalloonsHelper, VaricolouredBalloons", false, false);
        [MyCmpGet]
        Storage storage;

        private Chore.Precondition HasNoBalloon = new Chore.Precondition()
        {
            id = nameof(HasNoBalloon),
            description = "Duplicant doesn't have a balloon already",
            fn = (Chore.PreconditionFn)((ref Chore.Precondition.Context context, object data) => !((UnityEngine.Object)context.consumerState.consumer == (UnityEngine.Object)null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon"))
        };
        public static float BloongasUsage = 5f;

        public override void OnSpawn()
        {

            base.OnSpawn();
            this.smi.StartSM(); 

            //OverwriteSymbol();
        }

        public override void OnCleanUp() => base.OnCleanUp();

        private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
        {
            string str = tag.ProperName();
            Descriptor descriptor = new Descriptor();
            descriptor.SetupDescriptor(string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object)str, (object)GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object)str, (object)GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), Descriptor.DescriptorType.Requirement);
            descs.Add(descriptor);
        }
        private void OverwriteSymbol()
        {
            var artist = GetComponent(BawoongiverWorkable.VaricolouredBalloonsHelperType);
            if (artist != null)
            {
                var symbolidx = (uint)Traverse.Create(artist).Method("get_ArtistBalloonSymbolIdx").GetValue();
                SgtLogger.debuglog("id: " + symbolidx);
                Traverse.Create(artist).Method("ApplySymbolOverrideByIdx", new[] { symbolidx } ).GetValue();
                //Traverse.Create(artist).Method("ApplySymbolOverrideByIdx").GetValue(symbolidx);
            }
        }


        List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(
          GameObject go)
        {
            List<Descriptor> descs = new List<Descriptor>();
            Descriptor descriptor = new Descriptor();
            descriptor.SetupDescriptor((string)global::STRINGS.UI.BUILDINGEFFECTS.RECREATION, (string)global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
            descs.Add(descriptor);
            //Effect.AddModifierDescriptions(this.gameObject, descs, "Balloonfriend", true);
            this.AddRequirementDesc(descs, ModAssets.Tags.BalloonGas, Bawoongiver.BloongasUsage);
            return descs;
        }

        public class States :
          GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver>
        {
            private GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State unoperational;
            private GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State operational;
            private Bawoongiver.States.ReadyStates ready;

            public override void InitializeStates(out StateMachine.BaseState default_state)
            {
                default_state = (StateMachine.BaseState)this.unoperational;
                this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational);
                this.operational.PlayAnim("off").TagTransition(GameTags.Operational, this.unoperational, true).Transition((GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State)this.ready, new StateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.Transition.ConditionCallback(this.IsReady)).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State)this.ready, new StateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.Transition.ConditionCallback(this.IsReady));
                this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<Bawoongiver.StatesInstance, Chore>(this.CreateChore), this.operational);
                this.ready.idle
                    .Enter((smi) => smi.master.OverwriteSymbol())
                    .PlayAnim("on", KAnim.PlayMode.Loop)
                    .WorkableStartTransition((Func<Bawoongiver.StatesInstance, Workable>)
                    (smi => (Workable)smi.master.GetComponent<BawoongiverWorkable>()), this.ready.working)
                    .Transition(this.operational, GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>
                    .Not(new StateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.Transition.ConditionCallback(this.IsReady)))
                    .EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>
                    .Not(new StateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.Transition.ConditionCallback(this.IsReady)));
                this.ready.working.PlayAnim("working_pre").QueueAnim("working", true).WorkableStopTransition((Func<Bawoongiver.StatesInstance, Workable>)(smi => (Workable)smi.master.GetComponent<BawoongiverWorkable>()), this.ready.post);
                this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete((GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State)this.ready);
            }

            private Chore CreateChore(Bawoongiver.StatesInstance smi)
            {
                Workable component = (Workable)smi.master.GetComponent<BawoongiverWorkable>();
                WorkChore<BawoongiverWorkable> chore = new WorkChore<BawoongiverWorkable>(Db.Get().ChoreTypes.Relax, (IStateMachineTarget)component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high, ignore_building_assignment: true);
                chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object)component);
                chore.AddPrecondition(smi.master.HasNoBalloon, (object)chore);
                chore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object)chore);

                return (Chore)chore;
            }

            private bool IsReady(Bawoongiver.StatesInstance smi)
            {
                foreach (var item in smi.master.storage.items)
                {
                    if (item.HasTag(ModAssets.Tags.BalloonGas) && item.TryGetComponent<PrimaryElement>(out var targetElement))
                    {
                        if (targetElement.Mass >= Bawoongiver.BloongasUsage)
                            return true;
                    }
                }
                return false;
            }

            public class ReadyStates :
              GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State
            {
                public GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State idle;
                public GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State working;
                public GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.State post;
            }
        }



        public class StatesInstance :
          GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>.GameInstance
        {
            public StatesInstance(Bawoongiver smi)
              : base(smi)
            {
            }
        }
    }
}
