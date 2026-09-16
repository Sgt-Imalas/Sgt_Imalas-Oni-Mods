using HarmonyLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.ModPatches
{
    class Localization_Patches
    {
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
				ModAssets.ModTraits.RegisterTraits();
			}
		}
	}
}
