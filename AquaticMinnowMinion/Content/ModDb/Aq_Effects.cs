using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Effects
	{
		public static Effect
			ItchyGills, 
			GillsFilteringLiquid,
			GillsFilteringLiquid_Skilled,

			DryGills_Minor,
			DryGills_Major,
			DryGills_Extreme,

			DrySuitAir,
			MoisturizedGills_OneTime
			;

		const string GillsFilteringGroup = "Aq_GillsFilteringGroup";
		const string DryGillsGroup = "Aq_DryGillsGroup";
		internal static void Register(Db db)
		{
			var stressDelta = db.Amounts.Stress.deltaAttribute.Id;
			var peeDelta = db.Amounts.Bladder.deltaAttribute.Id;
			var carryCapacity = db.Attributes.CarryAmount.Id;
			var strength = db.Attributes.Strength.Id;
			var athletics = db.Attributes.Athletics.Id;
			var airConsumptionRate = db.Attributes.AirConsumptionRate.Id;
			var morale = db.Attributes.QualityOfLife.Id;
			var gillsMoistureDelta = Aq_Amounts.Aquatic_GillMoisture.deltaAttribute.Id;

			new EffectBuilder("AQ_ItchyGills", 0.2f * CONSTS.CYCLE_LENGTH, true)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_ITCHYGILLS.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_ITCHYGILLS.TOOLTIP)
				.Modifier(airConsumptionRate, 0.030f)
				.Modifier(stressDelta, 15f / CONSTS.CYCLE_LENGTH) //todo: check vals; 
				.Add(db, out ItchyGills);

			new EffectBuilder("AQ_GillsFilteringLiquid", CONSTS.EFFECTDURATION.PERSISTENT, false)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_BREATHINGGILLS.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_BREATHINGGILLS.TOOLTIP)
				.Modifier(airConsumptionRate, 0.020f)
				.StompGroup(GillsFilteringGroup)
				.Add(db, out GillsFilteringLiquid);

			new EffectBuilder("AQ_GillsFilteringLiquid_Skilled", CONSTS.EFFECTDURATION.PERSISTENT, false)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_BREATHINGGILLS.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_BREATHINGGILLS.TOOLTIP_SKILLED)
				.Modifier(airConsumptionRate, -0.005f)
				.StompGroup(GillsFilteringGroup)
				.Add(db, out GillsFilteringLiquid_Skilled);

			new EffectBuilder("AQ_DryGills_Minor", CONSTS.EFFECTDURATION.PERSISTENT, true)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYGILLS_MINOR.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYGILLS_MINOR.TOOLTIP)
				.Modifier(stressDelta, 5f / CONSTS.CYCLE_LENGTH)
				.StompGroup(DryGillsGroup)
				.Add(db, out DryGills_Minor);

			new EffectBuilder("AQ_DryGills_Major", CONSTS.EFFECTDURATION.PERSISTENT, true)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYGILLS_MAJOR.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYGILLS_MAJOR.TOOLTIP)
				.Modifier(stressDelta, 25f / CONSTS.CYCLE_LENGTH)
				.StompGroup(DryGillsGroup)
				.Add(db, out DryGills_Major);

			new EffectBuilder("AQ_DryGills_Extreme", CONSTS.EFFECTDURATION.PERSISTENT, true)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYGILLS_EXTREME.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYGILLS_EXTREME.TOOLTIP)
				.Modifier(stressDelta, 100f / CONSTS.CYCLE_LENGTH)
				.StompGroup(DryGillsGroup)
				.Add(db, out DryGills_Extreme);

			new EffectBuilder("AQ_DrySuitAir", CONSTS.EFFECTDURATION.PERSISTENT, true)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYSUITAIR.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_DRYSUITAIR.TOOLTIP)
				.Modifier(stressDelta, 10f / CONSTS.CYCLE_LENGTH)
				.Modifier(gillsMoistureDelta, AQ_TUNING.GILL_MOISTURE.SUITAIR_DELTA)
				.Add(db, out DrySuitAir);
			

		}
	}
}
