using ComplexFabricatorRibbonController.Content.Defs.Buildings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
