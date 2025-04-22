using HarmonyLib;
using Imalas_TwitchChaosEvents.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.ModPatches
{
    class GeneratedBuildings_Patches
	{
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Oxygen, InvertedElectrolyzerConfig.ID, ElectrolyzerConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, InvertedWaterPurifierConfig.ID, WaterPurifierConfig.ID);
			}
		}

	}
}
