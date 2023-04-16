using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Duplicant Stat Selector")]
    class ModConfig : SingletonOptions<ModConfig>
    {


        [Option("Starting Duplicants", "Choose the amount of duplicants you want to start with")]
        [Limit(1, 100)]
        [JsonProperty]
        public int DuplicantStartAmount { get; set; }

        [Option("Modification of Printing Pod Dupes", "Enable this option to add the modify button to printing pod dupes\nWhen disabled, the option only appears on the starter dupe selection.\nOption also enables the use of presets.")]
        [JsonProperty]
        public bool ModifyDuringGame { get; set; }

        [Option("Reroll Printing Pod Dupes", "Enable this option to add the reroll button to printing pod dupes.")]
        [JsonProperty]
        public bool RerollDuringGame { get; set; }

        [Option("Printing Pod Cooldown", "Time it takes for the printing pod to provide another print in cycles.\nDefault is 3")]
        [JsonProperty]
        [Limit(0.1, 10)]
        public float PrintingPodRechargeTime { get; set; }


        [Option("Extra Starting Resources", "Add some extra startup resources for your additional duplicants.\nOnly goes in effect with more than 3 dupes.\nOnly accounts for extra dupes above 3.")]
        [JsonProperty]
        public bool StartupResources { get; set; }
        
        [Option("Supported Days", "Amount of days the extra starting resources should last.\nNo effect if \"Extra Starting Resources\" is disabled.")]
        [JsonProperty]
        [Limit(0, 10)]
        public int SupportedDays { get; set; }

        public ModConfig()
        {
            DuplicantStartAmount = 3;
            PrintingPodRechargeTime = 3;
            ModifyDuringGame = false;
            RerollDuringGame = false;
            StartupResources = false;
            SupportedDays = 5;
        }
    }
}
