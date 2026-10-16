using HarmonyLib;
using UtilLibs;
using WallAttachmentPumps.Content.Defs;

namespace WallAttachmentPumps.Patches
{
	internal class GeneratedBuildings_Patch
	{

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Ventilation, WallAttachmentPumpGasConfig.ID,  GasPumpConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Plumbing, WallAttachmentPumpLiquidConfig.ID,  LiquidPumpConfig.ID);

				InjectionMethods.AddBuildingToTechnology( GameStrings.Technology.Power.ValveMiniaturization, WallAttachmentPumpGasConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.ValveMiniaturization, WallAttachmentPumpLiquidConfig.ID);
			}
		}
	}
}
