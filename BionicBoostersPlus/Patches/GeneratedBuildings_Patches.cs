using BionicBoostersPlus.Content.Defs.Buildings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace BionicBoostersPlus.Patches
{
	internal class GeneratedBuildings_Patches
	{

		[HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Postfix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Stations, BoosterRecyclerConfig.ID, AdvancedCraftingTableConfig.ID, ordering: ModUtil.BuildingOrdering.Before);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.BruteForceRefinement, BoosterRecyclerConfig.ID);

				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Stations, Mk3BoosterMakerConfig.ID, AdvancedCraftingTableConfig.ID, ordering: ModUtil.BuildingOrdering.After);
				
				if(DlcManager.IsExpansion1Active())
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadiationRefinement, Mk3BoosterMakerConfig.ID);
				else
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.SoundAmplifiers, Mk3BoosterMakerConfig.ID);
			}
		}
	}
}
