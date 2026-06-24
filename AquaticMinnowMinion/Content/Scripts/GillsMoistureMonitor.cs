using AquaticMinnowMinion.Content.ModDb;
using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace AquaticMinnowMinion.Content.Scripts
{

    public class GillsMoistureMonitor : GameStateMachine<GillsMoistureMonitor, GillsMoistureMonitor.Instance, IStateMachineTarget, GillsMoistureMonitor.Def>
	{
		public static CellOffset GillCellOffset = new CellOffset(0, 1);

		private State openAir;
        private State suitAir; //is dry af.
        private State inWater;
        private State doingGillMoisturizingTask;


		public override void InitializeStates(out BaseState default_state)
        {
            default_state = openAir;

			openAir
				.UpdateTransition(inWater, (smi, dt) => InWater(smi))
				.EventTransition(ModAssets.AqHashes.StartedMoisturizingTask, doingGillMoisturizingTask)
				.TagTransition(GameTags.HasAirtightSuit, suitAir)
				;
			doingGillMoisturizingTask
				.ToggleAttributeModifier("moisturizing task", (Instance smi) => smi.inWaterMoistureModifier)
				.EventTransition(GameHashes.WorkerPlayPostAnim, openAir)
				;
			inWater
				.UpdateTransition(openAir, (smi, dt) => !InWater(smi))
				.ToggleAttributeModifier("moisturizing", (Instance smi) => smi.inWaterMoistureModifier)
				.TagTransition(GameTags.HasAirtightSuit, suitAir)
				;
			suitAir
				.TagTransition(GameTags.HasAirtightSuit, openAir, true)
				.ToggleEffect(Aq_Effects.DrySuitAir.Id)
				;
		}
		
		static bool InWater(Instance smi)
		{
			if (smi.nav.CurrentNavType == NavType.Tube)
				return false;

			int gillCell = Grid.PosToCell(smi);
			if (smi.nav.CurrentNavType != NavType.Swim)
				gillCell = Grid.CellAbove(gillCell);

			if (!Grid.IsValidCell(gillCell))
				return false;

			return Grid.IsSubstantialLiquid(gillCell,0.1f) && Grid.Element[gillCell].HasTag(ModAssets.Tags.BreathableWater);
		}

        public class Def : BaseDef
        {
        }

        public new class Instance : GameInstance
		{
			[MyCmpReq]
			public Navigator nav;
			[MyCmpReq]
			public Effects effects;
			public AmountInstance moistureAmount;
			public AttributeModifier inWaterMoistureModifier;
			public Instance(IStateMachineTarget master) : base(master)
			{
				this.moistureAmount = Aq_Amounts.Aquatic_GillMoisture.Lookup(this.gameObject);
				this.inWaterMoistureModifier = new AttributeModifier(this.moistureAmount.amount.deltaAttribute.Id, AQ_TUNING.GILL_MOISTURE.MOISTURE_GAIN_IN_WATER, global::STRINGS.CREATURES.MODIFIERS.MOISTURE_GAIN_RATE.NAME);
			}
        }
    }
}
