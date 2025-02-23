using HarmonyLib;
using Rockets_TinyYetBig;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Engines;
using Rockets_TinyYetBig.Buildings.Fuel;
using Rockets_TinyYetBig.Buildings.Generators;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.Buildings.Nosecones;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using UtilLibs;
using static UtilLibs.RocketryUtils;

namespace RoboRockets.Rockets_TinyYetBig
{
	class AddBuildingsPatches
	{


		/// <summary>
		/// This patch registers all buildings to the build screen 
		/// and all rocket modules to the rocket module selection screen and their building categories, added by RE 
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Postfix()
			{
				//Assign categories to each vanilla module
				CategorizeVanillaModules();

				AddRocketModuleToBuildList(SolarPanelModuleWideConfig.ID, RocketCategory.power, SolarPanelModuleConfig.ID);


				if (Config.Instance.EnableExtendedHabs)
				{
					AddRocketModuleToBuildList(HabitatModuleSmallExpandedConfig.ID, new RocketCategory[] { RocketCategory.habitats, RocketCategory.nosecones }, HabitatModuleSmallConfig.ID);
					AddRocketModuleToBuildList(HabitatModuleMediumExpandedConfig.ID, RocketryUtils.RocketCategory.habitats, HabitatModuleMediumConfig.ID);
					AddRocketModuleToBuildList(HabitatModuleStargazerConfig.ID, new RocketCategory[] { RocketCategory.habitats, RocketCategory.nosecones }, NoseconeBasicConfig.ID);
					AddRocketModuleToBuildList(HabitatModulePlatedNoseconeLargeConfig.ID, new RocketCategory[] { RocketCategory.habitats, RocketCategory.nosecones }, HabitatModuleSmallExpandedConfig.ID);
				}

				//if (Config.Instance.Boosters)
				//{
				//    AddRocketModuleToBuildList(PetroleumBoosterClusterConfig.ID, RocketryUtils.RocketCategory.engines, KeroseneEngineClusterConfig.ID);
				//    AddRocketModuleToBuildList(IonEngineBoosterClusterConfig.ID, RocketryUtils.RocketCategory.engines, HydrogenEngineClusterConfig.ID);
				//}
				//if (Config.Instance.EnableElectricEngine)
				//{
				//    AddRocketModuleToBuildList(IonEngineClusterConfig.ID, RocketryUtils.RocketCategory.engines, HydrogenEngineClusterConfig.ID);
				//}

				if (Config.Instance.EnableRadboltStorage)
					AddRocketModuleToBuildList(HEPBatteryModuleConfig.ID, RocketryUtils.RocketCategory.cargo, GasCargoBayClusterConfig.ID);

				if (Config.Instance.EnableCritterStorage)
					AddRocketModuleToBuildList(CritterStasisModuleConfig.ID, RocketryUtils.RocketCategory.cargo, GasCargoBayClusterConfig.ID);


				if (Config.Instance.EnableFridge)
				{
					AddRocketModuleToBuildList(FridgeModuleConfig.ID, new RocketCategory[] { RocketCategory.cargo, RocketCategory.utility }, ArtifactCargoBayConfig.ID, true);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Food, FridgeModuleAccessHatchConfig.ID, RefrigeratorConfig.ID);
				}


				if (Config.Instance.EnableLaserDrill)
				{
					AddRocketModuleToBuildList(NoseConeHEPHarvestConfig.ID, RocketCategory.nosecones, NoseconeHarvestConfig.ID);
				}
				if (Config.Instance.EnableDrillSupport)
				{
					AddRocketModuleToBuildList(DrillconeStorageModuleConfig.ID, new RocketCategory[] { RocketCategory.cargo, RocketCategory.utility }, NoseconeHarvestConfig.ID);
				}


				if (Config.Instance.EnableGenerators)
				{
					AddRocketModuleToBuildList(CoalGeneratorModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID);
					AddRocketModuleToBuildList(RTGModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID);
					AddRocketModuleToBuildList(SteamGeneratorModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID);
				}

