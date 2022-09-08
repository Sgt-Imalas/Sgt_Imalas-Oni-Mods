using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SaveGame;

namespace SaveGameModLoader
{
    public class SaveGameModList
    {
        public enum DLCType
        {
            undefined =0,
            baseGame = 1,
            spacedOut = 2,

        }

        public string ReferencedColonySaveName;
        public string ModlistPath;
        public bool IsModPack = false;
        public DLCType Type = 0;

        public Dictionary<string, List<KMod.Label>> SavePoints = new();

        public static bool ModListFileExists(string filePath)
        {
            bool exist = File.Exists(filePath);
            return exist;
        }
        public static SaveGameModList ReadModlistListFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.Log("No stored ModList found.");
                return null;
            }
            else
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<SaveGameModList>(jsonString);
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
        public SaveGameModList(string referencedColonySave, bool _isModPack=false)
        {
            if (!_isModPack) { 
                ReferencedColonySaveName = GetModListFileName(referencedColonySave);
                ModlistPath = GetModListFileName(referencedColonySave);
            }
            else
            {
                ReferencedColonySaveName = referencedColonySave;
                ModlistPath = referencedColonySave;
            }
            Type = DlcManager.IsExpansion1Active() ? DLCType.spacedOut : DLCType.baseGame;
        }

        public static string GetModListFileName(string pathOfReference)
        {
            string FileNameInSpe = Directory.GetParent(pathOfReference).Name;
            if (FileNameInSpe.Contains("auto_save"))
            {
                FileNameInSpe = GetModListFileName(Directory.GetParent(pathOfReference).FullName);
            }
            return FileNameInSpe;
        }


        public void WriteModlistToFile()
        {
            try 
            {
                if (!this.IsModPack)
                {
                    Debug.Log("Writing mod config to " + ModlistPath);
                    File.WriteAllText(ModAssets.ModPath + ModlistPath + ".json", JsonConvert.SerializeObject(this));
                }
                else
                {
                    Debug.Log("Writing mod pack to " + ModlistPath);
                    File.WriteAllText(ModAssets.ModPacksPath + ModlistPath + ".json", JsonConvert.SerializeObject(this));
                }
            }
            catch(Exception e)
            {
                Debug.LogError("Could not write file, Exception: " + e);
            }
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
