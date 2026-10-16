using HarmonyLib;
using UtilLibs;

namespace Planticants.Patches
{
	class Localization_Patches
	{

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
