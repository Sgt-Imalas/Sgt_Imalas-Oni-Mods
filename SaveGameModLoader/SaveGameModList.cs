using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static SaveGame;

namespace SaveGameModLoader
{
    public class SaveGameModList
    {
        public enum DLCType
        {
            undefined = 0,
            baseGame = 1,
            spacedOut = 2,
            modPack = 3

        }

        public string ReferencedColonySaveName;
        public string ModlistPath;
        public bool IsModPack = false;
        public DLCType Type = 0;


        public Dictionary<string, Dictionary<string, MPM_POptionDataEntry>> PlibModConfigSettings = new();
        public Dictionary<string, List<KMod.Label>> SavePoints = new();

        public Dictionary<string, List<KMod.Label>> GetSavePoints() => SavePoints;
        public static SaveGameModList ReadModlistListFromFile(FileInfo filePath)
        {

            //SgtLogger.l("filepath "+ filePath);
            if (!filePath.Exists || filePath.Extension != ".json")
            {
                SgtLogger.logwarning("Not a valid ModList.");
                return null;
            }
            else
            {
                FileStream filestream = filePath.OpenRead();
                using (var sr = new StreamReader(filestream))
                {
                    string jsonString = sr.ReadToEnd();
                    SaveGameModList modlist = JsonConvert.DeserializeObject<SaveGameModList>(jsonString);
                    return modlist;
                }
            }
        }

        private Dictionary<string, SaveLoader.SaveFileEntry> _saveLoaderFiles = null;

        public void CleanupDuplicates()
        {
            List<string> savePointsToCleanup = new List<string>(16);
            foreach (var entry in this.SavePoints)
            {
                string currentSavePoint = entry.Key;
                if (!File.Exists(currentSavePoint))
                {
                    savePointsToCleanup.Add(currentSavePoint);
                }
            }

            if (savePointsToCleanup.Count > 0)
            {
                SgtLogger.l($"cleaning {savePointsToCleanup.Count} obsolete entries from mod profile {ReferencedColonySaveName}");
                foreach (var savePointKey in savePointsToCleanup)
                {
                    SavePoints.Remove(savePointKey);
                    PlibModConfigSettings.Remove(savePointKey);
                }
                WriteModlistToFile();
            }
        }
        public bool DeleteFileIfEmpty()
        {
            var path = !IsModPack ? Path.Combine(ModAssets.ModPath, ModlistPath + ".json") : Path.Combine(ModAssets.ModPacksPath, ModlistPath + ".json");

            if (SavePoints.Count == 0)
            {
                if (File.Exists(path))
                    File.Delete(path);
                return true;
            }
            return false;
        }


        public void WriteModlistToFile()
        {
            try
            {
                var path = !IsModPack ? Path.Combine(ModAssets.ModPath, ModlistPath + ".json") : Path.Combine(ModAssets.ModPacksPath, ModlistPath + ".json");

                if (DeleteFileIfEmpty())
                    return;



                var fileInfo = new FileInfo(path);

                FileStream fcreate = fileInfo.Open(FileMode.Create);//(path, FileMode.Create);

                var JsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (var streamWriter = new StreamWriter(fcreate))
                {
                    if (!IsModPack)
                    {
                        SgtLogger.log("Writing save game profile to " + ModlistPath);
                    }
                    else
                    {
                        SgtLogger.log("Writing custom profile to " + ModlistPath);
                    }
                    streamWriter.Write(JsonString);
                }
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not write file, Exception: " + e);
            }
        }

        /// <summary>
        /// When Modlist is created by Deserializing
        /// </summary>
        public SaveGameModList() { }

        /// <summary>
        /// When Modlist is created by the game
        /// </summary>
        /// <param name="referencedColonySave"></param>
        /// <param name="guid"></param>
        public SaveGameModList(string referencedColonySave, bool _isModPack = false)
        {
            if (!_isModPack)
            {
                ReferencedColonySaveName = GetModListFileName(referencedColonySave);
                ModlistPath = GetModListFileName(referencedColonySave);
                Type = DlcManager.IsExpansion1Active() ? DLCType.spacedOut : DLCType.baseGame;
            }
            else
            {
                ReferencedColonySaveName = ModAssets.GetSanitizedNamePath(referencedColonySave);
                ModlistPath = ModAssets.GetSanitizedNamePath(referencedColonySave);
                Type = DLCType.modPack;
            }
        }




