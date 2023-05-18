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

namespace DupePrioPresetManager
{
    internal class MinionPrioPreset
    {
        public string FileName;
        public string ConfigName;
        public Dictionary<string, int> ChoreGroupPriorities = new Dictionary<string, int>();


        public void OpenPopUpToChangeName(System.Action callBackAction = null)
        {
            FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, Global.Instance.globalCanvas);
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
            DeleteFile();
            ConfigName = newName; 
            FileName =  FileNameWithHash(newName);
            WriteToFile();
        }

        static string FileNameWithHash(string filename)
        {
            return filename.Replace(" ", "_") + "_" + GenerateHash(System.DateTime.Now.ToString());
        }

        public MinionPrioPreset(string fileName, string configName, IPersonalPriorityManager priorityManager)
        {
            FileName = fileName;
            ConfigName = configName;

            foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
            {
                if (resource.userPrioritizable)
                {
                    ChoreGroupPriorities[resource.Id] = priorityManager.GetPersonalPriority(resource);
                }
            }
        }
        public MinionPrioPreset() { }
        public static string GenerateHash(string str)
        {
            using (var md5Hasher = MD5.Create())
            {
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
                return BitConverter.ToString(data).Replace("-", "").Substring(0, 6);
            }
        }


        public static MinionPrioPreset CreateFromPriorityManager(IPersonalPriorityManager priorityManager, string nameOverride = "")
        {
            
            string dupeName = nameOverride.Length>0 ? nameOverride : STRINGS.UNNAMEDPRESET;

            var config = new MinionPrioPreset(
                FileNameWithHash(dupeName),
                dupeName, priorityManager);
            return config;
        }
        public static MinionPrioPreset ReadFromFile(FileInfo filePath)
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
                    MinionPrioPreset modlist = JsonConvert.DeserializeObject<MinionPrioPreset>(jsonString);
                    return modlist;
                }
            }
        }

        public void ChangeValue(ChoreGroup group, int delta)
        {
            if(group != null && ChoreGroupPriorities.ContainsKey(group.Id))
            {
                int val = (ChoreGroupPriorities[group.Id] + delta) % 6;
                if (val < 0) val += 6;

                ChoreGroupPriorities[group.Id] = val;
                DeleteFile();
                WriteToFile();
            }
        }



        internal void ApplyPreset(IPersonalPriorityManager priorityManager)
        {
            if (priorityManager != null)
            {
                var dbChoreGroups = Db.Get().ChoreGroups;
                foreach (var prio in this.ChoreGroupPriorities)
                {
                    if (dbChoreGroups.TryGet(prio.Key) != null && !priorityManager.IsChoreGroupDisabled(dbChoreGroups.TryGet(prio.Key)))
                    {
                        priorityManager.SetPersonalPriority(dbChoreGroups.TryGet(prio.Key), prio.Value);
                    }
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
                SgtLogger.logError("Could not delete file, Exception: " + e);
            }
        }
    }
}
