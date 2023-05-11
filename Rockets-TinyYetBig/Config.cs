using Newtonsoft.Json;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods", "preview.png")]
    public class Config : SingletonOptions<Config>
    {
        public static bool SpaceStationsPossible =>
            Instance.CompressInteriors
            && Instance.EnableAdvWorldSelector
            && Instance.SpaceStationsAndTech
            && Instance.NeutroniumMaterial
            ;


        //[Option("test2", ".")]
        //public Action<object> clickButton2 { get { return i => SgtLogger.debuglog("test"); }  }


        #region vanillaplus
        [Option("Advanced World Selector", "Enable a better world selector.", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool EnableAdvWorldSelector { get; set; }

        [Option("Compress Interiors & Remove Rocket Limit", "Disable this Option to use the default 32x32 size rocket interiors. This will also reenable the Rocket Limit of 16 (changing this option only affects new Rockets)", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool CompressInteriors { get; set; }

        [Option("Extended Spacefarer Modules", STRINGS.OPTIONS.TOGGLEMULTI, "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool EnableExtendedHabs { get; set; }

        [Option("Rocket Building Categories", "Enable a more modular rocket build menu that sorts the modules into categories.", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool EnableBuildingCategories { get; set; }

        [Option("Cartographic Module Scan Range", "Cartographic Modules will scan hexes in this radius.", "(1) Rocketry Vanilla+")]
        [Limit(1, 6)]
        [JsonProperty]
        public int ScannerModuleRangeRadius { get; set; }
        [Option("Cartographic Module Scan Speed", "Time it takes for the module to reveal one hex in cycles.", "(1) Rocketry Vanilla+")]
        [Limit(0.1f, 1f)]
        [JsonProperty]
        public float ScannerModuleScanSpeed { get; set; }

        [Option("Habitat Power Connector", "Add a power connector to the habitat modules.", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool HabitatPowerPlug { get; set; }

        [Option("Habitat Radiation", "Rocket interior radiation reflects the radiation outside when landed.", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool HabitatInteriorRadiation { get; set; }

        [Option("Habitat Interior Port Improvements", "Rocket  Connectors count as Rocket Wall for buildings that can only be attached to it.\nRocket Ports block the same amount of radiation as rocket wall", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool HabitatInteriorPortImprovements { get; set; }


        #endregion

        #region mining&shipping

        [Option("Laser Drillcone", STRINGS.OPTIONS.TOGGLESINGLE, "(2) Mining & Shipping")]
        [JsonProperty]
        public bool EnableLaserDrill { get; set; }

        [Option("Laser Drillcone Speed", "Mining speed in Kg/s for the Laser Drillcone. (The Basic Drillcone mines at 7.5kg/s).", "(2) Mining & Shipping")]
        [Limit(1f, 15f)]
        [JsonProperty]
        public float LaserDrillconeSpeed { get; set; }


        [Option("Drillcone Service Module", STRINGS.OPTIONS.TOGGLESINGLE, "(2) Mining & Shipping")]
        [JsonProperty]
        public bool EnableDrillSupport { get; set; }

        [Option("Infinite Mining Capacity", "Mining POI become infinite. Does not affect artifacts.", "(2) Mining & Shipping")]
        [JsonProperty]
        public bool InfinitePOI { get; set; }


        [Option("Enable Fridge Module", STRINGS.OPTIONS.TOGGLESINGLE, "(2) Mining & Shipping")]
        [JsonProperty]
        public bool EnableFridge { get; set; }
        

        [Option("Large Cargo Modules", STRINGS.OPTIONS.TOGGLEMULTI, "(2) Mining & Shipping")]
        [JsonProperty]
        public bool EnableLargeCargoBays { get; set; }


        [Option("Radbolt Storage Module", STRINGS.OPTIONS.TOGGLESINGLE, "(2) Mining & Shipping")]
        [JsonProperty]
        public bool EnableRadboltStorage { get; set; }

        [Option("Critter Containment Module", STRINGS.OPTIONS.TOGGLESINGLE, "(2) Mining & Shipping")]
        [JsonProperty]
        public bool EnableCritterStorage { get; set; }

        [Option("Critter Containment Module Capacity", "Amount of critters the module can hold at once", "(2) Mining & Shipping")]
        [Limit(1, 15)]
        [JsonProperty]
        public int CritterStorageCapacity { get; set; }

        [Option("Rebalanced Cargobay Capacity", "Cargo Bays have increased and rebalanced Cargo Capacity", "(2) Mining & Shipping")]
        [JsonProperty]
        public bool RebalancedCargoCapacity { get; set; }

        [Option("Gas Cargobay KG/Unit", STRINGS.OPTIONS.UNITDESCRIPTION, "(2) Mining & Shipping")]
        [Limit(200, 1500)]
        [JsonProperty]
        public int GasCargoBayUnits { get; set; }
        [Option("Liquid Cargobay KG/Unit", STRINGS.OPTIONS.UNITDESCRIPTION, "(2) Mining & Shipping")]
        [Limit(500, 2000)]
        [JsonProperty]
        public int LiquidCargoBayUnits { get; set; }
        [Option("Solid Cargobay KG/Unit", STRINGS.OPTIONS.UNITDESCRIPTION, "(2) Mining & Shipping")]
        [Limit(800, 6000)]
        [JsonProperty]
        public int SolidCargoBayUnits { get; set; }
        #endregion

        #region Fuel&Logistics

        [Option("Buff Large Oxidizer Module", "Buff storage capacity of the large Oxidizer Module from 900kg to 1350kg.", "(3) Fuel & Logistics")]
        [JsonProperty]
        public bool BuffLargeOxidizer { get; set; }

        [Option("Burn Ethanol as fuel", "Allows Petroleum Engines to also burn Ethanol as fuel.", "(3) Fuel & Logistics")]
        [JsonProperty]
        public bool EthanolEngines { get; set; }

        [Option("Natural Gas Engine Module", STRINGS.OPTIONS.TOGGLESINGLE, "(3) Fuel & Logistics")]
        [JsonProperty]
        public bool EnableNatGasEngine { get; set; }

        [Option("Natural Gas Engine Range", "Set the max range of a natural gas engine.", "(3) Fuel & Logistics")]
        [JsonProperty]
        [Limit(8, 18)]
        public int EnableNatGasEngineRange { get; set; }

        [Option("Early Game Fuel Tanks", STRINGS.OPTIONS.TOGGLEMULTI, "(3) Fuel & Logistics")]
        [JsonProperty]
        public bool EnableEarlyGameFuelTanks { get; set; }

        [Option("Fuel Loaders", STRINGS.OPTIONS.TOGGLEMULTI, "(3) Fuel & Logistics")]
        [JsonProperty]
        public bool EnableFuelLoaders { get; set; }


        [Option("Loader Adapters", STRINGS.OPTIONS.TOGGLEMULTI, "(3) Fuel & Logistics")]
        [JsonProperty]
        public bool EnableWallAdapter { get; set; }

        [Option("Fortified & Advanced Rocket Platform", STRINGS.OPTIONS.TOGGLEMULTI, "(3) Fuel & Logistics")]
        [JsonProperty]
        public bool EnableBunkerPlatform { get; set; }
        #endregion

        #region Power&Utility

        [Option("Solar Nosecone", STRINGS.OPTIONS.TOGGLESINGLE, "(4) Power & Utility")]
        [JsonProperty]
        public bool EnableSolarNosecone { get; set; }

        [Option("Generator Modules", STRINGS.OPTIONS.TOGGLEMULTI, "(4) Power & Utility")]
        [JsonProperty]
        public bool EnableGenerators { get; set; }
        

        [Option("Small Battery Module", STRINGS.OPTIONS.TOGGLESINGLE, "(4) Power & Utility")]
        [JsonProperty]
        public bool EnableSmolBattery { get; set; }
        
        [Option("Radioisotope Decay time", "Time in cycles for all the enriched uranium in the RTG to decay into depleted uranium. RTG needs a refill if all enriched uranium has decayed.", "(4) Power & Utility")]
        [Limit(10f, 200f)]
        [JsonProperty]
        public float IsotopeDecayTime { get; set; }
        #endregion
        #region SpaceStations

        [Option("Space Stations & Deep Space Science", "", "(5) Space Expansion")]
        [JsonProperty]
        protected bool SpaceStationsAndTech { get; set; }

        [Option("Docking", "Dock rockets in space to transfer dupes and contents of the interiors", "(5) Space Expansion")]
        [JsonProperty]
        public bool RocketDocking { get; set; }

        [Option("Neutronium Alloy", "Gather Neutronium Dust by analyzing artifacts and refine it into Neutronium Alloy.\nNeutronium Alloys are required in the construction of large space structures", "(5) Space Expansion")]
        [JsonProperty]
        public bool NeutroniumMaterial { get; set; }

        #endregion

        #region EasterEggs

        [Option("Dune Spice", "When consuming Rocketeer Spice, Dupes will gain the spice eyes from Dune.", "(6) Easter Eggs")]
        [JsonProperty]
        public bool SpiceEyes { get; set; }

        #endregion

        public Config()
        {
            ///Vanilla+
            EnableAdvWorldSelector = true;
            CompressInteriors = true;
            EnableBuildingCategories = true;
            ScannerModuleRangeRadius = 4;
            ScannerModuleScanSpeed = 0.33f;
            HabitatPowerPlug = true;
            EnableExtendedHabs = true;
            HabitatInteriorRadiation = true;
            HabitatInteriorPortImprovements = true;

            ///Drilling&Shipping
            EnableCritterStorage = true;
            CritterStorageCapacity = 5;
            EnableLaserDrill = true;
            LaserDrillconeSpeed = 3.75f;
            EnableFridge = true;
            InfinitePOI = false;
            EnableLargeCargoBays = true; 
            EnableRadboltStorage = true; 
            EnableDrillSupport = true;

            RebalancedCargoCapacity = true;
            GasCargoBayUnits = 500;
            LiquidCargoBayUnits = 1250;
            SolidCargoBayUnits = 2000;


            /// Fuel&Logistics
            BuffLargeOxidizer = true;
            EthanolEngines = true;
            EnableNatGasEngine = true;
            EnableNatGasEngineRange = 15;
            EnableEarlyGameFuelTanks = true;
            EnableFuelLoaders = true;
            EnableWallAdapter = true;
            EnableBunkerPlatform = true;

            /// Power&Utility
            EnableSolarNosecone = true;
            EnableGenerators = true;
            EnableSmolBattery = true;
            IsotopeDecayTime = 50;

            /// SpaceStations
            SpaceStationsAndTech = false;
            RocketDocking = true;
            NeutroniumMaterial = true;

            /// EasterEggs
            SpiceEyes = true;
        }
    }
}
