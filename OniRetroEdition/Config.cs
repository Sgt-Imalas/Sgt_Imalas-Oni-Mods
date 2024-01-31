using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OniRetroEdition
{
    [Serializable]
    [RestartRequired]
    //[ConfigFile(SharedConfigLocation: true)]
    public class Config : SingletonOptions<Config>
    {
        public enum EarlierVersion
        {
            Alpha,
            Beta
        }

        [Option("Iron Ore Tile version", "")]
        [JsonProperty]
        public EarlierVersion IronOreTexture { get; set; }

        [Option("Connect certain tile tops", "")]
        [JsonProperty]
        public bool TileTopsMerge { get; set; }

        [Option("mop becomes water succ", "")]
        [JsonProperty]
        public bool succmop { get; set; }

        [Option("manual space can opener", "")]
        [JsonProperty]
        public bool manualRailgunPayloadOpener { get; set; }

        [Option("manual slime machine", "")]
        [JsonProperty]
        public bool manualSlimemachine { get; set; }
        [Option("gas element sensor power requirement", "")]
        [JsonProperty]
        public bool gassensorpower { get; set; }
        [Option("liquid element sensor power requirement", "")]
        [JsonProperty]
        public bool liquidsensorpower { get; set; }
        public Config()
        {
            TileTopsMerge = true;
            manualRailgunPayloadOpener = true;
            manualSlimemachine = false;
            IronOreTexture = EarlierVersion.Beta;
            gassensorpower = true; 
            liquidsensorpower = false;
            succmop = true;

        }
    }
}
