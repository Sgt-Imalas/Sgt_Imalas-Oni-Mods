using ComplexFabricatorRibbonController.Content.Defs.Buildings;
using HarmonyLib;
using UtilLibs;

namespace ComplexFabricatorRibbonController.Patches
{
    class Db_Patches
    {

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
			{
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Computers.ParallelAutomation, ComplexFabricatorRecipeControlAttachmentConfig.ID);
			}
        }
    }
}
