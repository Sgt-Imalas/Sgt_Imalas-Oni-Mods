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

        //[Option("mop becomes water succ", "")]
        //[JsonProperty]
        //public bool succmop { get; set; }

        //[Option("manual space can opener", "")]
        //[JsonProperty]
        //public bool manualRailgunPayloadOpener { get; set; }

        [Option("manual slime machine", "")]
        [JsonProperty]
        public bool manualSlimemachine { get; set; }
        [Option("Gas Element Sensor takes power", "")]
        [JsonProperty]
        public bool gassensorpower { get; set; }
        [Option("liquid element sensor power requirement", "")]
        [JsonProperty]
        public bool liquidsensorpower { get; set; }
        [Option("Duplicants rot forever", "when activated, unburied duplicants will rot forever. otherwise they will decompose into bones.")]
        [JsonProperty]
        public bool endlessRotting { get; set; }


        [Option("Old Pipe Icons", "pipe input and output icons are replaced with older versions that change based on the connection state")]
        [JsonProperty]
        public bool oldPipeIcons { get; set; }
        public Config()
        {
            TileTopsMerge = true;
            //manualRailgunPayloadOpener = true;
            manualSlimemachine = true;
            IronOreTexture = EarlierVersion.Beta;
            gassensorpower = true; 
            liquidsensorpower = false;
            endlessRotting = false;

            oldPipeIcons = true;
            //succmop = true;

        }
    }
}
