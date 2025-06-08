using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UtilLibs.RocketryUtils;
using UtilLibs;
using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using HarmonyLib;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
    class BuildingInjection
	{
		public static void AddBuildingsToPlanscreen()
		{
			AddBuildingsToPlanscreen_ChemicalProcessingIndustrialOverhaul();
		}
		private static void AddBuildingsToPlanscreen_ChemicalProcessingIndustrialOverhaul()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_AdvancedKilnConfig.ID, KilnConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_AdvancedMetalRefineryConfig.ID, MetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_BallCrusherMillConfig.ID, MetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Oxygen, Chemical_Co2PumpConfig.ID, CO2ScrubberConfig.ID);

			if (DlcManager.IsExpansion1Active())
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Co2RecyclerDLC1Config.ID, OxyliteRefineryConfig.ID);
			else
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Co2RecyclerConfig.ID, OxyliteRefineryConfig.ID);

			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Coal_BoilerConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_CrudeOilRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_ElectricBoilerConfig.ID, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, Chemical_EndothermicUnitConfig.ID, LiquidConditionerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_FlocculationSieveConfig.ID, WaterPurifierConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Gas_BoilerConfig.ID, Chemical_ElectricBoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_GlassFoundryConfig.ID, GlassForgeConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SmallCrusherMillConfig.ID, RockCrusherConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_NaphthaReformerConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_PropaneReformerConfig.ID, Chemical_NaphthaReformerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_PyrolysisKilnConfig.ID, KilnConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_RawGasRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_RayonLoomConfig.ID, EthanolDistilleryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SelectiveArcFurnaceConfig.ID, SupermaterialRefineryConfig.ID, ordering:ModUtil.BuildingOrdering.Before) ;


		}


		public static void AddBuildingsToTech()
		{
			AddBuildingsToTech_ChemicalProcessingIndustrialOverhaul();
		}
		private static void AddBuildingsToTech_ChemicalProcessingIndustrialOverhaul()
		{
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, Chemical_AdvancedKilnConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_AdvancedMetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_BallCrusherMillConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, Chemical_Co2PumpConfig.ID);

			if (DlcManager.IsExpansion1Active())
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.PortableGasses, Chemical_Co2RecyclerDLC1Config.ID);
			else
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.PortableGasses, Chemical_Co2RecyclerConfig.ID);

			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_CrudeOilRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_ElectricBoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidTuning, Chemical_EndothermicUnitConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidBasedRefinementProcess, Chemical_FlocculationSieveConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_Gas_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_GlassFoundryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.BruteForceRefinement, Chemical_SmallCrusherMillConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_NaphthaReformerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_PropaneReformerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.BruteForceRefinement, Chemical_PyrolysisKilnConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_RawGasRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.TextileProduction, Chemical_RayonLoomConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, Chemical_SelectiveArcFurnaceConfig.ID);
		}
	}
}
