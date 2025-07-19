using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class RotatableSmallSolarPanel : Generator
	{
		// from PartialLightBlocking
		private const byte PartialLightBlockingProperties = 48;
		[MyCmpReq]
		Rotatable rotatable;

		private MeterController meter;
		private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
		private RotatableSmallSolarPanel.StatesInstance smi;
		private Guid statusHandle;
		[Serialize]
		[SerializeField]
		public float MaxWattage = 80f;
		public CellOffset[] solarCellOffsets = [new(0,2), new(0,3), new (0,4)];
		private static readonly EventSystem.IntraObjectHandler<RotatableSmallSolarPanel> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<RotatableSmallSolarPanel>((component, data) => component.OnActiveChanged(data));

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Subscribe(824508782, OnActiveChangedDelegate);
			this.smi = new StatesInstance(this);
			this.smi.StartSM();
			this.accumulator = Game.Instance.accumulators.Add("Element", this);
			this.meter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]
			{
				"meter_target",
				"meter_fill",
				"meter_frame",
				"meter_OL"
			});
			SgtLogger.l("onspawn");
			SetLightBlockIfHorizontal(true);
		}
		void SetLightBlockIfHorizontal(bool blockLight)
		{
			if (!rotatable.IsRotated)
				return;

			var orientation = rotatable.GetOrientation();
			if ( orientation == Orientation.R90 || orientation == Orientation.R270)
			{
				int ownCell = Grid.PosToCell(this);
				foreach (var cellOffset in solarCellOffsets)
				{
					 var solarCell = Grid.OffsetCell(ownCell, building.GetRotatedOffset(cellOffset));
					if (Grid.IsValidCell(solarCell))
					{
						if(blockLight)
							SimMessages.SetCellProperties(solarCell, PartialLightBlockingProperties);
						else
							SimMessages.ClearCellProperties(solarCell, PartialLightBlockingProperties);
					}
				}
			}
		}

		public override void OnCleanUp()
		{
			SetLightBlockIfHorizontal(false);
			this.smi.StopSM("cleanup");
			Game.Instance.accumulators.Remove(this.accumulator);
			base.OnCleanUp();
		}

		protected void OnActiveChanged(object data)
		{
			StatusItem status_item = ((Operational)data).IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline;
			this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, this);
		}

		private void UpdateStatusItem()
		{
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage);
			if (this.statusHandle == Guid.Empty)
			{
				this.statusHandle = this.selectable.AddStatusItem(StatusItemsDatabase.CG_RotatableSolarPanelWattage, this);
			}
			else
			{
				if (!(this.statusHandle != Guid.Empty))
					return;
				this.GetComponent<KSelectable>().ReplaceStatusItem(this.statusHandle,  StatusItemsDatabase.CG_RotatableSolarPanelWattage, this);
			}
		}

		public override void EnergySim200ms(float dt)
		{
			base.EnergySim200ms(dt);
			int circuitId = CircuitID;
			this.operational.SetFlag(Generator.wireConnectedFlag, true);
			this.operational.SetFlag(Generator.generatorConnectedFlag, true);
			if (!this.operational.IsOperational)
				return;
			float currentWattage = 0.0f;
			foreach (CellOffset solarCellOffset in this.solarCellOffsets)
			{
				CellOffset rotatedOffset = building.GetRotatedOffset(solarCellOffset);
				int luxAtCell = Grid.LightIntensity[Grid.OffsetCell(Grid.PosToCell(this), rotatedOffset)];
				currentWattage += luxAtCell * 0.00053f;
			}
			float clampedWattage = Mathf.Clamp(currentWattage, 0.0f, MaxWattage);
			this.operational.SetActive((double)clampedWattage > 0.0);
			Game.Instance.accumulators.Accumulate(this.accumulator, clampedWattage * dt);
			if ((double)clampedWattage > 0.0)
				this.GenerateJoules(Mathf.Max(clampedWattage * dt, 1f * dt));
			this.meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(this.accumulator) / MaxWattage);
			this.UpdateStatusItem();
		}

		public float CurrentWattage => Game.Instance.accumulators.GetAverageRate(this.accumulator);

		public class StatesInstance :
		  GameStateMachine<RotatableSmallSolarPanel.States, RotatableSmallSolarPanel.StatesInstance, RotatableSmallSolarPanel, object>.GameInstance
		{
			public StatesInstance(RotatableSmallSolarPanel master)
			  : base(master)
			{
			}
		}

		public class States :
		  GameStateMachine<RotatableSmallSolarPanel.States, RotatableSmallSolarPanel.StatesInstance, RotatableSmallSolarPanel>
		{
			public GameStateMachine<RotatableSmallSolarPanel.States, RotatableSmallSolarPanel.StatesInstance, RotatableSmallSolarPanel, object>.State idle;
			public GameStateMachine<RotatableSmallSolarPanel.States, RotatableSmallSolarPanel.StatesInstance, RotatableSmallSolarPanel, object>.State launch;

			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = idle;
				this.idle.DoNothing();
			}
		}
	}
}
