using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;

namespace Planticants.Content.ModDb
{
	class PlantPersonalities
	{


		public static string SYLVIA = "FLORAL_SYLVIA", CAITHE = "FLORAL_CAITHE", CANACH = "FLORAL_CANACH", WYNNE = "FLORAL_WYNNE";

		/// <summary>
		/// replace cheek symbols with custom ones later;
		/// </summary>
		public static Dictionary<HashedString, string> headKanims = new();
		public static void RegisterPersonalities(Personalities personalities)
		{
			SgtLogger.l("Registering plant personalities...");
			var sylvia = new Personality(
				SYLVIA,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_SYLVIA.NAME,
				"Female",
				null,
				"Aggressive",
				"BalloonArtist",
				null,
				null,
				4,
				4,
				1,
				4,
				38,
				4,
				1,
				1,
				1,
				1,
				1,
				1,
				4,
				4,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_SYLVIA.DESCRIPTION,
				true,
				null,
				ModTags.PlantMinion,
				0
				);
			personalities.Add(sylvia);

			var caithe = new Personality(
				CAITHE,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_CAITHE.NAME,
				"Female",
				null,
				"Aggressive",
				"BalloonArtist",
				null,
				null,
				4,
				4,
				1,
				4,
				38,
				4,
				1,
				1,
				1,
				1,
				1,
				1,
				4,
				4,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_CAITHE.DESCRIPTION,
				true,
				null,
				ModTags.PlantMinion,
				0
				);
			personalities.Add(caithe);

			var canach = new Personality(
				CANACH,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_CANACH.NAME,
				"MALE",
				null,
				"Aggressive",
				"BalloonArtist",
				null,
				null,
				4,
				4,
				1,
				4,
				38,
				4,
				1,
				1,
				1,
				1,
				1,
				1,
				4,
				4,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_CANACH.DESCRIPTION,
				true,
				null,
				ModTags.PlantMinion,
				0
				);
			personalities.Add(canach); 
			

			var wynne = new Personality(
				WYNNE,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_WYNNE.NAME,
				"MALE",
				null,
				"Aggressive", 
				"BalloonArtist",
				null,
				null,
				4,
				4,
				1,
				4,
				38,
				4,
				1,
				1,
				1,
				1,
				1,
				1,
				4,
				4,
				STRINGS.DUPLICANTS.PERSONALITIES.FLORAL_WYNNE.DESCRIPTION,
				true,
				null,
				ModTags.PlantMinion,
				0
				);
			personalities.Add(wynne);

			SgtLogger.l("Plant personalities registered successfully.");
		}
	}
}
