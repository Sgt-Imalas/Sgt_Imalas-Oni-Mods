using HarmonyLib;
using Rockets_TinyYetBig;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RoboRockets.Rockets_TinyYetBig
{
    class AddBuildingsPatches
    {


        /// <summary>
        /// Adding Rocket buildings to build Screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {

                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleSmallExpandedConfig.ID, HabitatModuleSmallConfig.ID);
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleStargazerConfig.ID, NoseconeBasicConfig.ID);
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleMediumExpandedConfig.ID, HabitatModuleMediumConfig.ID);

                RocketryUtils.AddRocketModuleToBuildList(HEPBatteryModuleConfig.ID, BatteryModuleConfig.ID);
                RocketryUtils.AddRocketModuleToBuildList(CritterContainmentModuleConfig.ID, GasCargoBayClusterConfig.ID);

                RocketryUtils.AddRocketModuleToBuildList(NoseConeHEPHarvestConfig.ID, NoseconeHarvestConfig.ID);

                RocketryUtils.AddRocketModuleToBuildList(GeneratorTestConfig.ID, BatteryModuleConfig.ID);
                RocketryUtils.AddRocketModuleToBuildList(RTGModuleConfig.ID, BatteryModuleConfig.ID); 
                RocketryUtils.AddRocketModuleToBuildList(SteamGeneratorModuleConfig.ID, BatteryModuleConfig.ID);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, BunkeredLaunchPadConfig.ID,null,LaunchPadConfig.ID);
            }
        }

        /// <summary>
        /// Add new Buildings to Technologies
        /// </summary>
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.SpaceProgram, HabitatModuleSmallExpandedConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.DurableLifeSupport, HabitatModuleMediumExpandedConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.CelestialDetection, HabitatModuleStargazerConfig.ID);

                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadboltContainment, HEPBatteryModuleConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.AnimalControl, CritterContainmentModuleConfig.ID);

                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, NoseConeHEPHarvestConfig.ID);

                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadiationRefinement, RTGModuleConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, BunkeredLaunchPadConfig.ID);
            }
        }
    }
}
