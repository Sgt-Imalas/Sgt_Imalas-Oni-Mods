using System;
using UnityEngine;
using static UL_UniversalLyzer.ModAssets;

namespace UL_UniversalLyzer
{
	public class MultiConverterElectrolyzer : StateMachineComponent<MultiConverterElectrolyzer.StatesInstance>
	{
		[SerializeField]
		public bool hasMeter = true;
		[SerializeField]
		public CellOffset emissionOffset = CellOffset.none;
		[MyCmpAdd]
		private Storage storage;

		[MyCmpGet]
		public ElementConverter converter;
		[MyCmpReq]
		private Operational operational;
		private MeterController meter;

		[MyCmpGet]
		KBatchedAnimController controller;

		[MyCmpGet]
		EnergyConsumer energyConsumer;
		[MyCmpGet]
		Building building;
		public override void OnSpawn()
		{

			if (this.hasMeter)
				this.meter = new MeterController(controller, "U2H_meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(-0.4f, 0.5f, -0.1f), new string[4]
				{
					"U2H_meter_target",
					"U2H_meter_tank",
					"U2H_meter_waterbody",
					"U2H_meter_level"
				});
			this.smi.StartSM();
			UpdateColor();
			this.UpdateMeter();
			Tutorial.Instance.oxygenGenerators.Add(this.gameObject);
		}

		public override void OnCleanUp()
		{
			Tutorial.Instance.oxygenGenerators.Remove(this.gameObject);
			base.OnCleanUp();
		}

		public void UpdateMeter()
		{
			if (!this.hasMeter)
				return;
			this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
			UpdateColor();
		}
		public void UpdateColor()
		{
			var liquid = storage.FindFirstWithMass(GameTags.AnyWater, 0.2f);
			if (liquid != null && liquid.TryGetComponent<PrimaryElement>(out var element))
			{
				var color = element.Element.substance.conduitColour;
				meter.SetSymbolTint("u2h_meter_waterlevel", color);
				meter.SetSymbolTint("u2h_meter_waterbody", color);
				controller.SetSymbolTint("u1h_fxbubbles", color);
				controller.SetSymbolTint("filterwater", color);
				controller.SetSymbolTint("bub", color);
			}
		}


		private bool RoomForPressure => !GameUtil.FloodFillCheck(new Func<int, MultiConverterElectrolyzer, bool>(MultiConverterElectrolyzer.OverPressure), this, Grid.OffsetCell(Grid.PosToCell(this.transform.GetPosition()), this.emissionOffset), 3, true, true);

		private static bool OverPressure(int cell, MultiConverterElectrolyzer MultiConverterElectrolyzer) => (double)Grid.Mass[cell] > MultiConverterElectrolyzer.LastActiveConfig.OverpressurisationThreshold;

		public class StatesInstance :
		  GameStateMachine<States, StatesInstance, MultiConverterElectrolyzer, object>.GameInstance
		{
			public StatesInstance(MultiConverterElectrolyzer smi)
			  : base(smi)
			{
			}
		}

		public class States :
		  GameStateMachine<States, StatesInstance, MultiConverterElectrolyzer>
		{
			public State disabled;
			public State waiting;
			public State converting;
			public State overpressure;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = disabled;
				this.root
					.EventTransition(GameHashes.OperationalChanged, this.disabled, smi => !smi.master.operational.IsOperational)
					.EventHandler(GameHashes.OnStorageChange, smi => smi.master.UpdateMeter());
				this.disabled
					.EventTransition(GameHashes.OperationalChanged, this.waiting, smi => smi.master.operational.IsOperational);
				this.waiting
					.Enter("Waiting", smi => smi.master.operational.SetActive(false))
					.EventTransition(GameHashes.OnStorageChange, this.converting, smi => smi.master.HasEnoughMassToStartConverting())
					.Transition(this.overpressure, smi => !smi.master.RoomForPressure)
					.Update((smi, dt) =>
					{
						smi.master.UpdateConverter();
						smi.master.UpdateColor();
					})
					.UpdateTransition(converting, (smi, dt) => smi.master.HasEnoughMassToStartConverting())
					;
				this.converting.Enter("Ready", smi => smi.master.operational.SetActive(true))
						  .Transition(this.waiting, smi => !smi.master.CanConvertAtAll())
						  .Transition(this.overpressure, smi => !smi.master.RoomForPressure);

				this.overpressure
					.Enter("OverPressure", smi => smi.master.operational.SetActive(false))
					.ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk)
					.Transition(this.converting, smi => smi.master.RoomForPressure);
			}
		}
		private bool CanConvertAtAll() => converter.CanConvertAtAll();
		private bool HasEnoughMassToStartConverting() => converter.HasEnoughMassToStartConverting();


		ElectrolyzerConfiguration LastActiveConfig = ModAssets.ElectrolyzerConfigurations[SimHashes.Water];

		private void UpdateConverter()
		{
			if (storage.items.Count == 0) return;


			var liquid = storage.FindFirstWithMass(GameTags.AnyWater, 0.1f);
			if (liquid != null && liquid.TryGetComponent<PrimaryElement>(out var element) && converter.smi.IsInsideState(converter.smi.sm.disabled))
			{
				var lastVal = LastActiveConfig;

				if (Config.Instance.PerLiquidSettings && ModAssets.ElectrolyzerConfigurations.ContainsKey(element.ElementID))
				{
					LastActiveConfig = ModAssets.ElectrolyzerConfigurations[element.ElementID];
				}
				else if (LastActiveConfig != ModAssets.ElectrolyzerConfigurations[SimHashes.Water])
				{
					LastActiveConfig = ModAssets.ElectrolyzerConfigurations[SimHashes.Water];
				}

				if (LastActiveConfig != lastVal)
				{
					CleaningUpOldAccumulators();
					converter.consumedElements = LastActiveConfig.InputElements.ToArray();
					converter.outputElements = LastActiveConfig.OutputElements.ToArray();
					CreatingNewAccumulators();
					energyConsumer.BaseWattageRating = LastActiveConfig.PowerConsumption;
				}
			}
		}

		private void CleaningUpOldAccumulators()
		{
			for (int i = converter.consumedElements.Length - 1; i >= 0; i--)
			{
				Game.Instance.accumulators.Remove(converter.consumedElements[i].Accumulator);
			}

			for (int i = converter.outputElements.Length - 1; i >= 0; i--)
			{
				Game.Instance.accumulators.Remove(converter.outputElements[i].accumulator);
			}
		}
		private void CreatingNewAccumulators()
		{

			for (int i = 0; i < converter.consumedElements.Length; i++)
			{
				converter.consumedElements[i].Accumulator = Game.Instance.accumulators.Add("ElementsConsumed", converter);
			}

			converter.totalDiseaseWeight = 0f;
			for (int j = 0; j < converter.outputElements.Length; j++)
			{
				converter.outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", converter);
				converter.totalDiseaseWeight += converter.outputElements[j].diseaseWeight;
			}
		}
	}

}
