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
        public string ColonyName;
        public string ModlistPath;
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
        public SaveGameModList(string referencedModFolder = "")
        {
            string ColonyToReference;
            if(referencedModFolder == "")
            {
                ColonyToReference = Path.GetFileName(SaveLoader.GetActiveSaveFilePath());
            }
        }


        public void WriteModlistToFile()
        {
            ModlistPath = ModAssets.ModPath + ColonyName + ".json";
            File.WriteAllText(ModlistPath, JsonConvert.SerializeObject(this));
        }


        public void AddEntryToModList(string subSavePath, List<KMod.Label> mods)
        {
            var Entry = new ModListEntree();
            Entry.EnabledMods = mods;
            Entry.referencedSavePath = subSavePath;

            if (!SavePoints.Contains(Entry))
            {
                SavePoints.Add(Entry);
                WriteModlistToFile();
            }
            else
                Debug.Log("Entry already exists");
        }

    }
    public class ModListEntree
    {
        public string referencedSavePath;
        public List<KMod.Label> EnabledMods = new();
    }
}
