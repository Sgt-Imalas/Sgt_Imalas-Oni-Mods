using HarmonyLib;
using UtilLibs;

namespace WallAttachmentPumps.Patches
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
			}
		}
	}
}
