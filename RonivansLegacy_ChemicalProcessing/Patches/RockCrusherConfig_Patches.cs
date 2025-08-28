using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class RockCrusherConfig_Patches
    {

        [HarmonyPatch(typeof(RockCrusherConfig), nameof(RockCrusherConfig.ConfigureBuildingTemplate))]
        public class RockCrusherConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix()
			{
				AdditionalRecipes.RegisterRecipes_RockCrusher();
			}
        }
    }
}
