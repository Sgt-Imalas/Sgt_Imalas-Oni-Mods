using Database;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Personalities
	{

		public static string AQUATIC_MINNOW = "AQUATIC_MINNOW";

		/// <summary>
		/// replace cheek symbols with custom ones later;
		/// </summary>
		public static Dictionary<HashedString, string> headKanims = new();
		public static void RegisterPersonalities(Personalities personalities)
		{
			SgtLogger.l("Registering aquatic personalities...");
			var minnowReference = personalities.Get("MINNOW");
			if (minnowReference == null)
			{
				SgtLogger.error("COULD NOT FIND MINNOW!");
				return;
			}
			var a_minnow = new Personality(
				AQUATIC_MINNOW,
				global::STRINGS.DUPLICANTS.PERSONALITIES.MINNOW.NAME,
				minnowReference.genderStringKey,
				minnowReference.personalityType,
				minnowReference.stresstrait,
				minnowReference.joyTrait,
				minnowReference.stickerType,
				null,// no freediver for u here, its built in
				minnowReference.headShape,
				minnowReference.mouth,
				minnowReference.neck,
				minnowReference.eyes,
				minnowReference.hair,
				minnowReference.body,
				minnowReference.belt,
				minnowReference.cuff,
				minnowReference.foot,
				minnowReference.hand,
				minnowReference.pelvis,
				minnowReference.leg,
				minnowReference.arm_skin,
				minnowReference.leg_skin,
				global::STRINGS.DUPLICANTS.PERSONALITIES.MINNOW.DESC,
				true,
				minnowReference.graveStone,
				Tags.AquaticMinion,
				minnowReference.speech_mouth
				);
			personalities.Add(a_minnow);


			if (!CharacterContainer.defaultShirtIdxToDefaultOutfitID.ContainsKey(a_minnow.body))
			{
				CharacterContainer.defaultShirtIdxToDefaultOutfitID.Add(a_minnow.body, "");
			}

			SgtLogger.l("aquatic personalities registered successfully.");
		}
	}
}
