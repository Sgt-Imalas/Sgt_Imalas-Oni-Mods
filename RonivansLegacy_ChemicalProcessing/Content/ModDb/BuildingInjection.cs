using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UtilLibs.RocketryUtils;
using UtilLibs;
using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
    class BuildingInjection
	{
		public static void AddBuildingsToPlanscreen()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_AdvancedKilnConfig.ID, KilnConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_AdvancedMetalRefineryConfig.ID, MetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_BallCrusherMillConfig.ID, MetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Oxygen, Chemical_Co2PumpConfig.ID, CO2ScrubberConfig.ID);

			if(DlcManager.IsExpansion1Active())
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Co2RecyclerDLC1Config.ID, OxyliteRefineryConfig.ID);
			else
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Co2RecyclerConfig.ID, OxyliteRefineryConfig.ID);
		}

		public static void AddBuildingsToTech()
		{
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, Chemical_AdvancedKilnConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_AdvancedMetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_BallCrusherMillConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, Chemical_Co2PumpConfig.ID);

			if(DlcManager.IsExpansion1Active())
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.PortableGasses, Chemical_Co2RecyclerDLC1Config.ID);
			else
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.PortableGasses, Chemical_Co2RecyclerConfig.ID);
		}
	}
}
