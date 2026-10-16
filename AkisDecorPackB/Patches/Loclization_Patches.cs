using HarmonyLib;

namespace AkisDecorPackB.Patches
{
	internal class Loclization_Patches
	{

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
			{
				UtilLibs.LocalisationUtil.Translate(typeof(STRINGS), true);
			}
        }
	}
}
