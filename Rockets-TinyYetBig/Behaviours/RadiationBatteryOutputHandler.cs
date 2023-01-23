using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    class RadiationBatteryOutputHandler : KMonoBehaviour,
        IHighEnergyParticleDirection,
        ISim200ms, ISaveLoadable,
        //IUserControlledCapacity, 
        ISingleSliderControl
    {
        [MyCmpReq]
        private KSelectable selectable;
        [Serialize]
        private EightDirection _direction;
        private MeterController directionController;

        private static readonly EventSystem.IntraObjectHandler<RadiationBatteryOutputHandler> OnStorageChangedDelegate
            = new EventSystem.IntraObjectHandler<RadiationBatteryOutputHandler>((component, data) => component.OnStorageChange(data));

        [MyCmpReq]
        public HighEnergyParticleStorage hepStorage;
        private MeterController m_meter;



        public bool AllowSpawnParticles => hasLogicWire && isLogicActive;
        private bool hasLogicWire;
        private bool isLogicActive;
        private float launchTimer = 0;
        private readonly float minLaunchInterval = 1f;
        public void Sim200ms(float dt)
        {
            bool hasSky = HasSkyVisibility();
            if (!hasSky)
                DoConsumeParticlesWhileDisabled(dt);
            else
                m_skipFirstUpdate = 10;

            UpdateDecayStatusItem(hasSky);
            launchTimer += dt;
            if (launchTimer < (double)minLaunchInterval || !AllowSpawnParticles || (double)hepStorage.Particles < particleThreshold)
                return;
            launchTimer = 0.0f;
            Fire();
        }


        private Guid statusHandle = Guid.Empty;
        public void UpdateDecayStatusItem(bool decaying)
        {
            if (!decaying)
            {
                if ((double)this.hepStorage.Particles > 0.0)
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

        public bool HasSkyVisibility()
        {
            int CellCenter = Grid.PosToCell(this);
            if (!Grid.IsValidCell(CellCenter))
            {
                m_skipFirstUpdate = 10;
                return true;
            }
            bool cellsClear = Grid.ExposedToSunlight[CellCenter-2] >= 1 && Grid.ExposedToSunlight[CellCenter+2] >= 1;
            return cellsClear;
        }

        private int m_skipFirstUpdate = 10;
        public void DoConsumeParticlesWhileDisabled(float dt)
        {
            if (this.m_skipFirstUpdate>0)
            {
                this.m_skipFirstUpdate--;
            }
            else
            {
                double num = (double)this.hepStorage.ConsumeAndGet(dt * 0.5f);
                OnStorageChange(null);
            }
        }

        public void RadboltDecay()
        {
            if(HasSkyVisibility()) return;

        }

        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (!(logicValueChanged.portID == HEPBattery.FIRE_PORT_ID))
                return;
            isLogicActive = logicValueChanged.newValue > 0;
            hasLogicWire = GetNetwork() != null;
        }
        private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(GetComponent<LogicPorts>().GetPortCell(HEPBattery.FIRE_PORT_ID));

        public int GetCircularHEPOutputCell()
        {
            int x = 0, y = 0;
            if (Direction.ToString().Contains("Down"))
                y -= 1;
            else if (Direction.ToString().Contains("Up"))
                y += 1;
            if (Direction.ToString().Contains("Right"))
                x += 1;
            else if (Direction.ToString().Contains("Left"))
                x -= 1;
            var build = GetComponent<Building>();

            var offset = build.GetHighEnergyParticleOutputOffset();
            offset.x += x;
            offset.y += y;

            int cell = Grid.OffsetCell(GetComponent<Building>().GetCell(), offset);
            return cell;
        }

        public void Fire()
        {
            int particleOutputCell = GetCircularHEPOutputCell();
            GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"HighEnergyParticle"), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
            gameObject.SetActive(true);
            if (!(gameObject != null))
                return;
            HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
            component.payload = hepStorage.ConsumeAndGet(particleThreshold);
            component.SetDirection(Direction);
        }


        //public LocString CapacityUnits => UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

        public IStorage Storage => hepStorage;
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

        #region CapacityChange

        public float physicalFuelCapacity;
        public float UserMaxCapacity
        {
            get => hepStorage.capacity;
            set
            {
                hepStorage.capacity = value;
                Trigger((int)GameHashes.ParticleStorageCapacityChanged, this);
            }
        }
        public float MinCapacity => 0.0f;
        public float MaxCapacity => physicalFuelCapacity;

        public float AmountStored => hepStorage.Particles;

        public bool WholeValues => false;
        #endregion

        public override void OnSpawn()
        {
            base.OnSpawn();
            //if (infoStatusItem_Logic == null)
            //{
            //    infoStatusItem_Logic = new StatusItem("HEPRedirectorLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            //    infoStatusItem_Logic.resolveStringCallback = new Func<string, object, string>(ResolveInfoStatusItem);
            //    infoStatusItem_Logic.resolveTooltipCallback = new Func<string, object, string>(ResolveInfoStatusItemTooltip);
            //}
            //this.selectable.AddStatusItem(infoStatusItem_Logic, (object)this);



            m_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            m_meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
            directionController = new MeterController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            directionController.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;

            Direction = Direction;
            OnStorageChange(null);
            //this.Subscribe<RadiationBatteryOutputHandler>((int)GameHashes.ParticleStorageCapacityChanged, OnStorageChangedDelegate);
            Subscribe((int)GameHashes.OnParticleStorageChanged, OnStorageChangedDelegate);
            Subscribe((int)GameHashes.LogicEvent, new Action<object>(OnLogicValueChanged));

        }


        private void OnStorageChange(object data) => m_meter.SetPositionPercent(hepStorage.Particles / Mathf.Max(1f, hepStorage.capacity));
        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        //private bool OnParticleCaptureAllowed(HighEnergyParticle particle) => true;

        #region SidescreenSliderForCapacityThrowout

        [Serialize]
        public float particleThreshold = 50f;

        public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TITLE";

        public string SliderUnits => (string)UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;


        public int SliderDecimalPlaces(int index) => 0;

        public float GetSliderMin(int index) => 25;

        public float GetSliderMax(int index) => 250;

        public float GetSliderValue(int index) => particleThreshold;

        public void SetSliderValue(float value, int index) => particleThreshold = value;

        public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP";

        string ISliderControl.GetSliderTooltip() => string.Format((string)Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP"), particleThreshold);

        #endregion
    }
}
