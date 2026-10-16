using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Immigration_Patches
	{

        [HarmonyPatch(typeof(Immigration), nameof(Immigration.ConfigureCarePackages))]
        public class Immigration_ConfigureCarePackages_Patch
        {
            public static void Postfix(Immigration __instance)
            {
                ModImmigration.AddModdedCarePackages(__instance);
			}
        }
	}
}
