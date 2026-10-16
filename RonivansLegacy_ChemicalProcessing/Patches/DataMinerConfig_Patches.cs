using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

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
