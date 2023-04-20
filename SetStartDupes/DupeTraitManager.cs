using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace SetStartDupes
{
    public class DupeTraitManager : KMonoBehaviour
    {
        [Serialize]
        public Dictionary<string, int> USEDSTATS = new();

        public List<string> currentTraitIds = new();
        [Serialize]
        public int strengthSkillHeightHolder = -1;


        [Serialize]
        public List<SkillGroup> ActiveInterests = new();

        public Dictionary<SkillGroup, float> referencedInterests;
        public Dictionary<string, int> dupeStatPoints;

        int FallBack = -1;

        public enum NextType
        {
            geneShufflerTrait,
            posTrait,
            needTrait,
            negTrait,
            joy,
            stress,
            undefined
        }

        public List<SkillGroup> GetInterestsWithStats()
        {
            //Debug.Log("EEEEEEEEEEEEEEEE");
            ActiveInterests.Clear();
            foreach (var skillGroup in referencedInterests)
            {
                ActiveInterests.Add(skillGroup.Key);
                //Debug.Log(skillGroup.Key.Name + ", " + skillGroup.Value);
            }
            
            return ActiveInterests;
        }
        public void GetNextInterest(int index, bool backwards = false)
        {
            //Debug.Log(index+ ", count "+ ActiveInterests.Count());
            if (index>= ActiveInterests.Count())
                return;
            GetNextInterest(ActiveInterests[index].Id, backwards);
        }

        public void GetNextInterest(string id, bool backwards = false)
        {
            var allSkills = Db.Get().SkillGroups.resources;
            SkillGroup OldSkill = allSkills.Find(skill => skill.Id == id);
            SkillGroup newSkill = OldSkill;
            int indexInAllSkills = allSkills.FindIndex(skill => skill == OldSkill);
            var availableSkills = allSkills.Except(referencedInterests.Keys);

            if (availableSkills.Count() == 0)
                return;
            //index += backwards ? -1 : 1; 

            ///Finding the next skill in Line thats not already on the list
            for (int i = 0; i < allSkills.Count; i++)
            {
                var potentialSkill = backwards
                    ? allSkills[(allSkills.Count - i + indexInAllSkills) % allSkills.Count]
                    : allSkills[(i + indexInAllSkills) % allSkills.Count];
                if (availableSkills.Contains(potentialSkill))
                {
                    newSkill = potentialSkill;
                    break;
                }
            }

            ///removing old boni
            int dupeStatValue = FallBack;
            foreach (var relevantAttribute in OldSkill.relevantAttributes)
            {
                //dupeStatPoints
                string statId = relevantAttribute.Id;
                dupeStatValue = dupeStatPoints[statId] > dupeStatValue
                    ? dupeStatPoints[statId] 
                    : dupeStatValue;
                if (DoesRemoveReduceStats(statId))
                {
                    dupeStatPoints[statId] = 0;
                }
                else
                {
                    dupeStatValue = FallBack;
                }
            }
            referencedInterests.Remove(OldSkill);

            
            //Debug.Log("end");
            ///adding new boni
            foreach (var relevantAttribute in newSkill.relevantAttributes)
            {
                //dupeStatPoints
                string statId = relevantAttribute.Id;
                if (IsThisANewSkill(statId))
                {
                    dupeStatPoints[statId] = dupeStatValue;
                }
                else
                {
                    FallBack = dupeStatValue;
                }
            }
            //Debug.Log("stats:: (dupestatval:" + dupeStatValue + ")");
            //foreach (var v in dupeStatPoints)
            //{
            //    Debug.Log(v.Key + ", " + v.Value);
            //}
            referencedInterests.Add(newSkill, 1);
            ActiveInterests[GetCurrentIndex(OldSkill.Id)] = newSkill;
        }

        public int GetCurrentIndex(string id)
        {
            return ActiveInterests.FindIndex(old => old.Id == id);
        }

        internal void AddSkillLevels(ref Dictionary<string, int> startingLevels)
        {
            //Debug.Log("AAAAAAAAAAAAAAAA");
            dupeStatPoints = startingLevels;
            foreach (var skillGroup in startingLevels)
            {
                //Debug.Log(skillGroup.Key + ", " + skillGroup.Value);
                if (skillGroup.Value > 0)
                {
                    if (!IsThisANewSkill(skillGroup.Key))
                    {
                        FallBack = new KRandom().Next(1, skillGroup.Value+1);
                    }
                }
            }
        }



        public void AddTrait(string id)
        {
            if (!currentTraitIds.Contains(id))
                currentTraitIds.Add(id);
        }
        public void ReplaceTrait(string old, string newS)
        {
            if(old == newS) return;
            if (!currentTraitIds.Contains(old) || currentTraitIds.Contains(newS))
                return;

            currentTraitIds.Remove(old);
            currentTraitIds.Add(newS);
        }
        public List<string> CurrentTraitsWithout(string thisTrait)
        {
            return currentTraitIds.Where(entry => entry != thisTrait).ToList();
        }






        public string GetNextTraitId(string currentId, bool backwards)
        {
            ModAssets.GetTraitListOfTrait(currentId, out var currentList);
            if (currentList == null)
                return currentId;
            int i = 0;

            i = currentList.FindIndex(t => t.id == currentId);
            if (i != -1)
            {
                do
                {
                    i += (backwards ? -1 : 1);
                    if (i == currentList.Count)
                        i = 0;
                    else if (i < 0)
                        i += currentList.Count;
                }
                while (CurrentTraitsWithout(currentId).Contains(currentList[i].id));
                return currentList[i].id;
            }
            return currentId;
        }


        public bool IsThisANewSkill(string stat)
        {
            if (!USEDSTATS.ContainsKey(stat))
            {
                USEDSTATS[stat] = 1;
                return true;
            }
            else
            {
                USEDSTATS[stat]++;
                return false;
            }
        }
        public bool DoesRemoveReduceStats(string stat)
        {
            if (USEDSTATS.ContainsKey(stat))
            {
                //Debug.Log(stat + ", Count " + USEDSTATS[stat]);
                if (USEDSTATS[stat] > 1)
                {
                    USEDSTATS[stat]--;
                    return false;
                }
                else
                {
                    USEDSTATS.Remove(stat);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        //public int GetBonusName(int index)
        //{
        //    Debug.Log("Index: " + index);
        //    var relevantAttribute = ActiveInterests[index].relevantAttributes.First().Id;
        //    Debug.Log(relevantAttribute);
        //    return dupeStatPoints[relevantAttribute];
        //}

        public int GetBonusValue(int index)
        {
            //Debug.Log("Index: " + index);
            var relevantAttribute = ActiveInterests[index].relevantAttributes.First().Id;
            //Debug.Log(relevantAttribute);
            return dupeStatPoints[relevantAttribute];
        }
    }
}
