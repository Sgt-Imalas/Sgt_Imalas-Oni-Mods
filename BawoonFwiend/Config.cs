using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BawoonFwiend
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Canned Food")]
    public class Config : SingletonOptions<Config>
    {
        [Option("STRINGS.MODCONFIG.STATBOOST.NAME", "STRINGS.MODCONFIG.STATBOOST.TOOLTIP")]
        [JsonProperty]
        [Limit(1, 8)]
        public int MachineGivenBalloonBuff { get; set; }
        public Config()
        {
            MachineGivenBalloonBuff = 3;
        }
    }
}
