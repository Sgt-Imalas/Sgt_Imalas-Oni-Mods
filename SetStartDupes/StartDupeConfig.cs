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
        [Limit(1, 6)]
        [JsonProperty]
        public int DuplicantStartAmount { get; set; }

        public StartDupeConfig()
        {
            DuplicantStartAmount = 1;
        }
    }
}
