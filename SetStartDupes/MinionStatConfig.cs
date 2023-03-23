using Database;
using Klei.AI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ResearchTypes;

namespace SetStartDupes
{
    internal class MinionStatConfig
    {
        public string FileName;
        public string ConfigName;

        public List<string> Traits = new List<string>();
        public string stressTrait;
        public string joyTrait;
        public List<KeyValuePair<string, int>> StartingLevels = new List<KeyValuePair<string, int>>();
        public List<KeyValuePair<string, float>> skillAptitudes = new List<KeyValuePair<string, float>>();


        public static MinionStatConfig ReadFromFile(FileInfo filePath)
        {
            if (!filePath.Exists || filePath.Extension != ".json")
            {
                SgtLogger.logwarning("Not a valid dupe preset.");
                return null;
            }
            else
            {
                FileStream filestream = filePath.OpenRead();
                using (var sr = new StreamReader(filestream))
                {
                    string jsonString = sr.ReadToEnd();
                    MinionStatConfig modlist = JsonConvert.DeserializeObject<MinionStatConfig>(jsonString);
                    return modlist;
                }
            }
        }

        public void ChangenName(string newName)
        {
            ConfigName= newName;
            WriteToFile();
        }

        public MinionStatConfig(string fileName, string configName, List<Trait> traits, Trait stressTrait, Trait joyTrait, List<KeyValuePair<string, int>> startingLevels, List<KeyValuePair<SkillGroup, float>> skillAptitudes)
        {
            FileName = fileName;
            ConfigName = configName;
            Traits = traits.Select(trait => trait.Id).ToList();
            this.stressTrait = stressTrait.Id;
            this.joyTrait = joyTrait.Id;
            StartingLevels = startingLevels;
            this.skillAptitudes = skillAptitudes.Select(kvp => new KeyValuePair<string, float> (kvp.Key.Id, kvp.Value)).ToList();
            WriteToFile();
        }

        public static MinionStatConfig CreateFromStartingStats(MinionStartingStats startingStats,string fileName)
        {
            List<KeyValuePair<string, float>> skillAptitudes = new List<KeyValuePair<string, float>>();
            foreach(var kvp in startingStats.skillAptitudes)
            {
                skillAptitudes.Add(new KeyValuePair<string, float>(kvp.Key.Id, kvp.Value));
            }

            List<Trait> traits = startingStats.Traits;
            var config = new MinionStatConfig(
                fileName, fileName, 
                startingStats.Traits,
                startingStats.stressTrait,
                startingStats.joyTrait,
                startingStats.StartingLevels.ToList(),
                startingStats.skillAptitudes.ToList());
            return config;
        }

        public void WriteToFile()
        {
            try
            {
                var path = Path.Combine(ModAssets.DupeTemplatePath, FileName + ".json");

                var fileInfo = new FileInfo(path);
                FileStream fcreate = fileInfo.Open(FileMode.Create);

                var JsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (var streamWriter = new StreamWriter(fcreate))
                {
                    streamWriter.Write(JsonString);
                }
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not write file, Exception: " + e);
            }
        }
    }
}
