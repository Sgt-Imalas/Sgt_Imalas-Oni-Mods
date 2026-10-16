using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class FabricatedWoodMakerConfig_Patches
	{

        [HarmonyPatch(typeof(FabricatedWoodMakerConfig), nameof(FabricatedWoodMakerConfig.ConfigureRecipes))]
        public class FabricatedWoodMakerConfig_ConfigureRecipes_Patch
        {
            public static void Postfix()
            {
                AdditionalRecipes.RegisterRecipes_FabricatedWoodMaker();
			}
        }
	}
}
