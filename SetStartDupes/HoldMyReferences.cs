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
        public string GetNextTraitId(string currentId,NextType nextType)
        {
            int i = 0;
            if (nextType == NextType.posTrait)
            {
                i = DUPLICANTSTATS.GOODTRAITS.FindIndex(t => t.id == currentId) + 1;
                if (i == DUPLICANTSTATS.GOODTRAITS.Count)
                    i = 0;
                return DUPLICANTSTATS.GOODTRAITS[i].id;
            }
            else if (nextType == NextType.negTrait)
            {
                i = DUPLICANTSTATS.BADTRAITS.FindIndex(t => t.id == currentId) + 1;
                if (i == DUPLICANTSTATS.BADTRAITS.Count)
                    i = 0;
                return DUPLICANTSTATS.BADTRAITS[i].id;
            }
            else if (nextType == NextType.joy)
            {
                i = DUPLICANTSTATS.JOYTRAITS.FindIndex(t => t.id == currentId) + 1;
                if (i == DUPLICANTSTATS.JOYTRAITS.Count)
                    i = 0;
                return DUPLICANTSTATS.JOYTRAITS[i].id;
            }
            else if (nextType == NextType.stress)
            {
                i = DUPLICANTSTATS.STRESSTRAITS.FindIndex(t => t.id == currentId) + 1;
                if (i == DUPLICANTSTATS.STRESSTRAITS.Count)
                    i = 0;
                return DUPLICANTSTATS.STRESSTRAITS[i].id;
            }
            return string.Empty;
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
        public IEnumerable<IListableOption> GiveNewSelections()
        {
            List<SkillGroup> list = new List<SkillGroup>((IEnumerable<SkillGroup>)Db.Get().SkillGroups.resources);
            foreach(var v in CurrentSkills)
            {
                list.Remove(v);
            }
            return list;
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
