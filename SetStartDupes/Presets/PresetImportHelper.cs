using Klei.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;

namespace SetStartDupes.Presets
{
    /// <summary>
    /// Imports crew presets from dgsm for people who upgraded to DSS from that garbage
    /// </summary>
    public static class PresetImportHelper
    {
        internal static bool TryImportCrew(FileInfo file, out MinionCrewPreset convertedCrew)
        {
            convertedCrew = null;
            if (file.Exists && IO_Utils.ReadFromFile<ImportedCrew>(file, out var importedCrew))
            {
                convertedCrew = importedCrew.ToCrewPreset(file);
            }
            return convertedCrew != null && convertedCrew.Crewmates.Any();
        }

        //crew preset
        public class ImportedCrew
        {
            public MinionCrewPreset ToCrewPreset(FileInfo filePath)
            {
                var preset = new MinionCrewPreset();
                preset.CrewName = Name;
                preset.CreationDate = Date;

                foreach (var importedMember in Info)
                {
                    var converted = importedMember.ToStatConfigWithName();
                    if (converted != null)
                        preset.Crewmates.Add(converted);
                }

                if (DLC1 && preset.Crewmates.Any())
                    preset.Crewmates.ForEach(item => item.second.DLCID = DlcManager.EXPANSION1_ID);
                preset.Imported = true;
                preset.OriginalFilePath = filePath;

				return preset;
            }
            public List<ImportedCrewMember> Info = new();
            public string Name;
            public string Date;
            public bool DLC1;
            public bool Vanilla;
        }
        //dupe preset
        public class ImportedCrewMember
        {
            public int Personality, JoyTrait, StressTrait; //hashed strings of the Ids by the looks of it
            public List<int> Traits = new();
            public string Name;
            public Dictionary<string, int> StartingLevels = new();
            public Dictionary<int, float> SkillAptitudes = new();

            internal Tuple<string, MinionStatConfig> ToStatConfigWithName()
            {
                var statConfig = new MinionStatConfig();
                var db = Db.Get();

                var personality = db.Personalities.TryGet(new HashedString(Personality));
                if (personality == null)
                    return null;

                statConfig.PersonalityID = personality.nameStringKey;
                statConfig.Model = personality.model;
                statConfig.SetName(Name);

                var traitsDb = db.traits;

                var joy = traitsDb.TryGet(new HashedString(JoyTrait));
                if (joy != null)
                    statConfig.joyTrait = joy.Id;

                var stress = traitsDb.TryGet(new HashedString(StressTrait));
                if (stress != null)
                    statConfig.stressTrait = stress.Id;

                foreach (var hashValue in Traits)
                {
                    var trait = traitsDb.TryGet(new HashedString(hashValue));
                    if (trait != null)
                        statConfig.Traits.Add(trait.Id);
                }

                foreach (var attributeId in ModAssets.GET_ALL_ATTRIBUTES())
                {
                    if (!StartingLevels.ContainsKey(attributeId))
                        StartingLevels[attributeId] = 0;
                }

                foreach (var attributeKVP in StartingLevels)
                {
                    statConfig.StartingLevels.Add(attributeKVP);
                }
                foreach (var hashedkvp in SkillAptitudes)
                {
                    var aptitude = db.SkillGroups.TryGet(new HashedString(hashedkvp.Key));
                    if (aptitude != null)
                    {
                        statConfig.skillAptitudes.Add(new(aptitude.Id, 1));
                    }
                }

                return new(personality.nameStringKey, statConfig);
            }
        }

    }
}
