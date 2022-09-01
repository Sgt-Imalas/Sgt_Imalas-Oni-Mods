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
                AiSpeedMultiplier = 1.0f;
               UsesNeuralVaccilatorRecharge = true;
        }


        [Option("Ai Speed Multiplier", "adjust the speed multiplier of AI Controlled Rockets; 0.5 is the vanilla autopilot speed, 1.0 is no penalty, 1.75 is the Equivalent of a dupe with maxed piloting")]
        [JsonProperty]
        [Limit(0.5f, 1.75f)]
        public float AiSpeedMultiplier { get; set; }
        
    }
}
