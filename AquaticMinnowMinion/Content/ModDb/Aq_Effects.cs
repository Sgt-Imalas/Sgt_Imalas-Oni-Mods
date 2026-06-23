using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Effects
	{
		public static Effect ItchyGills;
		const string GillIrritation = "Aq_GillIrritationGroup";
		internal static void Register(Db db)
		{
			var stressDelta = db.Amounts.Stress.deltaAttribute.Id;
			var peeDelta = db.Amounts.Bladder.deltaAttribute.Id;
			var carryCapacity = db.Attributes.CarryAmount.Id;
			var strength = db.Attributes.Strength.Id;
			var athletics = db.Attributes.Athletics.Id;
			var airConsumptionRate = db.Attributes.AirConsumptionRate.Id;
			var morale = db.Attributes.QualityOfLife.Id;

			new EffectBuilder("AQ_ItchyGills", 0.2f * CONSTS.CYCLE_LENGTH, true)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_ITCHYGILLS.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_ITCHYGILLS.TOOLTIP)
				.Modifier(airConsumptionRate, 0.030f)
				.Modifier(stressDelta, 15f / CONSTS.CYCLE_LENGTH) //todo: check vals; 
				.Add(db, out ItchyGills);
		}
	}
}
