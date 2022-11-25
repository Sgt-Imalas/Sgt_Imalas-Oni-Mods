using Newtonsoft.Json;
using PeterHan.PLib.Options;
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
        
        [Option("Cartographic Module Scan Range", "Cartographic Modules will instantly reveal hexes in this radius.", "Balancing")]
        [Limit(0, 3)]
        [JsonProperty]
        public int ScannerModuleRange { get; set; }

        [Option("Critter Containment Module Capacity", "Amount of critters the module can hold at once", "Balancing")]
        [Limit(1, 15)]
        [JsonProperty]
        public int CritterStorageCapacity { get; set; }

        [Option("Laser Drillcone Speed", "Mining speed in Kg/s for the Laser Drillcone. (The Basic Drillcone mines at 7.5kg/s).", "Balancing")]
        [Limit(1f, 15f)]
        [JsonProperty]
        public float LaserDrillconeSpeed { get; set; }


        [Option("Compress Interiors & Remove Rocket Limit", "Disable this Option to use the default 32x32 size rocket interiors. This will also reenable the Rocket Limit of 16 (changing this option only affects new Rockets)", "Features")]
        [JsonProperty]
        public bool CompressInteriors { get; set; }

        [Option("Rocket Building Categories", "Enable a more modular rocket build menu that sorts the modules into categories.", "Features")]
        [JsonProperty]
        public bool EnableBuildingCategories { get; set; }

        [Option("Buff Large Oxidizer Module", "Buff storage capacity of the large Oxidizer Module from 900kg to 1350kg.", "Features")]
        [JsonProperty]
        public bool BuffLargeOxidizer { get; set; }

        [Option("Hide Category Tooltips", "Hide category tooltips. Only has an effect if Rocket Building Categories are enabled", "Features")]
        [JsonProperty]
        public bool HideRocketCategoryTooltips { get; set; }

       // [Option("Land without pad", "Add landing legs to your rocket to land on planets without any installed landing pads", "Features")]
       // [JsonProperty]
       // public bool LandingLegs { get; set; }

        [Option("Habitat Power Connector", "Add a power connector to the habitat modules.", "Features")]
        [JsonProperty]
        public bool HabitatPowerPlug { get; set; }

        [Option("Burn Ethanol as fuel", "Allows Petroleum Engines to also burn Ethanol as fuel.", "Features")]
        [JsonProperty]
        public bool EthanolEngines { get; set; }


        [Option("Radioisotope Decay time", "Time in cycles for all the enriched uranium in the RTG to decay into depleted uranium. RTG needs a refill if all enriched uranium has decayed.", "Balancing")]
        [Limit(10f, 200f)]
        [JsonProperty]
        public float IsotopeDecayTime { get; set; }


        #region Modules
        ///MODULES
        [Option("Extended Spacefarer Modules", STRINGS.OPTIONS.TOGGLEMULTI, "Modules")]
        [JsonProperty]
        public bool EnableExtendedHabs { get; set; }

        [Option("Stargazer Module", STRINGS.OPTIONS.TOGGLESINGLE, "Modules")]
        [JsonProperty]
        public bool EnableStargazer { get; set; }

        [Option("Solar Nosecone", STRINGS.OPTIONS.TOGGLESINGLE, "Modules")]
        [JsonProperty]
        public bool EnableSolarNosecone { get; set; }

        [Option("Generator Modules", STRINGS.OPTIONS.TOGGLEMULTI, "Modules")]
        [JsonProperty]
        public bool EnableGenerators { get; set; }

        [Option("Laser Drillcone", STRINGS.OPTIONS.TOGGLESINGLE, "Modules")]
        [JsonProperty]
        public bool EnableLaserDrill { get; set; }

        [Option("Radbolt Storage Module", STRINGS.OPTIONS.TOGGLESINGLE, "Modules")]
        [JsonProperty]
        public bool EnableRadboltStorage { get; set; }

        [Option("Critter Containment Module", STRINGS.OPTIONS.TOGGLESINGLE, "Modules")]
        [JsonProperty]
        public bool EnableCritterStorage { get; set; }

        [Option("Fortified Rocket Platform", STRINGS.OPTIONS.TOGGLESINGLE, "Modules")]
        [JsonProperty]

        public bool EnableBunkerPlatform { get; set; }

        [Option("Fuel Loaders", STRINGS.OPTIONS.TOGGLEMULTI, "Modules")]
        [JsonProperty]
        public bool EnableFuelLoaders { get; set; }

        #endregion

        public Config()
        {
            ScannerModuleRange = 1;
            CritterStorageCapacity = 5;
            LaserDrillconeSpeed = 3.75f;
            IsotopeDecayTime = 40;

            ///Features
            CompressInteriors = true; 
            EnableBuildingCategories = true;
            HideRocketCategoryTooltips = false;
            //LandingLegs = true;
            EthanolEngines = true;
            HabitatPowerPlug = true;
            BuffLargeOxidizer = true;

            ///Modules:
            EnableExtendedHabs = true;
            EnableStargazer = true;
            EnableGenerators = true;
            EnableLaserDrill = true;
            EnableSolarNosecone = true;
            EnableRadboltStorage = true;
            EnableCritterStorage = true; 
            EnableBunkerPlatform = true;
            EnableFuelLoaders = true;
        }
    }
}
