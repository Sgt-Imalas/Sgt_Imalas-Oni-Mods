using AquaticMinnowMinion.Content.ModDb;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Content.Scripts
{

	public class WaterBreathingEfficiencyMonitor : GameStateMachine<WaterBreathingEfficiencyMonitor, WaterBreathingEfficiencyMonitor.Instance, IStateMachineTarget, WaterBreathingEfficiencyMonitor.Def>
	{
		private State idle;
		private State filteringDecision;
		private State filteringWater;
		private State filteringWaterSkilled;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;

			idle
				.EventTransition(ModAssets.AqHashes.StartedBreathingLiquid, filteringDecision);
			filteringDecision
				.EnterTransition(filteringWater, HasBasicGills)
				.EnterTransition(filteringWaterSkilled, HasAdvancedGills);				
			filteringWater
				.ToggleEffect(Aq_Effects.GillsFilteringLiquid.Id)
				.EventTransition(ModAssets.AqHashes.StoppedBreathingLiquid, idle);
			filteringWaterSkilled
				.ToggleEffect(Aq_Effects.GillsFilteringLiquid_Skilled.Id)
				.EventTransition(ModAssets.AqHashes.StoppedBreathingLiquid, idle);


		}
		public class Def : BaseDef
		{
		}
		public static bool HasBasicGills(Instance smi) => !HasAdvancedGills(smi);
		public static bool HasAdvancedGills(Instance smi) => smi.resume.HasPerk(Aq_SkillPerks.Adapt_WaterbreathingEfficiency);

		public new class Instance : GameInstance
		{
			Effects effects;
			public MinionResume resume;
			public Instance(IStateMachineTarget master) : base(master)
			{
				effects = master.gameObject.GetComponent<Effects>();
				resume = master.gameObject.GetComponent<MinionResume>();
			}
		}
	}

}
