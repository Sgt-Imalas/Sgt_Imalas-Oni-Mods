using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class Localization_Patches
	{
		/// <summary>
		/// Initializes Localisation for modded strings
		/// </summary>
		[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
				BuildingDatabase.RegisterExtraStrings();
				Mod.RegisterLocalizedDescription();
				StatusItemsDatabase.RegisterClonedStatusStrings();
			}
		}
	}
}
