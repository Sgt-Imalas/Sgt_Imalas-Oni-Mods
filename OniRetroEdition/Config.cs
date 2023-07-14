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

        public Config()
        {
            IronOreTexture = EarlierVersion.Beta;
        }
    }
}
