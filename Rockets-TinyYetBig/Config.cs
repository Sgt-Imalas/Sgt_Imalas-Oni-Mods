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
    //[ConfigFile(SharedConfigLocation: true)]
    [ModInfo("https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods", "preview.png")]
    public class Config : SingletonOptions<Config>
    {
        public static bool SpaceStationsPossible =>
            Instance.CompressInteriors
            && Instance.EnableAdvWorldSelector
            && Instance.SpaceStationsAndTech
            ;


        //[Option("test2", ".")]
        //public Action<object> clickButton2 { get { return i => Debug.Log("test"); }  }


        #region vanillaplus
        [Option("Advanced World Selector", "Enable a more structured world selector.", "(1) Rocketry Vanilla+")]
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

        [Option("Cartographic Module Scan Range", "Cartographic Modules will instantly reveal hexes in this radius.", "(1) Rocketry Vanilla+")]
        [Limit(0, 3)]
        [JsonProperty]
        public int ScannerModuleRange { get; set; }

        [Option("Habitat Power Connector", "Add a power connector to the habitat modules.", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool HabitatPowerPlug { get; set; }

        [Option("Habitat Radiation", "Rocket interior radiation reflects the radiation outside when landed.", "(1) Rocketry Vanilla+")]
        [JsonProperty]
        public bool HabitatInteriorRadiation { get; set; }


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


        [Option("Loader Adapter", STRINGS.OPTIONS.TOGGLESINGLE, "(3) Fuel & Logistics")]
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

        [Option("Radioisotope Decay time", "Time in cycles for all the enriched uranium in the RTG to decay into depleted uranium. RTG needs a refill if all enriched uranium has decayed.", "(4) Power & Utility")]
        [Limit(10f, 200f)]
        [JsonProperty]
        public float IsotopeDecayTime { get; set; }
        #endregion
        #region SpaceStations

        [Option("Space Stations & Deep Space Science", "", "(5) Space Stations")]
        [JsonProperty]
        protected bool SpaceStationsAndTech { get; set; }

        [Option("Docking", "Dock rockets in space to transfer dupes and contents of the interiors", "(5) Space Connections")]
        [JsonProperty]
        public bool RocketDocking { get; set; }

        #endregion


        public Config()
        {
            ///Vanilla+
            EnableAdvWorldSelector = true;
            CompressInteriors = true;
            EnableBuildingCategories = true;
            ScannerModuleRange = 1;
            HabitatPowerPlug = true;
            EnableExtendedHabs = true;
            HabitatInteriorRadiation = true;

            ///Drilling&Shipping
            EnableCritterStorage = true;
            CritterStorageCapacity = 5;
            EnableLaserDrill = true;
            LaserDrillconeSpeed = 3.75f;
            InfinitePOI = false;
            EnableLargeCargoBays = true;
            EnableRadboltStorage = true; 
            EnableDrillSupport = true;

            /// Fuel&Logistics
            BuffLargeOxidizer = true;
            EthanolEngines = true;
            EnableNatGasEngine = true;
            EnableNatGasEngineRange = 15;
            EnableEarlyGameFuelTanks = true;
            EnableFuelLoaders = true;
            EnableWallAdapter = true;
            EnableBunkerPlatform = true;

            // Power&Utility
            EnableSolarNosecone = true;
            EnableGenerators = true;
            IsotopeDecayTime = 40;

            // SpaceStations
            SpaceStationsAndTech = false;
            RocketDocking = true;

        }
    }
}
