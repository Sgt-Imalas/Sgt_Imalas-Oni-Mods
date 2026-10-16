using HarmonyLib;
using UtilLibs;

namespace MassMoveTo.Patches
{
	internal class Localization_Patches
	{
		/// <summary>
		/// Init. auto translation
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
