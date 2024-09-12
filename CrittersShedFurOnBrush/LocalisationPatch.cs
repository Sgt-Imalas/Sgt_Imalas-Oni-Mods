using HarmonyLib;
using UtilLibs;

namespace CrittersShedFurOnBrush
{
	class LocalisationPatch
	{
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
