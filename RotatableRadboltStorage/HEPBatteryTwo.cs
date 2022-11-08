using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RotatableRadboltStorage
{
    internal class HEPBatteryTwo :
  GameStateMachine<HEPBatteryTwo, HEPBatteryTwo.Instance, IStateMachineTarget, HEPBatteryTwo.Def>
    {
        public static readonly HashedString FIRE_PORT_ID = (HashedString)"HEPBatteryTwoFire";
        public GameStateMachine<HEPBatteryTwo, HEPBatteryTwo.Instance, IStateMachineTarget, HEPBatteryTwo.Def>.State inoperational;
        public GameStateMachine<HEPBatteryTwo, HEPBatteryTwo.Instance, IStateMachineTarget, HEPBatteryTwo.Def>.State operational;

        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            default_state = (StateMachine.BaseState)this.inoperational;
            this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational).Update((System.Action<HEPBatteryTwo.Instance, float>)((smi, dt) =>
            {
                smi.DoConsumeParticlesWhileDisabled(dt);
                smi.UpdateDecayStatusItem(false);
            }));
            this.operational.Enter("SetActive(true)",
                (StateMachine<HEPBatteryTwo, HEPBatteryTwo.Instance, IStateMachineTarget, HEPBatteryTwo.Def>.State.Callback)
                (smi => smi.operational.SetActive(true)))
                    .Exit("SetActive(false)", (StateMachine<HEPBatteryTwo, HEPBatteryTwo.Instance, IStateMachineTarget, HEPBatteryTwo.Def>.State.Callback)
                    (smi => smi.operational.SetActive(false))).PlayAnim("on", KAnim.PlayMode.Loop)
                    .TagTransition(GameTags.Operational, this.inoperational, true)
                    .Update(new System.Action<HEPBatteryTwo.Instance, float>(this.LauncherUpdate));
        }

        public void LauncherUpdate(HEPBatteryTwo.Instance smi, float dt)
        {
            smi.UpdateDecayStatusItem(true);
            smi.UpdateMeter();
            smi.operational.SetActive((double)smi.particleStorage.Particles > 0.0);
            smi.launcherTimer += dt;
            if ((double)smi.launcherTimer < (double)smi.def.minLaunchInterval || !smi.AllowSpawnParticles || (double)smi.particleStorage.Particles < (double)smi.particleThreshold)
                return;
            smi.launcherTimer = 0.0f;
            this.Fire(smi);
        }

        public void Fire(HEPBatteryTwo.Instance smi)
        {
            int particleOutputCell = smi.GetComponent<Building>().GetHighEnergyParticleOutputCell();
            GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"HighEnergyParticle"), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
            gameObject.SetActive(true);
            if (!((UnityEngine.Object)gameObject != (UnityEngine.Object)null))
                return;
            HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
            component.payload = smi.particleStorage.ConsumeAndGet(smi.particleThreshold);
            component.SetDirection(smi.def.direction);
        }

        public class Def : StateMachine.BaseDef
        {
            public float particleDecayRate;
            public float minLaunchInterval;
            public float minSlider;
            public float maxSlider;
            public EightDirection direction;
        }

        public new class Instance :
          GameStateMachine<HEPBatteryTwo, HEPBatteryTwo.Instance, IStateMachineTarget, HEPBatteryTwo.Def>.GameInstance,
            IHighEnergyParticleDirection,
          ISingleSliderControl,
          ISliderControl
        {
            [MyCmpReq]
            public HighEnergyParticleStorage particleStorage;
            [MyCmpGet]
            public Operational operational;
            [Serialize]
            public float launcherTimer;
            [Serialize]
            public float particleThreshold = 50f;
            [Serialize]
            private EightDirection _direction;

            public bool ShowWorkingStatus;
            private bool m_skipFirstUpdate = true;
            private MeterController directionController;
            private MeterController meterController;
            private Guid statusHandle = Guid.Empty;
            private bool hasLogicWire;
            private bool isLogicActive;

            public Instance(IStateMachineTarget master, HEPBatteryTwo.Def def)
              : base(master, def)
            {
                this.Subscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));
                this.meterController = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
                directionController = new MeterController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
                Direction = Direction;
                this.UpdateMeter();
            }

            public void DoConsumeParticlesWhileDisabled(float dt)
            {
                if (this.m_skipFirstUpdate)
                {
                    this.m_skipFirstUpdate = false;
                }
                else
                {
                    double num = (double)this.particleStorage.ConsumeAndGet(dt * this.def.particleDecayRate);
                    this.UpdateMeter();
                }
            }

            public EightDirection Direction
            {
                get => _direction;
                set
                {
                    _direction = value;
                    if (directionController == null)
                        return;
                    directionController.SetPositionPercent(45f * EightDirectionUtil.GetDirectionIndex(_direction) / 360f);
                }
            }

            public void UpdateMeter(object data = null) => this.meterController.SetPositionPercent(this.particleStorage.Particles / this.particleStorage.Capacity());

            public void UpdateDecayStatusItem(bool hasPower)
            {
                if (!hasPower)
                {
                    if ((double)this.particleStorage.Particles > 0.0)
                    {
                        if (!(this.statusHandle == Guid.Empty))
                            return;
                        this.statusHandle = this.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.LosingRadbolts);
                    }
                    else
                    {
                        if (!(this.statusHandle != Guid.Empty))
                            return;
                        this.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle);
                        this.statusHandle = Guid.Empty;
                    }
                }
                else
                {
                    if (!(this.statusHandle != Guid.Empty))
                        return;
                    this.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle);
                    this.statusHandle = Guid.Empty;
                }
            }

            public bool AllowSpawnParticles => this.hasLogicWire && this.isLogicActive;

            public bool HasLogicWire => this.hasLogicWire;

            public bool IsLogicActive => this.isLogicActive;

            private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(this.GetComponent<LogicPorts>().GetPortCell(HEPBatteryTwo.FIRE_PORT_ID));

            private void OnLogicValueChanged(object data)
            {
                LogicValueChanged logicValueChanged = (LogicValueChanged)data;
                if (!(logicValueChanged.portID == HEPBatteryTwo.FIRE_PORT_ID))
                    return;
                this.isLogicActive = logicValueChanged.newValue > 0;
                this.hasLogicWire = this.GetNetwork() != null;
            }

            public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TITLE";

            public string SliderUnits => (string)global::STRINGS.UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;


            public int SliderDecimalPlaces(int index) => 0;

            public float GetSliderMin(int index) => this.def.minSlider;

            public float GetSliderMax(int index) => this.def.maxSlider;

            public float GetSliderValue(int index) => this.particleThreshold;

            public void SetSliderValue(float value, int index) => this.particleThreshold = value;

            public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP";

            string ISliderControl.GetSliderTooltip() => string.Format((string)Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP"), (object)this.particleThreshold);
        }
    }

}
