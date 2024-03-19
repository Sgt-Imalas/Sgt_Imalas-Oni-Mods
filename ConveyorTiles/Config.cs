using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConveyorTiles
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Conveyor Tiles")]
    public class Config : SingletonOptions<Config>
    {
        [Option("STRINGS.MODCONFIG.CONVEYORSPEEDMULTIPLIER.NAME", "STRINGS.MODCONFIG.CONVEYORSPEEDMULTIPLIER.TOOLTIP")]
        [JsonProperty]
        [Limit(0.1f, 10f)]
        public float SpeedMultiplier { get; set; }

        [Option("STRINGS.MODCONFIG.CONVEYORUSESPOWER.NAME", "STRINGS.MODCONFIG.CONVEYORUSESPOWER.TOOLTIP")]
        [JsonProperty]
        [Limit(0, 20)]
        public int TileWattage { get; set; }
        [Option("STRINGS.MODCONFIG.IMMUNEDUPES.NAME", "STRINGS.MODCONFIG.IMMUNEDUPES.TOOLTIP")]
        [JsonProperty]
        public bool Immunes { get; set; }
        public Config()
        {
            TileWattage = 4;
            SpeedMultiplier = 1f;
            Immunes = false;
        }
    }
}
