using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
    class ModPersonalities
	{
		public static readonly string RONIVAN = "PROCESSING_AIO_RONIVAN";
		public static readonly int RONIVAN_HASH = Hash.SDBMLower(RONIVAN); // 311384679
		public static void Register(Personalities personalities)
        {
			var ronivanPersonality = new Personality(
				RONIVAN,
				STRINGS.DUPLICANTS.PROCESSING_AIO_RONIVAN.NAME,
				"Male",
				"Sweet",
				"StressVomiter",
				"SuperProductive",
				"glitter",
				"GrantSkill_Engineering1", //congenital mechatronics engineering
				3,
				3,
				0,
				3,
				17,
				2,
				1,
				1,
				1,
				1,
				1,
				1,
				3,
				3,
				STRINGS.DUPLICANTS.PROCESSING_AIO_RONIVAN.DESC,
				true,
				"hassan", //same hair style && grave stone icons are monochrome
				"Minion",
				0);

			if(!Config.Instance.RonivanDuplicant)
				ronivanPersonality.Disabled = true;

			personalities.Add(ronivanPersonality);
		}
    }
}
