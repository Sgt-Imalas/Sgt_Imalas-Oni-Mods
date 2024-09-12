using Klei.AI;
using System.Collections.Generic;

namespace SetStartDupes.DuplicityEditing.Helpers
{
	public static class AmountHelper
	{
		public static List<Amount> GetEditableAmounts()
		{

			var amounts = Db.Get().Amounts;
			var output = new List<Amount>
			{
				amounts.HitPoints,
				amounts.Stamina,
				amounts.Calories,
				amounts.Breath,
				amounts.Bladder,
				amounts.Stress,
				amounts.Decor,
			};
			if (DlcManager.IsExpansion1Active())
			{
				output.Add(amounts.RadiationBalance);
			}

			return output;
		}
	}
}
