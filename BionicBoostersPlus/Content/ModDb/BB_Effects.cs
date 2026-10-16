using Klei.AI;
using UtilLibs;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_Effects
	{
		public static Effect
			BB_WaterproofedStressReduction
			;

		internal static void Register(Db db)
		{
			var stressDelta = db.Amounts.Stress.deltaAttribute.Id;

			new EffectBuilder(nameof(BB_WaterproofedStressReduction), CONSTS.EFFECTDURATION.PERSISTENT, false)
				.Name(STRINGS.DUPLICANTS.MODIFIERS.BB_WATERPROOFEDSTRESSREDUCTION.NAME)
				.Description(STRINGS.DUPLICANTS.MODIFIERS.BB_WATERPROOFEDSTRESSREDUCTION.TOOLTIP)
				.Modifier(stressDelta, -185 / CONSTS.CYCLE_LENGTH) //counters the +200% of the BionicWaterStress effect
				.HideFloatingText()
				.Add(db, out BB_WaterproofedStressReduction);
		}
	}
}
