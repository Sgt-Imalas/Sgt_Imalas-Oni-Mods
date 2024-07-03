using Klei;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveGameModLoader
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    public class Config : SingletonOptions<Config>
    {


       // [Option("STRINGS.MPM_CONFIG.FILTERBUTTONS.NAME", "STRINGS.MPM_CONFIG.FILTERBUTTONS.TOOLTIP")]
        //[JsonProperty]
        //public FilterbuttonStyle ButtonStyle { get; set; }

        [Option("STRINGS.MPM_CONFIG.FOLDERPATH.NAME", "STRINGS.MPM_CONFIG.FOLDERPATH.TOOLTIP")]
        [JsonProperty]
        public string ModProfileFolder { get; set; }

        [Option("STRINGS.MPM_CONFIG.USECUSTOMFOLDERPATH.NAME", "STRINGS.MPM_CONFIG.USECUSTOMFOLDERPATH.TOOLTIP")]
        [JsonProperty]
        public bool UseCustomFolderPath { get; set; }

        [Option("STRINGS.MPM_CONFIG.NEVERDISABLE.NAME", "STRINGS.MPM_CONFIG.NEVERDISABLE.TOOLTIP")]
        [JsonProperty]
        public bool NeverDisable { get; set; }

        [Option("STRINGS.MPM_CONFIG.APPLYPLIBCONFIG.NAME", "STRINGS.MPM_CONFIG.APPLYPLIBCONFIG.TOOLTIP")]
        [JsonProperty]
        public bool SavePlibOptions { get; set; }

        public Config()
        {
            ModProfileFolder = FileSystem.Normalize(Path.Combine(Path.Combine(KMod.Manager.GetDirectory(), "config"), "[ModSync]StoredModConfigs"));
            NeverDisable = true;
            UseCustomFolderPath = false;
            SavePlibOptions = true;
        }
    }
}
