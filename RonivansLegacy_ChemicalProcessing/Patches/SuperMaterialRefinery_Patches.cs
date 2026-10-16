using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

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
