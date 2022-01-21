using HarmonyLib;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static Robo_Rockets.Utils;
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
                AddBuildingStrings(RoboRocketConfig.ID, RoboRocketConfig.DisplayName, RoboRocketConfig.Description, RoboRocketConfig.Effect);

                AddBuildingToPlanScreen(Utils.GameStrings.PlanMenuCategory.Rocketry, RoboRocketConfig.ID);

            }
        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, RoboRocketConfig.ID);
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

        
    }
}
