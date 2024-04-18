using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Radiator_Mod
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Space Radiator")]
    public class Config : SingletonOptions<Config>
    {
        [Option("STRINGS.MODCONFIG.OLDFORMULA.NAME", "STRINGS.MODCONFIG.OLDFORMULA.TOOLTIP")]
        [JsonProperty]
        public bool UseOldHeatDeletion { get; set; }

        [Option("STRINGS.MODCONFIG.DELETIONMULTIPLIER.NAME", "STRINGS.MODCONFIG.DELETIONMULTIPLIER.TOOLTIP")]
        [JsonProperty]
        public float RadiationMultiplicator { get; set; }

        public Config()
        {
            RadiationMultiplicator = 1f;
            UseOldHeatDeletion = false;
        }
    }
}