        public static string GetModListFileName(string pathOfReference)
        {
            if (string.IsNullOrEmpty(pathOfReference))
                return null;
            string FileNameInSpe = Directory.GetParent(pathOfReference).Name;
            if (FileNameInSpe.Contains("auto_save"))
            {
                FileNameInSpe = GetModListFileName(Directory.GetParent(pathOfReference).FullName);
            }
            return FileNameInSpe;
        }


        public bool TryGetModListEntry(string path, out List<KMod.Label> result) => this.SavePoints.TryGetValue(path, out result);

        public bool AddOrUpdateEntryToModList(string subSavePath, List<KMod.Label> mods, bool isModPack = false)
        {
            this.IsModPack = isModPack;
            bool hasBeenInitialized = false;
            if (!TryGetModListEntry(subSavePath, out _))
            {
                hasBeenInitialized = true;
            }
            SavePoints[subSavePath] = mods;

            var plibConfigs = ReadPlibOptions();
            if(plibConfigs != null && plibConfigs.Count() > 0)
            {
                PlibModConfigSettings[subSavePath] = plibConfigs;
            }
            this.WriteModlistToFile();
            return hasBeenInitialized;
        }

        public class MPM_POptionDataEntry
        {
            public string ConfigFilePath;
            public bool SharedLocation=false;
            public JObject ModConfigData;
            public MPM_POptionDataEntry(string path, bool shared,JObject data)
            {
                ConfigFilePath = path;
                ModConfigData = data;
                SharedLocation = shared;
            }
        }

        public bool TryGetPlibOptionsEntry(string path, out Dictionary<string, MPM_POptionDataEntry> entry)
        {
            entry = null;
            return (PlibModConfigSettings != null && PlibModConfigSettings.TryGetValue(path, out entry));
        }

        public static void WritePlibOptions(Dictionary<string, MPM_POptionDataEntry> modConfigEntries)
        {
            if (!Config.Instance.SavePlibOptions)
                return;
            var manager = Global.Instance.modManager;
            if(modConfigEntries!=null)
            {
                SgtLogger.l("applying mod options, " + modConfigEntries.Count() + " entries in profile");


                foreach(var modConfig in modConfigEntries)
                {
                    string modID = modConfig.Key;
                    var mod = manager.mods.FirstOrDefault(m => m.staticID == modID);
                    if(mod == null)
                    {
                        SgtLogger.l(modID + " not found.");
                        continue;
                    }

                    var config = modConfig.Value;
                    var fileName = Path.GetFileName(config.ConfigFilePath);
                    var parentFolder = config.SharedLocation ? Path.Combine(KMod.Manager.GetDirectory(), "config") : mod.ContentPath;
                    var TargetConfigFilePath = Path.Combine(parentFolder, fileName);
                    SgtLogger.l(TargetConfigFilePath, modID);
                }
            }
        }

        public Dictionary<string, MPM_POptionDataEntry> ReadPlibOptions()
        {
            IEnumerable<PForwardedComponent> allComponents = PRegistry.Instance.GetAllComponents("PeterHan.PLib.Options.POptions");

            //static id + data object
            Dictionary<string, MPM_POptionDataEntry> ModConfigs = new();
            SgtLogger.l("fetching plib config settings for mod profile");
            SgtLogger.l(allComponents.Count()+" components found");

            foreach (var component in allComponents)
            {
                IDictionary<string, Type> instanceData = component.GetInstanceData<IDictionary<string, Type>>();
                foreach (var op in instanceData)
                {
                    string modID = op.Key;
                    
                    var configFilePath = POptions.GetConfigFilePath(op.Value);
                    var attribute = op.Value.GetCustomAttribute<ConfigFileAttribute>();
                    bool shared = false; 
                    if (attribute != null)
                    {
                        shared = attribute.UseSharedConfigLocation;
                    }


                    if (!File.Exists(configFilePath))
                    {
                        SgtLogger.l("config file for " + op.Key + " not found");
                        continue;
                    }
                    try
                    {
                        using (StreamReader file = File.OpenText(configFilePath))
                        {
                            using (JsonTextReader reader = new JsonTextReader(file))
                            {
                                JObject o2 = (JObject)JToken.ReadFrom(reader);
                                SgtLogger.l(o2.ToString(), modID);
                                ModConfigs.Add(modID, new(configFilePath, shared, o2));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SgtLogger.warning(ex.Message);
                    }
                }
            }
            return ModConfigs;
        }
    }
}
