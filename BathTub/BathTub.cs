using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BathTub
{
    [SerializationConfig(MemberSerialization.OptIn)]
    internal class BathTub : StateMachineComponent<BathTub.StatesInstance>, IGameObjectEffectDescriptor
    {
        public string specificEffect;
        public string trackingEffect;
        public int basePriority;
        public CellOffset[] choreOffsets = new CellOffset[4]
        {
            new CellOffset(-1, 0),
            new CellOffset(1, 0),
            new CellOffset(0, 0),
            new CellOffset(2, 0)
        };
        private BathTubWorkable[] workables;
        private Chore[] chores;
        public HashSet<int> occupants = new HashSet<int>();
        public float waterCoolingRate;
        public float BathTubCapacity = 100f;


        [MyCmpGet]
        public Storage waterStorage;
        private MeterController waterMeter;
        private MeterController tempMeter;

        public float PercentFull => 100f * this.waterStorage.GetMassAvailable(SimHashes.Water) / this.BathTubCapacity;

        public override void OnSpawn()
        {
            base.OnSpawn();
            GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule), null, null);
            this.workables = new BathTubWorkable[this.choreOffsets.Length];
            this.chores = new Chore[this.choreOffsets.Length];
            for (int index = 0; index < this.workables.Length; ++index)
            {
                GameObject locator = ChoreHelpers.CreateLocator("BathTubWorkable", Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[index]), Grid.SceneLayer.Move));
                KSelectable kselectable = locator.AddOrGet<KSelectable>();
                kselectable.SetName(this.GetProperName());
                kselectable.IsSelectable = false;
                BathTubWorkable BathTubWorkable1 = locator.AddOrGet<BathTubWorkable>();
                int player_index = index;
                BathTubWorkable BathTubWorkable2 = BathTubWorkable1;
                BathTubWorkable2.OnWorkableEventCB = BathTubWorkable2.OnWorkableEventCB + ((workable, ev) => this.OnWorkableEvent(player_index, ev));
                this.workables[index] = BathTubWorkable1;
                this.workables[index].batTub = this;
            }
            this.waterMeter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_water_target", "meter_water", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
            {
                "meter_water_target"
            });
            this.smi.UpdateWaterMeter();
            this.tempMeter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_temperature_target", "meter_temp", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
            {
                "meter_temperature_target"
            });
            this.smi.TestWaterTemperature();
            this.smi.StartSM();
        }

        public override void OnCleanUp()
        {
            this.UpdateChores(false);
            for (int index = 0; index < this.workables.Length; ++index)
            {
                if ((bool)this.workables[index])
                {
                    Util.KDestroyGameObject(this.workables[index]);
                    this.workables[index] = null;
                }
            }
            base.OnCleanUp();
        }

        private Chore CreateChore(int i)
        {
            Workable workable = this.workables[i];
            ChoreType relax = Db.Get().ChoreTypes.Relax;
            Workable target = workable;
            ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
            Action<Chore> on_end = new Action<Chore>(this.OnSocialChoreEnd);
            ScheduleBlockType schedule_block = recreation;
            WorkChore<BathTubWorkable> chore = new WorkChore<BathTubWorkable>(relax, target, on_end: on_end, allow_in_red_alert: false, schedule_block: schedule_block, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
            chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
            return chore;
        }

        private void OnSocialChoreEnd(Chore chore)
        {
            if (!this.gameObject.HasTag(GameTags.Operational))
                return;
            this.UpdateChores();
        }

        public void UpdateChores(bool update = true)
        {
            for (int i = 0; i < this.choreOffsets.Length; ++i)
            {
                Chore chore = this.chores[i];
                if (update)
                {
                    if (chore == null || chore.isComplete)
                        this.chores[i] = this.CreateChore(i);
                }
                else if (chore != null)
                {
                    chore.Cancel("locator invalidated");
                    this.chores[i] = null;
                }
            }
        }

        public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
        {
            if (ev == Workable.WorkableEvent.WorkStarted)
                this.occupants.Add(player);
            else
                this.occupants.Remove(player);
            this.smi.sm.userCount.Set(this.occupants.Count, this.smi);
        }

        List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(
          GameObject go)
        {
            List<Descriptor> descs = new List<Descriptor>();
            Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Water);
            descs.Add(new Descriptor(BUILDINGS.PREFABS.BathTub.WATER_REQUIREMENT.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.BathTubCapacity)), BUILDINGS.PREFABS.BathTub.WATER_REQUIREMENT_TOOLTIP.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.BathTubCapacity)), Descriptor.DescriptorType.Requirement));
            descs.Add(new Descriptor(BUILDINGS.PREFABS.BathTub.TEMPERATURE_REQUIREMENT.Replace("{element}", elementByHash.name).Replace("{temperature}", GameUtil.GetFormattedTemperature(this.minimumWaterTemperature)), BUILDINGS.PREFABS.BathTub.TEMPERATURE_REQUIREMENT_TOOLTIP.Replace("{element}", elementByHash.name).Replace("{temperature}", GameUtil.GetFormattedTemperature(this.minimumWaterTemperature)), Descriptor.DescriptorType.Requirement));
            descs.Add(new Descriptor((string)UI.BUILDINGEFFECTS.RECREATION, (string)UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION));
            Effect.AddModifierDescriptions(this.gameObject, descs, this.specificEffect, true);
            return descs;
        }

        public class States : GameStateMachine<States, StatesInstance, BathTub>
        {
            public IntParameter userCount;
            public State unoperational;
            public OffStates off;
            public ReadyStates ready;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = ready;
                this.root.Update((smi, dt) =>
                {
                    smi.TestWaterTemperature();
                }, UpdateRate.SIM_4000ms).EventHandler(GameHashes.OnStorageChange, smi =>
                {
                    smi.UpdateWaterMeter();
                    smi.TestWaterTemperature();
                });
                this.unoperational
                    .TagTransition(GameTags.Operational, off)
                    .PlayAnim("off");
                this.off
                    .TagTransition(GameTags.Operational, this.unoperational, true)
                    .DefaultState(off.filling);
                this.off.filling.DefaultState(this.off.filling.normal)
                    .Transition(ready, smi => (double)smi.master.waterStorage.GetMassAvailable(SimHashes.Water) >= smi.master.BathTubCapacity)
                    .PlayAnim("off").Enter(smi => smi.GetComponent<ConduitConsumer>().SetOnState(true))
                    .Exit(smi => smi.GetComponent<ConduitConsumer>().SetOnState(false))
                    .ToggleMainStatusItem(Db.Get().BuildingStatusItems.HotTubFilling, (Func<StatesInstance, object>)(smi => smi.master));
                this.off.filling.normal
                this.off.draining.Transition(off.filling, smi => (double)smi.master.waterStorage.GetMassAvailable(SimHashes.Water) <= 0.0)
                    .Enter(smi => smi.GetComponent<ConduitDispenser>().SetOnState(true))
                    .Exit(smi => smi.GetComponent<ConduitDispenser>().SetOnState(false));
                this.ready
                    .DefaultState(this.ready.idle)
                    .Enter("CreateChore", smi => smi.master.UpdateChores())
                    .Exit("CancelChore", smi => smi.master.UpdateChores(false))
                    .TagTransition(GameTags.Operational, this.unoperational, true)
                    .Transition(off.filling, smi => smi.master.waterStorage.IsEmpty())
                    .ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal);
                this.ready.idle.PlayAnim("on")
                    .ParamTransition(userCount, this.ready.on.pre, (smi, p) => p > 0);
                this.ready.on
                    .Enter(smi => smi.SetActive(true))
                    .Exit(smi => smi.SetActive(false));
                this.ready.on.pre
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(this.ready.on.relaxing);
                this.ready.on.relaxing
                    .PlayAnim("working_loop", KAnim.PlayMode.Loop)
                    .ParamTransition(userCount, this.ready.on.post, (smi, p) => p == 0)
                    .ParamTransition(userCount, this.ready.on.relaxing_together, (smi, p) => p > 1);
                this.ready.on.relaxing_together
                    .PlayAnim("working_loop", KAnim.PlayMode.Loop)
                    .ParamTransition(userCount, this.ready.on.post, (smi, p) => p == 0).
                    ParamTransition(userCount, this.ready.on.relaxing, (smi, p) => p == 1);
                this.ready.on.post.PlayAnim("working_pst")
                    .OnAnimQueueComplete(this.ready.idle);
            }

            private string GetRelaxingAnim(StatesInstance smi)
            {
                bool flag1 = smi.master.occupants.Contains(0);
                bool flag2 = smi.master.occupants.Contains(1);
                if (flag1 && !flag2)
                    return "working_loop_one_p";
                return flag2 && !flag1 ? "working_loop_two_p" : "working_loop_coop_p";
            }

            public class OffStates :
              State
            {
                public State draining;
                public FillingStates filling;
            }

            public class OnStates :
              State
            {
                public State pre;
                public State relaxing;
                public State relaxing_together;
                public State post;
            }

            public class ReadyStates :
              State
            {
                public State idle;
                public OnStates on;
            }

            public class FillingStates :
              State
            {
                public State normal;
            }
        }

        public class StatesInstance :
          GameStateMachine<States, StatesInstance, BathTub, object>.GameInstance
        {
            private Operational operational;

            public StatesInstance(BathTub smi)
              : base(smi)
            {
                this.operational = this.master.GetComponent<Operational>();
            }

            public void SetActive(bool active) => this.operational.SetActive(this.operational.IsOperational & active);

            public void UpdateWaterMeter() => this.smi.master.waterMeter.SetPositionPercent(Mathf.Clamp(this.smi.master.waterStorage.GetMassAvailable(SimHashes.Water) / this.smi.master.BathTubCapacity, 0.0f, 1f));

            public void UpdateTemperatureMeter(float waterTemp)
            {
                Element element = ElementLoader.GetElement(SimHashes.Water.CreateTag());
                //this.smi.master.tempMeter.SetPositionPercent(Mathf.Clamp((float)(((double)waterTemp - (double)this.smi.master.minimumWaterTemperature) / ((double)element.highTemp - (double)this.smi.master.minimumWaterTemperature)), 0.0f, 1f));
            }

            public void TestWaterTemperature()
            {
                GameObject first = this.smi.master.waterStorage.FindFirst(new Tag(1836671383));
                float waterTemp = 0.0f;
                if ((bool)first)
                {
                    float temperature = first.GetComponent<PrimaryElement>().Temperature;
                    this.UpdateTemperatureMeter(temperature);
                }
                else
                {
                    this.UpdateTemperatureMeter(waterTemp);
                }
            }
        }
    }

}