				if (Config.Instance.EnableBunkerPlatform)
				{
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, BunkeredLaunchPadConfig.ID, LaunchPadConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, AdvancedLaunchPadConfig.ID, LaunchPadConfig.ID);
					//AddRocketModuleToBuildList(PlatformDeployerModuleConfig.ID, new RocketCategory[] { RocketCategory.deployables, RocketCategory.utility }, PioneerModuleConfig.ID);
				}

				if (Config.Instance.EnableSolarNosecone)
					AddRocketModuleToBuildList(NoseConeSolarConfig.ID, new RocketCategory[] { RocketCategory.nosecones, RocketCategory.power }, NoseconeBasicConfig.ID);

				if (Config.Instance.EnableSmolBattery)
					AddRocketModuleToBuildList(SmolBatteryModuleConfig.ID, RocketCategory.power, BatteryModuleConfig.ID, true);

				if (Config.Instance.EnableNatGasEngine)
					AddRocketModuleToBuildList(NatGasEngineClusterConfig.ID, RocketCategory.engines, SteamEngineClusterConfig.ID);

				//if (Config.Instance.LandingLegs)
				//    InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, InvisibleLandingPlatformConfig.ID, null, LaunchPadConfig.ID);
				//AddRocketModuleToBuildList(LandingLegConfig.ID, RocketCategory.utility);

