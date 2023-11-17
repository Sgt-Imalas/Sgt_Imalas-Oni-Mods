using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SaveGameModLoader
{
    public class MPM_Config
    {
        [JsonIgnore]
        public static MPM_Config Instance
        {
            get
            {
                if(IO_Util.ReadFromFile<MPM_Config>(ModAssets.ConfigPath, out var config))
                {
                    return config;
                }
                else
                {
                    return new MPM_Config();
                }
            }
        }

        public HashSet<string> PinnedMods = new HashSet<string>();

        public bool ModPinned(string id) => PinnedMods.Contains(id);

        public bool TogglePinnedMod(string pinnedMod)
        {
            if (!PinnedMods.Contains(pinnedMod))
            {
                PinnedMods.Add(pinnedMod);
                SaveToFile();
                return true;
            }
            else
            {
                PinnedMods.Remove(pinnedMod);
                SaveToFile();
                return false;
            }
        }

        public void SaveToFile()
        {
            IO_Util.WriteToFile(this, ModAssets.ConfigPath);
        }
    }
}
