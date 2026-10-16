using HarmonyLib;
using Rockets_TinyYetBig.Elements;
using UtilLibs;

namespace Rockets_TinyYetBig
{
	class Localization_Patches
	{
		/// <summary>
		/// Initializes Localisation for modded strings
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
				ModElements.RegisterAdditionalStrings();				
			}
		}
	}
}
