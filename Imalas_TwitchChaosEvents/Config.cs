using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents
{

    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Chaos Twitch Events")]
    public class Config : SingletonOptions<Config>
    {

        [Option("STRINGS.CHAOS_CONFIG.TACORAIN_MUSIC_NAME", "STRINGS.CHAOS_CONFIG.TACORAIN_MUSIC_TOOLTIP")]
        [JsonProperty]
        public bool TacoEventMusic { get; set; }
        [Option("STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_MUSIC_NAME", "STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_MUSIC_TOOLTIP")]
        [JsonProperty]
        public bool FakeTacoEventMusic { get; set; }

        [Option("STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_DURATION_NAME", "STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_DURATION_TOOLTIP")]
        [JsonProperty]
        public int FakeTacoEventDuration { get; set; }
        public Config()
        {
            FakeTacoEventMusic = false;
            TacoEventMusic = false;
            FakeTacoEventDuration = 50;
        }       

    }
}
