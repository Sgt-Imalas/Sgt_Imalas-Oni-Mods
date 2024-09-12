using TUNING;

namespace Dupery
{
	class PersonalityGenerator
	{
		public static string DEFAULT_GENDER = "NB";
		public static string DEFAULT_PERSONALITY_TYPE = "Doofy";
		public static string DEFAULT_STRESS_TRAIT = "Aggressive";
		public static string DEFAULT_JOY_TRAIT = "BalloonArtist";
		public static string DEFAULT_STICKER_TYPE = "sticker";

		public static string RollGender()
		{
			string[] genders = new string[3] { "Female", "NB", "Male" };
			int randomIndex = UnityEngine.Random.Range(0, genders.Length);
			return genders[randomIndex];
		}

		public static string RollPersonalityType()
		{
			string[] personalityTypes = new string[4] { "Doofy", "Cool", "Grumpy", "Sweet" };
			int randomIndex = UnityEngine.Random.Range(0, personalityTypes.Length);
			return personalityTypes[randomIndex];
		}

		public static string RollStressTrait()
		{
			int randomIndex = UnityEngine.Random.Range(0, DUPLICANTSTATS.STRESSTRAITS.Count);
			return DUPLICANTSTATS.STRESSTRAITS[randomIndex].id;
		}

		public static string RollJoyTrait()
		{
			int randomIndex = UnityEngine.Random.Range(0, DUPLICANTSTATS.JOYTRAITS.Count);
			return DUPLICANTSTATS.JOYTRAITS[randomIndex].id;
		}

		public static string RollStickerType()
		{
			string[] stickerTypes = new string[3] { "sticker", "glitter", "glowinthedark" };
			int randomIndex = UnityEngine.Random.Range(0, stickerTypes.Length);
			return stickerTypes[randomIndex];
		}

		public static int RollAccessory(AccessorySlot accessorySlot)
		{
			return UnityEngine.Random.Range(0, accessorySlot.accessories.Count) + 1;
		}

		public static Personality RandomPersonality()
		{
			string nameStringKey = string.Format("{0:00000}", UnityEngine.Random.Range(0, 100000));
			string name = $"No. {nameStringKey}";

			if (!DuperyPatches.Localizer.TryGet("Dupery.STRINGS.RANDOM_DUPLICANT_DESCRIPTION", out string description))
				description = STRINGS.RANDOM_DUPLICANT_DESCRIPTION;

			PersonalityOutline outline = new PersonalityOutline()
			{
				Randomize = true,
				Name = name,
				Description = description
			};

			return outline.ToPersonality(nameStringKey);
		}
	}
}
