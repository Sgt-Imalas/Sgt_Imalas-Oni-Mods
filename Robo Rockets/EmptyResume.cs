using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnastoronOniMods
{
    class EmptyResume:MinionResume
    {
        protected override void OnPrefabInit()
        {
        }
        protected override void OnSpawn()
        {
            if (this.GrantedSkillIDs == null)
                this.GrantedSkillIDs = new List<string>();
            List<string> stringList = new List<string>();
            foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
            {
                if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).deprecated)
                    stringList.Add(keyValuePair.Key);
            }
            foreach (string skillId in stringList)
                this.UnmasterSkill(skillId);
            foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
            {
                if (keyValuePair.Value)
                {
                    Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
                    foreach (SkillPerk perk in skill.perks)
                    {
                        if (perk.OnRemove != null)
                            perk.OnRemove(this);
                        if (perk.OnApply != null)
                            perk.OnApply(this);
                    }
                }
            }
        }
        protected override void OnCleanUp()
        {
        }
    }
}
