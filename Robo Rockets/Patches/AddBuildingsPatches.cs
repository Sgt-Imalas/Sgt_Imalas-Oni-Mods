using HarmonyLib;
using RoboRockets.LearningBrain;
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
                RocketryUtils.AddRocketModuleToBuildList(AIControlModuleLearningConfig.ID, RocketryUtils.RocketCategory.habitats, "HabitatModuleMedium");
                RocketryUtils.AddRocketModuleToBuildList(AINoseconeConfig.ID, new[] { RocketryUtils.RocketCategory.habitats, RocketryUtils.RocketCategory.nosecones }, "HabitatModuleMedium");
                RocketryUtils.AddRocketModuleToBuildList(EarlyGameAIControlModuleConfig.ID, RocketryUtils.RocketCategory.habitats, "HabitatModuleMedium");
            }
        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityTransport, EarlyGameAIControlModuleConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityTransport, AINoseconeConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, AIControlModuleLearningConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, BrainConfig.ID);
            }
        }
    }
}
