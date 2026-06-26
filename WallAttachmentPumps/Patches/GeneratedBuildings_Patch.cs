using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

				//InjectionMethods.AddBuildingToTechnology( GameStrings.Technology.Decor.FineArt, FountainConfig.ID);
				//InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, FloorLightConfig.ID);
			}
		}
	}
}
