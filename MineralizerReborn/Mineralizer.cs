using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineralizerReborn
{
    internal class Mineralizer : StateMachineComponent<Mineralizer.SMInstance>
    {
        public Storage saltStorage;
        public MeterController salt_meter { get; private set; }
        public Mineralizer() { }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            smi.StartSM();
            salt_meter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            this.Subscribe((int)GameHashes.OnStorageChange, OnStorageChange);
            OnStorageChange(null);
        }

        private void OnStorageChange(object data)
        {
            var salt = saltStorage.GetMassAvailable(SimHashes.Salt);
            this.salt_meter.SetPositionPercent(salt / (saltStorage.capacityKg-20f));
        } 

        public class SMInstance : GameStateMachine<States, SMInstance, Mineralizer, object>.GameInstance
        {
            public SMInstance(Mineralizer master) : base(master) { }
        }

        public class States : GameStateMachine<States, SMInstance, Mineralizer>
        {
            public State Working;
            public State StartWorking;
            public State StopWorking;
            public State Operational;
            public State NotOperational;

            public override void InitializeStates(out BaseState defaultState)
            {
                defaultState = NotOperational;

                NotOperational
                    .QueueAnim("off")
                    .EventTransition(GameHashes.OperationalChanged, Operational, smi => smi.GetComponent<Operational>().IsOperational);

                Operational
                    .QueueAnim("on")
                    .EventTransition(GameHashes.OperationalChanged, NotOperational, smi => !smi.GetComponent<Operational>().IsOperational)
                    .EventTransition(GameHashes.OnStorageChange, StartWorking, smi => smi.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());

                StartWorking
                    .PlayAnim("working_pre")
                    .Enter(smi => smi.GetComponent<LoopingSounds>().PlayEvent(new GameSoundEvents.Event("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_start")))
                    .OnAnimQueueComplete(Working);

                Working
                    .Enter(smi => smi.GetComponent<Operational>().SetActive(true, false))
                    .Enter(smi => smi.GetComponent<LoopingSounds>().StartSound("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_lP"))
                    .QueueAnim("working_loop", true)
                    .EventTransition(GameHashes.OnStorageChange, StopWorking, smi => !smi.GetComponent<ElementConverter>().CanConvertAtAll())
                    .EventTransition(GameHashes.OperationalChanged, StopWorking, smi => !smi.GetComponent<Operational>().IsOperational)
                    .Exit(smi => smi.GetComponent<Operational>().SetActive(false, false))
                    .Exit(smi => smi.GetComponent<LoopingSounds>().StopSound("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_lP"));

                StopWorking
                    .PlayAnim("working_pst")
                    .Enter(smi => smi.GetComponent<LoopingSounds>().PlayEvent(new GameSoundEvents.Event("event:/Buildings/BuildCategories/05Utilities/LiquidConditioner/LiquidConditioner_end")))
                    .OnAnimQueueComplete(Operational);
            }
        }
    }
}