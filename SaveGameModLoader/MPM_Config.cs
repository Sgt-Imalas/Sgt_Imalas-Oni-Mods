using Epic.OnlineServices.PlayerDataStorage;
using FMOD;
using KMod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static Operational;
using static SaveGameModLoader.STRINGS.UI.FRONTEND.MODTAGS;
using static STRINGS.CODEX;

namespace SaveGameModLoader
{
    public class MPM_Config
    {
        [JsonIgnore]
        private static MPM_Config _instance;
        [JsonIgnore]
        public static MPM_Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (IO_Utils.ReadFromFile<MPM_Config>(ModAssets.ConfigPath, out var config))
                    {
                        _instance = config;
                    }
                    else
                    {
                        _instance = new MPM_Config();
                    }
                }
                return _instance;
            }
        }

        public bool hideLocal, hideDev, hidePlatform, hideIncompatible, hideInactive, hideActive, hidePins;


        public void ToggleAll(bool state = true)
        {
            hideDev = state;
            hideLocal = state;
            hidePlatform = state;
            hideIncompatible = state;
            hideInactive = state;
            hideActive = state;
            hidePins = state;
        }

        [JsonIgnore] public bool HasPinned => PinnedMods.Count > 0;

        public HashSet<string> PinnedMods = new HashSet<string>();
        public Dictionary<string, bool> FilterTags = new();

        public Dictionary<string, HashSet<string>> ModTagConfig = new Dictionary<string, HashSet<string>>();

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
            IO_Utils.WriteToFile(this, ModAssets.ConfigPath);
        }

        public void AddFilterTag(string flag, bool save = false)
        {
            if (!FilterTags.ContainsKey(flag))
                FilterTags.Add(flag, false);

            if (save)
                SaveToFile();
        }
        public void RemoveFilterTag(string flag)
        {
            FilterTags.Remove(flag);

            List<string> toRemoveFrom = new();
            foreach (var modFlag in ModTagConfig)
            {
                if (modFlag.Value.Contains(flag))
                    toRemoveFrom.Add(modFlag.Key);
            }
            foreach (var modID in toRemoveFrom)
            {
                RemoveModTag(modID, flag, false);
            }
            SaveToFile();
        }

        internal List<string> GetCheckedTags(string targetModId)
        {
            if (targetModId == null)
            {
                return FilterTags.Keys.Where(filter => FilterTags[filter]).ToList();
            }
            else
            {
                if (ModTagConfig.TryGetValue(targetModId, out var value))
                {
                    return value.ToList();
                }
                return new();
            }
        }

        internal List<string> GetUncheckedTags(string targetModId)
        {
            return FilterTags.Keys.Except(GetCheckedTags(targetModId)).ToList();
        }
        public int GetActiveFilterCount()
        {
            return GetCheckedTags(null).Count();
        }
        public int GetFilterCount()
        {
            return FilterTags.Count;
        }

        internal void SetModTagConfigState(string targetModId, List<Tuple<string, bool>> modifiedRecords)
        {
            if (targetModId != null && targetModId.Length > 0)
            {
                foreach (var record in modifiedRecords)
                {
                    //flag enabled
                    if (record.second)
                    {
                        AddModTag(targetModId, record.first);
                    }
                    //flag disabled
                    else
                    {
                        RemoveModTag(targetModId, record.first);
                    }
                }
                GetModTagConfigUIString(targetModId);
            }
            else
            {
                foreach (var record in modifiedRecords)
                {
                    FilterTags[record.first] = record.second;
                }
            }
            SaveToFile();
        }

        [JsonIgnore] Dictionary<string, string> CachedModTagDescriptions = new();
        public string GetModTagConfigUIString(string targetModId)
        {
            if (targetModId == null || targetModId.Length == 0)
            {
                var tags = FilterTags.Keys.ToList();
                tags.Sort();

                var sb = new StringBuilder(TAGEDITWINDOW.ADJUST_TAG_FILTERS_TOOLTIP);
                sb.AppendLine();
                bool hasTagFiltersActive = false;
                for (int i = 0; i < tags.Count; i++)
                {
                    var taginfo = tags[i];
                    if (FilterTags[taginfo])
                    {
                        hasTagFiltersActive =true ;
                        sb.Append("• ");
                        sb.AppendLine(taginfo);
                    }
                }
                if (!hasTagFiltersActive)
                {
                    sb.Append("• ");
                    sb.AppendLine(TAGEDITWINDOW.NOFILTERS);
                }

                return sb.ToString();
            }
            else
            {
                if (CachedModTagDescriptions.ContainsKey(targetModId))
                    return CachedModTagDescriptions[targetModId];

                if (ModTagConfig.ContainsKey(targetModId))
                {
                    var tags = ModTagConfig[targetModId].ToList();
                    tags.Sort();

                    var sb = new StringBuilder(TAGEDITWINDOW.CURRENTTAGS);
                    sb.AppendLine();
                    for (int i = 0; i < tags.Count; i++)
                    {
                        sb.Append("• ");
                        sb.AppendLine(tags[i]);
                    }
                    CachedModTagDescriptions[targetModId] = sb.ToString();
                    return sb.ToString();
                }

            }

            return string.Empty;
        }
        public void AddModTag(string targetModId, string tag, bool save = true)
        {
            bool changed = false;

            if (!FilterTags.TryGetValue(tag, out _))
            {
                changed = true;
                AddFilterTag(tag, save);
            }

            if (!ModTagConfig.ContainsKey(targetModId))
            {
                ModTagConfig.Add(targetModId, new HashSet<string>());
                changed = true;
            }

            if (!ModTagConfig[targetModId].Contains(tag))
            {
                ModTagConfig[targetModId].Add(tag);
                changed = true;
            }

            if (changed)
            {
                CachedModTagDescriptions.Remove(targetModId);
                if (save)
                    SaveToFile();
            }
        }
        public void RemoveModTag(string targetModId, string tag, bool save = true)
        {
            bool changed = false;
            if (!ModTagConfig.ContainsKey(targetModId))
            {
                return;
            }

            if (ModTagConfig[targetModId].Contains(tag))
            {
                ModTagConfig[targetModId].Remove(tag);
                changed = true;
            }
            if (ModTagConfig[targetModId].Count == 0)
            {
                ModTagConfig.Remove(targetModId);
                changed = true;
            }

            if (changed)
            {
                CachedModTagDescriptions.Remove(targetModId);
                if (save)
                    SaveToFile();
            }
        }

        internal bool ModHasAnyTags(string staticModId) => ModTagConfig.TryGetValue(staticModId, out var val) && val.Count > 0;

        internal void AddAutoFetchedSteamTags(string modID, string m_rgchTags)
        {
            return;
            //too low quality information for meaningful tagging

            modID += ".Steam";
            string[] tags = m_rgchTags.Split(',');
            if (ModTagConfig.ContainsKey(modID))
                return;

            foreach (string tag in tags)
            {
                AddModTag(modID, tag, false);
            }
        }

        internal bool ApplyFilters(KMod.Mod mod)
        {
            var label = mod.label;
            bool showMod = true;

            if (showMod && hideLocal)
                showMod = label.distribution_platform != Label.DistributionPlatform.Local;

            if (showMod && hideDev)
                showMod = label.distribution_platform != Label.DistributionPlatform.Dev;

            if (showMod && hidePlatform)
                showMod = label.distribution_platform == Label.DistributionPlatform.Dev || label.distribution_platform == Label.DistributionPlatform.Local;

            if (showMod && hideIncompatible)
                showMod = mod.contentCompatability == ModContentCompatability.OK;

            bool modIsActive = mod.IsActive();

            if (showMod && hideActive)
                showMod = !modIsActive;

            if (showMod && hideInactive)
                showMod = modIsActive;

            if (showMod)
            {
                showMod = this.ShouldFilterMod(label.defaultStaticID);
            }

            return showMod;
        }

        private bool ShouldFilterMod(string defaultStaticID)
        {
            //while syncing , no filter tags created or no filter tags enabled
            if (ModlistManager.Instance.IsSyncing || FilterTags.Count == 0 || FilterTags.All(f => !f.Value))
                return true;


            if (ModTagConfig.TryGetValue(defaultStaticID, out var Taglist))
            {
                foreach (var Tag in Taglist)
                {
                    if (FilterTags.TryGetValue(Tag, out var showTag) && showTag)
                        return true;
                }
            }
            return false;
        }

        internal void ToggleAllFilters(bool v)
        {
            var keys = new List<string>(FilterTags.Keys);
            foreach (var key in keys)
            {
                FilterTags[key] = v;
            }
            SaveToFile();
        }
    }
}
