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
            fn = (ref Chore.Precondition.Context context, object data) => !(context.consumerState.consumer == null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon")
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
            descriptor.SetupDescriptor(string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, str, GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, str, GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), Descriptor.DescriptorType.Requirement);
            descs.Add(descriptor);
        }
        private void OverwriteSymbol()
        {
            if (VaricolouredBalloonsHelperType == null)
                return;
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
          GameStateMachine<States, StatesInstance, Bawoongiver>
        {
            private State unoperational;
            private State operational;
            private ReadyStates ready;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = unoperational;
                this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational);
                this.operational.PlayAnim("off").TagTransition(GameTags.Operational, this.unoperational, true).Transition(ready, new Transition.ConditionCallback(this.IsReady)).EventTransition(GameHashes.OnStorageChange, ready, new Transition.ConditionCallback(this.IsReady));
                this.ready.TagTransition(GameTags.Operational, this.unoperational, true)
                    .DefaultState(this.ready.idle)
                    .ToggleChore(new Func<StatesInstance, Chore>(this.CreateChore), this.operational);
                this.ready.idle
                    .Enter((smi) => smi.master.OverwriteSymbol())
                    .PlayAnim("on", KAnim.PlayMode.Once)
                    .WorkableStartTransition(
                    smi => smi.master.GetComponent<BawoongiverWorkable>(), this.ready.working)
                    .Transition(this.operational, GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>
                    .Not(new Transition.ConditionCallback(this.IsReady)))
                    .EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<Bawoongiver.States, Bawoongiver.StatesInstance, Bawoongiver, object>
                    .Not(new Transition.ConditionCallback(this.IsReady)));
                this.ready.working.PlayAnim("working_pre").QueueAnim("working", true).WorkableStopTransition(
                    smi => smi.master.GetComponent<BawoongiverWorkable>(), this.ready.post);
                this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete(ready);
            }

            private Chore CreateChore(StatesInstance smi)
            {
                Workable component = smi.master.GetComponent<BawoongiverWorkable>();
                WorkChore<BawoongiverWorkable> chore = new WorkChore<BawoongiverWorkable>(Db.Get().ChoreTypes.Relax, component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high, ignore_building_assignment: true);
                chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
                chore.AddPrecondition(smi.master.HasNoBalloon, chore);
                chore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, chore);

                return chore;
            }

            private bool IsReady(StatesInstance smi)
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
              State
            {
                public State idle;
                public State working;
                public State post;
            }
        }



        public class StatesInstance :
          GameStateMachine<States, StatesInstance, Bawoongiver, object>.GameInstance
        {
            public StatesInstance(Bawoongiver smi)
              : base(smi)
            {
            }
        }
    }
}
