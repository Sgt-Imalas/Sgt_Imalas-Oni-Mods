using HarmonyLib;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class Localization_Patch
    {
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				UtilLibs.LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}
