using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace KnastoronOniMods
{
    [Serializable]
    [RestartRequired]
    [ModInfo("Automated AI Rockets")]
    public class Config : SingletonOptions<Config>
    {
        [Option("Advanced Recipe", "Creating the AI Control Module requires a neural vaccilator recharge.")]
        [JsonProperty]
        public bool UsesNeuralVaccilatorRecharge { get; set; }
        public Config()
        {
            UsesNeuralVaccilatorRecharge = true;
        }
    }
}
