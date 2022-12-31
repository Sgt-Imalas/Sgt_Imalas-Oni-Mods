using HarmonyLib;
using Rockets_TinyYetBig;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
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
                    AddRocketModuleToBuildList(CritterStasisModuleConfig.ID, RocketryUtils.RocketCategory.cargo, GasCargoBayClusterConfig.ID);
                    //AddRocketModuleToBuildList(CritterContainmentModuleConfig.ID, RocketryUtils.RocketCategory.cargo, GasCargoBayClusterConfig.ID);

                if (Config.Instance.EnableLaserDrill)
                    AddRocketModuleToBuildList(NoseConeHEPHarvestConfig.ID, RocketCategory.nosecones, NoseconeHarvestConfig.ID);

                if (Config.Instance.EnableGenerators)
                {
                    AddRocketModuleToBuildList(CoalGeneratorModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID);
                    AddRocketModuleToBuildList(RTGModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID);
                    AddRocketModuleToBuildList(SteamGeneratorModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID); 
                }

                if (Config.Instance.EnableBunkerPlatform)
                    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, BunkeredLaunchPadConfig.ID, null, LaunchPadConfig.ID);

                if (Config.Instance.EnableSolarNosecone)
                    AddRocketModuleToBuildList(NoseConeSolarConfig.ID, new RocketCategory[] { RocketCategory.nosecones, RocketCategory.power }, NoseconeBasicConfig.ID);

                if (Config.Instance.EnableNatGasEngine) 
                AddRocketModuleToBuildList(NatGasEngineClusterConfig.ID, RocketCategory.engines, SteamEngineClusterConfig.ID);

                //if (Config.Instance.LandingLegs)
                //    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, InvisibleLandingPlatformConfig.ID, null, LaunchPadConfig.ID);
                //AddRocketModuleToBuildList(LandingLegConfig.ID, RocketCategory.utility);


                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, DockingTubeDoorConfig.ID,null, LaunchPadConfig.ID); 
               // ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, SpaceStationDockingDoorConfig.ID,null, LaunchPadConfig.ID);
               // AddRocketModuleToBuildList(SpaceStationBuilderModuleConfig.ID, RocketCategory.utility);

                if (Config.Instance.EnableWallAdapter)
                {
                    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, ConnectorWallAdapterConfig.ID, null, LandingBeaconConfig.ID);
                }

                if (Config.Instance.EnableFuelLoaders)
                {
                    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, UniversalFuelLoaderConfig.ID, null, LandingBeaconConfig.ID);
                    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, UniversalOxidizerLoaderConfig.ID, null, UniversalFuelLoaderConfig.ID);
                    ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, HEPFuelLoaderConfig.ID, null, UniversalOxidizerLoaderConfig.ID);
                }
                if (Config.Instance.EnableEarlyGameFuelTanks)
                {
                    AddRocketModuleToBuildList(CO2FuelTankConfig.ID, RocketryUtils.RocketCategory.fuel, CO2EngineConfig.ID);
                }
                //if (Config.Instance.EnableLargeCargoBays)
                //{
                //    AddRocketModuleToBuildList(SolidCargoBayClusterLargeConfig.ID, RocketCategory.cargo, GasCargoBayClusterConfig.ID);
                //    AddRocketModuleToBuildList(LiquidCargoBayClusterLargeConfig.ID, RocketCategory.cargo, SolidCargoBayClusterLargeConfig.ID);
                //    AddRocketModuleToBuildList(GasCargoBayClusterLargeConfig.ID, RocketCategory.cargo, LiquidCargoBayClusterLargeConfig.ID);
                //}

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
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.AnimalControl, CritterStasisModuleConfig.ID);
                    //InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.AnimalControl, CritterContainmentModuleConfig.ID);

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
                if (Config.Instance.EnableLargeCargoBays)
                {
                   // InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.AdvancedPowerRegulation, SolidCargoBayClusterLargeConfig.ID);
                }

                if (Config.Instance.EnableSolarNosecone)
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.SpacePower, NoseConeSolarConfig.ID);

                if (Config.Instance.EnableWallAdapter)
                {
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.TemperatureModulation, ConnectorWallAdapterConfig.ID);
                }

                if (Config.Instance.EnableFuelLoaders)
                {
                    //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, SolidOxidizerLoaderConfig.ID, null, LaunchPadConfig.ID);
                    //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, LiquidOxidizerLoaderConfig.ID, null, LaunchPadConfig.ID);
                    //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, GasFuelLoaderConfig.ID, null, LaunchPadConfig.ID);
                    //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, LiquidFuelLoaderConfig.ID, null, LaunchPadConfig.ID);
                    //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, SolidFuelLoaderConfig.ID, null, LaunchPadConfig.ID);
                }
                if (Config.Instance.EnableNatGasEngine)
                {
                    InjectionMethods.AddBuildingToTechnology("HydrocarbonPropulsion", NatGasEngineClusterConfig.ID);
                }

                if (Config.Instance.EnableEarlyGameFuelTanks)
                {
                    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.GasDistribution, CO2FuelTankConfig.ID);
                }
            }
        }
    }
}
