using HarmonyLib;
using NaturalConstruction.Content.Defs;
using UtilLibs;

namespace NaturalConstruction.Patches
{
	internal class GeneratedBuildings_Patches
	{

		[HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Prefix()
			{
				bool planters = Config.Instance.EnableWildPlanters;

				//build menu
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, NaturalTileBuildingConfig.ID, TileConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, NaturalBackwallBuildingConfig.ID, ExteriorWallConfig.ID);

				if (planters)
				{
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Farming, NaturalTilePlanterConfig.ID, FarmTileConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Farming, NaturalTilePlanterWideConfig.ID, WideFarmTileConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Farming, NaturalTilePlanterBackwallConfig.ID, LargeBackwallFarmConfig.ID);
				}

				//Techs
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.BasicFarming, NaturalTileBuildingConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.BasicFarming, NaturalBackwallBuildingConfig.ID);

				if (planters)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.MealPreparation, NaturalTilePlanterConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, NaturalTilePlanterWideConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, NaturalTilePlanterBackwallConfig.ID);
				}
			}
		}
	}
}
