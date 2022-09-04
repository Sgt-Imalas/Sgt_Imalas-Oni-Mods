using HarmonyLib;
using Robo_Rockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RoboRockets.Patches
{
    class AddBuildingsPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                RocketryUtils.AddRocketModuleToBuildList(AIControlModuleConfig.ID, "HabitatModuleMedium", RocketryUtils.RocketCategory.habitats);
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, AIControlModuleConfig.ID);
            }
        }
    }
}
