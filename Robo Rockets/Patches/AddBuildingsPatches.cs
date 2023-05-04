using HarmonyLib;
using RoboRockets.LearningBrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
                RocketryUtils.AddRocketModuleToBuildList(AIControlModuleLearningV2Config.ID, RocketryUtils.RocketCategory.habitats, "HabitatModuleMedium");
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
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, EarlyGameAIControlModuleConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, AINoseconeConfig.ID);
                //InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, AIControlModuleLearningConfig.ID);
                //InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, BrainConfig.ID);
            }
        }

        [HarmonyPatch(typeof(PropGravitasJar1Config))]
        [HarmonyPatch(nameof(PropGravitasJar1Config.CreatePrefab))]
        public class AddBrainDropperNo1
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGet<BrainDropperAddon>();
            }
        }
        [HarmonyPatch(typeof(GeneShufflerConfig))]
        [HarmonyPatch(nameof(GeneShufflerConfig.CreatePrefab))]
        public class AddBrainDropperNo2
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGet<BrainDropperAddon>();
            }
        }
    }
}
