using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
