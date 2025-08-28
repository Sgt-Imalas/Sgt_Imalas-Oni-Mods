using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class ChemicalRefinery_Patches
    {

        [HarmonyPatch(typeof(ChemicalRefineryConfig), nameof(ChemicalRefineryConfig.ConfigureBuildingTemplate))]
        public class ChemicalRefineryConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(ChemicalRefineryConfig __instance)
            {
                AdditionalRecipes.AdditionalnRecipes_ChemicalRefinery(ChemicalRefineryConfig.ID);

			}
        }
    }
}
