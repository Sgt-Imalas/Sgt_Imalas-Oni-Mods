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
    class HoldMyReferences : KMonoBehaviour
    {
        [Serialize]
        public Dictionary<string,int> USEDSTATS= new();

        public List<SkillGroup> CurrentSkills = new();

        public List<string> currentTraitIds = new();

        public enum NextType
        {
            posTrait,
            negTrait,
            joy,
            stress
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

        public string GetNextTraitId(string currentId,NextType nextType,bool backwards)
        {
            int i = 0;
            List<DUPLICANTSTATS.TraitVal> currentList = null;
            if (nextType == NextType.posTrait)
            {
                ///Compatibility for Akis sussy dupe ink
                if(DUPLICANTSTATS.GENESHUFFLERTRAITS.FindIndex(t => t.id == currentId) != -1) {
                    currentList = DUPLICANTSTATS.GENESHUFFLERTRAITS;
                }
                else 
                {
                    currentList = DUPLICANTSTATS.GOODTRAITS;
                }
            }
            else if (nextType == NextType.negTrait)
            {
                currentList = DUPLICANTSTATS.BADTRAITS;
            }
            else if (nextType == NextType.joy)
            {
                currentList = DUPLICANTSTATS.JOYTRAITS;
            }
            else if (nextType == NextType.stress)
            {
                currentList = DUPLICANTSTATS.STRESSTRAITS;
            }

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

        public void InstantiateSingleStatView(GameObject parent)
        {
            var window = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, parent);
            //window.SetActive(false);

#if DEBUG
            // Debug.Log("SINGLE LIST:");
            // UIUtils.ListAllChildren(window.transform);
#endif
            var oldComp = window.GetComponent<ModsScreen>();
            UnityEngine.Object.Destroy(oldComp);
            var mlv = (StatSelector)window.AddComponent(typeof(StatSelector));
            mlv.Build(this);
        }
    }
}
