using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace LogicRoomMassSensor
{
	internal class RoomPressureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim4000ms
	{
		[SerializeField]
		[Serialize]
		private float threshold;
		[SerializeField]
		[Serialize]
		private bool activateAboveThreshold = true;
		private bool wasOn;
		public float rangeMin;
		public float rangeMax = 1f;

		[Serialize]
		private double elementMass = 0;

		private int sampleIdx;

		[MyCmpAdd]
		private CopyBuildingSettings copyBuildingSettings;

		[MyCmpGet]
		LogicPorts logicPorts;

		HashSet<int> RoomCells = new HashSet<int>();

		private static readonly EventSystem.IntraObjectHandler<RoomPressureSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<RoomPressureSensor>((System.Action<RoomPressureSensor, object>)((component, data) => component.OnCopySettings(data)));

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.Subscribe(-905833192, OnCopySettingsDelegate);
		}

		public void Sim4000ms(float dt)
		{
			OnRoomRebuild(null);
			elementMass = RecalculateTotalMass();

			if (this.activateAboveThreshold)
			{
				if ((elementMass <= this.threshold || IsSwitchedOn) && (elementMass > this.threshold || !IsSwitchedOn))
					return;
				this.Toggle();
			}
			else
			{
				if ((elementMass <= threshold || !IsSwitchedOn) && (elementMass > threshold || IsSwitchedOn))
					return;
				this.Toggle();
			}
		}

		private void OnCopySettings(object data)
		{
			RoomPressureSensor component = ((GameObject)data).GetComponent<RoomPressureSensor>();
			if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
				return;
			this.Threshold = component.Threshold;
			this.ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
		public override void OnCleanUp()
		{
			this.Unsubscribe(-905833192, OnCopySettingsDelegate);
			//this.Unsubscribe((int)GameHashes.UpdateRoom, OnRoomRebuild);
			base.OnCleanUp();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			this.OnToggle += new System.Action<bool>(this.OnSwitchToggled);
			this.UpdateLogicCircuit();
			this.UpdateVisualState(true);
			this.wasOn = this.switchedOn;
			//this.Subscribe((int)GameHashes.UpdateRoom, OnRoomRebuild);
			OnRoomRebuild(null);
			elementMass = RecalculateTotalMass();
		}
		public void OnRoomRebuild(object data)
		{

			RoomCells.Clear();
			int myCell = Grid.PosToCell(this);
			var visited = new HashSet<int>();

			var startElement = Grid.Element[myCell];

			GrabCellsRecursive(myCell, ref visited, startElement);

			sampleIdx = 0;
		}
		double RecalculateTotalMass()
		{
			double totalTileMass = 0;

			int myCell = Grid.PosToCell(this);

			foreach (var cell in RoomCells)
			{
				if (Grid.IsValidCell(cell))
				{
					totalTileMass += Grid.Mass[cell];
				}
			}
			return totalTileMass;
		}
		void GrabCellsRecursive(int source, ref HashSet<int> visitedCells, Element sourceElement)
		{
			if (visitedCells.Contains(source))
				return;

			visitedCells.Add(source);

			if (Grid.Element[source] != sourceElement || visitedCells.Count >= 10000)
				return;

			RoomCells.Add(source);


			int above = Grid.CellAbove(source);
			int below = Grid.CellBelow(source);
			int left = Grid.CellLeft(source);
			int right = Grid.CellRight(source);

			if (Grid.IsValidCell(above))
				GrabCellsRecursive(above, ref visitedCells, sourceElement);
			if (Grid.IsValidCell(below))
				GrabCellsRecursive(below, ref visitedCells, sourceElement);
			if (Grid.IsValidCell(left))
				GrabCellsRecursive(left, ref visitedCells, sourceElement);
			if (Grid.IsValidCell(right))
				GrabCellsRecursive(right, ref visitedCells, sourceElement);
		}

		private void OnSwitchToggled(bool toggled_on)
		{
			this.UpdateLogicCircuit();
			this.UpdateVisualState();
		}

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

		public float CurrentValue
		{
			get
			{
				return (float)elementMass;
			}
		}

		public float RangeMin => this.rangeMin;

		public float RangeMax => this.rangeMax;

		public float GetRangeMinInputField() => this.rangeMin;

		public float GetRangeMaxInputField() => this.rangeMax;

		public LocString ThresholdValueName => global::STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;

		public string AboveToolTip => global::STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;

		public string BelowToolTip => global::STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;

		public string Format(float value, bool units)
		{
			GameUtil.MetricMassFormat metricMassFormat = GameUtil.MetricMassFormat.Kilogram;
			return GameUtil.GetFormattedMass(value, massFormat: (metricMassFormat), includeSuffix: units);
		}

		public float ProcessedSliderValue(float input)
		{
			input = Mathf.Round(input);
			return input;
		}

		public float ProcessedInputValue(float input)
		{
			return input;
		}

		public LocString ThresholdValueUnits() => GameUtil.GetCurrentMassUnit(false);

		public LocString Title => global::STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;

		public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

		public int IncrementScale => 1;

		public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(this.RangeMax);

		private void UpdateLogicCircuit() => logicPorts.SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);

		private void UpdateVisualState(bool force = false)
		{
			if (!(this.wasOn != this.switchedOn | force))
				return;
			this.wasOn = this.switchedOn;
			KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
			component.Play((HashedString)(this.switchedOn ? "on_pre" : "on_pst"));
			component.Queue((HashedString)(this.switchedOn ? "on" : "off"));
		}

		public override void UpdateSwitchStatus()
		{
			StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive;
			this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
		}

	}

}
