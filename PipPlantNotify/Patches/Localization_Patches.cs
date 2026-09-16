using HarmonyLib;
using UtilLibs;

namespace PipPlantNotify.Patches
{
	internal class Localization_Patches
	{
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}
