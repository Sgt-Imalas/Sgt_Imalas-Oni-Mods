using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static OniRetroEdition.Behaviors.ManualGeneratorDischargerWithMeter.States;
using static STRINGS.BUILDING.STATUSITEMS;

namespace OniRetroEdition.Behaviors
{

	public class ManualGeneratorDischargerWithMeter : StateMachineComponent<ManualGeneratorDischargerWithMeter.StatesInstance>, ISim200ms
	{
		[MyCmpReq]
		KBatchedAnimController kbac;

		[MyCmpReq]
		public Generator generator;
		[MyCmpReq]
		public ManualGenerator manualGenerator;
		[MyCmpReq]
		public Operational operational;
		public bool inFront = true;
		private MeterController storageMeter;

		[Serialize] public string meterTarget = "meter_target";

		[Serialize] public float maxValueOverride = -1;

		public float Runoff = 1000f;

		float lastBatteryPercentage;

		public override void OnSpawn()
		{
			foreach (var symbol in ManualGenerator.symbol_names)
				kbac.SetSymbolVisiblity(symbol, true);
			base.OnSpawn();
			//SgtLogger.l("initializing meter, meter target: " + meterTarget);

			storageMeter = new MeterController(kbac, meterTarget, "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, [meterTarget]);

			UpdateMeter();
			smi.StartSM();
		}
		void UpdateMeter()
		{
			storageMeter.SetPositionPercent(lastBatteryPercentage);
		}
		float GetAverageBatteryPercentage()
		{
			var batteries = Game.Instance.circuitManager.GetBatteriesOnCircuit(generator.CircuitID);
			float avgPercentage = generator.PercentFull;
			if (avgPercentage == float.NaN)
				avgPercentage = 0;
			if (batteries != null)
			{
				avgPercentage = 0;
				foreach (var batterie in batteries)
					avgPercentage += (batterie.PercentFull);
				avgPercentage /= batteries.Count;
			}
			return Mathf.Clamp01(avgPercentage);
		}

		public bool HasInternalCharge()
		{
			return lastBatteryPercentage > 0;
		}
		public bool IsCurrentlyWorking()
		{
			UpdateMeter();
			return manualGenerator.smi.IsInsideState(manualGenerator.smi.sm.working);
		}

		public void Sim200ms(float dt)
		{
			lastBatteryPercentage = GetAverageBatteryPercentage();
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, ManualGeneratorDischargerWithMeter, object>.GameInstance
		{
			public StatesInstance(ManualGeneratorDischargerWithMeter master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, ManualGeneratorDischargerWithMeter>
		{
			public class NotWorking : State
			{
				public State discharging_pre;
				public State discharging_loop;
				public State discharging_pst;
				public State notdischarging;
			}

			public State working;
			public NotWorking notWorking;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = notWorking;
				//root.Update((smi, dt) => smi.master.UpdateMeter());

				working
					.UpdateTransition(notWorking, (smi, dt) => !smi.master.IsCurrentlyWorking())
					.UpdateTransition(notWorking, (smi, dt) => !smi.master.IsCurrentlyWorking());
				notWorking
					.UpdateTransition(working, (smi, dt) => smi.master.IsCurrentlyWorking());

				notWorking.defaultState = notWorking.notdischarging;
				notWorking.notdischarging
					.UpdateTransition(notWorking.discharging_loop, (smi, dt) => smi.master.HasInternalCharge());
				//notWorking.discharging_pre
				//	.PlayAnim("discharge_pre")
				//	.OnAnimQueueComplete(notWorking.discharging_loop);
				notWorking.discharging_loop
					.PlayAnim("discharge_loop", KAnim.PlayMode.Loop)
					.UpdateTransition(notWorking.discharging_pst, (smi, dt) => !smi.master.HasInternalCharge());
				notWorking.discharging_pst
					.PlayAnim("discharge_pst")
					.OnAnimQueueComplete(notWorking.notdischarging);


			}
		}
	}
}
