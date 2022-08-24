using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SaveGame;
using static SaveGameModLoader.ModListEntree;

namespace SaveGameModLoader
{
    public class SaveGameModList
    {
        public string ReferencedColonySaveName;
        public string ModlistPath;
        public readonly string ColonyGuid;

        public List<ModListEntree> SavePoints = new();

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
        public SaveGameModList(string referencedColonySave, string guid)
        {
            ColonyGuid = guid;
            ReferencedColonySaveName = GetModListFileName(referencedColonySave);

            ModlistPath = ModAssets.ModPath + GetModListFileName(referencedColonySave);
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

        public static string StripAllPaths(string toStrip)
        {
            var fileOnly = Path.GetFileNameWithoutExtension(toStrip);
            var output = string.Empty;
            if (toStrip.Contains("auto_save"))
            {
                output = "\\auto_save\\";
            }
            output += fileOnly;
            return output;
        }

        public void WriteModlistToFile()
        {
            try 
            {
                Debug.Log("Writing mod config to " + ModlistPath);
                File.WriteAllText(ModlistPath + ".json", JsonConvert.SerializeObject(this));
            }
            catch(Exception e)
            {
                Debug.LogError("Could not write file, Exception: " + e);
            }
        }


        public bool AddOrUpdateEntryToModList(string subSavePath, List<KMod.Label> mods)
        {
            bool initializeCall = false;
            string SubSaveFileName = subSavePath;

            var Entry = SavePoints.Find(s => s.referencedSavePath == SubSaveFileName);
            if (Entry == null)
            {
                initializeCall = true;
                Entry = new ModListEntree();
                Entry.referencedSavePath = SubSaveFileName;
                SavePoints.Add(Entry);
            }
            else
                Debug.Log("Mod config already exists for this save game, overwriting..");
            Entry.EnabledMods.Clear();
            Entry.EnabledMods.AddRange(mods);

            this.WriteModlistToFile();
            return initializeCall;
        }

    }
    public class ModListEntree
    {
        public string referencedSavePath;
        public List<KMod.Label> EnabledMods = new();
    }
}
