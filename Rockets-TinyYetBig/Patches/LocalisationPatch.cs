using HarmonyLib;
using UtilLibs;

namespace Rockets_TinyYetBig
{
	class LocalisationPatch
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
				LocalisationUtil.FixRoomConstrains();
				ModAssets.InitializeCategoryTooltipDictionary();
			}
		}
	}
}
