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
using static UtilLibs.RocketryUtils;

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

            public static void Postfix()
            {
                CategorizeVanillaModules();
                if (Config.Instance.EnableExtendedHabs) 
                {
                    AddRocketModuleToBuildList(HabitatModuleSmallExpandedConfig.ID, new RocketCategory[] { RocketCategory.habitats,RocketCategory.nosecones}, HabitatModuleSmallConfig.ID);
                    AddRocketModuleToBuildList(HabitatModuleMediumExpandedConfig.ID, RocketryUtils.RocketCategory.habitats, HabitatModuleMediumConfig.ID);
                }

                if (Config.Instance.EnableStargazer)
                    AddRocketModuleToBuildList(HabitatModuleStargazerConfig.ID, new RocketCategory[] { RocketCategory.habitats, RocketCategory.nosecones }, NoseconeBasicConfig.ID);

                if (Config.Instance.EnableRadboltStorage)
                    AddRocketModuleToBuildList(HEPBatteryModuleConfig.ID, RocketryUtils.RocketCategory.cargo, GasCargoBayClusterConfig.ID);

                if (Config.Instance.EnableCritterStorage)
                    AddRocketModuleToBuildList(CritterContainmentModuleConfig.ID, RocketryUtils.RocketCategory.cargo, GasCargoBayClusterConfig.ID);

                if (Config.Instance.EnableLaserDrill)
                    AddRocketModuleToBuildList(NoseConeHEPHarvestConfig.ID, RocketCategory.nosecones, NoseconeHarvestConfig.ID);

                if (Config.Instance.EnableGenerators)
                {
                    AddRocketModuleToBuildList(CoalGeneratorModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID);
                    AddRocketModuleToBuildList(RTGModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID);
                    AddRocketModuleToBuildList(SteamGeneratorModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID); 
                }


                if (Config.Instance.EnableBunkerPlatform)
                    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, BunkeredLaunchPadConfig.ID,null,LaunchPadConfig.ID);


                if (Config.Instance.LandingLegs)
                    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, InvisibleLandingPlatformConfig.ID, null, LaunchPadConfig.ID);


                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, DockingTubeDoorConfig.ID,null, LaunchPadConfig.ID); 
                AddRocketModuleToBuildList(LandingLegConfig.ID, RocketCategory.utility); 
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
                if (Config.Instance.EnableExtendedHabs)
                {
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.SpaceProgram, HabitatModuleSmallExpandedConfig.ID);
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.DurableLifeSupport, HabitatModuleMediumExpandedConfig.ID);
                }
                if (Config.Instance.EnableStargazer)
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.CelestialDetection, HabitatModuleStargazerConfig.ID);

                if (Config.Instance.EnableRadboltStorage)
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadboltContainment, HEPBatteryModuleConfig.ID);

                if (Config.Instance.EnableCritterStorage)
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.AnimalControl, CritterContainmentModuleConfig.ID);

                if (Config.Instance.EnableLaserDrill)
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, NoseConeHEPHarvestConfig.ID);

                if (Config.Instance.EnableBunkerPlatform)
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, BunkeredLaunchPadConfig.ID);

                if (Config.Instance.EnableGenerators)
                {
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadboltPropulsion, RTGModuleConfig.ID);
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.RenewableEnergy, SteamGeneratorModuleConfig.ID);
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.AdvancedPowerRegulation, CoalGeneratorModuleConfig.ID);
                }
            }
        }
    }
}
