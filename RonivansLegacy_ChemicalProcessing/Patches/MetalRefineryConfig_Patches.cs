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
			/// priority high to not clear modded recipes that are added afterwards
			/// </summary>
			[HarmonyPriority(Priority.VeryHigh)]
			public static void Postfix(GameObject go)
			{
				if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				{
					var recipeManager = ComplexRecipeManager.Get();
					if (recipeManager == null)
						return;
					recipeManager.preProcessRecipes.RemoveWhere(r => r.fabricators.Contains(MetalRefineryConfig.ID));

					Custom_MetalRefineryConfig.ConfigureRecipes(MetalRefineryConfig.ID);


					LiquidCooledRefinery liquidCooledRefinery = go.AddOrGet<LiquidCooledRefinery>();
					liquidCooledRefinery.heatedTemperature = 320.15f;
					liquidCooledRefinery.thermalFudge = Config.Instance.ChemProc_RefineryFudge;
				}
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
				if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled) 
				{
					__result.EnergyConsumptionWhenActive = 800f;
					__result.SelfHeatKilowattsWhenActive = 10f;
				}
			}
		}
	}
}
