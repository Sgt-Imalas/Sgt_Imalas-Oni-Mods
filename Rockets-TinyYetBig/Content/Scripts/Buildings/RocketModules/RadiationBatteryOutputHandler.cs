using KSerialization;
using STRINGS;
using System;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Behaviours
{
	class RadiationBatteryOutputHandler : KMonoBehaviour,
		IHighEnergyParticleDirection,
		ISim200ms, ISaveLoadable,
		//IUserControlledCapacity, 
		ISingleSliderControl
	{
		[MyCmpGet] RocketModuleCluster module;

		[MyCmpReq]
		private LogicPorts logicPorts;
		[MyCmpReq]
		private KSelectable selectable;
		[MyCmpReq]
		private Operational operational;
		[Serialize]
		private EightDirection _direction;
		private MeterController directionController;

		private static readonly EventSystem.IntraObjectHandler<RadiationBatteryOutputHandler> OnStorageChangedDelegate
			= new EventSystem.IntraObjectHandler<RadiationBatteryOutputHandler>((component, data) => component.OnStorageChange(data));

		[MyCmpReq]
		public HighEnergyParticleStorage hepStorage;
		[MyCmpReq]
		public KBatchedAnimController kbac;
		private MeterController m_meter;

		[MyCmpAdd]
		public CopyBuildingSettings copyBuildingSettings;

		public bool AllowSpawnParticles => hasLogicWire && isLogicActive && ModuleLanded();
		private bool hasLogicWire;
		private bool isLogicActive;
		private float launchTimer = 0;
		private readonly float minLaunchInterval = 1f;

		public void Sim200ms(float dt)
		{
			RefreshSkyCoverage();
			bool shouldDecay = ShouldDecay(dt);
			if (shouldDecay)
				DoConsumeParticlesWhileDisabled(dt);
			else
				m_skipFirstUpdate = 10;

			UpdateDecayStatusItem(shouldDecay);
			SpawnRadboltIfReady(dt);
		}

		void SpawnRadboltIfReady(float dt)
		{
			launchTimer += dt;
			if (launchTimer < minLaunchInterval || !AllowSpawnParticles || hepStorage.Particles < particleThreshold)
				return;
			launchTimer = 0;
			Fire();
		}


		private Guid statusHandle = Guid.Empty;
		public void UpdateDecayStatusItem(bool decaying)
		{
			if (decaying)
			{
				if ((double)this.hepStorage.Particles > 0.0)
				{
					if (!(this.statusHandle == Guid.Empty))
						return;
					this.statusHandle = selectable.AddStatusItem(Db.Get().BuildingStatusItems.LosingRadbolts);
				}
				else
				{
					if (!(this.statusHandle != Guid.Empty))
						return;
					selectable.RemoveStatusItem(this.statusHandle);
					this.statusHandle = Guid.Empty;
				}
			}
			else
			{
				if (!(this.statusHandle != Guid.Empty))
					return;
				selectable.RemoveStatusItem(this.statusHandle);
				this.statusHandle = Guid.Empty;
			}
		}

		void RefreshSkyCoverage()
		{
			//only consume power if no sky visibility
			bool solarPanelsHaveLight = HasSkyVisibility();
			kbac.SetSymbolVisiblity("solar_panel_glow", solarPanelsHaveLight);
			//using the active flag for the power consumer so it can use power leeching as backup
			operational.SetActive(!solarPanelsHaveLight);
		}

		public bool ModuleLanded() => module.CraftInterface.m_clustercraft.Status == Clustercraft.CraftStatus.Grounded;

		public bool ShouldDecay(float dt)
		{
			if (HasSkyVisibility() || !ModuleLanded())
				return false;

			return !operational.IsOperational;
		}

		public bool HasSkyVisibility()
		{
			int CellCenter = Grid.PosToCell(this);
			if (!Grid.IsValidCell(CellCenter))
			{
				m_skipFirstUpdate = 10;
				return true;
			}

			bool leftSolarPanelClear = Grid.IsValidCell(CellCenter - 2) && Grid.ExposedToSunlight[CellCenter - 2] >= 1;
			bool rightSolarPanelClear = Grid.IsValidCell(CellCenter + 2) && Grid.ExposedToSunlight[CellCenter + 2] >= 1;
			return leftSolarPanelClear && rightSolarPanelClear;
		}

		private int m_skipFirstUpdate = 10;
		public void DoConsumeParticlesWhileDisabled(float dt)
		{
			if (this.m_skipFirstUpdate > 0)
			{
				this.m_skipFirstUpdate--;
			}
			else
			{
				double num = (double)this.hepStorage.ConsumeAndGet(dt * 5f);
				OnStorageChange(null);
			}
		}

		private void OnLogicValueChanged(object data)
		{
			RefreshLogicValue();
		}
		void RefreshLogicValue()
		{
			if(!ModuleLanded())
			{
				hasLogicWire = false;
				isLogicActive = false;
				return;
			}
			hasLogicWire = GetNetwork() != null;
			isLogicActive = hasLogicWire ? logicPorts.GetInputValue(HEPBattery.FIRE_PORT_ID) > 0 : false;
		}


		private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(logicPorts.GetPortCell(HEPBattery.FIRE_PORT_ID));

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

			int cell = Grid.OffsetCell(this.NaturalBuildingCell(), offset);
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
				directionController.SetPositionPercent((EightDirectionUtil.GetDirectionIndex(_direction)) / 7f);
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
			//this.selectable.AddStatusItem(infoStatusItem_Logic, this);



			m_meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			m_meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
			directionController = new MeterController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			directionController.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;

			Direction = Direction;
			OnStorageChange(null);
			//this.Subscribe<RadiationBatteryOutputHandler>((int)GameHashes.ParticleStorageCapacityChanged, OnStorageChangedDelegate);
			Subscribe((int)GameHashes.OnParticleStorageChanged, OnStorageChangedDelegate);
			Subscribe((int)GameHashes.LogicEvent, OnLogicValueChanged);
			Subscribe((int)GameHashes.CopySettings, OnCopySettings);
			Subscribe((int)GameHashes.ClustercraftStateChanged, OnLogicValueChanged);
		}

		public void OnCopySettings(object data)
		{
			GameObject sauceGameObject = data as GameObject;
			if (sauceGameObject != null && sauceGameObject.TryGetComponent<RadiationBatteryOutputHandler>(out var addon))
			{
				this.Direction = addon.Direction;
				this.particleThreshold = addon.particleThreshold;
			}
		}

		private void OnStorageChange(object data) => m_meter.SetPositionPercent(hepStorage.Particles / Mathf.Max(1f, hepStorage.capacity));
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.OnParticleStorageChanged, OnStorageChangedDelegate);
			Unsubscribe((int)GameHashes.LogicEvent, new Action<object>(OnLogicValueChanged));
			Unsubscribe(-905833192, OnCopySettings);
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

		public string GetSliderTooltip(int index) => string.Format((string)Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP"), particleThreshold);
		public string GetSliderTooltip() => string.Format((string)Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP"), particleThreshold);

		#endregion
	}
}
