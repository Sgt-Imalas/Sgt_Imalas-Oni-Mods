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
				LocalisationUtil.FixTranslationStrings();

				if (Config.Instance.EthanolEngines)
				{
					global::STRINGS.BUILDINGS.PREFABS.KEROSENEENGINECLUSTER.EFFECT = STRINGS.MODIFIEDVANILLASTRINGS.KEROSENEENGINECLUSTER_EFFECT;
					global::STRINGS.BUILDINGS.PREFABS.KEROSENEENGINECLUSTERSMALL.EFFECT = STRINGS.MODIFIEDVANILLASTRINGS.KEROSENEENGINECLUSTERSMALL_EFFECT;
				}
			}
		}
	}
}
