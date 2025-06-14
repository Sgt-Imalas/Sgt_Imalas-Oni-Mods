using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class MetalRefineryConfig_Patches
    {
        [HarmonyPatch(typeof(MetalRefineryConfig), nameof(MetalRefineryConfig.ConfigureBuildingTemplate))]
        public class MetalRefineryConfig_ConfigureBuildingTemplate_Patch
        {
            /// <summary>
            /// remove all vanilla recipes from the refinery, replace them with customized ones
            /// priority high to not clear modded recipes
            /// </summary>
            [HarmonyPriority(Priority.VeryHigh)]
            public static void Postfix(GameObject go)
            {
                var recipeManager = ComplexRecipeManager.Get();
                if (recipeManager == null)
                    return;
				foreach (var item in recipeManager.preProcessRecipes)
				{
                    item.fabricators.Remove(MetalRefineryConfig.ID);
				}
				Custom_MetalRefineryConfig.ConfigureRecipes(MetalRefineryConfig.ID);


				LiquidCooledRefinery liquidCooledRefinery = go.AddOrGet<LiquidCooledRefinery>();
				liquidCooledRefinery.heatedTemperature = 320.15f;
				liquidCooledRefinery.thermalFudge = 0.6f;
			}
        }

        /// <summary>
        /// Adjust metal refinery stats to mirror custom refinery
        /// </summary>
        [HarmonyPatch(typeof(MetalRefineryConfig), nameof(MetalRefineryConfig.CreateBuildingDef))]
        public class MetalRefineryConfig_CreateBuildingDef_Patch
        {
            public static void Postfix(BuildingDef __result)
			{
				__result.EnergyConsumptionWhenActive = 800f;
				__result.SelfHeatKilowattsWhenActive = 10f;
			}
        }
    }
}
