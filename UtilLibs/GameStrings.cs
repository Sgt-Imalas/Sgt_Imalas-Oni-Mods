using System.Collections.Generic;

namespace UtilLibs
{
	public static class GameStrings
	{
		public static class PlanMenuCategory
		{
			public const string Base = "Base";
			public const string Oxygen = "Oxygen";
			public const string Power = "Power";
			public const string Food = "Food";
			public const string Plumbing = "Plumbing";
			public const string Ventilation = "HVAC";
			public const string Refinement = "Refining";
			public const string Medicine = "Medical";
			public const string Furniture = "Furniture";
			public const string Stations = "Equipment";
			public const string Utilities = "Utilities";
			public const string Automation = "Automation";
			public const string Shipping = "Conveyance";
			public const string Rocketry = "Rocketry";
			public const string Radiation = "HEP";
		}
		public static class PlanMenuSubcategory
		{

			public const string Ladders = "ladders";
			public const string Tiles = "tiles";
			public const string Printingpods = "printingpods";
			public const string Doors = "doors";
			public const string Storage = "storage";
			public const string Transport = "transport";
			public const string Operations = "operations";
			public const string Producers = "producers";
			public const string Scrubbers = "scrubbers";
			public const string Generators = "generators";
			public const string Wires = "wires";
			public const string Batteries = "batteries";
			public const string Electrobankbuildings = "electrobankbuildings";
			public const string Powercontrol = "powercontrol";
			public const string Switches = "switches";
			public const string Cooking = "cooking";
			public const string Farming = "farming";
			public const string Ranching = "ranching";
			public const string Washroom = "washroom";
			public const string Pipes = "pipes";
			public const string Pumps = "pumps";
			public const string Valves = "valves";
			public const string Sensors = "sensors";
			public const string Buildmenuports = "buildmenuports";
			public const string Organic = "organic";
			public const string Materials = "materials";
			public const string Oil = "oil";
			public const string Advanced = "advanced";
			public const string Hygiene = "hygiene";
			public const string Medical = "medical";
			public const string Wellness = "wellness";
			public const string Beds = "beds";
			public const string Lights = "lights";
			public const string Dining = "dining";
			public const string Recreation = "recreation";
			public const string Decor = "decor";
			public const string Research = "research";
			public const string Archaeology = "archaeology";
			public const string Meteordefense = "meteordefense";
			public const string Exploration = "exploration";
			public const string Industrialstation = "industrialstation";
			public const string Workstations = "workstations";
			public const string Manufacturing = "manufacturing";
			public const string Equipment = "equipment";
			public const string Missiles = "missiles";
			public const string Temperature = "temperature";
			public const string Sanitation = "sanitation";
			public const string Logicaudio = "logicaudio";
			public const string Logicmanager = "logicmanager";
			public const string Logicgates = "logicgates";
			public const string Transmissions = "transmissions";
			public const string Conveyancestructures = "conveyancestructures";
			public const string Telescopes = "telescopes";
			public const string Rocketstructures = "rocketstructures";
			public const string Fittings = "fittings";
			public const string Rocketnav = "rocketnav";
			public const string Engines = "engines";
			public const string Tanks = "tanks";
			public const string Cargo = "cargo";
			public const string Module = "module";
			public const string Automated = "automated";
		}

		public static class Technology
		{
			public static class Food
			{
				public const string BasicFarming = "FarmingTech";
				public const string MealPreparation = "FineDining";
				public const string GourmetMealPreparation = "FinerDining";
				public const string FoodRepurposing = "FoodRepurposing";
				public const string Agriculture = "Agriculture";
				public const string Ranching = "Ranching";
				public const string AnimalControl = "AnimalControl";
				public const string GourmetMealPrep = "FinerDining";

				// Spaced Out!
				public const string Bioengineering = "Bioengineering";
			}

			public static class Power
			{
				public const string PowerRegulation = "PowerRegulation";
				public const string InternalCombustion = "Combustion";
				public const string FossilFuels = "ImprovedCombustion";
				public const string SoundAmplifiers = "Acoustics";
				public const string AdvancedPowerRegulation = "AdvancedPowerRegulation";
				public const string PlasticManufacturing = "Plastics";
				public const string LowResistanceConductors = "PrettyGoodConductors";
				public const string ValveMiniaturization = "ValveMiniaturization";
				public const string RenewableEnergy = "RenewableEnergy";
				public const string SpacePower = "SpacePower";
				public const string HydrocarbonPropulsion = "HydrocarbonPropulsion";
				public const string ImprovedHydrocarbonPropulsion = "BetterHydroCarbonPropulsion";

				// Spaced Out!
				public const string AdvancedCombustion = "SpaceCombustion";
			}

			public static class SolidMaterial
			{
				public const string BruteForceRefinement = "BasicRefinement";
				public const string RefinedRenovations = "RefinedObjects";
				public const string SmartStorage = "SmartStorage";
				public const string Smelting = "Smelting";
				public const string SolidTransport = "SolidTransport";
				public const string SuperheatedForging = "HighTempForging";
				public const string PressurizedForging = "HighPressureForging";
				public const string SolidControl = "SolidSpace";
				public const string SolidManagement = "SolidManagement";
				public const string HighVelocityTransport = "HighVelocityTransport";

