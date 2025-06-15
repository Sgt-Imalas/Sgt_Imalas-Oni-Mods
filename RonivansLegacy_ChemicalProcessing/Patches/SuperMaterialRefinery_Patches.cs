using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class SuperMaterialRefinery_Patches
    {

        [HarmonyPatch(typeof(SupermaterialRefineryConfig), nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
        public class SupermaterialRefineryConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix()
            {
                AdditionalRecipes.RegisterRecipes_SuperMaterialRefinery();
			}
        }
    }
}
