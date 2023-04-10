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
    [ModInfo("Balloon Dispenser")]
    public class Config : SingletonOptions<Config>
    {
        [Option("STRINGS.MODCONFIG.GASMASS.NAME", "STRINGS.MODCONFIG.GASMASS.TOOLTIP")]
        [JsonProperty]
        [Limit(0.1f, 10f)]
        public float GasMass { get; set; }

        [Option("STRINGS.MODCONFIG.STATBOOST.NAME", "STRINGS.MODCONFIG.STATBOOST.TOOLTIP")]
        [JsonProperty]
        [Limit(1, 8)]
        public int MachineGivenBalloonBuff { get; set; }
        public Config()
        {
            MachineGivenBalloonBuff = 3;
            GasMass = 5f;
        }
    }
}
