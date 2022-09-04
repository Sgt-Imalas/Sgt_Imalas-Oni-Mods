using HarmonyLib;
using RadiatorMod.Buildings;
using RadiatorMod.Util;
using RoboRockets.Buildings;
using System;
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
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.DurableLifeSupport, HabitatMediumRadiator.ID);
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
                InjectionMethods.AddBuildingStrings(RadiatorBaseConfig.ID, RadiatorBaseConfig.NAME, RadiatorBaseConfig.DESC, RadiatorBaseConfig.EFFECT);
                if (DlcManager.IsExpansion1Active())
                {
                    InjectionMethods.AddBuildingStrings(RadiatorRocketWallConfig.ID, RadiatorRocketWallConfig.NAME, RadiatorRocketWallConfig.DESC, RadiatorRocketWallConfig.EFFECT);
                    InjectionMethods.AddBuildingStrings(HabitatMediumRadiator.ID, HabitatMediumRadiator.NAME, HabitatMediumRadiator.DESC, HabitatMediumRadiator.EFFECT);

                    //add special habitat module
                    RocketryUtils.AddRocketModuleToBuildList(HabitatMediumRadiator.ID, "HabitatModuleMedium", RocketryUtils.RocketCategory.habitats);
                }

                //add buildings to build menu
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RadiatorBaseConfig.ID);


                //StatusItemInit.

                InjectionMethods.AddStatusItem(RadiatorBase.InSpaceRadiating,  RadiatorBase.Category, "Radiating {0}",
                    "This radiator is currently radiating heat at {0}.");

                InjectionMethods.AddStatusItem(RadiatorBase.NotInSpace,  RadiatorBase.Category, "Not in space",
                    "This radiators panels are not fully exposed to space, thus it won't radiate any heat into space.");

                InjectionMethods.AddStatusItem(RadiatorBase.BunkerDown, RadiatorBase.Category, "Bunkered down",
                    "This radiator is currently protected from meteor impacts.");
            }
        }
    }
}
