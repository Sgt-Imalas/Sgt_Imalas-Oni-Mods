using Database;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Personalities
	{

		public static readonly string AQUATIC_MINNOW = "AQUATIC_MINNOW";
		public static readonly string AQUATIC_PREFIX = "AQUATIC_";
		public static readonly string MINNOW = "MINNOW";

		public static readonly int MinnowClothing = -2124425596;

		/// <summary>
		/// Contains all dynamically generated personalities from normal minions, does not include amphibious minnow!
		/// </summary>
		public static HashSet<Personality> GeneratedPersonalities = new HashSet<Personality>();	
		/// <summary>
		/// Maps the aquatic personalities to their original dreamicon id
		/// </summary>
		public static Dictionary<string,Sprite> OriginalDreamIconMap= new ();


		/// <summary>
		/// replace cheek symbols with custom ones later;
		/// </summary>
		public static Dictionary<HashedString, string> headKanims = new();
		public static void RegisterPersonalities(Personalities personalities)
		{
			SgtLogger.l("Registering aquatic personalities...");
			var minnowReference = personalities.Get(MINNOW);
			if (minnowReference == null)
			{
				SgtLogger.error("COULD NOT FIND MINNOW!");
				return;
			}

			bool makeAquaticCrew = Config.Instance.AquaticCrew;


			//																				needs at least 1 valid aquatic minion or startscreen model lock crashes
			var a_minnow = AddAquaticMinionVariant(minnowReference, !makeAquaticCrew, AQUATIC_MINNOW, global::STRINGS.DUPLICANTS.PERSONALITIES.MINNOW.NAME, Aq_Traits.Aquatic_Freediver);
			personalities.Add(a_minnow);
			SgtLogger.l("aquatic minnow registered");

			if (makeAquaticCrew)
			{
				SgtLogger.l("registering dynamic personalities for the whole crew....");
				foreach (var basePersonality in personalities.resources)
				{
					if (basePersonality.nameStringKey == MINNOW || basePersonality.model != GameTags.Minions.Models.Standard)
						continue;

					GeneratedPersonalities.Add(AddAquaticMinionVariant(basePersonality, basePersonality.startingMinion));
				}
				foreach(var aquaticVariant in GeneratedPersonalities)
					personalities.Add(aquaticVariant);

				SgtLogger.l(GeneratedPersonalities.Count+" dynamic aquatic duplicants added.");
			}
			SgtLogger.l("aquatic personalities registered successfully.");
		}
		static Personality AddAquaticMinionVariant(Personality originalDupe,bool isValidStarter, string idOverride = null, string nameOverride = null, string congenitalOverride = null)
		{

			string aquaticId = AQUATIC_PREFIX + originalDupe.Id;
			string amphibiousName = UI.StripLinkFormatting(STRINGS.DUPLICANTS.MODEL.AQUATIC.NAME_ADJECTIVE) + " " + originalDupe.Name;

			string nameStringKey = idOverride != null ? idOverride : aquaticId;

			OriginalDreamIconMap[nameStringKey] = originalDupe.GetMiniIcon();

			var aquatic_variant = new Personality(
				nameStringKey,
				nameOverride != null ? nameOverride : amphibiousName,
				originalDupe.genderStringKey,
				originalDupe.personalityType,
				originalDupe.stresstrait,
				originalDupe.joyTrait,
				originalDupe.stickerType,
				congenitalOverride != null ? congenitalOverride : originalDupe.congenitaltrait,
				originalDupe.headShape,
				originalDupe.mouth,
				MinnowClothing, //originalDupe.neck,
				originalDupe.eyes,
				originalDupe.hair,
				MinnowClothing, //originalDupe.body,
				MinnowClothing, //originalDupe.belt,
				MinnowClothing, //originalDupe.cuff,
				MinnowClothing, //originalDupe.foot,
				MinnowClothing, //originalDupe.hand,
				MinnowClothing, //originalDupe.pelvis,
				MinnowClothing, //originalDupe.leg,
				originalDupe.arm_skin,
				originalDupe.leg_skin,
				global::STRINGS.DUPLICANTS.PERSONALITIES.MINNOW.DESC,
				isValidStarter,
				originalDupe.graveStone,
				Tags.AquaticMinion,
				originalDupe.speech_mouth
				);
			if (!CharacterContainer.defaultShirtIdxToDefaultOutfitID.ContainsKey(aquatic_variant.body))
			{
				CharacterContainer.defaultShirtIdxToDefaultOutfitID.Add(aquatic_variant.body, "");
			}

			return aquatic_variant;
		}
	}
}
