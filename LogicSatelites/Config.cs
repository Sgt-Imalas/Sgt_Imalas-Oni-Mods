using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites
{
    [Serializable]
    [RestartRequired]
    [ModInfo("Logic Satellites")]
    public class Config : SingletonOptions<Config>
    {

        [Option("Satellite Scan Range", "Range of the satellite space scanner")]
        [Limit(0, 5)]
        [JsonProperty]
        public int SatelliteScannerRange { get; set; }

        [Option("Satellite Scan Speed", "Time (in Cycles) the satellite takes for one tile scan")]
        [Limit(0.1f, 2f)]
        [JsonProperty]
        public float SatelliteScannerSpeed { get; set; }

        [Option("Satellite Logic Repeater Range", "Range of the satellites logic relay")]
        [Limit(0, 10)]
        [JsonProperty]
        public int SatelliteLogicRange { get; set; }

        public Config()
        {
            SatelliteScannerRange = 3;
            SatelliteScannerSpeed = 0.8f; 
            SatelliteLogicRange = 5;
        }
    }
}
