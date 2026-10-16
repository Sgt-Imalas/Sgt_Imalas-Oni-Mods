using ComplexFabricatorRibbonController.Content.Defs.Buildings;
using HarmonyLib;
using UtilLibs;

namespace ComplexFabricatorRibbonController.Patches
{
    class GeneratedBuildings_Patches
    {

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Automation, ComplexFabricatorRecipeControlAttachmentConfig.ID, LogicRibbonWriterConfig.ID);
			}
		}
    }
}
