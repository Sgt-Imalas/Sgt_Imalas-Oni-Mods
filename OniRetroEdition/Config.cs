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

        public Config()
        {
            TileTopsMerge = true;
            IronOreTexture = EarlierVersion.Beta;
        }
    }
}
