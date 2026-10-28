using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Engines;
using Rockets_TinyYetBig.Buildings.Fuel;
using Rockets_TinyYetBig.Buildings.Generators;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.Buildings.Nosecones;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.Defs.Buildings;
using Rockets_TinyYetBig.Content.Defs.Buildings.DerelictPropBuildings;
using Rockets_TinyYetBig.Content.Defs.Buildings.Research;
using Rockets_TinyYetBig.Content.Defs.Buildings.RocketModules.CargoBays;
using Rockets_TinyYetBig.Content.Defs.Buildings.RocketModuleUpgrades;
using Rockets_TinyYetBig.Content.Defs.Buildings.StationParts;
using Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using UtilLibs;
using static UtilLibs.GameStrings;
using static UtilLibs.RocketryUtils;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class BuildingDatabase
	{
		const string
			Category_FuelLoaders = "rocketfueling",
			Category_ModuleUpgrades = "rtb_moduleupgrades",
			Category_StationParts = "rtb_stationparts"
			;

		const string Sync_Freezer = "RTB_FreezerModuleGroup";

		public static void RegisterBuildings()
		{
			CategorizeVanillaModules();

			BuildingManager.LoadConfigFile();
			RegisterBuildings_RocketModules();
			RegisterBuildings_RegularBuildings();
			RegisterBuildings_DevBuildings();
			RegisterBuildings_StationParts();
			RegisterBuildings_ModuleUpgrades();
		}

		static void RegisterBuildings_RocketModules()
		{
			BuildingManager.CreateEntry<AIOCargoBayModuleConfig>()
			.AddToCategory(RocketCategory.cargo, ArtifactCargoBayConfig.ID, ModUtil.BuildingOrdering.Before)
			.AddToTech(Technology.ColonyDevelopment.DurableLifeSupport)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			//BuildingManager.CreateEntry<SolarPanelModuleWideConfig>()
			//.AddToCategory(RocketCategory.power, SolarPanelModuleConfig.ID)
			//.AddToTech(Technology.Power.SpacePower)
			//.PrimaryConfigCategory(RTB_BuildingCategoryType.D_POWERUTILITY);

			BuildingManager.CreateEntry<HabitatModulePlatedNoseconeLargeConfig>()
			.ConfigEnabled(() => Config.Instance.EnableExtendedHabs)
			.AddToCategory([RocketCategory.habitats, RocketCategory.nosecones], HabitatModuleSmallConfig.ID)
			.AddToTech(ModAssets.Techs.LargerRocketLivingSpaceTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);

			BuildingManager.CreateEntry<HabitatModuleSmallExpandedConfig>()
			.AddToCategory([RocketCategory.habitats, RocketCategory.nosecones], HabitatModuleSmallConfig.ID)
			.AddToTech(Technology.ColonyDevelopment.CrashPlan)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);

			BuildingManager.CreateEntry<HabitatModuleMediumExpandedConfig>()
			.ConfigEnabled(()=>Config.Instance.EnableExtendedHabs)
			.AddToCategory(RocketCategory.habitats, HabitatModuleMediumConfig.ID)
			.AddToTech(ModAssets.Techs.LargerRocketLivingSpaceTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);

			BuildingManager.CreateEntry<HabitatModuleStargazerConfig>()
			.ConfigEnabled(() => Config.Instance.EnableExtendedHabs)
			.AddToCategory([RocketCategory.habitats, RocketCategory.nosecones], NoseconeBasicConfig.ID)
			.AddToTech(Technology.ColonyDevelopment.CelestialDetection)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);
			BuildingManager.CreateEntry<HEPBatteryModuleConfig>()

			.AddToCategory(RocketCategory.cargo, GasCargoBayClusterConfig.ID)
			.AddToTech(Technology.RadiationTechnologies.RadboltContainment)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<CritterStasisModuleConfig>()
			.AddToCategory(RocketCategory.cargo, SpecialCargoBayClusterConfig.ID)
			.AddToTech(Technology.Food.AnimalControl)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<FridgeModuleConfig>()
			.CategoryLink(Sync_Freezer)
			.AddToCategory([RocketCategory.cargo, RocketCategory.utility], ArtifactCargoBayConfig.ID, ModUtil.BuildingOrdering.Before)
			.AddToTech(Technology.Food.FoodRepurposing)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<NoseConeHEPHarvestConfig>()
			.AddToCategory(RocketCategory.nosecones, NoseconeHarvestConfig.ID)
			.AddToTech(Technology.SolidMaterial.HighVelocityDestruction)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<DrillconeStorageModuleConfig>()
			.AddToCategory([RocketCategory.cargo, RocketCategory.utility], NoseconeHarvestConfig.ID)
			.AddToTech(Technology.SolidMaterial.HighVelocityDestruction)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<CoalGeneratorModuleConfig>()
			.ConfigEnabled(() => Config.Instance.EnableGenerators)
			.AddToCategory(RocketCategory.power, BatteryModuleConfig.ID)
			.AddToTech(Technology.Power.AdvancedPowerRegulation)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.D_POWERUTILITY);

			BuildingManager.CreateEntry<RTGModuleConfig>()
			.ConfigEnabled(() => Config.Instance.EnableGenerators)
			.AddToCategory(RocketCategory.power, BatteryModuleConfig.ID)
			.AddToTech(Technology.RadiationTechnologies.RadboltPropulsion)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.D_POWERUTILITY);

			BuildingManager.CreateEntry<SteamGeneratorModuleConfig>()
			.ConfigEnabled(() => Config.Instance.EnableGenerators)
			.AddToCategory(RocketCategory.power, BatteryModuleConfig.ID)
			.AddToTech(Technology.Power.RenewableEnergy)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.D_POWERUTILITY);

			BuildingManager.CreateEntry<NoseConeSolarConfig>()
			.AddToCategory([RocketCategory.nosecones, RocketCategory.power], NoseconeBasicConfig.ID)
			.AddToTech(Technology.Power.SpacePower)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.D_POWERUTILITY);

			BuildingManager.CreateEntry<SmolBatteryModuleConfig>()
			.AddToCategory(RocketCategory.power, BatteryModuleConfig.ID, ModUtil.BuildingOrdering.Before)
			.AddToTech(Technology.Power.SoundAmplifiers)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.D_POWERUTILITY);

			BuildingManager.CreateEntry<NatGasEngineClusterConfig>()
			.AddToCategory(RocketCategory.engines, SteamEngineClusterConfig.ID)
			.AddToTech(Technology.Food.GourmetMealPreparation)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);

			BuildingManager.CreateEntry<CO2FuelTankConfig>()
			.AddToCategory(RocketCategory.fuel, CO2EngineConfig.ID)
			.AddToTech(Technology.Gases.GasDistribution)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);

			BuildingManager.CreateEntry<LiquidFuelTankClusterSmallConfig>()
			.AddToCategory(RocketCategory.fuel, LiquidFuelTankClusterConfig.ID, ModUtil.BuildingOrdering.Before)
			.AddToTech(Technology.Liquids.LiquidTuning)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);

			BuildingManager.CreateEntry<LiquidChlorineOxidizerTankConfig>()
			.AddToCategory(RocketCategory.fuel, OxidizerTankLiquidClusterConfig.ID, ModUtil.BuildingOrdering.Before)
			.AddToTech(Technology.Power.ImprovedHydrocarbonPropulsion)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.A_ROCKETRYPLUS);

			BuildingManager.CreateEntry<GasCargoBayClusterLargeConfig>()
			.ConfigEnabled(() => Config.Instance.EnableLargeCargoBays)
			.AddToCategory(RocketCategory.cargo, GasCargoBayClusterConfig.ID)
			.AddToTech(ModAssets.Techs.HugeCargoBayTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<LiquidCargoBayClusterLargeConfig>()
			.ConfigEnabled(() => Config.Instance.EnableLargeCargoBays)
			.AddToCategory(RocketCategory.cargo, GasCargoBayClusterConfig.ID)
			.AddToTech(ModAssets.Techs.HugeCargoBayTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<SolidCargoBayClusterLargeConfig>()
			.ConfigEnabled(() => Config.Instance.EnableLargeCargoBays)
			.AddToCategory(RocketCategory.cargo, GasCargoBayClusterConfig.ID)
			.AddToTech(ModAssets.Techs.HugeCargoBayTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			BuildingManager.CreateEntry<SpaceStationBuilderModuleConfig>()
			.ConfigEnabled(() => Config.SpaceStationsPossible)
			.AddToCategory([RocketCategory.deployables, RocketCategory.utility], OrbitalCargoModuleConfig.ID)
			.AddToTech(ModAssets.Techs.SpaceStationTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.E_SPACEEXPANSION);
		}

		static void RegisterBuildings_RegularBuildings()
		{
			//launchpads
			BuildingManager.CreateEntry<BunkeredLaunchPadConfig>()
			.AddToCategory(PlanMenuCategory.Rocketry, LaunchPadConfig.ID)
			.AddToTech(Technology.SolidMaterial.SuperheatedForging)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<AdvancedLaunchPadConfig>()
			.AddToCategory(PlanMenuCategory.Rocketry, LaunchPadConfig.ID)
			.AddToTech(Technology.Computers.ParallelAutomation)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			//docking
			BuildingManager.CreateEntry<DockingTubeDoorConfig>()
			.ConfigEnabled(() => Config.Instance.RocketDocking)
			.AddToCategory(PlanMenuCategory.Rocketry, GantryConfig.ID)
			.AddToTech(ModAssets.Techs.DockingTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.E_SPACEEXPANSION);

			//fridge
			BuildingManager.CreateEntry<FridgeModuleAccessHatchConfig>()
			.CategoryLink(Sync_Freezer)
			.AddToCategory(PlanMenuCategory.Food, RefrigeratorConfig.ID)
			.AddToTech(Technology.Food.FoodRepurposing)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);

			//wall adapters

			BuildingManager.CreateEntry<VerticalAdapterBaseConfig>()
			.ConfigEnabled(() => Config.Instance.EnableWallAdapter)
			.AddToCategory(PlanMenuCategory.Rocketry, ModularLaunchpadPortBridgeConfig.ID)
			.AddToTech(Technology.SolidMaterial.Smelting)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<VerticalAdapterPieceConfig>()
			.ConfigEnabled(() => Config.Instance.EnableWallAdapter)
			.AddToCategory(PlanMenuCategory.Rocketry, ModularLaunchpadPortBridgeConfig.ID)
			.AddToTech(Technology.SolidMaterial.Smelting)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<LoaderLadderAdapterConfig>()
			.ConfigEnabled(() => Config.Instance.EnableWallAdapter)
			.AddToCategory(PlanMenuCategory.Rocketry, ModularLaunchpadPortBridgeConfig.ID)
			.AddToTech(Technology.SolidMaterial.Smelting)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<ReinforcedLadderConfig>()
			.ConfigEnabled(() => Config.Instance.EnableWallAdapter)
			.AddToCategory(PlanMenuCategory.Base, LadderConfig.ID)
			.AddToTech(Technology.SolidMaterial.Smelting)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<LoaderFirepoleAdapterConfig>()
			.ConfigEnabled(() => Config.Instance.EnableWallAdapter)
			.AddToCategory(PlanMenuCategory.Rocketry, ModularLaunchpadPortBridgeConfig.ID)
			.AddToTech(Technology.SolidMaterial.RefinedRenovations)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);


			BuildingManager.CreateEntry<ConnectorWallAdapterConfig>()
			.ConfigEnabled(() => Config.Instance.EnableWallAdapter)
			.AddToCategory(PlanMenuCategory.Rocketry, ModularLaunchpadPortBridgeConfig.ID)
			.AddToTech(Technology.Gases.TemperatureModulation)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<ConnectorWallAdapterBunkerConfig>()
			.ConfigEnabled(() => Config.Instance.EnableWallAdapter)
			.AddToCategory(PlanMenuCategory.Rocketry, ModularLaunchpadPortBridgeConfig.ID)
			.AddToTech(Technology.SolidMaterial.SuperheatedForging)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			//fuel loaders
			BuildingManager.CreateEntry<UniversalFuelLoaderConfig>()
			.ConfigEnabled(() => Config.Instance.EnableFuelLoaders)
			.AddToCategory(PlanMenuCategory.Rocketry, string.Empty)
			.SubCategory(Category_FuelLoaders)
			.AddToTech(ModAssets.Techs.FuelLoaderTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<UniversalOxidizerLoaderConfig>()
			.ConfigEnabled(() => Config.Instance.EnableFuelLoaders)
			.AddToCategory(PlanMenuCategory.Rocketry, string.Empty)
			.SubCategory(Category_FuelLoaders)
			.AddToTech(ModAssets.Techs.FuelLoaderTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			BuildingManager.CreateEntry<HEPFuelLoaderConfig>()
			.ConfigEnabled(() => Config.Instance.EnableFuelLoaders)
			.AddToCategory(PlanMenuCategory.Rocketry, string.Empty)
			.SubCategory(Category_FuelLoaders)
			.AddToTech(ModAssets.Techs.FuelLoaderTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.C_FUELLOGISTICS);

			//sensors
			BuildingManager.CreateEntry<POICapacitySensorConfig>()
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING)
			.AddToCategory(PlanMenuCategory.Automation, CometDetectorConfig.ID)
			.AddToTech(Technology.Computers.SensitiveMicroimaging);

			BuildingManager.CreateEntry<RocketLogicLaunchConditionSetterConfig>()
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING)
			.AddToCategory(PlanMenuCategory.Rocketry, LogicClusterLocationSensorConfig.ID)
			.AddToTech(Technology.Computers.Computing);

			//space stations
			BuildingManager.CreateEntry<DeepSpaceResearchCenterConfig>()
			.ConfigEnabled(() => Config.SpaceStationsPossible)
			.AddToCategory(PlanMenuCategory.Stations, DLC1CosmicResearchCenterConfig.ID)
			.AddToTech(ModAssets.Techs.SpaceStationTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.E_SPACEEXPANSION);

			BuildingManager.CreateEntry<DeepSpaceResearchTelescopeConfig>()
			.ConfigEnabled(() => Config.SpaceStationsPossible)
			.AddToCategory(PlanMenuCategory.Stations, DLC1CosmicResearchCenterConfig.ID)
			.AddToTech(ModAssets.Techs.SpaceStationTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.E_SPACEEXPANSION);

			BuildingManager.CreateEntry<SpaceStationDockingDoorConfig>()
			.ConfigEnabled(() => Config.SpaceStationsPossible )
			.AddToCategory(PlanMenuCategory.Rocketry, DockingTubeDoorConfig.ID)
			.AddToTech(ModAssets.Techs.SpaceStationTechID)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.E_SPACEEXPANSION);
		}

		static void RegisterBuildings_StationParts()
		{
			//station parts
			if (!Config.Derelicts && !Mod.IsDev)
				return;

			BuildingManager.CreateEntry<GravitonCoreConfig>()
			.ConfigEnabled(() => Config.SpaceStationsPossible)
			.AddToCategory(PlanMenuCategory.Rocketry, string.Empty)
			.SubCategory(Category_StationParts)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.E_SPACEEXPANSION);
		}
		static void RegisterBuildings_ModuleUpgrades()
		{
			BuildingManager.CreateEntry<Upgrade_CargoBayCollectionFilterConfig>()
			.ConfigEnabled(() => Config.Instance.EnableRocketModuleUpgrades)
			.AddToCategory(PlanMenuCategory.Rocketry, string.Empty)
			.SubCategory(Category_ModuleUpgrades)
			.AddToTech(Technology.Computers.SensitiveMicroimaging)
			.PrimaryConfigCategory(RTB_BuildingCategoryType.B_MININGSHIPPING);
		}

		static void RegisterBuildings_DevBuildings()
		{
			if (!Mod.Instance.mod.IsDev)
				return;

			BuildingManager.CreateEntry<StationPOISolarPanelConfig>()
			.AddToCategory(PlanMenuCategory.Rocketry, string.Empty)
			.SubCategory("power");
		}

		internal static void MoveVanillaBuildings()
		{
			if (Config.Instance.HabitatPowerPlug)
			{
				InjectionMethods.MoveItemToNewTech(RocketInteriorPowerPlugConfig.ID, GameStrings.Technology.Power.SpacePower, GameStrings.Technology.Power.AdvancedPowerRegulation);
			}
		}
	}
}
