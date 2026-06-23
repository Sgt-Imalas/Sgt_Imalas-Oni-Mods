using AquaticMinnowMinion.Content.ModDb;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace AquaticMinnowMinion.Content.Scripts
{

	public class DryGillsMonitor : GameStateMachine<DryGillsMonitor, DryGillsMonitor.Instance, IStateMachineTarget, DryGillsMonitor.Def>
	{
		private State wet;
		private State slightly_dry;
		private State very_dry;
		private State completely_dry;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = wet;

			wet
				.UpdateTransition(slightly_dry, (smi, dt) => BelowMoisture(smi, AQ_TUNING.GILL_MOISTURE.MOISTURE_DRY_THRESHOLD));
			slightly_dry
				.ToggleEffect(Aq_Effects.DryGills_Minor.Id)
				.UpdateTransition(wet, (smi, dt) => AboveMoisture(smi, AQ_TUNING.GILL_MOISTURE.MOISTURE_DRY_THRESHOLD))
				.UpdateTransition(very_dry, (smi, dt) => BelowMoisture(smi, AQ_TUNING.GILL_MOISTURE.MOISTURE_VERY_DRY_THRESHOLD));
			very_dry
				.ToggleEffect(Aq_Effects.DryGills_Major.Id)
				.UpdateTransition(slightly_dry, (smi, dt) => AboveMoisture(smi, AQ_TUNING.GILL_MOISTURE.MOISTURE_VERY_DRY_THRESHOLD))
				.UpdateTransition(completely_dry, (smi, dt) => BelowMoisture(smi, 0));
			completely_dry
				.ToggleEffect(Aq_Effects.DryGills_Extreme.Id)
				.UpdateTransition(very_dry, (smi, dt) => AboveMoisture(smi, 0));

		}
		private static bool AboveMoisture(Instance smi, float moistureTreshold)
		{
			return smi.moisture.value > moistureTreshold;
		}
		private static bool BelowMoisture(Instance smi, float moistureTreshold)
		{
			return smi.moisture.value <= moistureTreshold;
		}

		public class Def : BaseDef
		{
		}

		public new class Instance : GameInstance
		{
			public Navigator navigator;
			public AmountInstance moisture;
			public Instance(IStateMachineTarget master) : base(master)
			{
				this.moisture = Aq_Amounts.Aquatic_GillMoisture.Lookup(this.gameObject);
				this.navigator = this.smi.GetComponent<Navigator>();
			}
		}
	}
}
