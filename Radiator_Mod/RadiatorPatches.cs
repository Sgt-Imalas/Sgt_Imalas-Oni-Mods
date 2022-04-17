using HarmonyLib;
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
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidTuning, RadiatorBaseConfig.ID);
                //InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidTuning, RadiatorPanelConfig.ID);
                //Debug.Log("Initialized");
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(RadiatorBaseConfig.ID, RadiatorBaseConfig.NAME,RadiatorBaseConfig.DESC,RadiatorBaseConfig.EFFECT);
                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RadiatorBaseConfig.ID);

                InjectionMethods.AddStatusItem(RadiatorBase.InSpaceRadiating,  RadiatorBase.Category, "Radiating {0}",
                    "This radiator is currently radiating heat at {0}.");

                InjectionMethods.AddStatusItem(RadiatorBase.NotInSpace,  RadiatorBase.Category, "Not in space",
                    "This radiators panels are not fully exposed to space and won't radiate heat into space.");

                InjectionMethods.AddStatusItem(RadiatorBase.BunkerDown, RadiatorBase.Category, "Bunkered down",
                    "This radiator is currently protected from meteor impacts.");
            }
        }
    }
}
