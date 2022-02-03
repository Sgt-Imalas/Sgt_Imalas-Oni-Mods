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

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(RoboRocketConfig.ID, RoboRocketConfig.DisplayName, RoboRocketConfig.Description, RoboRocketConfig.Effect);

                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, RoboRocketConfig.ID);

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
        public class RocketControlStationConfig_CheckPilotBoarded_Patch
        {
            public static bool Prefix(ref GameObject go)
            {
                go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
                //__result.AddOrGet<RocketControlStationIdleWorkable>().workLayer = Grid.SceneLayer.BuildingUse;
                //__result.AddOrGet<RocketControlStationLaunchWorkable>().workLayer = Grid.SceneLayer.BuildingUse;
                go.AddOrGet<RocketControlStation>();
                go.AddOrGetDef<PoweredController.Def>();
                go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RocketInterior);
                Debug.Log("Should Skip now abasada");
                return false;
            }
        }
        [HarmonyPatch(typeof(RocketControlStation))]
        [HarmonyPatch("CreateLaunchChore")]
        public class RocketControlStation_CreateLaunchChore_Patch
        {
            public static bool Prefix(RocketControlStation.StatesInstance smi, ref Chore __result)
            {
                if (smi == null)
                {
                    __result = null;
                    return false;
                }
                else return true;
            }
        }
    }
}
