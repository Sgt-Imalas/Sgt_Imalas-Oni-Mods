using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;
using static TUNING.DUPLICANTSTATS;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class AQ_TUNING
	{
		///1.5 cycles for gills to dry out fully
		public class GILL_MOISTURE
		{
			public static float MAX = 100;
			public static float MOISTURE_DRY_THRESHOLD = 35;
			public static float MOISTURE_VERY_DRY_THRESHOLD = 15;
			public static float MOISTURE_BASE_DELTA = -100f / (CONSTS.CYCLE_LENGTH * 1.5f);
			public static float MOISTURE_GAIN_IN_WATER = 5; //per second
			public static float SUITAIR_DELTA = -15f / (CONSTS.CYCLE_LENGTH);
		}
		public class ADAPTATION_PERKS
		{
			public static float ADAPTATION_FAT_INSULATION = 0.005f;
			///Extend the moisturization to last 2 days, +~26.67%
			public static float ADAPTATION_MOIST_GILLS =  (-GILL_MOISTURE.MOISTURE_BASE_DELTA) - (50f / CONSTS.CYCLE_LENGTH);
		}

		public static DUPLICANTSTATS AQUATICMINIONSTATS = new DUPLICANTSTATS
		{
			BaseStats = new BASESTATS
			{
				OXYGEN_USED_PER_SECOND = 0.095f//g/s
			},
		};
		public static void RegisterType() => DUPLICANT_TYPES[Tags.AquaticMinion] = AQUATICMINIONSTATS;
	}
}
