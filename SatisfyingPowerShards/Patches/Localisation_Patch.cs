using HarmonyLib;
using UtilLibs;

namespace SatisfyingPowerShards.Patches
{
	internal class Localisation_Patch
	{
		/// <summary>
		/// Init. auto translation
		/// </summary>
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
