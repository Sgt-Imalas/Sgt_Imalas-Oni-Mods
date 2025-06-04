using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using static TUNING.DUPLICANTSTATS;

namespace Planticants.Content.ModDb
{
    class PLANT_TUNING
    {
        public static float CO2_TANK_CAPACITY = 100;
        public static float WATER_TANK_CAPACITY = 100;

        public static DUPLICANTSTATS PLANTMINIONSTATS = new DUPLICANTSTATS
		{
			BaseStats = new BASESTATS
			{
				MAX_CALORIES = 0f
			},
			DiseaseImmunities = new DISEASEIMMUNITIES
			{
				IMMUNITIES = new string[1] { "FoodSickness" }
			}
		};
		public static void RegisterType() => DUPLICANT_TYPES[ModTags.PlantMinion] = PLANTMINIONSTATS;
	}
}
