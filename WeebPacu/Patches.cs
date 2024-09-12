using HarmonyLib;
using STRINGS;
using UtilLibs;

namespace WeebPacu
{
	internal class Patches
	{
		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(BasePacuConfig))]
		[HarmonyPatch(nameof(BasePacuConfig.CreatePrefab))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Prefix(ref string anim_file)
			{
				if (anim_file == "pacu_kanim")
					anim_file = "weeb_pacu_kanim";
				SoundUtils.CopySoundsToAnim("weeb_pacu_kanim", "pacu_kanim");
			}
		}
		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				CREATURES.FAMILY.PACU = (LocString)UI.FormatAsLink("PacUwU", "PACUSPECIES");
				CREATURES.FAMILY_PLURAL.PACUSPECIES = (LocString)UI.FormatAsLink("PacUwUs", nameof(CREATURES.FAMILY_PLURAL.PACUSPECIES));
				CREATURES.SPECIES.PACU.NAME = (LocString)UI.FormatAsLink("PacUwU", nameof(CREATURES.SPECIES.PACU));
				CREATURES.SPECIES.PACU.DESC = "Imagine a weeb pacu - pacUwU\n\n" + CREATURES.SPECIES.PACU.DESC;
				CREATURES.SPECIES.PACU.VARIANT_TROPICAL.NAME = (LocString)UI.FormatAsLink("Tropical PacUwU", "PACUTROPICAL");
				CREATURES.SPECIES.PACU.VARIANT_CLEANER.NAME = (LocString)UI.FormatAsLink("GUwUlp Fish", "PACUCLEANER");
			}
		}
	}
}
