using Database;
using System;
using System.Collections.Generic;
using System.Text;
using static STRINGS.DUPLICANTS.CHORES;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_ChoreTypes
	{
		public static ChoreType MoisturizeMe;
		internal static void Register(ChoreTypes choreTypes)
		{
			MoisturizeMe = choreTypes.Add(nameof(MoisturizeMe), [], nameof(Aq_Urges.MoisturizeMe), [], STRINGS.DUPLICANTS.CHORES.MOISTURIZEME.NAME, STRINGS.DUPLICANTS.CHORES.MOISTURIZEME.STATUS, STRINGS.DUPLICANTS.CHORES.MOISTURIZEME.TOOLTIP,false);
		}
	}
}
