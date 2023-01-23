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
    [ModInfo("Adjust Start Dupe Count")]
    class StartDupeConfig : SingletonOptions<StartDupeConfig>
    {


        [Option("Starting Duplicants", "Choose the amount of duplicants you want to start with")]
        [Limit(1, 100)]
        [JsonProperty]
        public int DuplicantStartAmount { get; set; }

        [Option("Modify Printing Pod Dupes", "Enable this option to add the modify button to printing pod dupes\nWhen disabled, the option only appears on the starter dupe selection.")]
        [JsonProperty]
        public bool ModifyDuringGame { get; set; }

        [Option("Extra Starting Resources", "Add some extra startup resources for your additional duplicants.\nOnly goes in effect with more than 3 dupes.\nOnly accounts for extra dupes above 3.")]
        [JsonProperty]
        public bool StartupResources { get; set; }

        [Option("Supported Days", "Amount of days the extra starting resources should last.\nNo effect if \"Starting Resources\" is disabled.")]
        [JsonProperty]
        [Limit(0, 10)]
        public int SupportedDays { get; set; }

        public StartDupeConfig()
        {
            DuplicantStartAmount = 3;
            ModifyDuringGame = false;
            StartupResources = false;
            SupportedDays = 5;
        }
    }
}
