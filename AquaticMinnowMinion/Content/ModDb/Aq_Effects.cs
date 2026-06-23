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
			new EffectBuilder("AQ_ItchyGills", 0.2f, true)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.AQ_ITCHYGILLS.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.AQ_ITCHYGILLS.TOOLTIP)
				.Modifier(Db.Get().Amounts.Breath.Id, 18f)
				.Add(db);
		}
	}
}
