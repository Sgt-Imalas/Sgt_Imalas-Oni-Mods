using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ModProfileManager_Addon.SaveGameModList;

namespace ModProfileManager_Addon.ModProfileData
{
    public class ModPresetEntry : IEqualityComparer<ModPresetEntry>
    {
        public string Path;
        public SaveGameModList ModList;
        public ModPresetEntry( SaveGameModList modList, string path)
        {
            Path = path;
            ModList = modList;
        }
        public List<KMod.Label> GetActiveMods()
        {
            var result  = new List<KMod.Label>();

            if(ModList.TryGetModListEntry(Path, out var result1))
                return result1;

            return result;
        }
        public Dictionary<string, MPM_POptionDataEntry> GetActivePlibConfig()
        {
            var result = new Dictionary<string, MPM_POptionDataEntry>();

            if (ModList.TryGetPlibOptionsEntry(Path, out Dictionary<string, MPM_POptionDataEntry> result1))
                return result1;

            return result;
        }
        public bool Equals(ModPresetEntry x, ModPresetEntry y)
        {
            return x.ModList == y.ModList && x.Path == y.Path;
        }

        public int GetHashCode(ModPresetEntry obj)
        {
            return obj.Path.GetHashCode() + obj.ModList.GetHashCode();
        }
    }
}
