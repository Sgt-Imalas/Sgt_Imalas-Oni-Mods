using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ModProfileManager_Addon.SaveGameModList;

namespace ModProfileManager_Addon.ModProfileData
{
    public class ModPresetEntry : IEquatable<ModPresetEntry>
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
        public bool Clone => ModList!=null && ModList.IsClone();
        public Dictionary<string, MPM_POptionDataEntry> GetActivePlibConfig()
        {
            var result = new Dictionary<string, MPM_POptionDataEntry>();

            if (ModList.TryGetPlibOptionsEntry(Path, out Dictionary<string, MPM_POptionDataEntry> result1))
                return result1;

            return result;
        }
        public bool Equals(ModPresetEntry other)
        {
            return other?.ModList?.ModlistPath == this.ModList.ModlistPath && other?.Path == this.Path && other?.Clone == this.Clone;
        }
        public override bool Equals(object obj) => obj is ModPresetEntry other && Equals(other);

        public static bool operator ==(ModPresetEntry a, ModPresetEntry b) => a?.ModList?.ModlistPath == b?.ModList?.ModlistPath && a?.Path == b?.Path && b?.Clone == a?.Clone;
        public static bool operator !=(ModPresetEntry a, ModPresetEntry b) => !(a == b);
        public override int GetHashCode()
        {
            return Path.GetHashCode() ^ ModList.ModlistPath.GetHashCode() + ModList.IsClone().GetHashCode();
        }
    }
}
