using Klei;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.ITEMS;

namespace ClothingLockerMod
{
    internal class PillDispenser :
  StateMachineComponent<PillDispenser.SMInstance>,
  IGameObjectEffectDescriptor,
  IBasicBuilding
    {
        public bool dumpWhenFull;
        public bool alwaysUse;
        public bool canSanitizeStorage;
        private WorkableReactable reactable;
        private MeterController cleanMeter;
        private MeterController dirtyMeter;
        public Meter.Offset cleanMeterOffset;
        public Meter.Offset dirtyMeterOffset;

        [MyCmpGet]
        DirectionControl directionControl;
        [MyCmpGet]
        Storage storage;

        [Serialize]
        public int maxPossiblyRemoved;
        private static readonly EventSystem.IntraObjectHandler<PillDispenser> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<PillDispenser>((component, data) => component.OnStorageChange(data));

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.gameObject.FindOrAddComponent<Workable>();
        }

        private void RefreshMeters()
        {
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.smi.StartSM();
            this.cleanMeter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_clean_target", "meter_clean", this.cleanMeterOffset, Grid.SceneLayer.NoLayer, new string[1]
            {
                "meter_clean_target"
            });
            this.dirtyMeter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_dirty_target", "meter_dirty", this.dirtyMeterOffset, Grid.SceneLayer.NoLayer, new string[1]
            {
                "meter_dirty_target"
            });
            this.RefreshMeters();
            //Components.PillDispensers.Add(this);
            Components.BasicBuildings.Add(this);
            this.Subscribe(-1697596308, PillDispenser.OnStorageChangeDelegate);
            directionControl.onDirectionChanged += new Action<WorkableReactable.AllowedDirection>(this.OnDirectionChanged);
            this.OnDirectionChanged(directionControl.allowedDirection);
        }

        public override void OnCleanUp()
        {
            Components.BasicBuildings.Remove(this);
            //Components.PillDispensers.Remove(this);
            base.OnCleanUp();
        }

        private void OnDirectionChanged(
          WorkableReactable.AllowedDirection allowed_direction)
        {
            if (this.reactable == null)
                return;
            this.reactable.allowedDirection = allowed_direction;
        }

        public List<Descriptor> RequirementDescriptors() => new List<Descriptor>()
        {
            //new Descriptor(string.Format((string) global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) ElementLoader.FindElementByHash(this.consumedElement).name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) ElementLoader.FindElementByHash(this.consumedElement).name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), Descriptor.DescriptorType.Requirement)
        };

        public List<Descriptor> EffectDescriptors()
        {
            List<Descriptor> descriptorList = new List<Descriptor>();
            //if (this.outputElement != SimHashes.Vacuum)
            //    descriptorList.Add(new Descriptor(string.Format((string)UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, (object)ElementLoader.FindElementByHash(this.outputElement).name, (object)GameUtil.GetFormattedMass(this.massConsumedPerUse)), string.Format((string)UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, (object)ElementLoader.FindElementByHash(this.outputElement).name, (object)GameUtil.GetFormattedMass(this.massConsumedPerUse))));
            //descriptorList.Add(new Descriptor(string.Format((string)UI.BUILDINGEFFECTS.DISEASECONSUMEDPERUSE, (object)GameUtil.GetFormattedDiseaseAmount(this.diseaseRemovalCount)), string.Format((string)UI.BUILDINGEFFECTS.TOOLTIPS.DISEASECONSUMEDPERUSE, (object)GameUtil.GetFormattedDiseaseAmount(this.diseaseRemovalCount))));
            return descriptorList;
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            List<Descriptor> descriptors = new List<Descriptor>();
            descriptors.AddRange(this.RequirementDescriptors());
            descriptors.AddRange(this.EffectDescriptors());
            return descriptors;
        }

        private void OnStorageChange(object data)
        {
            this.RefreshMeters();
        }

        private class PopPillReactable : WorkableReactable
        {
            public PopPillReactable(
              Workable workable,
              ChoreType chore_type,
              AllowedDirection allowed_direction = WorkableReactable.AllowedDirection.Any) : base(workable, (HashedString)"PopPill", chore_type, allowed_direction)
            {
            }

            public override bool InternalCanBegin(
              GameObject new_reactor,
              Navigator.ActiveTransition transition)
            {
                if (base.InternalCanBegin(new_reactor, transition))
                {
                    PillDispenser dispenser = this.workable.GetComponent<PillDispenser>();
                    if (!dispenser.smi.IsReady())
                        return false;
                    if (dispenser.alwaysUse)
                        return true;
                    PrimaryElement component2 = new_reactor.GetComponent<PrimaryElement>();
                    if (component2 != null)
                        return component2.DiseaseIdx != byte.MaxValue;
                }
                return false;
            }
        }

        public class SMInstance :
          GameStateMachine<States, SMInstance, PillDispenser, object>.GameInstance
        {
            public SMInstance(PillDispenser master)
              : base(master)
            {
            }

            private bool HasSufficientMass()
            {
                bool flag = false;
                var pillItem = smi.master.storage.FindFirst(GameTags.Medicine);
                if(pillItem==null)
                    return false;
                var primaryElement = pillItem.GetComponent<PrimaryElement>(); 

                if (primaryElement != null && primaryElement.Units >= 1)
                {
                    return true;
                }
                return false;
            }

            public bool IsReady() => this.HasSufficientMass();

        }

        public class States :
          GameStateMachine<States, SMInstance, PillDispenser>
        {
            public State notready;
            public ReadyStates ready;
            public State notoperational;
            public State full;
            public State empty;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = notready;
                this.notoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.notready);
                this.notready.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, ready, smi => smi.IsReady()).TagTransition(GameTags.Operational, this.notoperational, true);
                this.ready.DefaultState(this.ready.free)
                    .ToggleReactable((Func<SMInstance, Reactable>)(smi => smi.master.reactable = 
                    new PopPillReactable((Workable)smi.master.GetComponent<PopPillWorkable>(), Db.Get().ChoreTypes.TakeMedicine, smi.master.GetComponent<DirectionControl>().allowedDirection)))
                    .TagTransition(GameTags.Operational, this.notoperational, true);
                this.ready.free.PlayAnim("on").WorkableStartTransition(smi => smi.GetComponent<PopPillWorkable>(), this.ready.occupied);
                this.ready.occupied.PlayAnim("working_pre")
                    .QueueAnim("working_loop", true)
                    .WorkableStopTransition(smi => smi.GetComponent<PopPillWorkable>(), this.notready);
            }

            public class ReadyStates :
              State
            {
                public State free;
                public State occupied;
            }
        }

        [AddComponentMenu("KMonoBehaviour/Workable/Work")]
        public class PopPillWorkable : Workable, IGameObjectEffectDescriptor
        {
            MedicinalPill currentPill;
            public override void OnPrefabInit()
            {
                base.OnPrefabInit();
                this.resetProgressOnStop = true;
                this.shouldTransferDiseaseWithWorker = false;
            }
            public override void OnSpawn()
            {
                base.OnSpawn();
                showProgressBar = false;
                synchronizeAnims = false;
                SetWorkTime(10f);
            }
            public override void OnStartWork(Worker worker)
            {
                base.OnStartWork(worker); 
                var medicine = this.GetComponent<Storage>().FindFirst(GameTags.Medicine);
                if (medicine.TryGetComponent<MedicinalPill>(out var pill))
                {
                    
                    currentPill = pill;
                }
            }
            
            public override bool OnWorkTick(Worker worker, float dt)
            {
                return base.OnWorkTick(worker, dt);
            }

            public override void OnCompleteWork(Worker worker)
            {
                if(currentPill!=null)
                {
                    if (!string.IsNullOrEmpty(currentPill.info.effect))
                    {
                        Effects component = worker.GetComponent<Effects>();
                        EffectInstance effectInstance = component.Get(currentPill.info.effect);
                        if (effectInstance != null)
                        {
                            effectInstance.timeRemaining = effectInstance.effect.duration;
                        }
                        else
                        {
                            component.Add(currentPill.info.effect, should_save: true);
                        }
                    }

                    Sicknesses sicknesses = worker.GetSicknesses();
                    foreach (string curedSickness in currentPill.info.curedSicknesses)
                    {
                        SicknessInstance sicknessInstance = sicknesses.Get(curedSickness);
                        if (sicknessInstance != null)
                        {
                            Game.Instance.savedInfo.curedDisease = true;
                            sicknessInstance.Cure();
                        }
                    }
                    currentPill.TryGetComponent<Pickupable>(out var pickupable);
                    pickupable.Take(1);
                        
                }
                base.OnCompleteWork(worker);
            }
        }
    }

}
