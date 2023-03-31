using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
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

        public Dictionary<string, List<KMod.Label>> SavePoints = new();

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

        public void WriteModlistToFile()
        {
            try
            {
                var path = !IsModPack ? Path.Combine(ModAssets.ModPath, ModlistPath + ".json") : Path.Combine(ModAssets.ModPacksPath, ModlistPath + ".json");

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
                ReferencedColonySaveName = referencedColonySave;
                ModlistPath = referencedColonySave;
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


        public List<KMod.Label> TryGetModListEntry(string path)
        {
            this.SavePoints.TryGetValue(path, out var result);
            return result;
        }
        public bool AddOrUpdateEntryToModList(string subSavePath, List<KMod.Label> mods, bool isModPack = false)
        {
            this.IsModPack = isModPack;
            bool hasBeenInitialized = false;
            if (TryGetModListEntry(subSavePath) == null)
            {
                hasBeenInitialized = true;
            }
            SavePoints[subSavePath] = mods;
            this.WriteModlistToFile();
            return hasBeenInitialized;
        }

    }
}
