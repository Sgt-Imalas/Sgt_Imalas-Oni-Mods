using Database;
using Klei.AI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ModInfo;
using static ResearchTypes;
using static STRINGS.UI.TOOLS;

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


        public void OpenPopUpToChangeName(System.Action callBackAction = null)
        {
            FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, PauseScreen.Instance.transform.parent.gameObject);
            fileNameDialog.SetTextAndSelect(ConfigName);
            fileNameDialog.onConfirm = (System.Action<string>)(newName =>
            {
                if (newName.EndsWith(".sav"))
                {
                    int place = newName.LastIndexOf(".sav");

                    if (place != -1)
                        newName = newName.Remove(place, 4);
                }
                this.ChangenName(newName);

                if(callBackAction!=null) 
                    callBackAction.Invoke();
            });
        }

        public void ChangenName(string newName)
        {
            ConfigName = newName;
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
            this.skillAptitudes = skillAptitudes.Select(kvp => new KeyValuePair<string, float>(kvp.Key.Id, kvp.Value)).ToList();
        }
        public MinionStatConfig() { }
        public MinionStatConfig(string fileName, string configName, List<string> traits, string stressTrait, string joyTrait, List<KeyValuePair<string, int>> startingLevels, List<KeyValuePair<string, float>> skillAptitudes)
        {
            FileName = fileName;
            ConfigName = configName;
            Traits = traits;
            this.stressTrait = stressTrait;
            this.joyTrait = joyTrait;
            StartingLevels = startingLevels;
            this.skillAptitudes = skillAptitudes;
            //WriteToFile();
        }

        public static string GenerateHash(string str)
        {
            using (var md5Hasher = MD5.Create())
            {
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
                return BitConverter.ToString(data).Replace("-", "").Substring(0, 16);
            }
        }


        public static MinionStatConfig CreateFromStartingStats(MinionStartingStats startingStats, string fileName)
        {
            List<KeyValuePair<string, float>> skillAptitudes = new List<KeyValuePair<string, float>>();
            foreach (var kvp in startingStats.skillAptitudes)
            {
                skillAptitudes.Add(new KeyValuePair<string, float>(kvp.Key.Id, kvp.Value));
            }

            List<Trait> traits = startingStats.Traits;
            var config = new MinionStatConfig(
                fileName + GenerateHash(System.DateTime.Now.ToString()), fileName,
                startingStats.Traits,
                startingStats.stressTrait,
                startingStats.joyTrait,
                startingStats.StartingLevels.ToList(),
                startingStats.skillAptitudes.ToList());
            return config;
        }
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
        public void DeleteFile()
        {
            try
            {
                var path = Path.Combine(ModAssets.DupeTemplatePath, FileName + ".json");

                var fileInfo = new FileInfo(path);
                fileInfo.Delete();
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not write file, Exception: " + e);
            }
        }
    }
}
