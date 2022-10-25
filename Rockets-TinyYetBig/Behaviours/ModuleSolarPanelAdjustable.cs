using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ModuleSolarPanelAdjustable : Generator
    {
        private MeterController meter;
        private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
        private ModuleSolarPanelAdjustable.StatesInstance smi;
        private Guid statusHandle;
        [Serialize]
        public float Wattage = 140f;
        public CellOffset[] solarCellOffsets = new CellOffset[5]
        {
            new CellOffset(-2, 0),
            new CellOffset(-1, 0),
            new CellOffset(0, 0),
            new CellOffset(1, 0),
            new CellOffset(2, 0)
        };
        private static readonly EventSystem.IntraObjectHandler<ModuleSolarPanelAdjustable> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ModuleSolarPanelAdjustable>((System.Action<ModuleSolarPanelAdjustable, object>)((component, data) => component.OnActiveChanged(data)));

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.IsVirtual = true;
        }

        protected override void OnSpawn()
        {
            this.VirtualCircuitKey = (object)this.GetComponent<RocketModuleCluster>().CraftInterface;
            base.OnSpawn();
            this.Subscribe<ModuleSolarPanelAdjustable>(824508782, ModuleSolarPanelAdjustable.OnActiveChangedDelegate);
            this.smi = new ModuleSolarPanelAdjustable.StatesInstance(this);
            this.smi.StartSM();
            this.accumulator = Game.Instance.accumulators.Add("Element", (KMonoBehaviour)this);
            BuildingDef def = this.GetComponent<BuildingComplete>().Def;
            Grid.PosToCell((KMonoBehaviour)this);
            this.meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]
            {
                "meter_target",
                "meter_fill",
                "meter_frame",
                "meter_OL"
            });
            this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
        }

        protected override void OnCleanUp()
        {
            this.smi.StopSM("cleanup");
            Game.Instance.accumulators.Remove(this.accumulator);
            base.OnCleanUp();
        }

        protected void OnActiveChanged(object data)
        {
            StatusItem status_item = ((Operational)data).IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline;
            this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, (object)this);
        }

        private void UpdateStatusItem()
        {
            this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage);
            if (this.statusHandle == Guid.Empty)
            {
                this.statusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.ModuleSolarPanelWattage, (object)this);
            }
            else
            {
                if (!(this.statusHandle != Guid.Empty))
                    return;
                this.GetComponent<KSelectable>().ReplaceStatusItem(this.statusHandle, Db.Get().BuildingStatusItems.ModuleSolarPanelWattage, (object)this);
            }
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            int circuitId = (int)this.CircuitID;
            this.operational.SetFlag(Generator.wireConnectedFlag, true);
            this.operational.SetFlag(Generator.generatorConnectedFlag, true);
            if (!this.operational.IsOperational)
                return;
            float num1 = 0.0f;
            if (Grid.IsValidCell(Grid.PosToCell((KMonoBehaviour)this)) && (int)Grid.WorldIdx[Grid.PosToCell((KMonoBehaviour)this)] != (int)ClusterManager.INVALID_WORLD_IDX)
            {
                foreach (CellOffset solarCellOffset in this.solarCellOffsets)
                {
                    int num2 = Grid.LightIntensity[Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), solarCellOffset)];
                    num1 += (float)num2 * 0.00053f;
                }
            }
            else
                num1 = Wattage;
            float num3 = Mathf.Clamp(num1, 0.0f, Wattage);
            this.operational.SetActive((double)num3 > 0.0);
            Game.Instance.accumulators.Accumulate(this.accumulator, num3 * dt);
            if ((double)num3 > 0.0)
                this.GenerateJoules(Mathf.Max(num3 * dt, 1f * dt));
            this.meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(this.accumulator) / Wattage);
            this.UpdateStatusItem();
        }

        public float CurrentWattage => Game.Instance.accumulators.GetAverageRate(this.accumulator);

        public class StatesInstance :
          GameStateMachine<ModuleSolarPanelAdjustable.States, ModuleSolarPanelAdjustable.StatesInstance, ModuleSolarPanelAdjustable, object>.GameInstance
        {
            public StatesInstance(ModuleSolarPanelAdjustable master)
              : base(master)
            {
            }
        }

        public class States :
          GameStateMachine<ModuleSolarPanelAdjustable.States, ModuleSolarPanelAdjustable.StatesInstance, ModuleSolarPanelAdjustable>
        {
            public GameStateMachine<ModuleSolarPanelAdjustable.States, ModuleSolarPanelAdjustable.StatesInstance, ModuleSolarPanelAdjustable, object>.State idle;
            public GameStateMachine<ModuleSolarPanelAdjustable.States, ModuleSolarPanelAdjustable.StatesInstance, ModuleSolarPanelAdjustable, object>.State launch;

            public override void InitializeStates(out StateMachine.BaseState default_state)
            {
                default_state = (StateMachine.BaseState)this.idle;
                this.idle.EventTransition(GameHashes.DoLaunchRocket, this.launch).DoNothing();
                this.launch.EventTransition(GameHashes.RocketLanded, this.idle);
            }
        }
    }
}
