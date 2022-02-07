using HarmonyLib;
using KnastoronOniMods;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using UtilLibs;
namespace Robo_Rockets
{
    public class RoboRocketPatches
    {
        //[HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        //public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        //{
        //    public static void Postfix(Dictionary<string, CodexEntry> __result)
        //    {
        //        InjectionMethods.AddRobotStrings(AiBrainConfig.ID, AiBrainConfig.NAME, AiBrainConfig.DESCR);
        //        InjectionMethods.Action(AiBrainConfig.ID, AiBrainConfig.NAME, __result);
        //    }
        //}

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(RoboRocketConfig.ID, RoboRocketConfig.DisplayName, RoboRocketConfig.Description, RoboRocketConfig.Effect);
                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, RoboRocketConfig.ID);

                InjectionMethods.AddBuildingStrings(RocketAiControlstationConfig.ID, "Ai core");
                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, RocketAiControlstationConfig.ID);
            }
        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, RoboRocketConfig.ID);
            }
        }

        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch("OnSpawn")]
        public class SelectModuleSideScreen_OnSpawn_Patch
        {

            public static void Prefix()
            {
                int i = SelectModuleSideScreen.moduleButtonSortOrder.IndexOf("HabitatModuleMedium");
                int j = (i == -1) ? SelectModuleSideScreen.moduleButtonSortOrder.Count : ++i;
                SelectModuleSideScreen.moduleButtonSortOrder.Insert(j, RoboRocketConfig.ID);
            }
        }

        [HarmonyPatch(typeof(PassengerRocketModule))]
        [HarmonyPatch("CheckPassengersBoarded")]
        public class PassengerRocketModule_CheckPassengersBoarded_Patch
        {
            public static void Postfix(PassengerRocketModule __instance, ref bool __result)
            {
                if (__instance.GetType()== typeof(AIPassengerModule))
                {
                    __result = true;
                }
            }
        }
        [HarmonyPatch(typeof(PassengerRocketModule))]
        [HarmonyPatch("CheckPilotBoarded")]
        public class PassengerRocketModule_CheckPilotBoarded_Patch
        {
            public static void Postfix(PassengerRocketModule __instance, ref bool __result)
            {
                if (__instance.GetType() == typeof(AIPassengerModule))
                {
                    __result = true;
                }
            }
        }
        [HarmonyPatch(typeof(RocketControlStationConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public class RocketControlStationDoPostConfigureComplete_Patch
        {
            public static void Postfix(ref GameObject go)
            {
                if(go.GetComponent<RocketControlStation>())
                go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
            }
        }
    }
}
