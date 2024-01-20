using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmogusMorb
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    public class Config : SingletonOptions<Config>
    {
        
        [Option("Extra Sus", "crewmate")]
        [JsonProperty]
        public bool SussyPlus { get; set; }

        public Config()
        {
            SussyPlus = true; 
        }
    }
}
