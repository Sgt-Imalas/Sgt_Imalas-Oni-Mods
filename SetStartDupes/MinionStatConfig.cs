using Beached_ModAPI;
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
    public class MinionStatConfig
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
            FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, ModAssets.ParentScreen);
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
            return filename.Replace(" ", "_");// + "_" + GenerateHash(System.DateTime.Now.ToString());
        }

        public MinionStatConfig(string fileName, string configName, List<Trait> traits, Trait stressTrait, Trait joyTrait, List<KeyValuePair<string, int>> startingLevels, List<KeyValuePair<SkillGroup, float>> skillAptitudes)
        {
            FileName = fileName;
            ConfigName = configName;
            Traits = traits.Select(trait => trait.Id).ToList();
            Traits.RemoveAll(trait => trait == ANCIENTKNOWLEDGE);
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
                return BitConverter.ToString(data).Replace("-", "").Substring(0, 6);
            }
        }
        

        public static MinionStatConfig CreateFromStartingStats(MinionStartingStats startingStats)
        {
            List<KeyValuePair<string, float>> skillAptitudes = new List<KeyValuePair<string, float>>();
            foreach (var kvp in startingStats.skillAptitudes)
            {
                skillAptitudes.Add(new KeyValuePair<string, float>(kvp.Key.Id, kvp.Value));
            }
            string dupeName = startingStats.Name+ " "+STRINGS.UNNAMEDPRESET;

            var config = new MinionStatConfig(
                FileNameWithHash(dupeName),
                dupeName,
                startingStats.Traits,
                startingStats.stressTrait,
                startingStats.joyTrait,
                startingStats.StartingLevels.ToList(),
                startingStats.skillAptitudes.ToList());

            if (ModAssets.BeachedActive)
                config.Traits.Add(Beached_API.GetCurrentLifeGoal(startingStats).Id);

            return config;
        }

        static string ANCIENTKNOWLEDGE = "AncientKnowledge";
        public void ApplyPreset(MinionStartingStats referencedStats)
        {
            

            referencedStats.Name = this.ConfigName.Replace(STRINGS.UNNAMEDPRESET,string.Empty);
            bool HadAncientKnowledge = referencedStats.Traits.Any(trait => trait.Id == ANCIENTKNOWLEDGE);
            referencedStats.Traits.Clear();
            var traitRef = Db.Get().traits;

            if (!Traits.Contains(MinionConfig.MINION_BASE_TRAIT_ID))
                Traits.Add(MinionConfig.MINION_BASE_TRAIT_ID);


            if (HadAncientKnowledge)
            {
                Traits.Add(ANCIENTKNOWLEDGE);
            }
            else
                Traits.RemoveAll(trait => trait == ANCIENTKNOWLEDGE);


            foreach (var traitID in this.Traits)
            {
                var Trait = traitRef.TryGet(traitID);

                if (ModAssets.GetTraitListOfTrait(Trait) == DupeTraitManager.NextType.Beached_LifeGoal)
                {
                    Beached_API.RemoveLifeGoal(referencedStats);
                    Beached_API.SetLifeGoal(referencedStats, Trait,false);
                    continue;
                }

                if (Trait != null && ModAssets.TraitAllowedInCurrentDLC(traitID))
                {
                    referencedStats.Traits.Add(Trait);
                }
            }
            
            referencedStats.StartingLevels.Clear();
            foreach(var startLevel in this.StartingLevels)
            {
                referencedStats.StartingLevels[startLevel.Key] = startLevel.Value; 
            }
            if(!ModConfig.Instance.NoJoyReactions)
            {
                referencedStats.joyTrait = traitRef.Get(this.joyTrait);
            }
            else
            {
                referencedStats.joyTrait = traitRef.Get("None");
            }
            if(!ModConfig.Instance.NoStressReactions)
            {
                referencedStats.stressTrait = traitRef.Get(this.stressTrait);
            }
            else
            {
                referencedStats.stressTrait = traitRef.Get("None");
            }

            if (ModAssets.DupeTraitManagers.ContainsKey(referencedStats))
            {
                ModAssets.DupeTraitManagers[referencedStats].ResetPool();
            }

            var AptitudeRef = Db.Get().SkillGroups;
            referencedStats.skillAptitudes.Clear();

            foreach(var skillAptitude in this.skillAptitudes)
            {
                SkillGroup targetGroup = AptitudeRef.TryGet(skillAptitude.Key);
                referencedStats.skillAptitudes[targetGroup] = skillAptitude.Value;
            }
            if (ModAssets.OtherModBonusPoints.ContainsKey(referencedStats))
            {
                ModAssets.OtherModBonusPoints.Remove(referencedStats);
            }
            if (ModAssets.DupeTraitManagers.ContainsKey(referencedStats))
            {
                ModAssets.DupeTraitManagers[referencedStats].RecalculateAll();
            }
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
        public string SkillGroupName(string groupID)
        {
            if (groupID == null)
                return "";
            else
            {

                var skillGroup = Db.Get().SkillGroups.TryGet(groupID);
                if(skillGroup == null)
                {
                    return STRINGS.MISSINGSKILLGROUP;
                }
                else
                {
                    string relevantSkillID = skillGroup.relevantAttributes.First().Id;
                    return string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, ModAssets.GetChoreGroupNameForSkillgroup(skillGroup), SkillGroup(skillGroup), SkillLevel(relevantSkillID));
                }
            }
        }
        public string SkillGroupDesc(string groupID)
        {
            if (groupID == null)
                return "";
            else
            {
                var skillGroup = Db.Get().SkillGroups.TryGet(groupID);
                return ModAssets.GetSkillgroupDescription(skillGroup,id: groupID);
            }
        }
        public string SkillGroup(SkillGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME");
        }
        string SkillLevel(string skillID)
        {
            return StartingLevels.Find((skill) => skill.Key == skillID).Value.ToString();
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
