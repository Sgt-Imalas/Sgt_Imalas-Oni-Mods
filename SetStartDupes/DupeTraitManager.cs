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
        public Dictionary<string,int> USEDSTATS= new();

        public List<SkillGroup> CurrentSkills = new();

        public List<string> currentTraitIds = new();
        [Serialize]
        public int strengthSkillHeightHolder = -1;

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

        public void AddTrait(string id)
        {
            if (!currentTraitIds.Contains(id))
                currentTraitIds.Add(id);
        }
        public void ReplaceTrait(string old, string newS)
        {
            if (!currentTraitIds.Contains(old)|| currentTraitIds.Contains(newS))
                return;

            currentTraitIds.Remove(old);
            currentTraitIds.Add(newS);
        }
        public List<string> CurrentTraitsWithout(string thisTrait)
        {
            return currentTraitIds.Where(entry => entry != thisTrait).ToList();
        }

        public static NextType GetTraitListOfTrait(string traitId, out List<DUPLICANTSTATS.TraitVal> TraitList)
        {
            if (DUPLICANTSTATS.GENESHUFFLERTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.GENESHUFFLERTRAITS;
                return NextType.geneShufflerTrait;
            }
            else if (DUPLICANTSTATS.GOODTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.GOODTRAITS;
                return NextType.posTrait;
            }
            else if (DUPLICANTSTATS.BADTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.BADTRAITS;
                return NextType.negTrait;
            }
            else if (DUPLICANTSTATS.NEEDTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.NEEDTRAITS;
                return NextType.needTrait;
            }
            else if (DUPLICANTSTATS.JOYTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.JOYTRAITS;
                return NextType.joy;
            }
            else if (DUPLICANTSTATS.STRESSTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.STRESSTRAITS;
                return NextType.stress;
            }
            TraitList = null;
            return NextType.undefined;

        }


        public string GetNextTraitId(string currentId,bool backwards)
        {
            GetTraitListOfTrait(currentId, out var currentList);
            int i = 0;

            i = currentList.FindIndex(t => t.id == currentId);
            if (i != -1) { 
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


        public bool HasCurrentSkill(string id)
        {
            foreach(var skill in CurrentSkills)
            {
                if (skill.Id == id)
                    return true;
            }
            return false;
        }

        public bool AddOrIncreaseToStat(string stat)
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
        public bool DoesRemoveReduceStats(string stat, bool remove)
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
    }
}
