using HarmonyLib;
using UtilLibs;

namespace Radiator_Mod
{
	public class RadiatorPatches
	{
		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				//add buildings to technology tree
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidTuning, RadiatorBaseConfig.ID);

				if (DlcManager.IsExpansion1Active())
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidTuning, RadiatorRocketWallBuildable.ID);
				}
				//Debug.Log("Initialized");
			}
		}

		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Prefix()
			{
				//add buildings to the game
				//InjectionMethods.AddBuildingStrings(RadiatorBaseConfig.ID, RadiatorBaseConfig.NAME, RadiatorBaseConfig.DESC, RadiatorBaseConfig.EFFECT);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, RadiatorBaseConfig.ID, SpaceHeaterConfig.ID);
				if (DlcManager.IsExpansion1Active())
				{
					//InjectionMethods.AddBuildingStrings(RadiatorRocketWallConfig.ID, RadiatorRocketWallConfig.NAME, RadiatorRocketWallConfig.DESC, RadiatorRocketWallConfig.EFFECT);
					//InjectionMethods.AddBuildingStrings(HabitatMediumRadiator.ID, HabitatMediumRadiator.NAME, HabitatMediumRadiator.DESC, HabitatMediumRadiator.EFFECT);

					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, RadiatorRocketWallBuildable.ID, RadiatorBaseConfig.ID);
					//add special habitat module
					//RocketryUtils.AddRocketModuleToBuildList(HabitatMediumRadiator.ID, "HabitatModuleMedium", RocketryUtils.RocketCategory.habitats);
				}

				//add buildings to build menu


				//StatusItemInit.

				InjectionMethods.AddStatusItem(RadiatorBase.InSpaceRadiating, RadiatorBase.Category, STRINGS.BUILDING.STATUSITEMS.RM_INSPACERADIATING.NAME,
					STRINGS.BUILDING.STATUSITEMS.RM_INSPACERADIATING.TOOLTIP);

				InjectionMethods.AddStatusItem(RadiatorBase.NotInSpace, RadiatorBase.Category, STRINGS.BUILDING.STATUSITEMS.RM_NOTINSPACE.NAME,
					STRINGS.BUILDING.STATUSITEMS.RM_NOTINSPACE.TOOLTIP);

				InjectionMethods.AddStatusItem(RadiatorBase.BunkerDown, RadiatorBase.Category, STRINGS.BUILDING.STATUSITEMS.RM_BUNKERDOWN.NAME,
					STRINGS.BUILDING.STATUSITEMS.RM_BUNKERDOWN.TOOLTIP);
			}
		}
	}
}
