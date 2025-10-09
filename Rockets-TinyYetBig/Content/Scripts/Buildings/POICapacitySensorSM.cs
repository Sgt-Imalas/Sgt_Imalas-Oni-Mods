using KSerialization;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static KAnim.Build;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	internal class POICapacitySensorSM : StateMachineComponent<POICapacitySensorSM.StatesInstance>, ISaveLoadable, ISim200ms, IThresholdSwitch
	//, IGameObjectEffectDescriptor
	{
		[MyCmpGet]
		Building building;
		[MyCmpGet]
		ClusterDestinationSelector locationSelector;
		[MyCmpGet]
		private Operational operational;
		[MyCmpGet]
		SymbolOverrideController SymbolController;
		[MyCmpGet]
		KBatchedAnimController animController;
		[MyCmpGet]
		LogicPorts logicPorts;


		public override void OnSpawn()
		{
			base.OnSpawn();
			this.smi.StartSM();
			this.operational.SetFlag(LogicBroadcaster.spaceVisible, this.HasLineOfSight());
			this.Subscribe((int)GameHashes.ClusterDestinationChanged, (data) => UpdateEntity(data));

			UpdateEntity(null);
			UpdateLogicState(true);
		}
		public bool HasLineOfSight()
		{
			Extents extents = building.GetExtents();
			int x1 = extents.x;
			int num = extents.x + extents.width - 1;
			for (int x2 = x1; x2 <= num; ++x2)
			{
				int cell = Grid.XYToCell(x2, extents.y);
				if ((double)Grid.ExposedToSunlight[cell] == (byte)0)
					return false;
			}
			return true;
		}

		bool ArtifactOnly = false;
		ArtifactPOIStates.Instance artifactpoistatus = null;
		HarvestablePOIStates.Instance harvestpoistatus = null;

		//bool LastArtifactState = false;
		//bool LastThresholdState = false;
		//bool lastWasNonOperational = false;

		void UpdateLogicState(bool force = false)
		{
			if (!operational.IsOperational)
			{
				logicPorts.SendSignal(POICapacitySensorConfig.PORT_ID_ARTIFACT, 0);
				logicPorts.SendSignal(POICapacitySensorConfig.PORT_ID_MASS_THRESHOLD, 0);
				UpdateVisualState(false, force);
				return;
			}

			bool artifactIsAvailable = artifactpoistatus != null ? artifactpoistatus.CanHarvestArtifact() : false;
			bool aboveMassThreshold = harvestpoistatus != null ?
				activateAboveThreshold
					? harvestpoistatus.poiCapacity >= threshold
					: harvestpoistatus.poiCapacity < threshold
				: false;

			//if (LastArtifactState != artifactIsAvailable || force)
			//{
			//	LastArtifactState = artifactIsAvailable;
				logicPorts.SendSignal(POICapacitySensorConfig.PORT_ID_ARTIFACT, artifactIsAvailable ? 1 : 0);
			//}
			//if (LastThresholdState != aboveMassThreshold || force)
			//{
				//LastThresholdState = aboveMassThreshold;
				logicPorts.SendSignal(POICapacitySensorConfig.PORT_ID_MASS_THRESHOLD, aboveMassThreshold ? 1 : 0);
			//}

			bool ShouldBeGreen = (artifactIsAvailable && ArtifactOnly || aboveMassThreshold);

			UpdateVisualState(ShouldBeGreen, force);
		}
		bool lastVisualState = false;

		void UpdateVisualState(bool newState, bool force = false)
		{
			if (lastVisualState == newState && !force) return;

			if (!force)
				lastVisualState = newState;


			Color color = newState ? AccessibilityUtils.LogicGood : AccessibilityUtils.LogicBad;
			animController.SetSymbolTint("body_active", color);
		}

		void UpdateEntity(object data)
		{
			var entity = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(locationSelector.GetDestination(), EntityLayer.POI);
			Symbol symbol = null;
			if (entity != null)
			{
				var artifactcmp = entity.GetSMI<ArtifactPOIStates.Instance>();
				var harvestablecmp = entity.GetSMI<HarvestablePOIStates.Instance>();

				if (harvestablecmp != null)
				{
					rangeMax = harvestablecmp.configuration.GetMaxCapacity();
				}
				var newSprite = Def.GetUISpriteFromMultiObjectAnim(entity.AnimConfigs.First().animFile, entity.AnimConfigs.First().initialAnim);

				if (entity.TryGetComponent<ArtifactPOIClusterGridEntity>(out _))
				{
					ArtifactOnly = true;
				}
				else
					ArtifactOnly = false;

				artifactpoistatus = artifactcmp;
				harvestpoistatus = harvestablecmp;
				symbol = UIUtils.GetSymbolFromMultiObjectAnim(entity.AnimConfigs.First().animFile, entity.AnimConfigs.First().initialAnim);

			}
			else
			{
				artifactpoistatus = null;
				harvestpoistatus = null;
				rangeMax = 1;
			}

			if (symbol != null)
			{
				SgtLogger.l("replacing image");
				SymbolController.RemoveSymbolOverride((HashedString)"ToReplace");
				SymbolController.AddSymbolOverride((HashedString)"ToReplace", symbol);
			}
			else
			{
				SgtLogger.l(message: "undoing replacement image");
				SymbolController.RemoveSymbolOverride((HashedString)"ToReplace");
			}
			UpdateLogicState();
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		public void Sim200ms(float dt)
		{
			operational.SetFlag(LogicBroadcaster.spaceVisible, HasLineOfSight());
		}

		#region sidescreen
		public float Threshold
		{
			get => this.threshold;
			set => this.threshold = value;
		}
		public bool ActivateAboveThreshold
		{
			get => this.activateAboveThreshold;
			set => this.activateAboveThreshold = value;
		}

		public float CurrentValue => harvestpoistatus != null ? harvestpoistatus.poiCapacity : 0;

		private float rangeMin = 0f;
		private float rangeMax = 1f;

		[Serialize]
		private float threshold;
		[Serialize]
		private bool activateAboveThreshold = true;
		public float RangeMin => this.rangeMin;

		public float RangeMax => this.rangeMax;

		public LocString Title => STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.SIDESCREENTITLE;

		public LocString ThresholdValueName => STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.REMAININGMASS;

		public string AboveToolTip => STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.REMAININGMASS_TOOLTIP_ABOVE;

		public string BelowToolTip => STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.REMAININGMASS_TOOLTIP_BELOW;

		public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

		public int IncrementScale => 1;

		public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(this.RangeMax);

		public float GetRangeMinInputField() => RangeMin;

		public float GetRangeMaxInputField() => RangeMax;

		public LocString ThresholdValueUnits() => GameUtil.GetCurrentMassUnit();
		public string Format(float value, bool units) => GameUtil.GetFormattedMass(value, massFormat: GameUtil.MetricMassFormat.Kilogram, includeSuffix: units);
		public float ProcessedSliderValue(float input) => input;
		public float ProcessedInputValue(float input) => input;

		#endregion
		#region StateMachine

		public class StatesInstance : GameStateMachine<States, StatesInstance, POICapacitySensorSM>.GameInstance
		{
			//public MinionAssignablesProxy minionProxy;
			public StatesInstance(POICapacitySensorSM master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<POICapacitySensorSM.States, POICapacitySensorSM.StatesInstance, POICapacitySensorSM, object>
		{
			public class OnStates : State
			{
				public State noPoiSelected;
				public State poiSelected;
			}
			public class OffStates : State
			{
				public State NormalOff;
				public State NoLOS;
				public State from_on;
			}

			public OffStates offStates;
			public OnStates onStates;

			public override void InitializeStates(out BaseState defaultState)
			{

				defaultState = offStates;

				offStates
					.Enter((smi) => smi.master.UpdateLogicState())
					.Exit((smi) => smi.master.UpdateLogicState())
				.DefaultState(this.offStates.NormalOff)
				.TagTransition(GameTags.Operational, this.onStates.noPoiSelected)
				.PlayAnim("off")
				;

				offStates.NormalOff
				.Transition(offStates.NoLOS, (smi => !smi.master.operational.GetFlag(LogicBroadcaster.spaceVisible)))
				;

				offStates.NoLOS
				.Transition(offStates.NormalOff, (smi => smi.master.operational.GetFlag(LogicBroadcaster.spaceVisible)))
				.ToggleStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight)
				;

				onStates
					.Enter((smi) => smi.master.UpdateLogicState(true))
					.PlayAnim("on_pre")
					.QueueAnim("on", true)
					.DefaultState(this.onStates.noPoiSelected)
					.TagTransition(GameTags.Operational, this.offStates.from_on, true);

				onStates.noPoiSelected
					.UpdateTransition(onStates.poiSelected, (smi, dt) => { return smi.master.artifactpoistatus != null; })
					;

				onStates.poiSelected
					.UpdateTransition(onStates.noPoiSelected, (smi, dt) => { return smi.master.artifactpoistatus == null; })
					.Update((smi, dt) =>
					{
						smi.master.UpdateLogicState();
					}, UpdateRate.SIM_1000ms);
				offStates.from_on
					.PlayAnim("on_pst")
					.OnAnimQueueComplete(this.offStates);
			}
		}
		#endregion
	}
}


