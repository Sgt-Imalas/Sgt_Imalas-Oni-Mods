using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig
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
            = new EventSystem.IntraObjectHandler<RadiationBatteryOutputHandler>((System.Action<RadiationBatteryOutputHandler, object>)((component, data) => component.OnStorageChange(data)));

        [MyCmpReq]
        public HighEnergyParticleStorage hepStorage;
        private MeterController m_meter;



        public bool AllowSpawnParticles => this.hasLogicWire && this.isLogicActive;
        private bool hasLogicWire;
        private bool isLogicActive;
        private float launchTimer = 0;
        private readonly float minLaunchInterval = 1f;
        public void Sim200ms(float dt)
        {
            launchTimer += dt;
            if ((double)launchTimer < (double)minLaunchInterval || !AllowSpawnParticles || (double)hepStorage.Particles < (double)particleThreshold)
                return;
            launchTimer = 0.0f;
            this.Fire();
        }

        private void OnLogicValueChanged(object data)
        {
            LogicValueChanged logicValueChanged = (LogicValueChanged)data;
            if (!(logicValueChanged.portID == HEPBattery.FIRE_PORT_ID))
                return;
            this.isLogicActive = logicValueChanged.newValue > 0;
            this.hasLogicWire = this.GetNetwork() != null;
        }
        private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(this.GetComponent<LogicPorts>().GetPortCell(HEPBattery.FIRE_PORT_ID));

        public int GetCircularHEPOutputCell()
        {
            int x = 0, y = 0;
            if (this.Direction.ToString().Contains("Down"))
                y -= 1;
            else if (this.Direction.ToString().Contains("Up"))
                y += 1;
            if (this.Direction.ToString().Contains("Right"))
                x += 1;
            else if (this.Direction.ToString().Contains("Left"))
                x -= 1;
            var build = this.GetComponent<Building>();

            var offset = build.GetHighEnergyParticleOutputOffset();
            offset.x += x;
            offset.y += y;

            int cell = Grid.OffsetCell(this.GetComponent<Building>().GetCell(), offset);
            return cell;
        }

        public void Fire()
        {
            int particleOutputCell = this.GetCircularHEPOutputCell();
            GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"HighEnergyParticle"), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
            gameObject.SetActive(true);
            if (!((UnityEngine.Object)gameObject != (UnityEngine.Object)null))
                return;
            HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
            component.payload = hepStorage.ConsumeAndGet(particleThreshold);
            component.SetDirection(Direction);
        }


        public LocString CapacityUnits => UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

        public IStorage Storage => (IStorage)this.hepStorage;
        public EightDirection Direction
        {
            get => this._direction;
            set
            {
                this._direction = value;
                if (this.directionController == null)
                    return;
                this.directionController.SetPositionPercent( (45f*EightDirectionUtil.GetDirectionIndex(this._direction))/360f);
            }
        }

        #region CapacityChange

        public float physicalFuelCapacity;
        public float UserMaxCapacity
        {
            get => this.hepStorage.capacity;
            set
            {
                this.hepStorage.capacity = value;
                this.Trigger((int)GameHashes.ParticleStorageCapacityChanged, (object)this);
            }
        }
        public float MinCapacity => 0.0f;
        public float MaxCapacity => this.physicalFuelCapacity;

        public float AmountStored => this.hepStorage.Particles;

        public bool WholeValues => false;
        #endregion

        protected override void OnSpawn()
        {
            base.OnSpawn();
            //if (infoStatusItem_Logic == null)
            //{
            //    infoStatusItem_Logic = new StatusItem("HEPRedirectorLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
            //    infoStatusItem_Logic.resolveStringCallback = new Func<string, object, string>(ResolveInfoStatusItem);
            //    infoStatusItem_Logic.resolveTooltipCallback = new Func<string, object, string>(ResolveInfoStatusItemTooltip);
            //}
            //this.selectable.AddStatusItem(infoStatusItem_Logic, (object)this);



            this.m_meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            this.m_meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
            this.directionController = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
            this.directionController.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;

            Direction = Direction;
            this.OnStorageChange((object)null);
            //this.Subscribe<RadiationBatteryOutputHandler>((int)GameHashes.ParticleStorageCapacityChanged, OnStorageChangedDelegate);
            this.Subscribe<RadiationBatteryOutputHandler>((int)GameHashes.OnParticleStorageChanged, OnStorageChangedDelegate);
            this.Subscribe((int)GameHashes.LogicEvent, new System.Action<object>(this.OnLogicValueChanged));

        }


        private void OnStorageChange(object data) => this.m_meter.SetPositionPercent(this.hepStorage.Particles / Mathf.Max(1f, this.hepStorage.capacity));
        protected override void OnCleanUp()
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

        public float GetSliderMin(int index) => (float)25;

        public float GetSliderMax(int index) => (float)250;

        public float GetSliderValue(int index) => this.particleThreshold;

        public void SetSliderValue(float value, int index) => this.particleThreshold = value;

        public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP";

        string ISliderControl.GetSliderTooltip() => string.Format((string)Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP"), (object)this.particleThreshold);

        #endregion
    }
}
