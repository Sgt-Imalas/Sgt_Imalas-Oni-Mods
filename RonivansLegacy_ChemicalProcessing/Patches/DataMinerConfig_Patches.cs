using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class DataMinerConfig_Patches
	{

        [HarmonyPatch(typeof(DataMinerConfig), nameof(DataMinerConfig.ConfigureBuildingTemplate))]
        public class DataMinerConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(DataMinerConfig __instance)
            {
                AdditionalRecipes.RegisterRecipes_DataMiner();
            }
        }
	}
}