				// Spaced Out!
				public const string HighVelocityDestruction = "HighVelocityDestruction";
			}

			public static class ColonyDevelopment
			{
				public const string CelestialDetection = "SkyDetectors";
				public const string Employment = "Jobs";
				public const string AdvancedResearch = "AdvancedResearch";
				public const string RadiationRefinement = "NuclearRefinement";
				public const string CryoFuelPropulsion = "CryoFuelPropulsion";
				public const string SpaceProgram = "SpaceProgram";
				public const string CrashPlan = "CrashPlan";
				public const string DurableLifeSupport = "DurableLifeSupport";
				public const string AtomicResearch = "NuclearResearch";
				public const string RadboltPropulsion = "NuclearPropulsion";
				public const string NotificationSystems = "NotificationSystems";
				public const string ArtificialFriends = "ArtificialFriends";
				public const string RoboticTools = "RoboticTools";
			}

			// Spaced Out!
			public static class RadiationTechnologies
			{
				public const string MaterialsScienceResearch = "NuclearResearch";
				public const string MoreMaterialsScienceResearch = "AdvancedNuclearResearch";
				public const string RadboltContainment = "NuclearStorage";
				public const string RadiationRefinement = "NuclearRefinement";
				public const string RadboltPropulsion = "NuclearPropulsion";
			}

			public static class Medicine
			{
				public const string Pharmacology = "MedicineI";
				public const string MedicalEquipment = "MedicineII";
				public const string PathogenDiagnostics = "MedicineIII";
				public const string MicroTargetedMedicine = "MedicineIV";
				public const string RadiationProtection = "RadiationProtection";
			}

			public static class Liquids
			{
				public const string Plumbing = "LiquidPiping";
				public const string AirSystems = "ImprovedOxygen";
				public const string Sanitation = "SanitationSciences";
				public const string AdvancedSanitation = "AdvancedSanitation";
				public const string Filtration = "AdvancedFiltration";
				public const string LiquidBasedRefinementProcess = "LiquidFiltering";
				public const string Distillation = "Distillation";
				public const string Emulsification = "AdvancedDistillation";
				public const string ImprovedPlumbing = "ImprovedLiquidPiping";
				public const string LiquidTuning = "LiquidTemperature";
				public const string AdvancedCaffeination = "PrecisionPlumbing";
				public const string FlowRedirection = "FlowRedirection";
				public const string LiquidDistribution = "LiquidDistribution";
				public const string Projectiles = "Jetpacks";
			}

			public static class Gases
			{
				public const string Ventilation = "GasPiping";
				public const string PressureManagement = "PressureManagement";
				public const string TemperatureModulation = "TemperatureModulation";
				public const string Decontamination = "DirectedAirStreams";
				public const string ImprovedVentilation = "ImprovedGasPiping";
				public const string HVAC = "HVAC";
				public const string Catalytics = "Catalytics";
				public const string PortableGasses = "PortableGasses";

				// Spaced Out!
				public const string AdvancedGasFlow = "SpaceGas";
				public const string GasDistribution = "GasDistribution";
			}

			public static class Exosuits
			{
				public const string HazardProtection = "Suits";
				public const string TransitTubes = "TravelTubes";
			}

			public static class Decor
			{
				public const string InteriorDecor = "InteriorDecor";
				public const string ArtisticExpression = "Artistry";
				public const string TextileProduction = "Clothing";
				public const string FineArt = "FineArt";
				public const string HomeLuxuries = "Luxury";
				public const string HighCulture = "RefractiveDecor";
				public const string GlassBlowing = "GlassFurnishings";
				public const string RenaissanceArt = "RenaissanceArt";
				public const string EnvironmentalAppreciation = "EnvironmentalAppreciation";
				public const string NewMedia = "Screens";
				public const string Monuments = "Monuments";
			}

			public static class Computers
			{
				public const string SmartHome = "LogicControl";
				public const string GenericSensors = "GenericSensors";
				public const string AdvancedAutomation = "LogicCircuits";
				public const string Computing = "DupeTrafficControl";
				public const string ParallelAutomation = "ParallelAutomation";
				public const string Multiplexing = "Multiplexing";

				// Spaced Out!
				public const string SensitiveMicroimaging = "AdvancedScanners";
			}

			public static class Rocketry
			{
				public const string CelestialDetection = "SkyDetectors";
				public const string IntroductoryRocketry = "BasicRocketry";
				public const string SolidFuelCombustion = "EnginesI";
				public const string SolidCargo = "CargoI";
				public const string HydrocarbonCombustion = "EnginesII";
				public const string LiquidAndGasCargo = "CargoII";
				public const string CryofuelCombustion = "EnginesIII";
				public const string UniqueCargo = "CargoIII";

			}
		}
	}
}