				if (Config.Instance.RocketDocking)
				{

					//AddRocketModuleToBuildList(AI_DockingPortConfig.ID, RocketCategory.utility, ScannerModuleConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, DockingTubeDoorConfig.ID, GantryConfig.ID);
				}
				if (
					Config.SpaceStationsPossible
					)
				{
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, SpaceStationDockingDoorConfig.ID, DockingTubeDoorConfig.ID);
					AddRocketModuleToBuildList(SpaceStationBuilderModuleConfig.ID, new RocketCategory[] { RocketCategory.deployables, RocketCategory.utility }, OrbitalCargoModuleConfig.ID);
				}

				if (Config.Instance.EnableWallAdapter)
				{
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, VerticalAdapterPieceConfig.ID, ModularLaunchpadPortBridgeConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, VerticalAdapterBaseConfig.ID, ModularLaunchpadPortBridgeConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, LoaderLadderAdapterConfig.ID, ModularLaunchpadPortBridgeConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, ConnectorWallAdapterConfig.ID, ModularLaunchpadPortBridgeConfig.ID);
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Rocketry, ConnectorWallAdapterBunkerConfig.ID, ModularLaunchpadPortBridgeConfig.ID);
					//InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, LoaderTravelTubeAdapterConfig.ID, "rocketfueling"); ///Too buggy atm

				}

				if (Config.Instance.EnableFuelLoaders)
				{
					InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, UniversalFuelLoaderConfig.ID, "rocketfueling");
					InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, UniversalOxidizerLoaderConfig.ID, "rocketfueling");
					InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, HEPFuelLoaderConfig.ID, "rocketfueling");
				}
				if (Config.Instance.EnableEarlyGameFuelTanks)
				{
					AddRocketModuleToBuildList(CO2FuelTankConfig.ID, RocketryUtils.RocketCategory.fuel, CO2EngineConfig.ID);
					AddRocketModuleToBuildList(LiquidFuelTankClusterSmallConfig.ID, RocketryUtils.RocketCategory.fuel, LiquidFuelTankClusterConfig.ID, true);
					AddRocketModuleToBuildList(LiquidChlorineOxidizerTankConfig.ID, RocketryUtils.RocketCategory.fuel, OxidizerTankLiquidClusterConfig.ID, true);
				}
				if (Config.Instance.EnableLargeCargoBays)
				{
					AddRocketModuleToBuildList(SolidCargoBayClusterLargeConfig.ID, RocketCategory.cargo, GasCargoBayClusterConfig.ID);
					AddRocketModuleToBuildList(LiquidCargoBayClusterLargeConfig.ID, RocketCategory.cargo, SolidCargoBayClusterLargeConfig.ID);
					AddRocketModuleToBuildList(GasCargoBayClusterLargeConfig.ID, RocketCategory.cargo, LiquidCargoBayClusterLargeConfig.ID);
				}

				if (Config.Instance.EnablePOISensor)
					InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Automation, POICapacitySensorConfig.ID, CometDetectorConfig.ID);
				return;
				//InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, PartWorkshopConfig.ID, "stationParts");
				//InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, Part_A_1_Config.ID, "stationParts");
				//InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, Part_A_2_Config.ID, "stationParts");
				// InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, Part_A_3_Config.ID, "stationParts");
			}
		}

		/// <summary>
		/// Register Buildings to existing Technologies (newly added techs are in "ResearchTreePatches" class
		/// </summary>
		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.SpacePower, SolarPanelModuleWideConfig.ID);
				if (Config.Instance.EnableExtendedHabs)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.CrashPlan, HabitatModuleSmallExpandedConfig.ID);
					//InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.DurableLifeSupport, HabitatModuleMediumExpandedConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.CelestialDetection, HabitatModuleStargazerConfig.ID);
				}

				if (Config.Instance.EnableRadboltStorage)
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadboltContainment, HEPBatteryModuleConfig.ID);

				if (Config.Instance.EnableCritterStorage)
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.AnimalControl, CritterStasisModuleConfig.ID);
				//InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.AnimalControl, CritterContainmentModuleConfig.ID);

				if (Config.Instance.EnableFridge)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.FoodRepurposing, FridgeModuleConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.FoodRepurposing, FridgeModuleAccessHatchConfig.ID);
				}
				//if (Config.Instance.Boosters)
				//{
				//    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.HydrocarbonPropulsion, PetroleumBoosterClusterConfig.ID);
				//    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.CryoFuelPropulsion, IonEngineBoosterClusterConfig.ID);
				//}
				//if (Config.Instance.EnableElectricEngine)
				//{
				//    InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.CryoFuelPropulsion, IonEngineClusterConfig.ID);
				//}


				if (Config.Instance.EnableLaserDrill)
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, NoseConeHEPHarvestConfig.ID);

				if (Config.Instance.EnableDrillSupport)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, DrillconeStorageModuleConfig.ID);
				}

				if (Config.Instance.EnableBunkerPlatform)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, BunkeredLaunchPadConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Computers.ParallelAutomation, AdvancedLaunchPadConfig.ID);
				}

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

				if (Config.Instance.EnableSmolBattery)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.SoundAmplifiers, SmolBatteryModuleConfig.ID);

				}
				if (Config.Instance.EnableSmolBattery || Config.Instance.HabitatPowerPlug)
				{
					InjectionMethods.MoveItemToNewTech(RocketInteriorPowerPlugConfig.ID, GameStrings.Technology.Power.SpacePower, GameStrings.Technology.Power.AdvancedPowerRegulation);
				}

				if (Config.Instance.EnableWallAdapter)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.TemperatureModulation, ConnectorWallAdapterConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, ConnectorWallAdapterBunkerConfig.ID);					
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, LoaderLadderAdapterConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, VerticalAdapterBaseConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, VerticalAdapterPieceConfig.ID);
				}

				if (Config.Instance.EnableNatGasEngine)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.GourmetMealPreparation, NatGasEngineClusterConfig.ID);
				}

				if (Config.Instance.EnableEarlyGameFuelTanks)
				{
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.GasDistribution, CO2FuelTankConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidTuning, LiquidFuelTankClusterSmallConfig.ID);
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.ImprovedHydrocarbonPropulsion, LiquidChlorineOxidizerTankConfig.ID);
				}
				if (Config.Instance.EnablePOISensor)
					InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Computers.SensitiveMicroimaging, POICapacitySensorConfig.ID);
			}
		}
	}
}
