using HarmonyLib;
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
                Debug.Log("Initialized");
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(RadiatorBaseConfig.ID, RadiatorBaseConfig.NAME);
               // InjectionMethods.AddBuildingStrings(RadiatorPanelConfig.ID, RadiatorPanelConfig.NAME);

                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RadiatorBaseConfig.ID);
                //InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RadiatorPanelConfig.ID);
            }
        }
    }
}
