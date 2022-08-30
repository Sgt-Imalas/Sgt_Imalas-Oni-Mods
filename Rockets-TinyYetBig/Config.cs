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
    [ModInfo("Rocketry Expanded")]
    public class Config : SingletonOptions<Config>
    {

        [Option("Cartographic Module Scan Range", "Cartographic Modules will instantly reveal hexes in this radius.")]
        [Limit(0, 3)]
        [JsonProperty]
        public int ScannerModuleRange { get; set; }

        [Option("Critter Containment Module Capacity", "Amount of critters the module can hold at once")]
        [Limit(1, 15)]
        [JsonProperty]
        public int CritterStorageCapacity { get; set; }

        [Option("Laser Drillcone Speed", "Mining speed in Kg/s for the Laser Drillcone. (The Basic Drillcone mines at 7.5kg/s).")]
        [Limit(1f, 15f)]
        [JsonProperty]
        public float LaserDrillconeSpeed { get; set; }


        [Option("Compress Interiors & Remove Rocket Limit", "Disable this Option to use the default 32x32 size rocket interiors. This will also reenable the Rocket Limit of 16 (changing this option only affects new Rockets)")]
        [JsonProperty]
        public bool CompressInteriors { get; set; }

        [Option("Radioisotope Decay time", "Time in cycles for all the enriched uranium in the RTG to decay into depleted uranium. RTG needs a rebuild if all enriched uranium has decayed.")]
        [Limit(100f, 2000f)]
        [JsonProperty]
        public float IsotopeDecayTime { get; set; }
        public Config()
        {
            ScannerModuleRange = 1;
            CritterStorageCapacity = 5;
            LaserDrillconeSpeed = 3.75f;
            IsotopeDecayTime = 200;
            CompressInteriors = true;
        }
    }
}
