using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using static AquaticMinnowMinion.ModAssets;
using static TUNING.DUPLICANTSTATS;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class AQ_TUNING
	{
		public static float CO2_TANK_CAPACITY = 100;
		public static float WATER_TANK_CAPACITY = 180;

		public static DUPLICANTSTATS AQUATICMINIONSTATS = new DUPLICANTSTATS
		{
			BaseStats = new BASESTATS
			{
				OXYGEN_USED_PER_SECOND = 0.090f//g/s
			},
		};
		public static void RegisterType() => DUPLICANT_TYPES[ModTags.AquaticMinion] = AQUATICMINIONSTATS;
	}
}
