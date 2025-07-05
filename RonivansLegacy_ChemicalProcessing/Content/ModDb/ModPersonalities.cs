//using Database;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TUNING;

//namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
//{
//	class ModPersonalities
//	{
//		public static readonly string RONIVAN = "PROCESSING_AIO_RONIVAN";
//		public static readonly int RONIVAN_HASH = Hash.SDBMLower(RONIVAN); // 311384679
//		public static readonly string
//			Mechatronics = "GrantSkill_Engineering1",
//			MasterArtist = "GrantSkill_Arting3",
//			ArtistInterest = "Art",
//			OperatingInterest = "Technicals"
//			;

//		public static void Register(Personalities personalities)
//		{
//			return;
//			//not opening a can of worms with disrespecting the person or sth like that

//			var ronivanPersonality = new Personality(
//				RONIVAN,
//				STRINGS.DUPLICANTS.PROCESSING_AIO_RONIVAN.NAME,
//				"Male",
//				"Sweet",
//				"StressVomiter",
//				"SuperProductive",
//				"glitter",
//				MasterArtist, //congenital skill
//				3,
//				3,
//				0,
//				3,
//				17,
//				2,
//				1,
//				1,
//				1,
//				1,
//				1,
//				1,
//				3,
//				3,
//				STRINGS.DUPLICANTS.PROCESSING_AIO_RONIVAN.DESC,
//				true,
//				"hassan", //same hair style && grave stone icons are monochrome
//				"Minion",
//				0);

//			if (!Config.Instance.RonivanDuplicant)
//				ronivanPersonality.Disabled = true;

//			personalities.Add(ronivanPersonality);
//		}
//		public static void OnAptitudeRoll(MinionStartingStats stats, ref string guaranteedAptitudeID)
//		{
//			if (stats.personality.Id == RONIVAN)
//			{
//				guaranteedAptitudeID = OperatingInterest;
//			}
//		}
//		//public static void OnPostAptitudeRoll(MinionStartingStats stats)
//		//{
//		//	if (stats.personality.Id == RONIVAN)
//		//	{
//		//		var dbAptitudes = Db.Get().SkillGroups;

//		//		if (!stats.skillAptitudes.ContainsKey(dbAptitudes.Get(ArtistInterest)))
//		//			stats.skillAptitudes.Add(dbAptitudes.Get(ArtistInterest), DUPLICANTSTATS.APTITUDE_BONUS);

//		//		if (!stats.skillAptitudes.ContainsKey(dbAptitudes.Get(OperatingInterest)))
//		//			stats.skillAptitudes.Add(dbAptitudes.Get(OperatingInterest), DUPLICANTSTATS.APTITUDE_BONUS);

//		//	}
//		//}
//	}
//}
