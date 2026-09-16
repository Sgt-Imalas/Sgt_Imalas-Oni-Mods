using HarmonyLib;
using UtilLibs;

namespace NaturalConstruction.Patches
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
			}
		}
	}
}
