using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorDrops
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Meteor Drops")]
    public class Config : SingletonOptions<Config>
    {
        [Option("Mass Percentage", "Percentage of its mass a meteor drops when blasted.")]
        [JsonProperty]
        [Limit(0, 100)]
        public int MassPercentage { get; set; }
        public Config()
        {
            MassPercentage = 50;
        }
    }
}
